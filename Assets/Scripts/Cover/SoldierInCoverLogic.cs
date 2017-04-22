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
        Ray ray = new Ray(point + Vector3.up * 100, Vector3.down);
        RaycastHit hit;
        return trigger.Raycast(ray, out hit, float.MaxValue);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        CharacterState character = other.gameObject.GetComponent<CharacterState>();
        if (character != null)
        {
            soldiersInCover.Add(character);
            ++character.numberOfCovers;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterState character = other.gameObject.GetComponent<CharacterState>();
        if (character != null)
        {
            soldiersInCover.Remove(character);
            --character.numberOfCovers;
        }
    }

}
