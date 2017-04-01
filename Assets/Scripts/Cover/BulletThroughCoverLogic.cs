using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletThroughCoverLogic : MonoBehaviour
{

    [SerializeField]
    private SoldierInCoverLogic soldiersInCoverLogic;

    public bool CheckBullet(Bullet bullet)
    {
        return !soldiersInCoverLogic.GetSoldiersInCover().Contains(bullet.owner) 
            && ((Random.Range(0, 99) & 1) == 0);
    }
}
