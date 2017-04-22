using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{

    [RequireComponent(typeof(SoldierController))]
    [RequireComponent(typeof(Weapon))]
    public class Shooting : MonoBehaviour
    {

        private SoldierController controller;
        private Weapon weapon;
        private CharacterState agentState;
        private CharacterState targetState;
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
            weapon = GetComponentInChildren<Weapon>();
            agentState = controller.GetState();
            targetState = agentState.enemyState;
        }

        // Update is called once per frame
        void Update()
        {
            if (agentState.isDead)
            {
                return;
            }

            if (enableAiming)
            {
                Vector3 shootingTarget = CalculateShootingTarget();
                Debug.DrawLine(transform.position, shootingTarget, (agentState.side == Defines.Side.Blue) ? Color.blue : Color.red);

                controller.TurnToPoint(shootingTarget);

                if (enableShooting)
                {
                    controller.Shoot(shootingTarget);
                }
            }

            controller.TurnToPoint(agentState.transform.position + agentState.movementDirection);
        }

        private Vector3 CalculateShootingTarget()
        {
            float vb = weapon.GetBulletSpeed();
            float vt = targetState.speed;
            float angle = Vector3.Angle(targetState.movementDirection, agentState.position - targetState.position);
            float dst = agentState.distanceToEnemy;
            float a = vt * vt - vb * vb;
            float b = 2 * dst * vt * Mathf.Cos(Mathf.Deg2Rad * angle);
            float c = dst * dst;
            float D = b * b - 4 * a * c;
            float sqrtD = Mathf.Sqrt(D);
            float x1 = (-b + sqrtD) / (2 * a);
            float x2 = (-b - sqrtD) / (2 * a);
            float bulletTime = Mathf.Max(x1, x2);
            return targetState.position + targetState.movementDirection * bulletTime;
        }
    }

}