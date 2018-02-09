Shader "Projector/Caustics" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_CausticTex ("Cookie", 2D) = "" {}
		_Speed("Caustic Speed",float) = 1
		_Tiling("Tiling",float) = 1
		_FalloffTex ("FallOff", 2D) = "" {}
		_Size("Size",float) = 5
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend SrcColor One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;

				float4 uvFalloff 	: TEXCOORD0;
				float4 worldPos		: TEXCOORD1;
				float3 worldNormal 	: TEXCOORD2;

				float4 xUV			: TEXCOORD3;
				float4 yUV			: TEXCOORD4;
				float4 zUV			: TEXCOORD5;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			float _Speed;
			float _Tiling;
			float _Size;
			
			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
			{
				v2f o;
				o.pos 			= UnityObjectToClipPos (vertex);
				o.uvFalloff 	= mul (unity_ProjectorClip, vertex);
				o.worldPos		= mul (unity_ObjectToWorld,vertex);
				o.worldNormal 	= UnityObjectToWorldNormal(normal);

				float time 	= frac(_Time.x * _Speed);

				// Anim UV
				o.xUV.xy	= o.worldPos.zy * _Tiling + time;
				o.xUV.zw 	= o.worldPos.zy * _Tiling * 0.5 - time;

				o.yUV.xy	= o.worldPos.xz * _Tiling + time;
				o.yUV.zw 	= o.worldPos.xz * _Tiling * 0.5 - time;

				o.zUV.xy	= o.worldPos.xy * _Tiling + time;
				o.zUV.zw 	= o.worldPos.xy * _Tiling * 0.5 - time;

				return o;
			}
			
			fixed4 _Color;
			sampler2D _CausticTex;
			sampler2D _FalloffTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff)).a;

				half3 blendWeights = abs(i.worldNormal);
				//blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

				fixed tex 	= max(tex2D (_CausticTex, i.xUV.xy), tex2D (_CausticTex, i.xUV.zw))	* blendWeights.x;
				fixed tex2 	= max(tex2D (_CausticTex, i.yUV.xy), tex2D (_CausticTex, i.yUV.zw))	* blendWeights.y;
				fixed tex3 	= max(tex2D (_CausticTex, i.zUV.xy), tex2D (_CausticTex, i.zUV.zw)) * blendWeights.z;

				fixed res = tex + tex2 + tex3;

				//fixed4 pos = fixed4(unity_ObjectToWorld._m03_m13_m23,1);


				return 2.0 * res * _Color * texF;
			}
			ENDCG
		}
	}
}
