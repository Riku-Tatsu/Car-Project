Shader "RetroAesthetics/RetroEmissiveColors" {
	Properties {
		_MainColor ("Main color", Color) = (0.1, 0.1, 0.1, 1)
		_LineColor ("Line color", Color) = (0.1, 1, 0.1, 1)
		_EmissionColor ("Emission color", Color) = (1, 1, 1, 1)
		_EmissionGain ("Emission gain", Range(0, 1)) = 0.5
        _GridTex("Grid texture", 2D) = "white" {}
		[Toggle] _UseSpecular ("Use Shininess", float) = 0
		_Specular ("Shininess", Range(1, 1000)) = 100
		_ShadowColor ("Shadow color", Color) = (0, 0, 0, 1)
	}

	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 800

		CGPROGRAM
		#pragma surface surf SimpleSpecular

		uniform float4 _ShadowColor;
		uniform float _UseSpecular;

		half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			half3 h = normalize (lightDir + viewDir);

			half diff = max (0, dot (s.Normal, lightDir));

			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular) * _UseSpecular;

			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			c.rgb += _ShadowColor.xyz * (1.0-atten);
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
			o.Albedo = lerp(_LineColor, _MainColor, val);
			o.Alpha = 1.0;
			o.Emission = t * exp(_EmissionGain * 5.0f) * _EmissionColor;
			o.Specular = _Specular;
		}
		ENDCG
	}

	Fallback "VertexLit"
}