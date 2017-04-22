using System.Collections.Generic;
using UnityEngine;

public class SoldierInCoverLogic : MonoBehaviour
{

    private HashSet<CharacterState> soldiersInCover = new HashSet<CharacterState>();
    private Collider trigger;

    void Start()
    {
        trigger = GetComponent<Collider>();
    }

    public HashSet<CharacterState> GetSoldiersInCover() {
        return soldiersInCover;
    }

    public bool CheckIfPointInCover(Vector3 point)
    {
        return trigger.bounds.Contains(point);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        CharacterState character = other.gameObject.GetComponent<CharacterState>();
        if (character != null)
        {
            soldiersInCover.Add(character);
            character.isInCover = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterState character = other.gameObject.GetComponent<CharacterState>();
        if (character != null)
        {
            soldiersInCover.Remove(character);
            character.isInCover = false;
        }
    }

}
