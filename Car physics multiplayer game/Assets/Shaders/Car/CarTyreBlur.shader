Shader "RaptorCat/Car/CarTyreBlur" {
	Properties {
		_Color ("Tint", Color) = (1,1,1,1)
		_MainTex ("Mask Depth (Static,Blurred,NULL)", 2D) = "white" {}
		_TopColor("Peak Diffuse Color", Color) = (0.08,0.08,0.08)
		_BottomColor("Bottom Diffuse Color", Color) = (0.04,0.04,0.04)
		_NormalStatic("Static Normals", 2D) = "bump" {}
		_NormalBlurred("Blurred Normals", 2D) = "bump" {}
		_TopSpec("Peak Specular", Color) = (0.1,0.1,0.1)
		_TopGloss("Peak Gloss", Range(0,1)) = 0.1
		_BottomSpec("Bottom Specular", Color) = (0.025,0.025,0.025)
		_BottomGloss("Bottom Gloss", Range(0,1)) = 0.025
		_Occlusion("Occlusion Darkness", Range(0,1)) = 0.5
		_Blur("Blur", Vector) = (0,0,0,0)

		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalStatic;
		sampler2D _NormalBlurred;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalStatic;

			float4 vCol : COLOR;
		};

		fixed4 _Color;
		uniform half3 _TopColor;
		uniform half3 _BottomColor;
		uniform half3 _TopSpec;
		uniform half3 _BottomSpec;
		uniform half _TopGloss;
		uniform half _BottomGloss;
		uniform fixed _Occlusion;
		uniform half4 _Blur;


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			
			fixed4 colorMask = tex2D (_MainTex, IN.uv_MainTex);

			fixed finalBlur = 0;
			if (IN.vCol.r >= 0.5)
			{
				finalBlur = lerp(_Blur.r, _Blur.g, (IN.vCol.r - 0.5) * 2);
			}
			else if(IN.vCol.g >= 0.5)
			{
				finalBlur = lerp(_Blur.b, _Blur.a, (IN.vCol.g - 0.5) * 2);
			}
			else
			{
				finalBlur = 0;
			}
			finalBlur = 1 - finalBlur;
			finalBlur *= finalBlur * finalBlur;
			finalBlur = 1 - finalBlur;

			fixed height = lerp(colorMask.r, colorMask.g, finalBlur);

			o.Albedo = lerp(_BottomColor, _TopColor, height) * _Color;

			o.Specular = lerp(_BottomSpec, _TopSpec, height);
			o.Smoothness = lerp(_BottomGloss, _TopGloss, height);

			o.Occlusion = max(height, _Occlusion);

			half4 normStatic = tex2D(_NormalStatic, IN.uv_NormalStatic);
			half4 normBlurred = tex2D(_NormalBlurred, IN.uv_NormalStatic);
			half4 norm = lerp(normStatic, normBlurred, finalBlur);
			o.Normal = UnpackNormal(norm);

			
			o.Alpha = colorMask.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
