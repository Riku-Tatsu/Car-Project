// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:Skybox/Procedural,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:14,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-1679-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31964,y:32708,ptovrint:False,ptlb:Top,ptin:_Top,varname:_Top,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_ViewVector,id:6727,x:31779,y:33036,varname:node_6727,prsc:2;n:type:ShaderForge.SFN_Dot,id:4476,x:31964,y:32900,varname:node_4476,prsc:2,dt:1|A-8747-OUT,B-6727-OUT;n:type:ShaderForge.SFN_Vector3,id:8747,x:31779,y:32900,varname:node_8747,prsc:2,v1:0,v2:-1,v3:0;n:type:ShaderForge.SFN_Color,id:3881,x:31964,y:32490,ptovrint:False,ptlb:Zenith,ptin:_Zenith,varname:_Zenith,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5540658,c2:0.8200497,c3:0.8970588,c4:1;n:type:ShaderForge.SFN_Lerp,id:9381,x:32242,y:32714,varname:node_9381,prsc:2|A-3881-RGB,B-7241-RGB,T-4476-OUT;n:type:ShaderForge.SFN_Dot,id:6207,x:31964,y:33093,varname:node_6207,prsc:2,dt:1|A-6727-OUT,B-6997-OUT;n:type:ShaderForge.SFN_Vector3,id:6997,x:31779,y:33189,varname:node_6997,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_RemapRange,id:2968,x:32127,y:33093,varname:node_2968,prsc:2,frmn:0,frmx:0.1,tomn:0,tomx:1|IN-6207-OUT;n:type:ShaderForge.SFN_Clamp01,id:5934,x:32295,y:33093,varname:node_5934,prsc:2|IN-2968-OUT;n:type:ShaderForge.SFN_Lerp,id:1679,x:32499,y:32843,varname:node_1679,prsc:2|A-9381-OUT,B-2818-RGB,T-5934-OUT;n:type:ShaderForge.SFN_Color,id:2818,x:32242,y:32910,ptovrint:False,ptlb:Ground,ptin:_Ground,varname:_Ground,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.03970587,c2:0.1985294,c3:0,c4:1;proporder:7241-3881-2818;pass:END;sub:END;*/

Shader "RaptorCat/Sky/SkyGradient" {
    Properties {
        _Top ("Top", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Zenith ("Zenith", Color) = (0.5540658,0.8200497,0.8970588,1)
        _Ground ("Ground", Color) = (0.03970587,0.1985294,0,1)
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Top;
            uniform float4 _Zenith;
            uniform float4 _Ground;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
////// Lighting:
////// Emissive:
                float3 emissive = lerp(lerp(_Zenith.rgb,_Top.rgb,max(0,dot(float3(0,-1,0),viewDirection))),_Ground.rgb,saturate((max(0,dot(viewDirection,float3(0,1,0)))*10.0+0.0)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Skybox/Procedural"
    CustomEditor "ShaderForgeMaterialInspector"
}
