Shader "SineVFX/LivingParticles/LivingParticleMaskedPbr"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_ColorTint("Color Tint", Color) = (1,1,1,1)
		_MetallicSmoothness("MetallicSmoothness", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_Normal("Normal", 2D) = "bump" {}
		_Emission("Emission", 2D) = "white" {}
		_FinalColor("Final Color", Color) = (1,0,0,1)
		_FinalColor2("Final Color 2", Color) = (0,0,0,0)
		_FinalPower("Final Power", Range( 0 , 10)) = 6
		_FinalMaskMultiply("Final Mask Multiply", Range( 0 , 10)) = 2
		[Toggle]_RampEnabled("Ramp Enabled", Int) = 0
		_Ramp("Ramp", 2D) = "white" {}
		_Distance("Distance", Float) = 1
		_DistancePower("Distance Power", Range( 0.2 , 4)) = 1
		[Toggle]_OffsetYLock("Offset Y Lock", Int) = 0
		_OffsetPower("Offset Power", Float) = 0
		[Toggle]_CenterMaskEnabled("Center Mask Enabled", Int) = 0
		_CenterMaskMultiply("Center Mask Multiply", Float) = 4
		_CenterMaskSubtract("Center Mask Subtract", Float) = 0.75
		[Toggle]_VertexDistortionEnabled("Vertex Distortion Enabled", Int) = 0
		_VertexOffsetTexture("Vertex Offset Texture", 2D) = "white" {}
		_VertexDistortionPower("Vertex Distortion Power", Float) = 0.1
		_VertexDistortionTiling("Vertex Distortion Tiling", Float) = 1
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 4.0
		#pragma shader_feature _RAMPENABLED_ON
		#pragma shader_feature _VERTEXDISTORTIONENABLED_ON
		#pragma shader_feature _CENTERMASKENABLED_ON
		#pragma shader_feature _OFFSETYLOCK_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			fixed ASEVFace : VFACE;
			float4 uv_tex4coord;
			float4 uv2_tex4coord2;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _ColorTint;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Ramp;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _Affector;
		uniform float _Distance;
		uniform float _DistancePower;
		uniform float _FinalMaskMultiply;
		uniform float4 _FinalColor2;
		uniform float4 _FinalColor;
		uniform float _FinalPower;
		uniform float _Metallic;
		uniform sampler2D _MetallicSmoothness;
		uniform float4 _MetallicSmoothness_ST;
		uniform float _Smoothness;
		uniform float _VertexDistortionPower;
		uniform sampler2D _VertexOffsetTexture;
		uniform float _VertexDistortionTiling;
		uniform float _CenterMaskSubtract;
		uniform float _CenterMaskMultiply;
		uniform float _OffsetPower;
		uniform float _Cutoff = 0.5;


		inline float4 TriplanarSamplingSV( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2Dlod( topTexMap, float4( ( tilling * worldPos.zy * float2( nsign.x, 1.0 ) ).xy, 0, 0 ) ) );
			yNorm = ( tex2Dlod( topTexMap, float4( ( tilling * worldPos.xz * float2( nsign.y, 1.0 ) ).xy, 0, 0 ) ) );
			zNorm = ( tex2Dlod( topTexMap, float4( ( tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ).xy, 0, 0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float4 triplanar111 = TriplanarSamplingSV( _VertexOffsetTexture, ase_worldPos, ase_worldNormal, 1.0, _VertexDistortionTiling );
			float3 appendResult115 = (float3(triplanar111.x , triplanar111.y , triplanar111.z));
			float3 temp_cast_0 = (0.0).xxx;
			#ifdef _VERTEXDISTORTIONENABLED_ON
				float3 staticSwitch120 = ( _VertexDistortionPower * (float3( -1,-1,-1 ) + (appendResult115 - float3( 0,0,0 )) * (float3( 1,1,1 ) - float3( -1,-1,-1 )) / (float3( 1,1,1 ) - float3( 0,0,0 ))) );
			#else
				float3 staticSwitch120 = temp_cast_0;
			#endif
			float4 uv_Emission40 = v.texcoord;
			uv_Emission40.xy = v.texcoord.xy * _Emission_ST.xy + _Emission_ST.zw;
			float4 uv2_Emission41 = v.texcoord1;
			uv2_Emission41.xy = v.texcoord1.xy * _Emission_ST.xy + _Emission_ST.zw;
			float4 appendResult17 = (float4(uv_Emission40.z , uv_Emission40.w , uv2_Emission41.x , 0.0));
			float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
			float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (1.0 - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
			float ResultMask53 = pow( clampResult23 , _DistancePower );
			float clampResult105 = clamp( ( ResultMask53 - _CenterMaskSubtract ) , 0.0 , 1.0 );
			#ifdef _CENTERMASKENABLED_ON
				float staticSwitch109 = ( ResultMask53 - ( clampResult105 * _CenterMaskMultiply ) );
			#else
				float staticSwitch109 = ResultMask53;
			#endif
			float4 normalizeResult41 = normalize( ( appendResult17 - _Affector ) );
			float4 CenterVector44 = normalizeResult41;
			float3 temp_cast_2 = (1.0).xxx;
			#ifdef _OFFSETYLOCK_ON
				float3 staticSwitch49 = float3(1,0,1);
			#else
				float3 staticSwitch49 = temp_cast_2;
			#endif
			v.vertex.xyz += ( float4( staticSwitch120 , 0.0 ) + ( staticSwitch109 * CenterVector44 * _OffsetPower * float4( staticSwitch49 , 0.0 ) ) ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode91 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float3 switchResult125 = (((i.ASEVFace>0)?(( float3(1,1,1) * tex2DNode91 )):(( tex2DNode91 * float3(-1,-1,-1) ))));
			o.Normal = switchResult125;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode89 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( _ColorTint * tex2DNode89 ).rgb;
			float4 uv_Emission4 = i.uv_tex4coord;
			uv_Emission4.xy = i.uv_tex4coord.xy * _Emission_ST.xy + _Emission_ST.zw;
			float4 uv2_Emission4 = i.uv2_tex4coord2;
			uv2_Emission4.xy = i.uv2_tex4coord2.xy * _Emission_ST.xy + _Emission_ST.zw;
			float4 appendResult17 = (float4(uv_Emission4.z , uv_Emission4.w , uv2_Emission4.x , 0.0));
			float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
			float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (1.0 - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
			float ResultMask53 = pow( clampResult23 , _DistancePower );
			float clampResult88 = clamp( ( ResultMask53 * _FinalMaskMultiply ) , 0.0 , 1.0 );
			float2 appendResult83 = (float2(clampResult88 , 0.0));
			float4 lerpResult37 = lerp( _FinalColor2 , _FinalColor , clampResult88);
			#ifdef _RAMPENABLED_ON
				float4 staticSwitch81 = tex2D( _Ramp, appendResult83 );
			#else
				float4 staticSwitch81 = lerpResult37;
			#endif
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			o.Emission = ( staticSwitch81 * i.vertexColor * _FinalPower * i.vertexColor.a * tex2D( _Emission, uv_Emission ).r ).rgb;
			float2 uv_MetallicSmoothness = i.uv_texcoord * _MetallicSmoothness_ST.xy + _MetallicSmoothness_ST.zw;
			float4 tex2DNode90 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
			o.Metallic = ( _Metallic * tex2DNode90.r );
			o.Smoothness = ( tex2DNode90.a * _Smoothness );
			o.Alpha = 1;
			clip( tex2DNode89.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}