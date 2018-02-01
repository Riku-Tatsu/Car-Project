using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVisualsLights : MonoBehaviour {

	CarPhysics carScript;

	[Header("Sources")]
	public Material lightMaterial;
	public LensFlare[] headlightFlares;
	public LensFlare[] brakelightFlares;
	public LensFlare[] reverselightFlares;

	[Header("Appearance")]
	public Color headlights;
	public Color brakelights;
	public Color reverselights;
	public float unlitBrightness = 0.15f;
	public float fadeRate = 4.0f;
	[Space]
	public float flareFullAngle = 10.0f;
	public float flareObscuredAngle = 60.0f;

	public Material lightMat;
	bool useHeadlights = false;

	// Use this for initialization
	void Start ()
	{
		carScript = GetComponent<CarPhysics>();

		if(lightMaterial)
		{
			lightMat = new Material(lightMaterial);
			lightMat.name = "Instanced Lights";

			MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

			for(int i = 0; i < renderers.Length; i++)
			{
				for(int j = 0; j < renderers[i].sharedMaterials.Length; j++)
				{
					if(renderers[i].sharedMaterials[j] == lightMaterial)
					{
						Debug.Log("found material match at " + renderers[i].name);
						Material[] mats = renderers[i].sharedMaterials;
						mats[j] = lightMat;
						renderers[i].sharedMaterials = mats;

						if(renderers[i].sharedMaterials[j] == lightMat)
						{
							Debug.Log("material replacement successful");
						}
					}
				}
			}

			Debug.Log(Shader.PropertyToID("Headglass"));

			lightMat.SetColor("_Headglass", headlights * unlitBrightness);
			lightMat.SetColor("_Tailglass", brakelights * unlitBrightness);
			lightMat.SetColor("_Reverseglass", reverselights * unlitBrightness);

			lightMat.SetColor("_Headlights", headlights);
			lightMat.SetColor("_Taillights", brakelights);
			lightMat.SetColor("_Reverselights", reverselights);
			lightMat.SetFloat("_Hlight", 1);
			lightMat.SetFloat("_Tlight", 1);
			lightMat.SetFloat("_Rlight", 1);
		}
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if(carScript)
		{
			if(lightMat)
			{
				if(carScript.headlightsOn)
				{
					lightMat.SetFloat("_Hlight", Mathf.Lerp(lightMat.GetFloat("_Hlight"), 1, Time.deltaTime * fadeRate));
				}
				else
				{
					lightMat.SetFloat("_Hlight", Mathf.Lerp(lightMat.GetFloat("_Hlight"), unlitBrightness, Time.deltaTime * fadeRate));
				}

				if (carScript.isBraking)
				{
					lightMat.SetFloat("_Tlight", Mathf.Lerp(lightMat.GetFloat("_Tlight"), 1, Time.deltaTime * fadeRate));
				}
				else
				{
					lightMat.SetFloat("_Tlight", Mathf.Lerp(lightMat.GetFloat("_Tlight"), unlitBrightness, Time.deltaTime * fadeRate));
				}

				if (carScript.isReversing)
				{
					lightMat.SetFloat("_Rlight", Mathf.Lerp(lightMat.GetFloat("_Rlight"), 1, Time.deltaTime * fadeRate));
				}
				else
				{
					lightMat.SetFloat("_Rlight", Mathf.Lerp(lightMat.GetFloat("_Rlight"), unlitBrightness, Time.deltaTime * fadeRate));
				}

				for (int i = 0; i < headlightFlares.Length; i++)
				{
					float brightness = (Vector3.Dot(headlightFlares[i].transform.forward, Camera.main.transform.forward) + 1) * 90;
					brightness = Mathf.InverseLerp(flareObscuredAngle, flareFullAngle, brightness);

					if (carScript.headlightsOn)
					{
						headlightFlares[i].color = Color.Lerp(headlightFlares[i].color, headlights * brightness, Time.deltaTime * fadeRate);
					}
					else
					{
						headlightFlares[i].color = Color.Lerp(headlightFlares[i].color, Color.black, Time.deltaTime * fadeRate);
					}
				}

				for (int i = 0; i < brakelightFlares.Length; i++)
				{
					float brightness = (Vector3.Dot(brakelightFlares[i].transform.forward, Camera.main.transform.forward) + 1) * 90;
					brightness = Mathf.InverseLerp(flareObscuredAngle, flareFullAngle, brightness);

					if(carScript.isBraking)
					{
						brakelightFlares[i].color = Color.Lerp(brakelightFlares[i].color, brakelights * brightness, Time.deltaTime * fadeRate);
					}
					else
					{
						brakelightFlares[i].color = Color.Lerp(brakelightFlares[i].color, Color.black, Time.deltaTime * fadeRate);
					}
				}

				for (int i = 0; i < reverselightFlares.Length; i++)
				{
					float brightness = (Vector3.Dot(reverselightFlares[i].transform.forward, Camera.main.transform.forward) + 1) * 90;
					brightness = Mathf.InverseLerp(flareObscuredAngle, flareFullAngle, brightness);

					if (carScript.isReversing)
					{
						reverselightFlares[i].color = Color.Lerp(reverselightFlares[i].color, reverselights * brightness, Time.deltaTime * fadeRate);
					}
					else
					{
						reverselightFlares[i].color = Color.Lerp(reverselightFlares[i].color, Color.black, Time.deltaTime * fadeRate);
					}
				}
			}
		}	
	}
}
