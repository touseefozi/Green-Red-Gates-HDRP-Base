Shader "Custom/Special/Monster"
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
    	
    	[Header(Emission Settings)]
        [Space]
        [Toggle(EMISSION_ENABLED)]
        _EnableEmission("Enabled", Float) = 0
        [ShowIf(EMISSION_ENABLED)] [NoScaleOffset] _EmissionMap("Texture", 2D) = "black" {}
        [ShowIf(EMISSION_ENABLED)] _EmissionIntensity("Intensity", Range(0,1)) = 1
        [ShowIf(EMISSION_ENABLED)] [HDR] _EmissionColor("Color", Color) = (1,1,1,1)
        
        [Header(CubeMap Settings)]
        [Space]
        [NoScaleOffset] _CubeMap ("Cubemap", CUBE) = "" { }
        _CubeIntensity("Intensity", Range(0,1)) = 1
        _CubeColor("Color", Color) = (1,1,1,1)
        
        [Header(Specular Settings)]
        _SpecIntensity("Intensity", Range(0, 1)) = 1
        _Shininess("Pow", Float) = 10
    
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
                fixed3 normal : NORMAL;
                float light : TEXCOORD0;
                float depth : TEXCOORD1;
				float2 uv : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float3 localNormal : TEXCOORD4;
                float3 cubeNormal : TEXCOORD5;
            	float3 fogViewDir : TEXCOORD6;
            };
            
            uniform fixed4 _Color;
            uniform fixed4 _HighlightColor;
            uniform fixed _Highlight;
            uniform fixed _Desaturate;
            uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
            
            uniform float4 _CubeColor;
            uniform fixed _CubeIntensity;
            uniform samplerCUBE _CubeMap;
            
#if EMISSION_ENABLED
            uniform fixed4 _EmissionColor;
            uniform sampler2D _EmissionMap;
			uniform float4 _EmissionMap_ST;
            uniform fixed _EmissionIntensity;
#endif
            
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

            fixed4 frag(FragmentData fragment) : SV_Target
            {
				float light = fragment.light;
            	light += Specular(fragment.normal, fragment.viewDir);
            	
            	fixed4 color = _Color;
            	color *= tex2D(_MainTex, fragment.uv);
                color += texCUBE(_CubeMap, fragment.cubeNormal.xyz) * _CubeColor * _CubeIntensity;
            	
            	color *= _LightColor0 * light;
#if EMISSION_ENABLED
            	color += tex2D(_EmissionMap, fragment.uv) * _EmissionColor * _EmissionIntensity;
#endif
            	color = lerp(color, Luminance(color), _Desaturate);
				color = lerp(color, _HighlightColor, _Highlight);
            	color = ApplyCubeFog(color, fragment.depth, fragment.fogViewDir);
            	color.a = 1;
            	return color;
            }
            ENDCG
        }
    	
        UsePass "Custom/OutlinePass/Outline"
        UsePass "Custom/Shadowcaster/SHADOWCASTER"
    }
}