using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletThroughCoverLogic : MonoBehaviour
{

    [SerializeField]
    private SoldierInCoverLogic soldiersInCoverLogic;

    private Dictionary<int, int> bulletsHistory = new Dictionary<int, int>(); // key is soldier's gameobject id

    public bool CheckBullet(Bullet bullet)
    {
        int id = bullet.owner.GetInstanceID();
        if (!bulletsHistory.ContainsKey(id))
        {
            bulletsHistory.Add(id, Random.Range(0, 1));
        }
        ++bulletsHistory[id];
        return !CheckIfSoldierInCover(bullet.owner) && ((bulletsHistory[id] & 1) == 0);
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
