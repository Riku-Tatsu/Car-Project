Shader "CalmWater/Calm Water [Single Sided]"{
	Properties {
		_Color("Shallow Color",Color) = (1,1,1,1)
		_DepthColor("Depth Color",Color) = (0,0,0,0)
		_Depth("Depth",float) = 0.5

		[Toggle(_DEPTHFOG_ON)] _EnableFog ("Enable Depth Fog", Float) = 0		
		_EdgeFade("Edge Fade",float) = 1
		
		//Spec
		_SpecColor ("SpecularColor",Color) = (1,1,1,1)
		_Smoothness ("Smoothness",Range(0.01,5)) = 0.5
		
		//Normal Map
		_BumpMap("Micro Detail", 2D) = "bump" {}
		_BumpStrength ("Bump Strength",Range(0,1)) = 1

		[Toggle(_BUMPLARGE_ON)] _EnableLargeBump ("Enable Large Detail", Float) = 0
		_BumpMapLarge("Large Detail", 2D) = "bump" {}
		_BumpLargeStrength ("Bump Large Strength",Range(0,1)) = 1

		//Animation
		[Toggle(_WORLDSPACE_ON)] _WorldSpace ("World UV", Float) = 0
		_Speeds ("Speeds",vector) = (0.5,0.5,0.5,0.5)
		_SpeedsLarge ("Speeds Large",vector) = (0.5,0.5,0,0)

		//[Header(Distortion)]
		//Distortion
		_Distortion("Distortion", Range(0,100) ) = 50.0
		[KeywordEnum(High,Low)]
		_DistortionQuality("Distortion Quality",Float) = 0

		//[Header(Reflection)]
		//Reflection
		[KeywordEnum(None,Mixed,RealTime,CubeMap)] 
		_ReflectionType("ReflectionType", Float)  = 0
		
		_CubeColor("CubeMap Color [RGB] Intensity [A]",Color) = (1,1,1,1)
		[NoScaleOffset]
		_Cube("CubeMap", Cube) = "black" {}
		[NoScaleOffset]
		_ReflectionTex ("Internal reflection", 2D) = "white" {}
		
		_Reflection("Reflection", Range(0,1) ) = 1
		_RimPower("Fresnel Angle", Range(1,20) ) = 5

		//[Header(Foam)]
		//Foam
		[Toggle(_FOAM_ON)] _FOAM ("Enable Foam", Float) = 0
		_FoamColor("FoamColor",Color) = (1,1,1,1)
		_FoamTex("Foam Texture", 2D) = "white" {}
		_FoamSize("Fade Size",float) = 0.5

		//[Header(Displacement)]
		//Displacement
		[KeywordEnum(Off,Wave,Gerstner)] 
		_DisplacementMode("Mode", Float)  = 0

		//WaveMode
		_Amplitude("Amplitude", float) = 0.05
		_Frequency("Frequency",float) = 1
		_Speed("Wave Speed", float) = 1

		//GerstnerMode
		_Steepness ("Wave Steepness",float) = 1
		_WSpeed ("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
		_WDirectionAB ("Wave1 Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
		_WDirectionCD ("Wave2 Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)

		//Normal Smoothing
		_Smoothing("Smoothing",range(0,1)) = 1

	}
	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" "PreviewType"="Sphere" }
		ZWrite Off
		SubShader {
	 		GrabPass
			{
				"_GrabTexture"
				Name "BASE"
				Tags { 
					"LightMode" = "Always" 
				}
			}

	        Pass {
	       		Tags {"LightMode" = "ForwardBase"}
	            Name "FORWARD"
	            Cull Back
	 
	            CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma multi_compile_fwdbase
	            #pragma multi_compile_fog
	            #pragma fragmentoption ARB_precision_hint_fastest
	       
	            #pragma target 3.0
	            #define UNITY_PASS_FORWARDBASE

	            #include "UnityCG.cginc"
	            #include "AutoLight.cginc"
	            #include "CalmWater.cginc"

	            #pragma shader_feature _ _BUMPLARGE_ON
				#pragma shader_feature _REFLECTIONTYPE_MIXED _REFLECTIONTYPE_CUBEMAP _REFLECTIONTYPE_REALTIME 
				#pragma shader_feature _DISTORTIONQUALITY_HIGH _DISTORTIONQUALITY_LOW
				#pragma shader_feature _ _FOAM_ON
				#pragma shader_feature _ _DEPTHFOG_ON
				#pragma shader_feature _ _DISPLACEMENTMODE_WAVE _DISPLACEMENTMODE_GERSTNER
				#pragma shader_feature _ _WORLDSPACE_ON

	            ENDCG
	        }

	        Pass {
	            Tags {"LightMode" = "ForwardAdd"}
	            Name "FORWARDADD"
	            Blend One One
	            Cull Back
	            CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma multi_compile_fwdadd_fullshadows
	            #pragma fragmentoption ARB_precision_hint_fastest
	            #pragma target 3.0

	            #define UNITY_PASS_FORWARDADD
	            #include "UnityCG.cginc"
	            #include "AutoLight.cginc"
	            #include "CalmWater.cginc"

	            #pragma shader_feature _ _BUMPLARGE_ON
				#pragma shader_feature _REFLECTIONTYPE_MIXED _REFLECTIONTYPE_CUBEMAP _REFLECTIONTYPE_REALTIME 
				#pragma shader_feature _DISTORTIONQUALITY_HIGH _DISTORTIONQUALITY_LOW
				#pragma shader_feature _ _FOAM_ON
				#pragma shader_feature _ _DEPTHFOG_ON
				#pragma shader_feature _ _DISPLACEMENTMODE_WAVE _DISPLACEMENTMODE_GERSTNER
				#pragma shader_feature _ _WORLDSPACE_ON

	        	ENDCG
	        }

	    }
    }
    CustomEditor "CalmWaterInspector"
    FallBack Off
}
 