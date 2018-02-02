Shader "RetroAesthetics/RetroGridGenerative" {
	Properties {
		_MainColor ("Main Color", Color) = (0.1, 0.1, 0.1, 1)
		_LineColor ("Line Color", Color) = (0.1, 1, 0.1, 1)
		_LineWidth ("Line width", Range(0, 1)) = 0.1
		_CellSize ("Cell Size", Range(0, 100)) = 5
		_EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
		_EmissionGain ("Emission Gain", Range(0, 1)) = 0.5
	}

	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		float4 _LineColor;
		float4 _MainColor;
		fixed _LineWidth;
		float _CellSize;
		fixed4 _EmissionColor;
		float _EmissionGain; 

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half val1 = step(_LineWidth, frac(IN.worldPos.x / _CellSize));
			half val2 = step(_LineWidth, frac(IN.worldPos.z / _CellSize));
			fixed val = val1 * val2;
			o.Albedo = lerp(_LineColor, _MainColor, val);
			o.Alpha = 1.0;
			o.Emission = lerp(_EmissionColor * (exp(_EmissionGain * 10.0f)), 0, val);
		}
		ENDCG
	}

	FallBack "Diffuse"
}