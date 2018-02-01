Shader "SineVFX/LivingParticles/LivingParticleAlphaBlended"
{
	Properties
	{
		_FinalTexture("Final Texture", 2D) = "white" {}
		_FinalColor("Final Color", Color) = (1,0,0,1)
		_FinalColor2("Final Color 2", Color) = (0,0,0,0)
		_FinalPower("Final Power", Range( 0 , 10)) = 6
		_FinalMaskMultiply("Final Mask Multiply", Range( 0 , 10)) = 2
		[Toggle]_RampEnabled("Ramp Enabled", Int) = 0
		_Ramp("Ramp", 2D) = "white" {}
		_Distance("Distance", Float) = 1
		_DistancePower("Distance Power", Range( 0.2 , 4)) = 1
		_OffsetPower("Offset Power", Float) = 0
		[Toggle]_OffsetYLock("Offset Y Lock", Int) = 0
		[Toggle]_CameraFadeEnabled("Camera Fade Enabled", Int) = 1
		_CameraFadeDistance("Camera Fade Distance", Float) = 1
		_CameraFadeOffset("Camera Fade Offset", Float) = 0.2
		[Toggle]_CloseFadeEnabled("Close Fade Enabled", Int) = 0
		_CloseFadeDistance("Close Fade Distance", Float) = 0.65
		_SoftParticleDistance("Soft Particle Distance", Float) = 0.25
		[Toggle]_MaskAffectsTransparency("Mask Affects Transparency", Int) = 0
		_MaskOpacityPower("Mask Opacity Power", Range( 0 , 10)) = 1
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _RAMPENABLED_ON
		#pragma shader_feature _CAMERAFADEENABLED_ON
		#pragma shader_feature _CLOSEFADEENABLED_ON
		#pragma shader_feature _MASKAFFECTSTRANSPARENCY_ON
		#pragma shader_feature _OFFSETYLOCK_ON
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv_tex4coord;
			float4 uv2_tex4coord2;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 screenPos;
			float eyeDepth;
		};

		uniform sampler2D _Ramp;
		uniform sampler2D _FinalTexture;
		uniform float4 _FinalTexture_ST;
		uniform float4 _Affector;
		uniform float _Distance;
		uniform float _DistancePower;
		uniform float _FinalMaskMultiply;
		uniform float4 _FinalColor2;
		uniform float4 _FinalColor;
		uniform float _FinalPower;
		uniform float _MaskOpacityPower;
		uniform sampler2D _CameraDepthTexture;
		uniform float _SoftParticleDistance;
		uniform float _CameraFadeDistance;
		uniform float _CameraFadeOffset;
		uniform float _CloseFadeDistance;
		uniform float _OffsetPower;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
			float4 uv_FinalTexture40 = v.texcoord;
			uv_FinalTexture40.xy = v.texcoord.xy * _FinalTexture_ST.xy + _FinalTexture_ST.zw;
			float4 uv2_FinalTexture41 = v.texcoord1;
			uv2_FinalTexture41.xy = v.texcoord1.xy * _FinalTexture_ST.xy + _FinalTexture_ST.zw;
			float4 appendResult17 = (float4(uv_FinalTexture40.z , uv_FinalTexture40.w , uv2_FinalTexture41.x , 0.0));
			float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
			float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (1.0 - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
			float temp_output_33_0 = pow( clampResult23 , _DistancePower );
			float ResultMask53 = temp_output_33_0;
			float4 normalizeResult41 = normalize( ( appendResult17 - _Affector ) );
			float4 CenterVector44 = normalizeResult41;
			float3 temp_cast_0 = (1.0).xxx;
			#ifdef _OFFSETYLOCK_ON
				float3 staticSwitch49 = float3(1,0,1);
			#else
				float3 staticSwitch49 = temp_cast_0;
			#endif
			v.vertex.xyz += ( ResultMask53 * CenterVector44 * _OffsetPower * float4( staticSwitch49 , 0.0 ) ).xyz;
		}

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 uv_FinalTexture4 = i.uv_tex4coord;
			uv_FinalTexture4.xy = i.uv_tex4coord.xy * _FinalTexture_ST.xy + _FinalTexture_ST.zw;
			float4 uv2_FinalTexture4 = i.uv2_tex4coord2;
			uv2_FinalTexture4.xy = i.uv2_tex4coord2.xy * _FinalTexture_ST.xy + _FinalTexture_ST.zw;
			float4 appendResult17 = (float4(uv_FinalTexture4.z , uv_FinalTexture4.w , uv2_FinalTexture4.x , 0.0));
			float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
			float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (1.0 - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
			float temp_output_33_0 = pow( clampResult23 , _DistancePower );
			float ResultMask53 = temp_output_33_0;
			float clampResult88 = clamp( ( ResultMask53 * _FinalMaskMultiply ) , 0.0 , 1.0 );
			float2 appendResult83 = (float2(clampResult88 , 0.0));
			float4 lerpResult37 = lerp( _FinalColor2 , _FinalColor , clampResult88);
			#ifdef _RAMPENABLED_ON
				float4 staticSwitch81 = tex2D( _Ramp, appendResult83 );
			#else
				float4 staticSwitch81 = lerpResult37;
			#endif
			o.Emission = ( staticSwitch81 * i.vertexColor * _FinalPower ).rgb;
			float3 desaturateVar95 = lerp( staticSwitch81.rgb,dot(staticSwitch81.rgb,float3(0.299,0.587,0.114)).xxx,1.0);
			float2 uv_FinalTexture = i.uv_texcoord * _FinalTexture_ST.xy + _FinalTexture_ST.zw;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth56 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth56 = abs( ( screenDepth56 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _SoftParticleDistance ) );
			float clampResult58 = clamp( distanceDepth56 , 0.0 , 1.0 );
			float cameraDepthFade60 = (( i.eyeDepth -_ProjectionParams.y - _CameraFadeOffset ) / _CameraFadeDistance);
			float clampResult61 = clamp( cameraDepthFade60 , 0.0 , 1.0 );
			#ifdef _CAMERAFADEENABLED_ON
				float staticSwitch63 = clampResult61;
			#else
				float staticSwitch63 = 1.0;
			#endif
			float clampResult71 = clamp( ( ( temp_output_33_0 - _CloseFadeDistance ) * 8.0 ) , 0.0 , 1.0 );
			#ifdef _CLOSEFADEENABLED_ON
				float staticSwitch78 = clampResult71;
			#else
				float staticSwitch78 = 0.0;
			#endif
			float TooClose75 = staticSwitch78;
			float temp_output_57_0 = ( tex2D( _FinalTexture, uv_FinalTexture ).r * clampResult58 * staticSwitch63 * ( 1.0 - TooClose75 ) * i.vertexColor.a );
			#ifdef _MASKAFFECTSTRANSPARENCY_ON
				float staticSwitch91 = ( _MaskOpacityPower * desaturateVar95.x * temp_output_57_0 );
			#else
				float staticSwitch91 = temp_output_57_0;
			#endif
			float clampResult74 = clamp( staticSwitch91 , 0.0 , 1.0 );
			o.Alpha = clampResult74;
		}

		ENDCG
	}
}