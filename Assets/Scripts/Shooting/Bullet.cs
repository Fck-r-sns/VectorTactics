using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed { get; set; }
    public float damage { get; set; }
    public SoldierController owner { get; set; }

    private int soldiersLayer;
    private int wallsLayer;
    private LayerMask coverLogicLayerMask;
    private LayerMask physicalObstaclesLayerMask;

    // Use this for initialization
    void Start()
    {
        soldiersLayer = LayerMask.NameToLayer("Soldiers");
        wallsLayer = LayerMask.NameToLayer("Walls");
        coverLogicLayerMask = LayerMask.GetMask("Cover");
        physicalObstaclesLayerMask = LayerMask.GetMask("Soldiers", "Walls");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 shift = transform.forward * 0.5f;
        float distancePerFrame = speed * Time.deltaTime;
        float raycastDistance = distancePerFrame + shift.magnitude;
        RaycastHit hit;
        Ray ray = new Ray(transform.position - shift, transform.forward); // shift origin to fix bug, when ray orign can be inside enemy collider and ignore it

        if (Physics.Raycast(ray, out hit, raycastDistance, coverLogicLayerMask))
        {
            BulletThroughCoverLogic cover = hit.transform.gameObject.GetComponent<BulletThroughCoverLogic>();
            if (cover.CheckBullet(this))
            {
                OnCollisionWithCover(cover.gameObject);
            }
        }

        if (Physics.Raycast(ray, out hit, raycastDistance, physicalObstaclesLayerMask))
        {
            GameObject obstacle = hit.transform.gameObject;
            if (obstacle.layer == soldiersLayer)
            {
                OnCollisionWithSoldier(obstacle);
            }
            if (obstacle.layer == wallsLayer)
            {
                OnCollisionWithWall(obstacle);
            }
            return;
        }

        transform.position += transform.forward * distancePerFrame;
    }

    private void OnCollisionWithCover(GameObject cover)
    {
        Destroy(gameObject);
    }

    private void OnCollisionWithWall(GameObject wall)
    {
        Destroy(gameObject);
    }

    private void OnCollisionWithSoldier(GameObject soldier)
    {
        SoldierController sc = soldier.GetComponent<SoldierController>();
        sc.OnHit(damage);
        Destroy(gameObject);
    }

}
