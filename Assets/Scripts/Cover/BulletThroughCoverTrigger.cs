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


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(Time.time + ": Trigger, " + other.tag);
        if (other.tag == "Bullet")
        {
            Bullet b = other.gameObject.GetComponent<Bullet>();
            b.speed = 0.0f;
        }
    }
}
