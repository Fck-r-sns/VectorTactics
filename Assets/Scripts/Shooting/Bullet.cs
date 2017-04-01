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
    private int coversLayer;
    private LayerMask coverLogicLayerMask;
    private LayerMask physicalObstaclesLayerMask;

    // Use this for initialization
    void Start()
    {
        soldiersLayer = LayerMask.NameToLayer("Soldiers");
        wallsLayer = LayerMask.NameToLayer("Walls");
        coversLayer = LayerMask.NameToLayer("CoverTriggers");
        coverLogicLayerMask = LayerMask.GetMask("CoverTriggers");
        physicalObstaclesLayerMask = LayerMask.GetMask("Soldiers", "Walls");
    }

    // Update is called once per frame
    void Update()
    {
        float distancePerFrame = speed * Time.deltaTime;
        RaycastHit hit;
        Ray ray = new Ray(transform.position - transform.forward * 0.5f, transform.forward); // shift origin to fix bug, when ray orign can be inside enemy collider and ignore it

        if (Physics.Raycast(ray, out hit, distancePerFrame, coverLogicLayerMask))
        {
            BulletThroughCoverTrigger cover = hit.transform.gameObject.GetComponent<BulletThroughCoverTrigger>();
            if (cover.CheckBullet(this))
            {
                OnCollisionWithCover(cover.gameObject);
            }
        }

        if (Physics.Raycast(ray, out hit, distancePerFrame, physicalObstaclesLayerMask))
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
