using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonsterPickupNetSetupScript : NetworkBehaviour
{

	// Use this for initialization
	void Start ()
    {
        if (isLocalPlayer)
        {
            //GetComponent<CarPhysics>().enabled = true;
            Camera.main.GetComponent<Camera_Chase>().target = transform;
        }	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
