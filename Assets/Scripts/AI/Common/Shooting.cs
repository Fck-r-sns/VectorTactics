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

        [SerializeField]
        private Weapon weapon;

        private SoldierController controller;
        private CharacterState state;
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
            state = controller.GetState();
        }

        // Update is called once per frame
        void Update()
        {
            if (state.isDead)
            {
                return;
            }

            float vb = weapon.GetBulletSpeed();
            float vt = state.speed;
            float angle = Vector3.Angle(state.movementDirection, transform.position - target.transform.position);
            float dst = Vector3.Distance(target.transform.position, transform.position);
            float a = vt * vt - vb * vb;
            float b = 2 * dst * vt * Mathf.Cos(Mathf.Deg2Rad * angle);
            float c = dst * dst;
            float D = b * b - 4 * a * c;
            float sqrtD = Mathf.Sqrt(D);
            float x1 = (-b + sqrtD) / (2 * a);
            float x2 = (-b - sqrtD) / (2 * a);
            float bulletTime = Mathf.Max(x1, x2);
            Vector3 shootingTarget = target.transform.position + state.movementDirection * bulletTime;
            Debug.DrawLine(transform.position, shootingTarget, (state.side == Defines.Side.Blue) ? Color.blue : Color.red);

            if (enableAiming)
            {
                controller.TurnToPoint(shootingTarget);
            }

            if (enableShooting)
            {
                controller.Shoot(shootingTarget);
            }
        }
    }

}