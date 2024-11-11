Shader "Custom/Canvas/PopupBackground"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorR ("Color R", Color) = (1,0,0,1)
        _ColorG ("Color G", Color) = (0,1,0,1)
        _ColorB ("Color B", Color) = (0,0,1,1)
        
        [Header(Pattern Settings)]
        [NoScaleOffset] _Pattern ("Pattern Texture", 2D) = "black" {}
        _PatternColor ("Color", Color) = (1, 1, 1, 1)
        _PatternSize ("Size", Range(32, 512)) = 128
        _PatternAlpha ("Apha", Range(0, 1)) = 1
        _PatternOffsetX ("Offset X", Range(-1.0, 1.0)) = 0
        _PatternOffsetY ("Offset Y", Range(-1.0, 1.0)) = 0
        
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

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _ColorR;
            fixed4 _ColorG;
            fixed4 _ColorB;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float Epsilon = 1e-10;

            sampler2D _Pattern;
            float4 _Pattern_ST;
            fixed4 _PatternColor;
            fixed _PatternAlpha;
            float _PatternSize;
            float _PatternOffsetX;
            float _PatternOffsetY;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 pixel = tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd;
                
                fixed4 color = pixel.r * _ColorR + pixel.g * _ColorG + pixel.b * _ColorB;
                color.a = pixel.a;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                float2 pos = IN.worldPosition;
                float posX = pos.x + _PatternOffsetX * _PatternSize;
                float posY = pos.y + _PatternOffsetY * _PatternSize;
                float2 uv = float2(posX / _PatternSize, posY / _PatternSize);

                fixed4 pattern = tex2D(_Pattern, uv) * pixel.r;
                
                color = lerp(color, _PatternColor, pattern * _PatternAlpha);
                
                return color;
            }
        ENDCG
        }
    }
}