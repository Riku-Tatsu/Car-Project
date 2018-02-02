Shader "RetroAesthetics/RetroEmissiveTextured" {
	Properties {
		_EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
		_EmissionGain ("Emission Gain", Range(0, 1)) = 0.5
        _GridTex("Grid texture", 2D) = "white" {}
		_Specular ("Shininess", Range(1, 1000)) = 100
	}

	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf SimpleSpecular

		half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			half3 h = normalize (lightDir + viewDir);

			half diff = max (0, dot (s.Normal, lightDir));

			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular);

			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			c.a = s.Alpha;
			return c;
		}

		struct Input {
			float2 uv_GridTex;
		};

		sampler2D _GridTex;

		float4 _LineColor;
		float4 _MainColor;
		
		fixed4 _EmissionColor;
		float _EmissionGain;

		float _Specular;
		
		void surf(Input IN, inout SurfaceOutput o) {
			half3 t = tex2D(_GridTex, IN.uv_GridTex);
			fixed val = saturate(1 - (t.r + t.g + t.b));
			o.Albedo = t;
			o.Alpha = 1.0;
			o.Emission = t * exp(_EmissionGain * 5.0f) * _EmissionColor;
			o.Specular = _Specular;
		}
		ENDCG
	}

	FallBack "Diffuse"
}