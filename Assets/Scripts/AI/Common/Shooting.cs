using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{

    [RequireComponent(typeof(SoldierController))]
    public class Shooting : MonoBehaviour
    {

        [SerializeField]
        private SoldierController target;

        private SoldierController controller;
        private bool enableAiming = false;
        private bool enableShooting = false;

        public void SetAimingEnabled(bool enabled)
        {
            enableAiming = enabled;
        }

        public void SetShootingEnabled(bool enabled)
        {
            enableShooting = enabled;
        }

        // Use this for initialization
        void Start()
        {
            controller = GetComponent<SoldierController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (controller.IsDead())
            {
                return;
            }

            if (enableAiming)
            {
                controller.TurnToPoint(target.transform.position);
            }

            if (enableShooting)
            {
                controller.Shoot(target.transform.position);
            }
        }
    }

}