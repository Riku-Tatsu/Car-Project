// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Stix Games/Grass Dynamics/Wind"
{
	Properties
	{
		_Mask("Mask", 2D) = "white" {}
		_WindParams("Wind WaveStrength(X), WaveSpeed(Y), RippleStrength(Z), RippleSpeed(W)", Vector) = (0.3, 1.2, 0.15, 1.3)
		_Scale("Wind Scale", float) = 1
		_InfluenceStrength("Influence Strength", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off
		
		Blend One One, DstColor Zero
		BlendOp Add

		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 pos : TEXCOORD0;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD1;
			};

			float _InfluenceStrength;

			float _GrassDisplacementBorderArea;
			float4 _GrassRenderTextureArea;

			fixed4 _WindParams;
			float _Scale;

			sampler2D _Mask;
			float4 _Mask_ST;

			#include "../GrassWind.cginc"

			v2f vert (appdata v)
			{
				v2f o;
				#if UNITY_VERSION < 540
					o.vertex = UnityObjectToClipPos(v.vertex);
				#else
					o.vertex = UnityObjectToClipPos(v.vertex);
				#endif
				o.pos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				//Smooth out the border of the displacement. 
				//If the displacement area too big to see the border, you should probably remove that for performance.
				float borderSmoothing = smoothstep(_GrassRenderTextureArea.x, _GrassRenderTextureArea.x + _GrassDisplacementBorderArea, i.pos.x);
				borderSmoothing *= smoothstep(_GrassRenderTextureArea.y, _GrassRenderTextureArea.y + _GrassDisplacementBorderArea, i.pos.z);

				float xBorder = _GrassRenderTextureArea.x + _GrassRenderTextureArea.z;
				borderSmoothing *= smoothstep(xBorder, xBorder - _GrassDisplacementBorderArea, i.pos.x);

				float yBorder = _GrassRenderTextureArea.y + _GrassRenderTextureArea.w;
				borderSmoothing *= smoothstep(yBorder, yBorder - _GrassDisplacementBorderArea, i.pos.z);

				half2 windDir = wind(float3(i.uv.x, 0, i.uv.y) * _Scale);
				half3 normal = normalize(half3(windDir.x, 1, windDir.y));

				half mask = tex2D(_Mask, TRANSFORM_TEX(i.uv, _Mask)).r;

				float3 c = normalize(normal * borderSmoothing + float3(0,1,0) * (1-borderSmoothing)).xzy * _InfluenceStrength * mask;

				return float4(c, 1);
			}
			ENDCG
		}
	}
}
