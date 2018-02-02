// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "RetroAesthetics/Retro Skybox" {
    Properties {
        _Color2("Top color", Color) = (0.97, 0.67, 0.51, 0)
        _Color1("Bottom color", Color) = (0, 0.7, 0.74, 0)

        [Space]
        _Intensity("Intensity", Range (0, 2)) = 1.0
        _Exponent("Exponent", Range (0, 3)) = 1.0

        [Space]
        _DirectionYaw("Gradient Yaw", Range (0, 180)) = 0
        _DirectionPitch("Gradient Pitch", Range (0, 180)) = 0
        
        [Space]
        [Toggle] _SunEnabled("Retro sun", Float) = 1
        [Space]
        _SunYaw("Sun yaw", Range (0, 180)) = 0
        _SunPitch("Sun pitch", Range (0, 180)) = 0
        [Space]
        _ColorSun1("Sun color top", Color) = (0.97, 0.67, 0.51, 0)
        _ColorSun2("Sun color bottom", Color) = (0.97, 0.17, 0.21, 0)
        _SunExponent("Sun exponent", Range(1, 10)) = 5
        [Space]
        _SunSize("Sun size", Range(0, 0.15)) = 0.01
        [Space]
        _StripesWidth("Stripes width", Range(0, 0.02)) = 0.01
        _StripesDistance("Stripes distance", Range(0, 0.02)) = 0.01
        _StripesHeight("Stripes top", Range(0, 1)) = 0.45
        _StripesExponent1("Stripes exponent", Range(1, 5)) = 4

        [Space]
        _MainTex("Scatter texture", 2D) = "white" {}
        _Gain("Gain", Range(0, 5)) = 2.0

        [HideInInspector]
        _Direction ("Direction", Vector) = (0, 1, 0, 0)
        [HideInInspector]
        _SunDirection ("SunDirection", Vector) = (0, 1, 0, 0)
        [HideInInspector]
        _SunDirectionP ("SunDirectionP", Vector) = (0, 0, 0, 0)
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata {
        float4 position : POSITION;
        float2 tex : TEXCOORD0;
        float3 uv : TEXCOORD1;
    };
    
    struct v2f {
        float4 position : SV_POSITION;
        float2 tex : TEXCOORD0;
        float3 uv : TEXCOORD1;
    };
    
    half4 _Color1;
    half4 _Color2;
    half3 _Direction;
    half3 _SunDirection;
    half3 _SunDirectionP;
    half _Intensity;
    half _Exponent;
    
    half _SunEnabled;
    half _SunSize;
    half4 _ColorStripe;
    half4 _ColorSun1;
    half4 _ColorSun2;
    half _StripesWidth;
    half _StripesDistance;
    half _StripesHeight;
    half _StripesExponent1;
    half _SunExponent;

    sampler2D _MainTex;
	float4 _MainTex_ST;
    float _Gain;
    
    v2f vert(appdata v) {
        v2f o;
        o.position = UnityObjectToClipPos(v.position);
        o.uv = v.uv;
        o.tex = TRANSFORM_TEX(v.tex, _MainTex);
        return o;
    }
    
    fixed4 frag (v2f i) : COLOR {
        half d = dot(normalize(i.uv), _Direction) * 0.5f + 0.5f;
        fixed4 gradient = lerp(_Color1, _Color2, pow(d, _Exponent)) * _Intensity;

        half fragDotSun = dot(normalize(i.uv), _SunDirection) * 0.5f + 0.5f;
        half fragDotSunP = dot(normalize(i.uv), _SunDirectionP) * 0.5f + 0.5f;
        if (_SunEnabled && fragDotSun < _SunSize) {
            if (fragDotSunP < _StripesHeight || 
                    pow(1 - fragDotSunP, _StripesExponent1) % _StripesDistance < _StripesWidth)
                return lerp(_ColorSun1, _ColorSun2, pow(fragDotSunP, _SunExponent) * 10.0);
        }
        
        return gradient / pow(tex2D(_MainTex, i.tex), _Gain);
    }

    ENDCG

    SubShader {
        Tags { "RenderType"="Background" "Queue"="Background" }

        Pass {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }

    CustomEditor "RetroSkyboxEditor"
}
