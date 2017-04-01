using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletThroughCoverTrigger : MonoBehaviour
{
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBulletCollide(Bullet bullet)
    {
        if (Random.Range(0, 99) < 50)
        {
            Destroy(bullet.gameObject);
        }
    }
}
