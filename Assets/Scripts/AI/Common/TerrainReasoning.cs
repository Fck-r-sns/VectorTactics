using System.Collections;
using System.Collections.Generic;
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
        private List<Waypoint> waypoints = new List<Waypoint>();

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
                if (wp.behindWall)
                {
                    weight = 0.45f;
                }
                else if (wp.behindCover)
                {
                    if (wp.inCover)
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

        void Update()
        {
            foreach (Waypoint wp in waypoints)
            {
                wp.Reset();

                // check cover information ====================================================
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
                                wp.behindCover = true;
                            }

                            if (coverLogic.CheckIfPointInCover(wp.position))
                            {
                                wp.inCover = true;
                            }

                            // continue ray over cover
                            Vector3 oldOrigin = ray.origin;
                            ray.origin = hit.point + ray.direction.normalized * 0.1f;
                            raycastDistance -= Vector3.Distance(ray.origin, oldOrigin);
                            continue;
                        }
                        else if (layer == wallsLayer)
                        {
                            wp.behindWall = true;
                        }
                    }

                    break;

                } while (true);
                // ============================================================================

                //// distance to agent ==========================================================
                //{
                //    var path = agentNavigationHelper.GetPathTo(wp.position);
                //    wp.distanceToAgent = agentNavigationHelper.CalculatePathLength(path);
                //}
                //// ============================================================================

                //// distance to enemy ==========================================================
                //{
                //    var path = enemyNavigationHelper.GetPathTo(wp.position);
                //    wp.distanceToAgent = enemyNavigationHelper.CalculatePathLength(path);
                //}
                //// ============================================================================

                wp.weight = waypointProcessor(wp);
            }
        }

        public List<Waypoint> GetWaypoints()
        {
            return waypoints;
        }

        public void SetWaypointProcessor(WaypointProcessor waypointProcessor)
        {
            this.waypointProcessor = waypointProcessor;
        }

        private List<Waypoint> GenerateWaypoints()
        {
            switch (mode)
            {
                case WaypointGenerationMode.Naive:
                    return NaiveWaypointGenerator();
                default:
                    return null;
            }
        }

        private List<Waypoint> NaiveWaypointGenerator()
        {
            List<Waypoint> waypoints = new List<Waypoint>();
            for (float x = -Defines.MAP_WIDTH / 2.0f; x <= Defines.MAP_WIDTH / 2.0f; x += gridStep)
            {
                for (float z = -Defines.MAP_HEIGHT / 2.0f; z <= Defines.MAP_HEIGHT / 2.0f; z += gridStep)
                {
                    waypoints.Add(new Waypoint(new Vector3(x, 0, z)));
                }
            }
            return waypoints;
        }

    }

}