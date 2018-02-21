using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarVisuals_SolidAxel : NetworkBehaviour {

	[System.Serializable]
	public class AxelData
	{
		public bool isFront;
		public float minSuspensionHeight;
		[Space]
		public Transform wheelMeshLeft;
		public Transform wheelMeshRight;
		public int wheelIndexLeft;
		public int wheelIndexRight;
		[Space]
		public Transform hubLeft;
		public Transform hubRight;
		[Space]
		public Transform axel;
		public Transform armLeft;
		public Transform armRight;
		[HideInInspector]
		public float armLengthLeft;
		[HideInInspector]
		public float armLengthRight;

		public Transform armTargetLeft;
		public Transform armTargetRight;
		[Space]
		public Transform driveshaftCentreDiff;
		public Transform driveshaftCentreAxel;
		public Transform driveshaftEndAxel;
		public Transform driveshaftEndDiff;
	}

	[System.Serializable]
	public class SpringPair
	{
		public Transform springUpper;
		public Transform springLower;
		public Vector3 localUpAxis;
	}

	public AxelData[] axelData;
	public SpringPair[] springPairs;

	CarPhysics carScript;


	void Start()
	{
		carScript = GetComponent<CarPhysics>();

		for(int i = 0; i < axelData.Length; i++)
		{
			axelData[i].armLengthLeft = Vector3.Distance(axelData[i].armLeft.position, axelData[i].armTargetLeft.position);
			axelData[i].armLengthRight = Vector3.Distance(axelData[i].armRight.position, axelData[i].armTargetRight.position);
		}
	}

    
	void LateUpdate ()
	{
		for(int i = 0; i < axelData.Length; i++)
		{
			axelData[i].hubLeft.localRotation = Quaternion.Euler(0, carScript.wheels[axelData[i].wheelIndexLeft].wheelPivot.localEulerAngles.y, 0);
			axelData[i].hubRight.localRotation = Quaternion.Euler(0, carScript.wheels[axelData[i].wheelIndexRight].wheelPivot.localEulerAngles.y, 0);

			axelData[i].wheelMeshLeft.localRotation = Quaternion.Euler(carScript.wheels[axelData[i].wheelIndexLeft].rotation, 0, 0);
			axelData[i].wheelMeshRight.localRotation = Quaternion.Euler(carScript.wheels[axelData[i].wheelIndexRight].rotation, 0, 0);

			Vector3 armTgtLeftOffset = axelData[i].armTargetLeft.localPosition;
			Vector3 armTgtRightOffset = axelData[i].armTargetRight.localPosition;
			Vector3 armLocalAxelOffset = (armTgtLeftOffset + armTgtRightOffset) * 0.5f;

			float hitDistanceLeftClamped = Mathf.Clamp(carScript.wheels[axelData[i].wheelIndexLeft].rHit.distance, axelData[i].minSuspensionHeight, carScript.sMaxDistance);
			float hitDistanceRightClamped = Mathf.Clamp(carScript.wheels[axelData[i].wheelIndexRight].rHit.distance, axelData[i].minSuspensionHeight, carScript.sMaxDistance);

			Vector3 leftWheelPos = (hitDistanceLeftClamped - carScript.wheels[axelData[i].wheelIndexLeft].wheelRadius) * -transform.up + carScript.wheels[axelData[i].wheelIndexLeft].wheelPivot.position;
			Vector3 rightWheelPos = (hitDistanceRightClamped - carScript.wheels[axelData[i].wheelIndexRight].wheelRadius) * -transform.up + carScript.wheels[axelData[i].wheelIndexRight].wheelPivot.position;
			Vector3 averageWheelPos = (leftWheelPos + rightWheelPos) * 0.5f;

			Vector3 armsCentrePos = (axelData[i].armLeft.position + axelData[i].armRight.position) * 0.5f;

			axelData[i].axel.position = averageWheelPos;
			Vector3 axelUp = Quaternion.AngleAxis(-90, transform.forward) * (leftWheelPos - rightWheelPos).normalized;
			Vector3 axelFwd = (averageWheelPos - armsCentrePos).normalized * (axelData[i].isFront ? 1 : -1);
			Quaternion axelRot = Quaternion.LookRotation(axelFwd, axelUp);
			axelData[i].axel.rotation = axelRot;

			axelData[i].armLeft.LookAt(axelData[i].armTargetLeft, transform.up);
			axelData[i].armRight.LookAt(axelData[i].armTargetRight, transform.up);

			Vector3 armEndLeft = axelData[i].armLeft.position + axelData[i].armLeft.forward * axelData[i].armLengthLeft;
			Vector3 armEndRight = axelData[i].armRight.position + axelData[i].armRight.forward * axelData[i].armLengthRight;
			Vector3 armEndCentre = (armEndLeft + armEndRight) * 0.5f;

			axelData[i].axel.position = armEndCentre + -axelFwd * armLocalAxelOffset.z;
			axelUp = Quaternion.AngleAxis(-90, transform.forward) * (armEndLeft - armEndRight).normalized;
			axelFwd = (armEndCentre - armsCentrePos).normalized * (axelData[i].isFront ? 1 : -1);
			axelRot = Quaternion.LookRotation(axelFwd, axelUp);
			axelData[i].axel.rotation = axelRot;

			axelData[i].driveshaftCentreAxel.LookAt(axelData[i].driveshaftEndAxel.position, transform.up);
			axelData[i].driveshaftEndAxel.LookAt(axelData[i].driveshaftCentreAxel.position, transform.up);
		}

		for(int i = 0; i < springPairs.Length; i++)
		{
			springPairs[i].springUpper.LookAt(springPairs[i].springLower.position, transform.TransformDirection(springPairs[i].localUpAxis));
			springPairs[i].springLower.LookAt(springPairs[i].springUpper.position, transform.TransformDirection(springPairs[i].localUpAxis));
		}
	}

}
