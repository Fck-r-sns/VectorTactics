using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{

    [SerializeField]
    private SoldierController blueSoldier;

    [SerializeField]
    private SoldierController redSoldier;

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

    public bool CheckVisibility(SoldierController from, SoldierController to)
    {
        Vector3 origin = from.transform.position;
        origin.y = 1.7f; // height of head
        Vector3 direction = to.transform.position - from.transform.position;
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
