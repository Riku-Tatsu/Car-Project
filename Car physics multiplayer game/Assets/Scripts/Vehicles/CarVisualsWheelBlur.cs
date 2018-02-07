using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVisualsWheelBlur : MonoBehaviour {

	[System.Serializable]
	public class WheelInfoList
	{
		public Material material;
		[Space]
		public int leftFront;
		public int rightFront;
		public int leftRear;
		public int rightRear;

		[Tooltip("Number of segments of the wheel. When it rotates an equal or greater degrees to this count it'll reach full blur.")]
		public float segmentsToMatch;

		[HideInInspector]
		public float leftFrontPrevRotation;
		[HideInInspector]
		public float rightFrontPrevRotation;
		[HideInInspector]
		public float leftRearPrevRotation;
		[HideInInspector]
		public float rightRearPrevRotation;
	}

	[Header("Renderer Info")]
	public MeshRenderer[] meshes;
	public SkinnedMeshRenderer[] skinnedMeshes;


	[Header("Settings")]
	public WheelInfoList[] wheelInfo;
	

	CarPhysics carScript;
	Material[] matInstances;
	float refreshTimer = 0;


	void Start ()
	{
		carScript = GetComponent<CarPhysics>();

		refreshTimer = 1f / Screen.currentResolution.refreshRate;

		if (wheelInfo.Length > 0)
		{
			matInstances = new Material[wheelInfo.Length];
			for(int x = 0; x < wheelInfo.Length; x++)
			{
				matInstances[x] = new Material(wheelInfo[x].material);
				matInstances[x].name = "Material Instance " + x.ToString();

				for (int i = 0; i < meshes.Length; i++)
				{
					for (int j = 0; j < meshes[i].sharedMaterials.Length; j++)
					{
						if (meshes[i].sharedMaterials[j] == wheelInfo[x].material)
						{
							Debug.Log("found material match at " + meshes[i].name);
							Material[] mats = meshes[i].sharedMaterials;
							mats[j] = matInstances[x];
							meshes[i].sharedMaterials = mats;

							if (meshes[i].sharedMaterials[j] == matInstances[x])
							{
								Debug.Log("material replacement successful");
							}
						}
					}
				}

				for (int i = 0; i < skinnedMeshes.Length; i++)
				{
					for (int j = 0; j < skinnedMeshes[i].sharedMaterials.Length; j++)
					{
						if (skinnedMeshes[i].sharedMaterials[j] == wheelInfo[x].material)
						{
							Debug.Log("found material match at " + skinnedMeshes[i].name);
							Material[] mats = skinnedMeshes[i].sharedMaterials;
							mats[j] = matInstances[x];
							skinnedMeshes[i].sharedMaterials = mats;

							if (skinnedMeshes[i].sharedMaterials[j] == matInstances[x])
							{
								Debug.Log("material replacement successful");
							}
						}
					}
				}
			}


		}
	}
	

	void LateUpdate ()
	{
		if(refreshTimer > 0)
		{
			refreshTimer -= Time.deltaTime;
		}
		else
		{
			refreshTimer = 1f / Screen.currentResolution.refreshRate;

			for (int i = 0; i < wheelInfo.Length; i++)
			{
				float maxRotation = 360f / wheelInfo[i].segmentsToMatch;

				float leftFrontRotation = Mathf.Abs(carScript.wheels[wheelInfo[i].leftFront].rotation - wheelInfo[i].leftFrontPrevRotation);
				float rightFrontRotation = Mathf.Abs(carScript.wheels[wheelInfo[i].rightFront].rotation - wheelInfo[i].rightFrontPrevRotation);
				float leftRearRotation = Mathf.Abs(carScript.wheels[wheelInfo[i].leftRear].rotation - wheelInfo[i].leftRearPrevRotation);
				float rightRearRotation = Mathf.Abs(carScript.wheels[wheelInfo[i].rightRear].rotation - wheelInfo[i].rightRearPrevRotation);

				float lfBlur = Mathf.Clamp01(leftFrontRotation / maxRotation);
				float rfBlur = Mathf.Clamp01(rightFrontRotation / maxRotation);
				float lrBlur = Mathf.Clamp01(leftRearRotation / maxRotation);
				float rrBlur = Mathf.Clamp01(rightRearRotation / maxRotation);

				matInstances[i].SetVector("_Blur", new Vector4(lfBlur, rfBlur, lrBlur, rrBlur));

				wheelInfo[i].leftFrontPrevRotation = carScript.wheels[wheelInfo[i].leftFront].rotation;
				wheelInfo[i].rightFrontPrevRotation = carScript.wheels[wheelInfo[i].rightFront].rotation;
				wheelInfo[i].leftRearPrevRotation = carScript.wheels[wheelInfo[i].leftRear].rotation;
				wheelInfo[i].rightRearPrevRotation = carScript.wheels[wheelInfo[i].rightRear].rotation;
			}
		}

	}
}
