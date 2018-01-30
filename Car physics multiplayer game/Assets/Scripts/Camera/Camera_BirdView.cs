using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_BirdView : MonoBehaviour {

	public Transform target;
	public Vector3 offset = new Vector3(0, 15, -20);
	public float velocityAmount = 0.75f;
	public float velocityMaxLookahead = 2.0f;
	public float velocityBlendTime = 0.5f;

	Vector3 oldPos;
	Vector3 oldVel;

	void Start () {
		
	}
	
	
	void LateUpdate ()
	{
		Vector3 velocity = (target.position - oldPos) / Time.deltaTime;
		oldVel = Vector3.Lerp(oldVel, Vector3.ClampMagnitude(velocity * velocityAmount, velocityMaxLookahead), Time.deltaTime * (1f / velocityBlendTime));

		oldPos = target.position;

		transform.position = target.position + offset;
		transform.LookAt(target.position + oldVel, Vector3.up);
	}
}
