// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/RetroCameraEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UseStaticNoise ("Use Static Noise", float) = 1.0
		_SecondaryTex ("Secondary Texture", 2D) = "white" {}
		_StaticIntensity ("Static Intensity", float) = 1.0
		_OffsetNoiseX ("Offset Noise X", float) = 0.0
		_OffsetNoiseY ("Offset Noise Y", float) = 0.0
		_OffsetPosY ("Offset position Y", float) = 0.0
		_UseChromaticAberration ("Use Chromatic Aberration", float) = 1.0
		_ChromaticAberration ("Chromatic Aberration", float) = 0.0
		_UseVignette ("Use Vignette", float) = 0.0
		_Vignette ("Vignette", float) = 0.0
		_UseDisplacement ("Use Displacement", float) = 1.0
		_DisplacementAmplitude ("Displacement Amplitude", float) = 0.001
		_DisplacementFrequency ("Displacement Frequency", float) = 10
		_DisplacementSpeed ("Displacement Speed", float) = 1
		_NoiseBottomHeight ("Bottom Noise Height", float) = 0.04
		_NoiseBottomIntensity ("Bottom Noise Intensity", float) = 1.0
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma shader_feature CHROMATIC_ON
			#pragma shader_feature NOISE_ON
			#pragma shader_feature DISPLACEMENT_ON
			#pragma shader_feature VIGNETTE_ON
			#pragma shader_feature NOISE_BOTTOM_ON
			#pragma shader_feature BOTTOM_STRETCH_ON
			#pragma shader_feature RADIAL_DISTORTION_ON
			#pragma shader_feature SCANLINES_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			half _OffsetNoiseX;
			half _OffsetNoiseY;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.uv2 = v.texcoord + float2(_OffsetNoiseX - 0.2f, _OffsetNoiseY);
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			sampler2D _SecondaryTex;

			fixed _Intensity;
			
			float _ChromaticAberration;
			float _Vignette;

			half _OffsetPosY;
			half _DisplacementAmplitude;
			half _DisplacementFrequency;
			half _DisplacementSpeed;
			half _StaticIntensity;

			half _NoiseBottomHeight;
			half _NoiseBottomIntensity;

        	uniform float _RadialDistortion = 0.1;
			uniform float _RadialDistortionCurvature = 1.0;

			uniform float _ScanlineSize = 256;
			uniform float _ScanlineIntensity = 1.0;

			uniform float _Gamma;
		
			float4 ScanlineWeights(float y, float4 color)
			{
				float4 width = 2.0f + 2.0f * pow(color, _ScanlineIntensity);
				return 1.0f * exp(-pow(y / 0.5f * rsqrt(0.5f * width), width)) / (0.3f + 0.2f * width);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half2 uv = i.uv;

				// DISPLACEMENT NOISE
				#if DISPLACEMENT_ON
					uv.x = frac(uv.x + cos((uv.y + _CosTime.y * _DisplacementSpeed) * _DisplacementFrequency) * 
								_DisplacementAmplitude);
				#endif
				uv.y = frac(uv.y + _OffsetPosY);
			
				half2 coords = (uv - 0.5) * 2.0;
				half coordDot = dot(coords, coords);

				// RADIAL DISTORION
				#if RADIAL_DISTORTION_ON
					half dist = coordDot * _RadialDistortion;
					uv += coords * (1.0 + dist) * pow(dist, _RadialDistortionCurvature);
				#endif

				// BOTTOM STRETCH
				#if BOTTOM_STRETCH_ON
					float uvY = uv.y;
					uv.y = max(uv.y, _NoiseBottomHeight - 0.01);
				#endif

				half4 color = tex2D(_MainTex, uv);

				// VIGNETTE
				#if VIGNETTE_ON
					float mask = 1.0 - coordDot * _Vignette;
				#endif

				// CHROMATIC ABERIATION
				#if CHROMATIC_ON
					// Update coords.
					coords = (uv - 0.5) * 2.0;
					coordDot = dot(coords, coords);
					half2 uvG = uv - _MainTex_TexelSize.xy * _ChromaticAberration * coords * coordDot;

					#if SHADER_API_D3D9
						// Work around Cg's code generation bug for D3D9 pixel shaders :(
						color.g = color.g * 0.0001 + tex2D(_MainTex, uvG).g;
					#else
						color.g = tex2D(_MainTex, uvG).g;
					#endif
					
					color.b = tex2D(_MainTex, uv + float2(-_ChromaticAberration, -_ChromaticAberration) * 0.0003).b;
				#endif

				// SCANLINES
				#if SCANLINES_ON
					float2 _One = 1.0f / _ScanlineSize;

					float2 ratio = uv * _ScanlineSize - float2(0.5f, 0.5f);
					float2 uvratio = frac(ratio);
				
					uv.y = (floor(ratio.y) + 0.5) / _ScanlineSize;
				
					float4 weights1 = ScanlineWeights(uvratio.y, color);
					float4 weights2 = ScanlineWeights(1.0f - uvratio.y, color);
					color = saturate(color * weights1 + color * weights2);
				#endif

				// Apply vignette
				#if VIGNETTE_ON
					color.rgb *= mask;
				#endif

				// STATIC NOISE
				#if NOISE_ON
					fixed4 noise = tex2D(_SecondaryTex, i.uv2);
					color = lerp(color, noise, (noise.r - _StaticIntensity * 0.9) * _StaticIntensity * 0.1);
				#endif

				// BOTTOM STRETCH CONTD.
				#if BOTTOM_STRETCH_ON
					uv.y = uvY;
				#endif

				// BOTTOM NOISE
				#if NOISE_BOTTOM_ON
					fixed condition = saturate(floor(_NoiseBottomHeight / uv.y));
					fixed4 noise_bottom = tex2D(_SecondaryTex, i.uv2 - 0.5) * condition * _NoiseBottomIntensity;
					color = lerp(color, noise_bottom, - noise_bottom * ((uv.y / (_NoiseBottomHeight)) - 1.0));
				#endif

				// Darken borders of radial distortion
				#if RADIAL_DISTORTION_ON
					if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1) {
						color.rgb *= 0.15;
					}
				#endif

				float exp = 1.0 / _Gamma;
        		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
			}
			ENDCG
		}
	}
}
