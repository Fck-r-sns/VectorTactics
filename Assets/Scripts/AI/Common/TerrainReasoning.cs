using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ai
{

    public class TerrainReasoning : MonoBehaviour
    {

        public enum WaypointGenerationMode
        {
            Naive
        }

        public delegate float WaypointProcessor(Waypoint wp);

        [SerializeField]
        private WaypointGenerationMode mode = WaypointGenerationMode.Naive;

        [SerializeField]
        private float gridStep = 0.5f;

        private CharacterState agentState;
        private CharacterState enemyState;

        private Navigation agentNavigationHelper;
        private Navigation enemyNavigationHelper;

        private LayerMask layerMask;
        private int coverLayer;
        private int wallsLayer;

        private WaypointProcessor waypointProcessor;

        private int xCount;
        private int xCenterIndex;
        private int zCount;
        private int zCenterIndex;
        private Waypoint[,] waypoints;

        public float GetGridStep()
        {
            return gridStep;
        }

        void Awake()
        {
            agentState = GetComponent<CharacterState>();
            enemyState = agentState.enemyState;

            agentNavigationHelper = agentState.gameObject.GetComponent<Navigation>();
            enemyNavigationHelper = enemyState.gameObject.GetComponent<Navigation>();

            layerMask = LayerMask.GetMask("Cover", "Walls");
            coverLayer = LayerMask.NameToLayer("Cover");
            wallsLayer = LayerMask.NameToLayer("Walls");

            // default waypoint processor
            waypointProcessor = wp =>
            {
                float weight = 0.0f;
                if (wp.isBehindWall)
                {
                    weight = 0.45f;
                }
                else if (wp.isBehindCover)
                {
                    if (wp.isInCover)
                    {
                        weight = 1.0f;
                    }
                    else
                    {
                        weight = 0.66f;
                    }
                }
                return weight;
            };

            waypoints = GenerateWaypoints();
        }

        void Start()
        {
            try
            {
                RestoreDistances();
                Debug.Log("Waypoint distances restored");
            }
            catch (Exception e)
            {
                Debug.Log("Failed to restore waypoint distances: " + e.Message);
                //CalculateDistances();
                //StoreDistances();
            }
        }

        void Update()
        {
            foreach (Waypoint wp in waypoints)
            {
                wp.Reset();

                Vector3 origin = agentState.lastEnemyPosition;
                origin.y = 0.0f;
                float distanceToEnemy = Vector3.Distance(wp.position, origin);
                origin.y = 1.7f; // height of soldiers' heads
                Vector3 direction = wp.position - origin;
                direction.y = 0.0f;

                Ray ray = new Ray(origin, direction);
                float raycastDistance = distanceToEnemy;
                RaycastHit hit;
                do
                {

                    if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
                    {
                        int layer = hit.transform.gameObject.layer;
                        if (layer == coverLayer)
                        {
                            BulletThroughCoverLogic coverLogic = hit.transform.gameObject.GetComponent<BulletThroughCoverLogic>();

                            if (!coverLogic.CheckIfPointInCover(agentState.lastEnemyPosition))
                            {
                                wp.isBehindCover = true;
                            }

                            if (coverLogic.CheckIfPointInCover(wp.position))
                            {
                                wp.isInCover = true;
                            }

                            // continue ray over cover
                            Vector3 oldOrigin = ray.origin;
                            ray.origin = hit.point + ray.direction.normalized * 0.1f;
                            raycastDistance -= Vector3.Distance(ray.origin, oldOrigin);
                            continue;
                        }
                        else if (layer == wallsLayer)
                        {
                            wp.isBehindWall = true;
                        }
                    }

                    break;

                } while (true);

                {
                    Waypoint agentWp = GetNearestWaypointToPoint(agentState.position);
                    wp.distanceToAgent = agentWp.distancesToOtherWaypoints[wp];
                }
                {
                    Waypoint enemyWp = GetNearestWaypointToPoint(agentState.lastEnemyPosition);
                    wp.distanceToEnemy = enemyWp.distancesToOtherWaypoints[wp];
                }

                wp.weight = waypointProcessor(wp);
            }
        }

        public Waypoint[,] GetWaypoints()
        {
            return waypoints;
        }

        public void SetWaypointProcessor(WaypointProcessor waypointProcessor)
        {
            this.waypointProcessor = waypointProcessor;
        }

        private Waypoint[,] GenerateWaypoints()
        {
            switch (mode)
            {
                case WaypointGenerationMode.Naive:
                    return NaiveWaypointGenerator();
                default:
                    return null;
            }
        }

        private Waypoint[,] NaiveWaypointGenerator()
        {
            xCount = (int)(Defines.MAP_WIDTH / 2.0f / gridStep) * 2 + 1;
            xCenterIndex = (xCount - 1) / 2;
            zCount = (int)(Defines.MAP_HEIGHT / 2.0f / gridStep) * 2 + 1;
            zCenterIndex = (zCount - 1) / 2;
            Waypoint[,] waypoints = new Waypoint[xCount, zCount];

            for (int xIndex = 0; xIndex < xCount; ++xIndex)
            {
                for (int zIndex = 0; zIndex < zCount; ++zIndex)
                {
                    float x = (xIndex - xCenterIndex) * gridStep;
                    float z = (zIndex - zCenterIndex) * gridStep;
                    waypoints[xIndex, zIndex] = new Waypoint(xIndex, zIndex, new Vector3(x, 0, z));
                }
            }
            return waypoints;
        }

        private void CalculateDistances()
        {
            Vector3 inititalPosition = transform.position;
            UnityEngine.AI.NavMeshAgent nma = agentNavigationHelper.GetComponent<UnityEngine.AI.NavMeshAgent>();
            foreach (Waypoint wp in waypoints)
            {
                nma.enabled = false;
                agentNavigationHelper.transform.position = wp.position;
                nma.enabled = true;
                foreach (Waypoint otherWp in waypoints)
                {
                    float length = 0.0f;
                    if (wp != otherWp)
                    {
                        var path = agentNavigationHelper.GetPathTo(otherWp.position);
                        length = agentNavigationHelper.CalculatePathLength(path);
                    }
                    wp.distancesToOtherWaypoints.Add(otherWp, length);
                }
            }
            agentNavigationHelper.transform.position = inititalPosition;
        }

        private void StoreDistances()
        {
            using (BinaryWriter file = new BinaryWriter(File.OpenWrite("./Files/waypoint_distances")))
            {
                file.Write(gridStep);
                foreach (Waypoint wp in waypoints)
                {
                    file.Write(wp.xIndex);
                    file.Write(wp.zIndex);
                    file.Write(wp.distancesToOtherWaypoints.Count);
                    foreach (var kv in wp.distancesToOtherWaypoints)
                    {
                        file.Write(kv.Key.xIndex);
                        file.Write(kv.Key.zIndex);
                        file.Write(kv.Value);
                    }
                }
            }
        }

        private void RestoreDistances()
        {
            using (BinaryReader file = new BinaryReader(File.OpenRead("./Files/waypoint_distances")))
            {
                gridStep = file.ReadSingle();
                while (file.BaseStream.Position != file.BaseStream.Length)
                {
                    int wpXIndex = file.ReadInt32();
                    int wpZIndex = file.ReadInt32();
                    Waypoint wp = waypoints[wpXIndex, wpZIndex];

                    int size = file.ReadInt32();
                    while (size-- > 0)
                    {
                        int otherXIndex = file.ReadInt32();
                        int otherZIndex = file.ReadInt32();
                        float distance = file.ReadSingle();

                        Waypoint otherWp = waypoints[otherXIndex, otherZIndex];
                        wp.distancesToOtherWaypoints.Add(otherWp, distance);
                    }
                }
            }
        }

        private Waypoint GetNearestWaypointToPoint(Vector3 point)
        {
            int xIndex = Mathf.RoundToInt(point.x / gridStep) + xCenterIndex;
            int zIndex = Mathf.RoundToInt(point.z / gridStep) + zCenterIndex;
            return waypoints[xIndex, zIndex];
        }

    }

}