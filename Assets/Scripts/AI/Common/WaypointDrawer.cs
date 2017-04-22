using System.Collections.Generic;
using UnityEngine;

namespace Ai
{

    [RequireComponent(typeof(TerrainReasoning))]
    public class WaypointDrawer : MonoBehaviour
    {

        public enum Mode
        {
            Height,
            Color,
            HeightAndColor
        }

        [SerializeField]
        private GameObject waypointPrefab;

        [SerializeField]
        private Mode mode = Mode.Height;

        [SerializeField]
        private float maxHeight = 3.0f;

        private TerrainReasoning terrain;
        private Dictionary<Waypoint, WaypointRepresentation> objects = new Dictionary<Waypoint, WaypointRepresentation>();

        void Start()
        {
            terrain = GetComponent<TerrainReasoning>();
            Waypoint[,] waypoints = terrain.GetWaypoints();
            foreach (Waypoint wp in waypoints)
            {
                GameObject obj = Instantiate(waypointPrefab, wp.position, Quaternion.identity);
                obj.transform.Translate(0.0f, 0.01f, 0.0f);
                WaypointRepresentation wpr = obj.GetComponent<WaypointRepresentation>();
                wpr.SetCellSize(terrain.GetGridStep());
                objects.Add(wp, wpr);
            }
        }

        void Update()
        {
            foreach(var pair in objects)
            {
                Waypoint wp = pair.Key;
                WaypointRepresentation obj = pair.Value;

                if ((mode == Mode.Height) || (mode == Mode.HeightAndColor))
                {
                    obj.SetHeight(wp.weight * maxHeight);
                }

                if ((mode == Mode.Color) || (mode == Mode.HeightAndColor))
                {
                    float red = Mathf.Clamp(wp.weight, 0.0f, 1.0f);
                    float green = 1 - red;
                    obj.SetColor(new Color(red, green, 0.0f));
                }
            }
        }
    }

}