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
    private float deviation = 0.2f;

    [SerializeField]
    private Transform shootingPoint;

    [SerializeField]
    private GameObject bulletPrefab;

    private float timePerShot;
    private float nextShotAvailableTime = 0.0f;

    public void fire(Vector3 target)
    {
        if (Time.time < nextShotAvailableTime)
        {
            return;
        }
        Vector3 shift = GetRandomShift();
        Debug.Log("Shift: " + shift);
        target += shift;
        target.y = shootingPoint.position.y;
        shootingPoint.LookAt(target, Vector3.up);
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        Destroy(bullet, 10.0f);
        nextShotAvailableTime = Time.time + timePerShot;
    }

	// Use this for initialization
	void Start () {
        timePerShot = 1.0f / shotsPerSecond;
	}
	
	// Update is called once per frame
	void Update () {
    }

    private Vector3 GetRandomShift()
    {
        float x = RandomGaussian.Next(0.0f, deviation);
        float z = RandomGaussian.Next(0.0f, deviation);
        return new Vector3(x, 0, z);
    }
}
