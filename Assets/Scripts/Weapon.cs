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
    private GameObject bulletPrefab;

    private float timePerShot;
    private float nextShotAvailableTime = 0.0f;

    public void fire(Vector3 direction)
    {
        if (Time.time < nextShotAvailableTime)
        {
            return;
        }
        Vector3 target = shootingPoint.position + direction.normalized;
        Vector3 shift = GetRandomShift();
        target += shift;
        target.y = shootingPoint.position.y;
        shootingPoint.LookAt(target, Vector3.up);
        GameObject bulletObject = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
        bullet.damage = damage;

        Destroy(bulletObject, 10.0f);
        nextShotAvailableTime = Time.time + timePerShot;
    }
        
	void Start () {
        timePerShot = 1.0f / shotsPerSecond;
	}
	
	
	void Update () {
    }

    private Vector3 GetRandomShift()
    {
        float x = RandomGaussian.Next(0.0f, deviation);
        float z = RandomGaussian.Next(0.0f, deviation);
        return new Vector3(x, 0, z);
    }
}
