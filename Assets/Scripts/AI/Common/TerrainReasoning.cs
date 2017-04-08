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
        private WaypointGenerationMode mode = WaypointGenerationMode.Naive;

        [SerializeField]
        private float mapWidth = 100.0f;

        [SerializeField]
        private float mapHeight = 100.0f;

        [SerializeField]
        private float gridStep = 0.5f;

        private List<Waypoint> waypoints = new List<Waypoint>();

        void Awake()
        {
            waypoints = GenerateWaypoints();
        }

        void Update()
        {
            foreach(Waypoint wp in waypoints)
            {
                wp.weight = Random.Range(0.0f, 1.0f);
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