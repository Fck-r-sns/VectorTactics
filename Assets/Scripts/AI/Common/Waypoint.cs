using System.Collections.Generic;
using UnityEngine;

namespace Ai
{

    public class Waypoint
    {

        public readonly int xIndex;
        public readonly int zIndex;
        public readonly Vector3 position;
        public readonly Dictionary<Waypoint, float> distancesToOtherWaypoints = new Dictionary<Waypoint, float>();
        public float distanceToAgent;
        public float distanceToEnemy;
        public bool isBehindCover = false;
        public bool isInCover = false;
        public bool isBehindWall = false;
        public float weight = 0.0f;

        public Waypoint(int xIndex, int zIndex, Vector3 position)
        {
            this.xIndex = xIndex;
            this.zIndex = zIndex;
            this.position = position;
        }

        public void Reset()
        {
            weight = 0.0f;
            distanceToAgent = 0.0f;
            distanceToEnemy = 0.0f;
            isBehindCover = false;
            isInCover = false;
            isBehindWall = false;
        }

    }

}