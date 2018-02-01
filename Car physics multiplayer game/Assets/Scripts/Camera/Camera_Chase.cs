using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Chase : MonoBehaviour {

	public Transform target;

	[Header("Positioning")]
	public float distance = 4.0f;
	public float height = 1.0f;
	public float pitch = -5.0f;

	[Header("Movement")]
	public float dampingDistance = 2.0f;

	[Header("Limits")]
	public float pitchMax = 85.0f;
	public float pitchMin = -70.0f;


	Vector3 dummyDir;
	Vector3 prevPos;
	Vector3 FlattenedDummyDir;

	void Start ()
	{
		dummyDir = target.forward;
		prevPos = target.position + -dummyDir * dampingDistance;
		FlattenedDummyDir = Vector3.ProjectOnPlane(-dummyDir, Vector3.up);
	}
	
	void LateUpdate ()
	{
		if(Input.GetKeyDown(KeyCode.E))
		{
			prevPos += Vector3.up;
		}

		dummyDir = (prevPos - target.position).normalized;

		Vector3 dirFlattened = Vector3.ProjectOnPlane(dummyDir, Vector3.up).normalized;
		float dummyAngle = (dummyDir.y >= 0 ? Mathf.Atan2(dummyDir.y, 1 - dummyDir.y) : Mathf.Atan2(dummyDir.y, 1 + dummyDir.y) ) * Mathf.Rad2Deg;
		dummyAngle = Vector3.Angle(dummyDir, dirFlattened) * Mathf.Sign(dummyDir.y);

		float maxAngle = Mathf.Clamp(pitchMax + pitch, 0, pitchMax);
		float minAngle = Mathf.Clamp(pitchMin - pitch, pitchMin, 0);

		bool hasFlipped = Mathf.Sign(dummyDir.x) != Mathf.Sign(FlattenedDummyDir.x) && Mathf.Sign(dummyDir.z) != Mathf.Sign(FlattenedDummyDir.z);
		bool fixPitch = dummyAngle > maxAngle || dummyAngle < minAngle;

		if(hasFlipped || fixPitch)
		{
			dummyDir = -target.forward;

			Vector3 correctedDir = FlattenedDummyDir;//Vector3.ProjectOnPlane(FlattenedDummyDir, Vector3.up).normalized;
			Vector3 correctedDirRight = Quaternion.AngleAxis(-90, Vector3.up) * correctedDir;

			Debug.DrawRay(transform.position, correctedDir, Color.white, 0, false);
			Debug.DrawRay(transform.position, correctedDirRight, Color.red, 0, false);

			Vector3 newDir = correctedDir;

			if (dummyAngle > maxAngle)
			{
				newDir = Quaternion.AngleAxis(maxAngle, correctedDirRight) * correctedDir;

				//Debug.Log("Fixed Max Pitch, dummy angle " + dummyAngle);
			}
			else if(dummyAngle < maxAngle)
			{
				newDir = Quaternion.AngleAxis(minAngle, correctedDirRight) * correctedDir;

				//Debug.Log("Fixed Min Pitch, dummy angle " + dummyAngle);
			}

			dummyDir = newDir;
		}
		prevPos = target.position + dummyDir * dampingDistance;
		FlattenedDummyDir = Vector3.ProjectOnPlane(dummyDir, Vector3.up).normalized; ;

		Vector3 xDir = Vector3.ProjectOnPlane(-dummyDir, Vector3.up).normalized;
		xDir = Quaternion.AngleAxis(90, Vector3.up) * xDir;

		//Debug.DrawRay(transform.position, xDir, Color.red, 0, false);
		//Debug.DrawRay(transform.position, -dummyDir, Color.blue, 0, false);

		Quaternion finalRotation = Quaternion.LookRotation(-dummyDir, Vector3.up);

		transform.rotation = Quaternion.Euler(finalRotation.eulerAngles.x + -pitch, finalRotation.eulerAngles.y, 0);
		transform.position = target.position + dummyDir * distance + transform.up * height;
	}
}
