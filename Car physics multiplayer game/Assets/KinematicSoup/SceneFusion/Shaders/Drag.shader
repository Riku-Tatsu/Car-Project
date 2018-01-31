/**
 * Vertex + pixel shader that renders a transparent colour.
 */
Shader "KS/Drag"
{
    Properties
    {
        [HideInInspector]
        m_colour("Color", Color) = (1, 0, 0, 0.8)
    }

    SubShader
    {
        Tags{ "Queue" = "Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            fixed4 m_colour;

#pragma vertex vertexShader
#pragma fragment pixelShader

            /**
             * Vertex shader output
             */
            struct v2f
            {
                float4 svPos : SV_POSITION;
            };

            /**
             * Vertex shader.
             *
             * @param   float4 pos - vertex position in world space.
             * @return  v2f vectex position in render space.
             */
            v2f vertexShader(float4 pos : POSITION)
            {
                v2f output;
                output.svPos = UnityObjectToClipPos(pos);
                return output;
            }

            /**
             * Pixel shader.
             *
             * @return  float4 colour to render.
             */
            fixed4 pixelShader(v2f input) : SV_TARGET
            {
                m_colour.a = 0.3;
                return m_colour;
            }

            ENDCG
        }
    }
}