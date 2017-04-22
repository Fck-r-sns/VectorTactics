using UnityEngine;

namespace Ai
{

    public class Waypoint
    {

        public readonly Vector3 position;
        public float weight = 0.0f;
        public float distanceToAgent = 0.0f;
        public float distanceToEnemy = 0.0f;
        public bool behindCover = false;
        public bool inCover = false;
        public bool behindWall = false;

        public Waypoint(Vector3 position)
        {
            this.position = position;
        }

        public void Reset()
        {
            weight = 0.0f;
            distanceToAgent = 0.0f;
            distanceToEnemy = 0.0f;
            behindCover = false;
            inCover = false;
            behindWall = false;
        }

    }

}