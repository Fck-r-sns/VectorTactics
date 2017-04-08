using UnityEngine;

namespace Ai
{

    public class Waypoint
    {

        public readonly Vector3 position;
        public float weight = 0.0f;

        public Waypoint(Vector3 position)
        {
            this.position = position;
        }

    }

}