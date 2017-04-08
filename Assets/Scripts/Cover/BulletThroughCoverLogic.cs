using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletThroughCoverLogic : MonoBehaviour
{

    [SerializeField]
    private SoldierInCoverLogic soldiersInCoverLogic;

    public bool CheckBullet(Bullet bullet)
    {
        return !CheckIfSoldierInCover(bullet.owner) && ((Random.Range(0, 99) & 1) == 0);
    }

    public bool CheckIfSoldierInCover(SoldierController soldier)
    {
        return soldiersInCoverLogic.GetSoldiersInCover().Contains(soldier);
    }

    public bool CheckIfPointInCover(Vector3 point)
    {
        return soldiersInCoverLogic.CheckIfPointInCover(point);
    }
}
