﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{

    [RequireComponent(typeof(SoldierController))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Navigation : MonoBehaviour
    {

        [SerializeField]
        private bool enableMouseControl = false;

        private SoldierController controller;
        private CharacterState state;
        private NavMeshAgent navMeshAgent;
        private int floorMask;
        private NavMeshPath path;
        private Vector3? destination = null;
        private int currentPathIndex = 0;

        public void SetDestination(Vector3? destination)
        {
            this.destination = destination;
            UpdatePath();
        }

        public bool IsDestinationReachable()
        {
            return destination.HasValue && (path.status == NavMeshPathStatus.PathComplete);
        }

        // Use this for initialization
        void Start()
        {
            controller = GetComponent<SoldierController>();
            state = controller.GetState();
            navMeshAgent = GetComponent<NavMeshAgent>();
            floorMask = LayerMask.GetMask("Floor");
            path = new NavMeshPath();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (state.isDead)
            {
                return;
            }

            if (enableMouseControl && Input.GetMouseButton(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
                {
                    SetDestination(hit.point);
                }
            }

            if (destination.HasValue && (path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 0))
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
            for (int i = currentPathIndex; (i < path.corners.Length); ++i)
            {
                Vector3 nextPoint = path.corners[i];
                nextPoint.y = 0;
                Vector3 currentPosition = transform.position;
                currentPosition.y = 0;
                if (Vector3.Distance(nextPoint, currentPosition) > 0.1f)
                {
                    currentPathIndex = i;
                    return nextPoint;
                }
            };
            return null;
        }

        private void UpdatePath()
        {
            if (destination.HasValue)
            {
                navMeshAgent.CalculatePath(destination.Value, path);
                currentPathIndex = 0;
            }
        }
    }

}
