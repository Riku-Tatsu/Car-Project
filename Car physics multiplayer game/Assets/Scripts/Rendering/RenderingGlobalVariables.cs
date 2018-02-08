using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingGlobalVariables : MonoBehaviour {

	bool frameSign = true;

	float frameTimer = 0;

	void Start ()
	{
		frameTimer = 1f / Screen.currentResolution.refreshRate;
	}
	
	
	void LateUpdate ()
	{
		if(frameTimer > 0)
		{
			frameTimer -= Time.deltaTime;
		}
		else
		{
			frameTimer = 1f / Screen.currentResolution.refreshRate;

			frameSign = !frameSign;

			Shader.SetGlobalFloat("_FrameSign", frameSign ? 1 : -1);
		}
		
	}
}
