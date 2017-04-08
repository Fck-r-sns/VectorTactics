using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{

    [RequireComponent(typeof(SoldierController))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Navigation : MonoBehaviour
    {

        private SoldierController controller;
        private NavMeshAgent navMeshAgent;
        private int floorMask;
        private NavMeshPath path;
        private Vector3? destination = null;

        // Use this for initialization
        void Start()
        {
            controller = GetComponent<SoldierController>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            floorMask = LayerMask.GetMask("Floor");
            path = new NavMeshPath();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (controller.IsDead())
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
                {
                    destination = hit.point;
                }
            }

            if (destination.HasValue)
            {
                navMeshAgent.CalculatePath(destination.Value, path);
            }

            if (path.status != NavMeshPathStatus.PathInvalid && path.corners.Length > 0)
            {
                Vector3? nextPoint = GetNextPoint();
                if (nextPoint.HasValue)
                {
                    controller.Move(nextPoint.Value - transform.position);
                }
                else
                {
                    controller.Move(Vector3.zero);
                }
            }
            else
            {
                controller.Move(Vector3.zero);
            }
        }

        private Vector3? GetNextPoint()
        {
            for (int i = 0; (i < path.corners.Length); ++i)
            {
                Vector3 nextPoint = path.corners[i];
                nextPoint.y = 0;
                Vector3 currentPosition = transform.position;
                currentPosition.y = 0;
                if (Vector3.Distance(nextPoint, currentPosition) > 0.1f)
                {
                    return nextPoint;
                }
            };
            return null;
        }
    }

}
