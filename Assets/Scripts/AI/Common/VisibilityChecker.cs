using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{

    [SerializeField]
    private SoldierController blueSoldier;

    [SerializeField]
    private SoldierController redSoldier;

    private LayerMask layerMask;
    private int soldiersLayer;
    private bool isVisible = false;

    public bool IsVisible()
    {
        return isVisible;
    }
    
    void Start()
    {
        layerMask = LayerMask.GetMask("Soldiers", "Walls");
        soldiersLayer = LayerMask.NameToLayer("Soldiers");
    }

    void Update()
    {
        Vector3 origin = blueSoldier.transform.position;
        origin.y = 1.7f; // height of head
        Vector3 direction = redSoldier.transform.position - blueSoldier.transform.position;
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            isVisible = (hit.transform.gameObject.layer == soldiersLayer);
        }
        else
        {
            isVisible = false;
        }
    }
}
