Shader "Custom/GradientTexture"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1, 0, 1, 1)
        _ColorB ("Color B", Color) = (1, 1, 0, 1)
        
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15

        [HideInInspector] [Toggle(UNITY_UI_ALPHACLIP)]
        _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct VertexData
            {
                fixed4 color : COLOR;
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct FragmentData
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float alpha : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _ColorA;
            fixed4 _ColorB;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            void vert(VertexData vertex, out FragmentData output)
            {
                UNITY_SETUP_INSTANCE_ID(vertex);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.worldPosition = vertex.position;
                output.vertex = UnityObjectToClipPos(output.worldPosition);
                output.uv = TRANSFORM_TEX(vertex.uv, _MainTex);
                output.alpha = vertex.color.a;
            }

            fixed4 frag(FragmentData fragment) : SV_Target
            {
                half4 color = (tex2D(_MainTex, fragment.uv) + _TextureSampleAdd);

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(fragment.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
                
                half4 resultColor = lerp(_ColorA, _ColorB, color.r);
                resultColor.a = color.a * fragment.alpha;
                
                return resultColor;
            }
        ENDCG
        }
    }
}