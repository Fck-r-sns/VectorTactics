using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField]
    private float shotsPerSecond = 1.0f;

    [SerializeField]
    private float bulletSpeed = 20.0f;

    [SerializeField]
    private float damage = 10.0f;

    [SerializeField]
    private float deviation = 0.2f;

    [SerializeField]
    private Transform shootingPoint;

    [SerializeField]
    private Transform aim;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private SoldierController owner;

    private float SHOOTING_MODE_THRESHOLD = 2.0f;

    private float timePerShot;
    private float nextShotAvailableTime = 0.0f;

    public void fire(Vector3 target)
    {
        if (Time.time < nextShotAvailableTime)
        {
            return;
        }
        Vector3 direction = (target - shootingPoint.position).normalized;
        if (Vector3.Distance(target, shootingPoint.position) < SHOOTING_MODE_THRESHOLD)
        {
            target = shootingPoint.position + shootingPoint.forward;
        }
        else
        {
            target = shootingPoint.position + direction;
        }
        Vector3 shift = GetRandomShift();
        target += shift;
        target.y = shootingPoint.position.y;
        aim.LookAt(target, Vector3.up);
        GameObject bulletObject = Instantiate(bulletPrefab, shootingPoint.position, aim.rotation); // TODO: object pool

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
        bullet.damage = damage;
        bullet.owner = owner;

        Destroy(bulletObject, 10.0f);
        nextShotAvailableTime = Time.time + timePerShot;
    }

    void Start()
    {
        timePerShot = 1.0f / shotsPerSecond;
    }
    
    void Update()
    {
    }

    private Vector3 GetRandomShift()
    {
        float x = RandomGaussian.Next(0.0f, deviation);
        float z = RandomGaussian.Next(0.0f, deviation);
        return new Vector3(x, 0, z);
    }
}
