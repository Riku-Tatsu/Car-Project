Shader "RaptorCat/Car/CarRimBlur" {
	Properties {
		_Color ("Tint", Color) = (1,1,1,1)
		_MainTex ("Mask (Mat Blend, Mat Blend, Transparency)", 2D) = "white" {}
		_FirstColor("First Diffuse", Color) = (0.5,0,0.75,1)
		_FirstSpecular("First Specular (Alpha Gloss)", Color) = (0.25,0.25,0.25,0.9)
		_SecondColor("Second Diffuse", Color) = (0,0.5,1,1)
		_SecondSpecular("Second Specular (Alpha Gloss)", Color) = (0.25,0.25,0.25,0.9)
		_ThirdColor("Third Diffuse", Color) = (0.1,0.1,0.1,1)
		_ThirdSpecular("Third Specular (Alpha Gloss)", Color) = (0.5,0.5,0.5,0.85)
		_DiffuseOcclusion("Diffuse Occlusion Darkness", Range(0,1)) = 0.5
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

		//float BinaryDither4x4(float value, float2 sceneUVs)
		float BinaryDither4x4(float value, float2 pos, float sign) {
			float4x4 mtx = float4x4(
				float4(1,  11,  3, 9) / 17.0,
				float4(13, 7, 15,  5) / 17.0,
				float4(4, 12,  2, 10) / 17.0,
				float4(16, 8, 14,  6) / 17.0
				);
			//if (sign < 0)
			//{
			//	mtx = float4x4(
			//	float4(1, 9, 3, 11) / 17.0,
			//	float4(13, 5, 15, 7) / 17.0,
			//	float4(4, 12, 2, 10) / 17.0,
			//	float4(16, 8, 14, 6) / 17.0
			//		);
			//}

			float2 px = floor(_ScreenParams.xy * pos);
			int xSmp = fmod(px.x + saturate(sign),2);
			int ySmp = fmod(px.y + saturate(sign),2);
			float4 xVec = 1 - saturate(abs(float4(0,1,2,3) - xSmp));
			float4 yVec = 1 - saturate(abs(float4(0,1,2,3) - ySmp));
			float4 pxMult = float4(dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec), dot(mtx[3],yVec));
			return round(value + dot(pxMult, xVec));
		}

		sampler2D _MainTex;
		float _FrameSign;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalStatic;

			float4 screenPos : VPOS;

			fixed4 vCol : COLOR;
		};

		fixed4 _Color;
		uniform half3 _FirstColor;
		uniform half4 _FirstSpecular;
		uniform half3 _SecondColor;
		uniform half4 _SecondSpecular;
		uniform half3 _ThirdColor;
		uniform half4 _ThirdSpecular;
		uniform fixed _DiffuseOcclusion;
		uniform fixed _Occlusion;
		uniform half4 _Blur;


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			
			fixed finalBlur = 0;
			if (IN.vCol.r >= 0.49)
			{
				finalBlur = lerp(_Blur.r, _Blur.g, (IN.vCol.r - 0.5) * 2);
			}
			else if(IN.vCol.g >= 0.49)
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

			fixed2 origUV = IN.uv_MainTex;
			fixed2 leftUV = fixed2(origUV.r * 0.5, origUV.g);
			fixed2 rightUV = fixed2(origUV.r * 0.5 + 0.5, origUV.g);

			fixed3 leftTile = tex2D(_MainTex, leftUV).rgb;
			fixed3 rightTile = tex2D(_MainTex, rightUV).rgb;

			fixed blueChannel = saturate((leftTile.b * (1 - (saturate(finalBlur - 0.3333) * 1.5))) + (rightTile.b * saturate(finalBlur * 3)));//lerp(leftTile.b, rightTile.b, finalBlur);

			fixed3 finalMask = fixed3(saturate(leftTile.rg + rightTile.rg), blueChannel);

			float2 pos = (IN.screenPos.xy / IN.screenPos.w);
			clip( BinaryDither4x4(finalMask.b - 1.5, pos, _FrameSign) );




			half3 diffuse = lerp(_FirstColor, lerp(_SecondColor, _ThirdColor, finalMask.g), finalMask.r);
			half4 specular = lerp(_FirstSpecular, lerp(_SecondSpecular, _ThirdSpecular, finalMask.g), finalMask.r);

			o.Albedo = diffuse.rgb * _Color.rgb * max(IN.vCol.b, _DiffuseOcclusion);;

			o.Specular = specular.rgb;
			o.Smoothness = specular.a;

			o.Occlusion = max(IN.vCol.b, _Occlusion);
			
			//o.Alpha = finalMask.b;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
