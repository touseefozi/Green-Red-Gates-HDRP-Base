Shader "Custom/SkyCubemap"
{
    Properties
    {
        _Color ("Tint Color", Color) = (.5, .5, .5, .5)
        [MainTexture] [NoScaleOffset] _Tex ("Cubemap", Cube) = "grey" {}
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "PreviewType" = "Skybox"
        }
        
        Cull Off 
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            fixed4 _Color;
            samplerCUBE _Tex;
            half4 _Tex_HDR;

            struct VertexData
            {
                float4 position : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct FragmentData
            {
                float4 position : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            void vert(VertexData vertex, inout FragmentData output)
            {
				UNITY_INITIALIZE_OUTPUT(FragmentData, output);

                float4 position = vertex.position;
                output.position = UnityObjectToClipPos(position);
                output.texcoord = position.xyz;
            }

            fixed4 frag(FragmentData fragment) : SV_Target
            {
                return texCUBE(_Tex, fragment.texcoord) * _Color;
            }
            ENDCG
        }
    }

    Fallback Off
}