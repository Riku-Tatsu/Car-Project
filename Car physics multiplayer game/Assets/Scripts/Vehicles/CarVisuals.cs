using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CarVisuals : MonoBehaviour {

	[System.Serializable]
	public class VisualData
	{
		public Transform wheelMesh;

		[HideInInspector]
		public Vector3 lastWorldPos;
		[HideInInspector]
		public float rotation;
		[HideInInspector]
		public float rpm;
	}

	public VisualData[] vData;

	CarPhysics carScript;
	

	void Start ()
	{
		carScript = GetComponent<CarPhysics>();
	}
	
	void LateUpdate ()
	{
		if(carScript)
		{
			CmdUpdateVisuals();
		}
	}

    //[Command]
    public void CmdUpdateVisuals()
    {
        RpcUpdateVisuals();
    }

    //[ClientRpc]
	public void RpcUpdateVisuals()
	{
		for(int i = 0; i < vData.Length; i++)
		{
			//if(carScript.wheels[i].isGrounded)
			//{
			//	float dirVel = -carScript.wheels[i].wheelCollider.InverseTransformPoint(vData[i].lastWorldPos).z;

			//	float rotation = 360 * dirVel * carScript.wheels[i].wheelRadius * Mathf.PI;
			//	vData[i].rpm = rotation;

			//	vData[i].rotation += rotation;
			//}
			//else
			//{
			//	vData[i].rpm *= 0.98f;

			//	vData[i].rotation += vData[i].rpm;
			//}
			//vData[i].lastWorldPos = carScript.wheels[i].wheelCollider.position;

			vData[i].wheelMesh.localRotation = Quaternion.Euler(carScript.wheels[i].rotation, carScript.wheels[i].wheelPivot.localEulerAngles.y, 0);
			//carScript.wheels[i].wheelMesh.rotation *= carScript.wheels[i].wheelCollider.rotation;

			vData[i].wheelMesh.localPosition = carScript.wheels[i].wheelPivot.localPosition + new Vector3(0, -carScript.wheels[i].rHit.distance + carScript.wheels[i].wheelRadius, 0);
		}
	}
}
