// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:14,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|diff-2753-OUT,spec-5978-OUT,gloss-7055-OUT,emission-8416-OUT;n:type:ShaderForge.SFN_Color,id:5927,x:32158,y:32404,ptovrint:False,ptlb:Primary Color,ptin:_PrimaryColor,varname:node_5927,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8235294,c2:0.8235294,c3:0.8235294,c4:1;n:type:ShaderForge.SFN_Multiply,id:2753,x:32982,y:32706,varname:node_2753,prsc:2|A-6860-OUT,B-6170-B;n:type:ShaderForge.SFN_VertexColor,id:6170,x:32163,y:32766,varname:node_6170,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:620,x:32757,y:32990,ptovrint:False,ptlb:Reflectivity,ptin:_Reflectivity,varname:node_620,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.15;n:type:ShaderForge.SFN_ValueProperty,id:7055,x:32757,y:33066,ptovrint:False,ptlb:Smoothness,ptin:_Smoothness,varname:node_7055,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.15;n:type:ShaderForge.SFN_NormalVector,id:5247,x:31404,y:33575,prsc:2,pt:False;n:type:ShaderForge.SFN_HalfVector,id:8280,x:31404,y:33454,varname:node_8280,prsc:2;n:type:ShaderForge.SFN_Dot,id:8697,x:31583,y:33454,varname:node_8697,prsc:2,dt:1|A-8280-OUT,B-5247-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:1304,x:32468,y:33565,varname:node_1304,prsc:2;n:type:ShaderForge.SFN_LightColor,id:7554,x:32468,y:33439,varname:node_7554,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4411,x:32666,y:33489,varname:node_4411,prsc:2|A-910-OUT,B-7554-RGB,C-1304-OUT,D-8122-OUT;n:type:ShaderForge.SFN_Exp,id:3532,x:31583,y:33312,varname:node_3532,prsc:2,et:1|IN-8050-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8050,x:31404,y:33312,ptovrint:False,ptlb:Metallic Concentration,ptin:_MetallicConcentration,varname:node_8050,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Power,id:2486,x:31760,y:33454,varname:node_2486,prsc:2|VAL-8697-OUT,EXP-3532-OUT;n:type:ShaderForge.SFN_Color,id:5868,x:32468,y:33717,ptovrint:False,ptlb:Primary Metallic,ptin:_PrimaryMetallic,varname:node_5868,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Noise,id:379,x:31404,y:33088,varname:node_379,prsc:2|XY-1199-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:3820,x:31086,y:33088,varname:node_3820,prsc:2;n:type:ShaderForge.SFN_Append,id:1199,x:31248,y:33088,varname:node_1199,prsc:2|A-3820-X,B-3820-Z;n:type:ShaderForge.SFN_Add,id:8386,x:32470,y:33149,varname:node_8386,prsc:2|A-7529-OUT,B-2486-OUT;n:type:ShaderForge.SFN_Multiply,id:7529,x:32256,y:33149,varname:node_7529,prsc:2|A-379-OUT,B-2486-OUT,C-3975-OUT,D-5059-OUT;n:type:ShaderForge.SFN_Vector1,id:3975,x:32089,y:33227,varname:node_3975,prsc:2,v1:4;n:type:ShaderForge.SFN_Multiply,id:771,x:31895,y:33227,varname:node_771,prsc:2|A-2054-OUT,B-8697-OUT,C-5059-OUT;n:type:ShaderForge.SFN_Add,id:1279,x:32837,y:33179,varname:node_1279,prsc:2|A-771-OUT,B-4411-OUT;n:type:ShaderForge.SFN_Clamp01,id:910,x:32470,y:33293,varname:node_910,prsc:2|IN-8386-OUT;n:type:ShaderForge.SFN_Vector1,id:8388,x:31404,y:33223,varname:node_8388,prsc:2,v1:0.9;n:type:ShaderForge.SFN_Step,id:2054,x:31583,y:33115,varname:node_2054,prsc:2|A-8388-OUT,B-379-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5059,x:31583,y:33258,ptovrint:False,ptlb:Metallic Flakes,ptin:_MetallicFlakes,varname:node_5059,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.05;n:type:ShaderForge.SFN_Multiply,id:8416,x:32999,y:33042,varname:node_8416,prsc:2|A-6170-B,B-1279-OUT;n:type:ShaderForge.SFN_Multiply,id:5978,x:32971,y:32845,varname:node_5978,prsc:2|A-6170-B,B-620-OUT;n:type:ShaderForge.SFN_Color,id:1955,x:32163,y:32579,ptovrint:False,ptlb:Secondary Color,ptin:_SecondaryColor,varname:_PrimaryColor_copy,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:6860,x:32420,y:32553,varname:node_6860,prsc:2|A-5927-RGB,B-1955-RGB,T-6170-R;n:type:ShaderForge.SFN_Color,id:6104,x:32468,y:33903,ptovrint:False,ptlb:Secondary Metallic,ptin:_SecondaryMetallic,varname:_MetallicPrimary_copy,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:8122,x:32718,y:33795,varname:node_8122,prsc:2|A-5868-RGB,B-6104-RGB,T-6170-R;proporder:5927-1955-620-7055-5868-6104-8050-5059;pass:END;sub:END;*/

