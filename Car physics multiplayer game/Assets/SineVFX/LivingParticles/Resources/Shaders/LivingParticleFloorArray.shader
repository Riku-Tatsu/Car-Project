Shader "SineVFX/LivingParticles/LivingParticleFloorArray"
{
	Properties
	{
		_AffectorCount("Affector Count", Int) = 5
		_FinalTexture("Final Texture", 2D) = "white" {}
		_FinalColor("Final Color", Color) = (1,0,0,1)
		_FinalColor2("Final Color 2", Color) = (0,0,0,0)
		_FinalPower("Final Power", Range( 0 , 10)) = 6
		_FinalExp("Final Exp", Range( 0.2 , 4)) = 2
		_FinalMaskMultiply("Final Mask Multiply", Range( 0 , 10)) = 2
		[Toggle]_RampEnabled("Ramp Enabled", Int) = 0
		_Ramp("Ramp", 2D) = "white" {}
		_Distance("Distance", Float) = 1
		_DistancePower("Distance Power", Range( 0.2 , 4)) = 1
		_OffsetPower("Offset Power", Float) = 0
		[Toggle]_IgnoreYAxis("Ignore Y Axis", Int) = 0
		_Noise01("Noise 01", 2D) = "white" {}
		_Noise02("Noise 02", 2D) = "white" {}
		_Noise01ScrollSpeed("Noise 01 Scroll Speed", Float) = 0.1
		_Noise02ScrollSpeed("Noise 02 Scroll Speed", Float) = 0.15
		_NoiseDistortion("Noise Distortion", 2D) = "white" {}
		_NoiseDistortionScrollSpeed("Noise Distortion Scroll Speed", Float) = 0.05
		_NoiseDistortionPower("Noise Distortion Power", Range( 0 , 0.2)) = 0.1
		_NoisePower("Noise Power", Range( 0 , 10)) = 4
		_NoiseTiling("Noise Tiling", Float) = 0.25
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _IGNOREYAXIS_ON
		#pragma shader_feature _RAMPENABLED_ON
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv_tex4coord;
			float4 uv2_tex4coord2;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform sampler2D _Ramp;
		uniform sampler2D _FinalTexture;
		uniform float4 _FinalTexture_ST;
		uniform float4 _Affector;
		uniform float _Distance;
		uniform float _DistancePower;
		uniform float _FinalMaskMultiply;
		uniform sampler2D _Noise02;
		uniform float _Noise02ScrollSpeed;
		uniform float _NoiseTiling;
		uniform sampler2D _NoiseDistortion;
		uniform float _NoiseDistortionScrollSpeed;
		uniform float _NoiseDistortionPower;
		uniform sampler2D _Noise01;
		uniform float _Noise01ScrollSpeed;
		uniform float _NoisePower;
		uniform float4 _FinalColor2;
		uniform float4 _FinalColor;
		uniform float _FinalExp;
		uniform float _FinalPower;
		uniform float _OffsetPower;
		uniform int _AffectorCount;
		uniform float4 _Affectors[20];

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 uv_FinalTexture40 = v.texcoord;
			uv_FinalTexture40.xy = v.texcoord.xy * _FinalTexture_ST.xy + _FinalTexture_ST.zw;
			float4 uv2_FinalTexture41 = v.texcoord1;
			uv2_FinalTexture41.xy = v.texcoord1.xy * _FinalTexture_ST.xy + _FinalTexture_ST.zw;
			float4 appendResult17 = (float4(uv_FinalTexture40.z , uv_FinalTexture40.w , uv2_FinalTexture41.x , 0.0));
			#ifdef _IGNOREYAXIS_ON
				float3 staticSwitch93 = float3(1,0,1);
			#else
				float3 staticSwitch93 = float3(1,1,1);
			#endif



			float DistanceMask45;
			for (int w = 0; w < _AffectorCount; w++) {
			if(w == 0){
			DistanceMask45 = distance( ( appendResult17 * float4( staticSwitch93 , 0.0 ) ) , ( _Affectors[w] * float4( staticSwitch93 , 0.0 ) ) );
			}else{
			DistanceMask45 = min( DistanceMask45, distance( ( appendResult17 * float4( staticSwitch93 , 0.0 ) ) , ( _Affectors[w] * float4( staticSwitch93 , 0.0 ) ) ) );	
			}
			}
			//float DistanceMask45 = distance( ( appendResult17 * float4( staticSwitch93 , 0.0 ) ) , ( _Affector * float4( staticSwitch93 , 0.0 ) ) );
			DistanceMask45 = 1 - DistanceMask45;



			float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (1.0 - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
			float ResultMask53 = pow( clampResult23 , _DistancePower );
			float2 appendResult104 = (float2(appendResult17.x , appendResult17.z));
			float2 ParticlePositionUV106 = appendResult104;
			float2 temp_output_119_0 = ( ParticlePositionUV106 * _NoiseTiling );
			float2 panner109 = ( temp_output_119_0 + ( _Time.y * _Noise02ScrollSpeed ) * float2( 1,1 ));
			float2 panner131 = ( temp_output_119_0 + ( _Time.y * _NoiseDistortionScrollSpeed ) * float2( 0.05,0.05 ));
			float4 tex2DNode132 = tex2Dlod( _NoiseDistortion, float4( panner131, 0, 0.0) );
			float2 temp_cast_2 = (tex2DNode132.r).xx;
			float2 lerpResult127 = lerp( panner109 , temp_cast_2 , _NoiseDistortionPower);
			float2 panner110 = ( temp_output_119_0 + ( _Time.y * _Noise01ScrollSpeed ) * float2( 1,1 ));
			float2 temp_cast_3 = (tex2DNode132.r).xx;
			float2 lerpResult128 = lerp( panner110 , temp_cast_3 , _NoiseDistortionPower);
			float ResultNoise111 = ( tex2Dlod( _Noise02, float4( lerpResult127, 0, 0.0) ).r * tex2Dlod( _Noise01, float4( lerpResult128, 0, 0.0) ).r * _NoisePower );
			float clampResult88 = clamp( ( ( ResultMask53 * _FinalMaskMultiply ) + ResultNoise111 ) , 0.0 , 1.0 );
			float ResultMaskModified90 = clampResult88;
			v.vertex.xyz += ( ( 1.0 - ResultMaskModified90 ) * _OffsetPower * float3(0,1,0) );
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
			#ifdef _IGNOREYAXIS_ON
				float3 staticSwitch93 = float3(1,0,1);
			#else
				float3 staticSwitch93 = float3(1,1,1);
			#endif



			float DistanceMask45;
			for (int w = 0; w < _AffectorCount; w++) {
			if(w == 0){
			DistanceMask45 = distance( ( appendResult17 * float4( staticSwitch93 , 0.0 ) ) , ( _Affectors[w] * float4( staticSwitch93 , 0.0 ) ) );
			}else{
			DistanceMask45 = min( DistanceMask45, distance( ( appendResult17 * float4( staticSwitch93 , 0.0 ) ) , ( _Affectors[w] * float4( staticSwitch93 , 0.0 ) ) ) );	
			}
			}
			//float DistanceMask45 = distance( ( appendResult17 * float4( staticSwitch93 , 0.0 ) ) , ( _Affector * float4( staticSwitch93 , 0.0 ) ) );
			DistanceMask45 = 1 - DistanceMask45;



			float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (1.0 - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
			float ResultMask53 = pow( clampResult23 , _DistancePower );
			float2 appendResult104 = (float2(appendResult17.x , appendResult17.z));
			float2 ParticlePositionUV106 = appendResult104;
			float2 temp_output_119_0 = ( ParticlePositionUV106 * _NoiseTiling );
			float2 panner109 = ( temp_output_119_0 + ( _Time.y * _Noise02ScrollSpeed ) * float2( 1,1 ));
			float2 panner131 = ( temp_output_119_0 + ( _Time.y * _NoiseDistortionScrollSpeed ) * float2( 0.05,0.05 ));
			float4 tex2DNode132 = tex2D( _NoiseDistortion, panner131 );
			float2 temp_cast_2 = (tex2DNode132.r).xx;
			float2 lerpResult127 = lerp( panner109 , temp_cast_2 , _NoiseDistortionPower);
			float2 panner110 = ( temp_output_119_0 + ( _Time.y * _Noise01ScrollSpeed ) * float2( 1,1 ));
			float2 temp_cast_3 = (tex2DNode132.r).xx;
			float2 lerpResult128 = lerp( panner110 , temp_cast_3 , _NoiseDistortionPower);
			float ResultNoise111 = ( tex2D( _Noise02, lerpResult127 ).r * tex2D( _Noise01, lerpResult128 ).r * _NoisePower );
			float clampResult88 = clamp( ( ( ResultMask53 * _FinalMaskMultiply ) + ResultNoise111 ) , 0.0 , 1.0 );
			float ResultMaskModified90 = clampResult88;
			float2 appendResult83 = (float2(ResultMaskModified90 , 0.0));
			float4 lerpResult37 = lerp( _FinalColor2 , _FinalColor , pow( ResultMaskModified90 , _FinalExp ));
			#ifdef _RAMPENABLED_ON
				float4 staticSwitch81 = tex2D( _Ramp, appendResult83 );
			#else
				float4 staticSwitch81 = lerpResult37;
			#endif
			float2 uv_FinalTexture = i.uv_texcoord * _FinalTexture_ST.xy + _FinalTexture_ST.zw;
			o.Emission = ( staticSwitch81 * i.vertexColor * _FinalPower * tex2D( _FinalTexture, uv_FinalTexture ).r ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}