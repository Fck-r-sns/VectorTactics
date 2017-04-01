using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierInCoverLogic : MonoBehaviour
{

    private HashSet<SoldierController> soldiersInCover = new HashSet<SoldierController>();

    public HashSet<SoldierController> GetSoldiersInCover() {
        return soldiersInCover;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        SoldierController sc = other.gameObject.GetComponent<SoldierController>();
        if (sc != null)
        {
            soldiersInCover.Add(sc);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SoldierController sc = other.gameObject.GetComponent<SoldierController>();
        if (sc != null)
        {
            soldiersInCover.Remove(sc);
        }
    }

}