Shader "Shader Forge/Car/CarPaint" {
    Properties {
        [HDR]_PrimaryColor ("Primary Color", Color) = (0.8235294,0.8235294,0.8235294,1)
        [HDR]_SecondaryColor ("Secondary Color", Color) = (0,0,0,1)
        _Reflectivity ("Reflectivity", Float ) = 0.15
        _Smoothness ("Smoothness", Float ) = 1.15
        [HDR]_PrimaryMetallic ("Primary Metallic", Color) = (1,0,0,1)
        [HDR]_SecondaryMetallic ("Secondary Metallic", Color) = (0,1,1,1)
        _MetallicConcentration ("Metallic Concentration", Float ) = 4
        _MetallicFlakes ("Metallic Flakes", Float ) = 0.05
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _PrimaryColor;
            uniform float _Reflectivity;
            uniform float _Smoothness;
            uniform float _MetallicConcentration;
            uniform float4 _PrimaryMetallic;
            uniform float _MetallicFlakes;
            uniform float4 _SecondaryColor;
            uniform float4 _SecondaryMetallic;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Smoothness;
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float node_5978 = (i.vertexColor.b*_Reflectivity);
                float3 specularColor = float3(node_5978,node_5978,node_5978);
                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = (lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,i.vertexColor.r)*i.vertexColor.b);
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float2 node_1199 = float2(i.posWorld.r,i.posWorld.b);
                float2 node_379_skew = node_1199 + 0.2127+node_1199.x*0.3713*node_1199.y;
                float2 node_379_rnd = 4.789*sin(489.123*(node_379_skew));
                float node_379 = frac(node_379_rnd.x*node_379_rnd.y*(1+node_379_skew.x));
                float node_8697 = max(0,dot(halfDirection,i.normalDir));
                float node_2486 = pow(node_8697,exp2(_MetallicConcentration));
                float3 emissive = (i.vertexColor.b*((step(0.9,node_379)*node_8697*_MetallicFlakes)+(saturate(((node_379*node_2486*4.0*_MetallicFlakes)+node_2486))*_LightColor0.rgb*attenuation*lerp(_PrimaryMetallic.rgb,_SecondaryMetallic.rgb,i.vertexColor.r))));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _PrimaryColor;
            uniform float _Reflectivity;
            uniform float _Smoothness;
            uniform float _MetallicConcentration;
            uniform float4 _PrimaryMetallic;
            uniform float _MetallicFlakes;
            uniform float4 _SecondaryColor;
            uniform float4 _SecondaryMetallic;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Smoothness;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float node_5978 = (i.vertexColor.b*_Reflectivity);
                float3 specularColor = float3(node_5978,node_5978,node_5978);
                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 diffuseColor = (lerp(_PrimaryColor.rgb,_SecondaryColor.rgb,i.vertexColor.r)*i.vertexColor.b);
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
