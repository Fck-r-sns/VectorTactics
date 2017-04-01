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

    public bool CheckBullet(Bullet bullet)
    {
        return (Random.Range(0, 99) < 50);
    }
}
