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

        [SerializeField]
        private WorldState worldState;

        [SerializeField]
        private WaypointGenerationMode mode = WaypointGenerationMode.Naive;

        [SerializeField]
        private float mapWidth = 100.0f;

        [SerializeField]
        private float mapHeight = 100.0f;

        [SerializeField]
        private float gridStep = 0.5f;

        private SoldierController owner;
        private SoldierController enemy;

        private LayerMask layerMask;
        private int coverLayer;
        private int wallsLayer;

        private List<Waypoint> waypoints = new List<Waypoint>();

        void Awake()
        {
            owner = GetComponent<SoldierController>();
            enemy = (worldState.blueSoldier == owner) ? worldState.redSoldier : worldState.blueSoldier;

            layerMask = LayerMask.GetMask("Cover", "Walls");
            coverLayer = LayerMask.NameToLayer("Cover");
            wallsLayer = LayerMask.NameToLayer("Walls");

            waypoints = GenerateWaypoints();
        }

        void Update()
        {
            foreach (Waypoint wp in waypoints)
            {
                Vector3 origin = enemy.transform.position;
                origin.y = 0.0f;
                float distanceToEnemy = Vector3.Distance(wp.position, origin);
                origin.y = 1.7f; // height of soldiers' head
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
                            if (coverLogic.CheckIfSoldierInCover(enemy))
                            {
                                Vector3 oldOrigin = ray.origin;
                                ray.origin = hit.point + ray.direction.normalized * 0.1f;
                                raycastDistance -= Vector3.Distance(ray.origin, oldOrigin);
                                continue;
                            }
                            wp.weight = 1.0f;
                        }
                        else if (layer == wallsLayer)
                        {
                            wp.weight = 0.5f;
                        }
                    }
                    else
                    {
                        wp.weight = 0.0f;
                    }
                    break;
                } while (true);
            }
        }

        public List<Waypoint> GetWaypoints()
        {
            return waypoints;
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
            for (float x = -mapWidth / 2.0f; x <= mapWidth / 2.0f; x += gridStep)
            {
                for (float z = -mapHeight / 2.0f; z <= mapHeight / 2.0f; z += gridStep)
                {
                    waypoints.Add(new Waypoint(new Vector3(x, 0, z)));
                }
            }
            return waypoints;
        }

    }

}