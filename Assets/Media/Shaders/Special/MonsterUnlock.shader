Shader "Custom/Special/MonsterUnlock"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _TintColor ("Tint", Color) = (1,1,1,1)
        _Fill ("Fill Amount", Range(0,1)) = 0.0

        [Header(HSL Settings)]
        _Contrast("Contrast", Range(0, 1)) = 0
        _Brightness("Brightness", Range(-1, 1)) = 0
        _Saturation("Saturation", Range(-1, 1)) = 0
        _Hue("Hue", Range(-1, 1)) = 0
        
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
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _TintColor;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _Fill;
            
            float _Contrast;
            float _Brightness;
            float _Hue;
            float _Saturation;
            
            float Epsilon = 1e-10;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color;
                return OUT;
            }

            inline float3 rgb2hcv(in float3 RGB)
            {
                float4 P = lerp(float4(RGB.bg, -1.0, 2.0 / 3.0), float4(RGB.gb, 0.0, -1.0 / 3.0), step(RGB.b, RGB.g));
                float4 Q = lerp(float4(P.xyw, RGB.r), float4(RGB.r, P.yzx), step(P.x, RGB.r));
                float C = Q.x - min(Q.w, Q.y);
                float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
                return float3(H, C, Q.x);
            }

            inline float3 rgb2hsl(in float3 RGB)
            {
                float3 HCV = rgb2hcv(RGB);
                float L = HCV.z - HCV.y * 0.5;
                float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
                return float3(HCV.x, S, L);
            }

            inline float3 hsl2rgb(float3 c)
            {
                c = float3(frac(c.x), clamp(c.yz, 0.0, 1.0));
                float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
                return c.z + c.y * (rgb - 0.5) * (1.0 - abs(2.0 * c.z - 1.0));
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                float4 blend = step(IN.texcoord.y, _Fill);
                half4 darkColor = color;
                darkColor.rgb = hsl2rgb(rgb2hsl(darkColor.rgb) + float3(_Hue, _Saturation, _Brightness)) * _TintColor;
                darkColor = saturate(lerp(_TintColor, darkColor, _Contrast));
                    
                half4 resultColor = lerp(darkColor, color, blend);
                resultColor.a = color.a;
                
                return resultColor;
            }
            ENDCG
        }
    }
}
