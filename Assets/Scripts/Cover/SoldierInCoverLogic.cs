using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierInCoverLogic : MonoBehaviour
{

    private HashSet<SoldierController> soldiersInCover = new HashSet<SoldierController>();
    private Collider trigger;

    void Start()
    {
        trigger = GetComponent<Collider>();
    }

    public HashSet<SoldierController> GetSoldiersInCover() {
        return soldiersInCover;
    }

    public bool CheckIfPointInCover(Vector3 point)
    {
        return trigger.bounds.Contains(point);
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
