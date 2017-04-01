using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField]
    private float shotsPerSecond = 1.0f;

    [SerializeField]
    private Transform shootingPoint;

    [SerializeField]
    private GameObject bulletPrefab;

    private float timePerShot;
    private float nextShotAvailableTime = 0.0f;

    public void fire(Vector3 target)
    {
        Debug.Log("Time: " + Time.time + "; next shot: " + nextShotAvailableTime);
        if (Time.time < nextShotAvailableTime)
        {
            return;
        }
        target.y = shootingPoint.position.y;
        shootingPoint.LookAt(target, Vector3.up);
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 20;
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
}
