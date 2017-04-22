using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    private LayerMask layerMask;
    private int soldiersLayer;
    
    void Start()
    {
        layerMask = LayerMask.GetMask("Soldiers", "Walls");
        soldiersLayer = LayerMask.NameToLayer("Soldiers");
    }

    void Update()
    {

    }

    public bool CheckVisibility(Transform from, Transform to)
    {
        Vector3 origin = from.position;
        origin.y = 1.7f; // height of head
        Vector3 direction = to.position - from.position;
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            return (hit.transform.gameObject.layer == soldiersLayer);
        }
        else
        {
            return false;
        }
    }
}
