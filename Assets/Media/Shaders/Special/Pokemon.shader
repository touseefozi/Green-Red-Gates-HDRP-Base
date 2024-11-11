Shader "Custom/Special/Pokemon"
{
    Properties
    {
        [MainTexture] [NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
        [MainColor] _Color("Color", Color) = (1,1,1,1)
		_Clamp("Clamp", Range(0, 1)) = 0.5
		_Desaturate("Desaturate", Range(0, 1)) = 0
    	
    	[Header(Hightlight Settijngs)]
    	[Space]
        [MainColor] _HighlightColor("HighlightColor", Color) = (1,1,1,1)
		_Highlight("Highlight", Range(0, 1)) = 0
    	
        [Header(CubeMap Settings)]
        [Space]
        [NoScaleOffset] _CubeMap ("Cubemap", CUBE) = "" { }
        _CubeIntensity("Intensity", Range(0,1)) = 1
        _CubeColor("Color", Color) = (1,1,1,1)
        
        [Header(Specular Settings)]
        _SpecIntensity("Intensity", Range(0, 1)) = 1
        _Shininess("Pow", Float) = 10
    	
        [Header(HSL Settings)]
        _Brightness("Brightness", Range(-1, 1)) = 0
        _Saturation("Saturation", Range(-1, 1)) = 0
        _Hue("Hue", Range(-1, 1)) = 0
    
        [Header(Outline Settings)] 
        _OutlineColor ("Color", Color) = (0, 0, 0, 1)
        _Outline ("Thickness", Range(0,0.1)) = 0.01
        _OutlineStencilRef ("Stencil Ref", Int) = 2
    }
	
	SubShader
    {
        Stencil 
        {
            Ref [_OutlineStencilRef]
			Comp Always
            Pass Replace
        }
    	
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "IgnoreProjector" = "True" 
        }
        
        Pass
        {
        	
	        Tags
	        {
				"LightMode" = "ForwardBase" 
	            "PassFlags" = "OnlyDirectional"
	        }
        	
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc" 
            #include "UnityLightingCommon.cginc" 
            #include "../Utils/LightUtils.cginc"
            #include "../Utils/SpecularUtils.cginc"
            #include "../Utils/FogUtils.cginc"
		    #pragma shader_feature EMISSION_ENABLED
            
            struct VertexData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
            };
            
            struct FragmentData
            {
                float4 position : SV_POSITION;
                float3 normal : NORMAL;
                float light : TEXCOORD0;
                float depth : TEXCOORD1;
				float2 uv : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float3 localNormal : TEXCOORD4;
                float3 cubeNormal : TEXCOORD5;
            	float3 fogViewDir : TEXCOORD6;
            };
            
            float4 _Color;
            float4 _HighlightColor;
            float _Highlight;
            float _Desaturate;
            sampler2D _MainTex;
			float4 _MainTex_ST;
            
            float4 _CubeColor;
            float _CubeIntensity;
            samplerCUBE _CubeMap;
            float _Saturation;
            float _Brightness;
            float _Hue;
            
            float Epsilon = 1e-10;
            
            void vert(VertexData vertex, out FragmentData output)
            {
                float4 position = vertex.position;
                float3 normal = vertex.normal;

				UNITY_INITIALIZE_OUTPUT(FragmentData, output);
                output.light = ClampedLambert(normal);
                output.position = UnityObjectToClipPos(position);
                output.depth = abs(UnityObjectToViewPos(position).z);
                output.normal = UnityObjectToWorldNormal(normal);
                output.viewDir = normalize(WorldSpaceViewDir(position));
				output.uv = TRANSFORM_TEX(vertex.uv, _MainTex);
                output.cubeNormal = CubeNormal(normal);
	            output.fogViewDir = GetFogViewDir(position);
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


            float4 frag(FragmentData fragment) : SV_Target
            {
				float light = fragment.light;
            	light += Specular(fragment.normal, fragment.viewDir);
            	
            	float4 color = _Color;
            	color *= tex2D(_MainTex, fragment.uv);
                color += texCUBE(_CubeMap, fragment.cubeNormal.xyz) * _CubeColor * _CubeIntensity;
            	
            	color *= _LightColor0 * light;
                color.rgb = hsl2rgb(rgb2hsl(color.rgb) + float3(_Hue, _Saturation, _Brightness));
            	
            	color = lerp(color, Luminance(color), _Desaturate);
				color = lerp(color, _HighlightColor, _Highlight);
            	color = ApplyCubeFog(color, fragment.depth, fragment.fogViewDir);
            	return color;
            }
            ENDCG
        }
    	
        UsePass "Custom/OutlinePass/Outline"
        UsePass "Custom/Shadowcaster/SHADOWCASTER"
    }
}