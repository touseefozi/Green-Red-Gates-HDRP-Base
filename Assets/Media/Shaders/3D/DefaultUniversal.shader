Shader "Custom/DefaultUniversal"
{
    Properties
    {
        [Header(Features)]
        [Space]
        [Toggle(FOG_ENABLED)]
        _EnableFog("Enable Fog", Float) = 0
        [Toggle(CUBE_FOG_ENABLED)]
        _EnableCubeFog("Enable Cube Fog", Float) = 0
        [Toggle(HD_ENABLED)]
        _EnableHD("Enable HD", Float) = 0
    	
        [Header(Diffuse Settings)]
        [Space]
        [MainColor] _Color("Color", Color) = (1,1,1,1)
		_Clamp("Clamp", Range(0, 1)) = 0.5
    	
        [Header(Vertex Colors)]
        [Space]
        [Toggle(VERTEX_COLORS_ENABLED)]
        _VertexColorsEnabled("Enabled", Float) = 0
        [ShowIf(VERTEX_COLORS_ENABLED)] _ColorR("Color R", Color) = (1,1,1,1)
        [ShowIf(VERTEX_COLORS_ENABLED)] _ColorG("Color G", Color) = (1,1,1,1)
        [ShowIf(VERTEX_COLORS_ENABLED)] _ColorB("Color B", Color) = (1,1,1,1)
    	
        [Header(Texture Settings)]
        [Space]
        [Toggle(TEXTURE_ENABLED)]
        _EnableTexture("Enabled", Float) = 0
        [ShowIf(TEXTURE_ENABLED)] [MainTexture] _MainTex("Texture", 2D) = "white" {}
    	
    	[Header(Emission Settings)]
        [Space]
        [Toggle(EMISSION_ENABLED)]
        _EnableEmission("Enabled", Float) = 0
        [ShowIf(EMISSION_ENABLED)] [MainTexture] _EmissionMap("Texture", 2D) = "black" {}
        [ShowIf(EMISSION_ENABLED)] _EmissionIntensity("Intensity", Range(0,1)) = 1
        [ShowIf(EMISSION_ENABLED)] [HDR] _EmissionColor("Color", Color) = (1,1,1,1)
        
        [Header(CubeMap Settings)]
        [Space]
        [Toggle(CUBEMAP_ENABLED)]
        _EnableCubemap("Enabled", Float) = 0
        [ShowIf(CUBEMAP_ENABLED)] [NoScaleOffset] _CubeMap ("Cubemap", CUBE) = "" { }
        [ShowIf(CUBEMAP_ENABLED)] _CubeIntensity("Intensity", Range(0,1)) = 1
        [ShowIf(CUBEMAP_ENABLED)] _CubeColor("Color", Color) = (1,1,1,1)
		
        [Header(Rim Settings)]
        [Space]
        [Toggle(RIM_ENABLED)]
        _EnableRim("Enabled", Float) = 0
        [ShowIf(RIM_ENABLED)] _RimIntensity("Intensity", Range(0,1)) = 0.5
        [ShowIf(RIM_ENABLED)] _RimPower("Power", Range(0.5,8.0)) = 3.0
    	
        [Header(Saturation Settings)]
        [Space]
        [Toggle(SATURATION_ENABLED)]
        _EnableSaturation("Enabled", Float) = 0
        [ShowIf(SATURATION_ENABLED)] _Saturation("Saturation", Range(0,1)) = 0.0
        
        [Header(Specular Settings)]
        [Space]
        [Toggle(SPECULAR_ENABLED)]
        _EnableSpecular("Enabled", Float) = 0
        [Toggle(CUSTOM_SUN_ENABLED)]
        _CustomSunEnabled("Custom Sun Position", Float) = 0
        [ShowIf(SPECULAR_ENABLED)] _SpecIntensity("Intensity", Range(0, 1)) = 1
        [ShowIf(SPECULAR_ENABLED)] _Shininess("Pow", Float) = 10
        [ShowIf(CUSTOM_SUN_ENABLED)] _SunPosition ("Sun Position", Vector) = (0, 0, 0, 0)
        
        [Header(Shadow Settings)]
        [Space]
        [Toggle(RECEIVE_SHADOWS)]
        _ReceiveShadows ("Enabled", Float) = 0
        [ShowIf(RECEIVE_SHADOWS)] _ShadowColor("Color", Color) = (1,1,1,1)
        [ShowIf(RECEIVE_SHADOWS)] _ShadowIntensity("Intensity", Range(0.0, 1.0)) = 0.25
    }
	
	SubShader
    {
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
			#pragma multi_compile_instancing
            #include "UnityCG.cginc" 
            #include "UnityLightingCommon.cginc" 
            #include "../Utils/LightUtils.cginc"
#if RECEIVE_SHADOWS
            #include "AutoLight.cginc"
            #include "../Utils/ShadowUtils.cginc"
#endif
#if SPECULAR_ENABLED
            #include "../Utils/SpecularUtils.cginc"
#endif
#if FOG_ENABLED
            #include "../Utils/FogUtils.cginc"
#endif
		    #pragma shader_feature HD_ENABLED
		    #pragma shader_feature VERTEX_COLORS_ENABLED
		    #pragma shader_feature TEXTURE_ENABLED
		    #pragma shader_feature EMISSION_ENABLED
		    #pragma shader_feature CUBEMAP_ENABLED
		    #pragma shader_feature FOG_ENABLED
		    #pragma shader_feature CUBE_FOG_ENABLED
		    #pragma shader_feature RIM_ENABLED
		    #pragma shader_feature SPECULAR_ENABLED
		    #pragma shader_feature CUSTOM_SUN_ENABLED
		    #pragma shader_feature RECEIVE_SHADOWS
		    #pragma shader_feature SATURATION_ENABLED
            
            struct VertexData
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 position : POSITION;
#if VERTEX_COLORS_ENABLED
            	fixed4 color : COLOR;
#endif
                float3 normal : NORMAL;
#if TEXTURE_ENABLED
				float2 uv : TEXCOORD0;
#endif
            };
            
            struct FragmentData
            {
#if !HD_ENABLED
                float light : TEXCOORD0;
#endif
                float4 position : SV_POSITION;
#if VERTEX_COLORS_ENABLED
            	fixed4 color : COLOR;
#endif
#if TEXTURE_ENABLED || EMISSION_ENABLED
				float2 uv : TEXCOORD1;
#endif
#if RIM_ENABLED
                fixed4 fresnel : TEXCOORD2;
#endif
#if FOG_ENABLED || RECEIVE_SHADOWS
                float depth : TEXCOORD3;
#endif
#if SPECULAR_ENABLED || (HD_ENABLED && RIM_ENABLED)
                fixed3 normal : NORMAL;
                float3 viewDir : TEXCOORD4;
#endif
#if RECEIVE_SHADOWS
                float4 shadowCoord : TEXCOORD5;
#endif
#if CUBEMAP_ENABLED
                float3 cubeNormal : TEXCOORD6;
#endif
                float3 localNormal : TEXCOORD7;
#if CUBE_FOG_ENABLED
            	float3 fogViewDir : TEXCOORD8;
#endif
            };
            
            fixed4 _Color;
#if VERTEX_COLORS_ENABLED
            fixed4 _ColorR;
            fixed4 _ColorG;
            fixed4 _ColorB;
#endif
#if TEXTURE_ENABLED
            sampler2D _MainTex;
			float4 _MainTex_ST;
#endif
#if EMISSION_ENABLED
            fixed4 _EmissionColor;
            sampler2D _EmissionMap;
			float4 _EmissionMap_ST;
            fixed _EmissionIntensity;
#endif
#if CUBEMAP_ENABLED
            uniform float4 _CubeColor;
            uniform fixed _CubeIntensity;
            uniform samplerCUBE _CubeMap;
#endif
#if CUSTOM_SUN_ENABLED
            uniform float3 _SunPosition;
#endif
#if SATURATION_ENABLED
            uniform fixed _Saturation;
#endif
            
            void vert(VertexData vertex, out FragmentData output)
            {
                float4 position = vertex.position;
                float3 normal = vertex.normal;

				UNITY_INITIALIZE_OUTPUT(FragmentData, output);
            	UNITY_SETUP_INSTANCE_ID(vertex);
#if HD_ENABLED
            	output.localNormal = normal;
#else
				float light = ClampedLambert(normal);
	#if RIM_ENABLED
				light += Rim(position, normal);
	#endif
                output.light = light;
#endif
#if VERTEX_COLORS_ENABLED && !TEXTURE_ENABLED
            	fixed4 color = vertex.color;
            	output.color = color.r * _ColorR + color.g * _ColorG + color.b * _ColorB;
#endif
                output.position = UnityObjectToClipPos(position);
#if FOG_ENABLED || RECEIVE_SHADOWS
                output.depth = abs(UnityObjectToViewPos(position).z);
#endif
#if SPECULAR_ENABLED || (HD_ENABLED && RIM_ENABLED)
                output.normal = UnityObjectToWorldNormal(normal);
                output.viewDir = normalize(WorldSpaceViewDir(position));
#endif
#if RECEIVE_SHADOWS
                output.shadowCoord = TransferShadows(vertex.position);
#endif
#if TEXTURE_ENABLED
				output.uv = TRANSFORM_TEX(vertex.uv, _MainTex);
#endif
#if CUBEMAP_ENABLED
                output.cubeNormal = CubeNormal(normal);
#endif
#if FOG_ENABLED && CUBE_FOG_ENABLED
	            output.fogViewDir = GetFogViewDir(position);
#endif
            }

            inline fixed4 GetColor(FragmentData fragment)
            {
#if VERTEX_COLORS_ENABLED && !TEXTURE_ENABLED
            	fixed4 color = fragment.color;
#else
            	fixed4 color = _Color;
#endif
#if TEXTURE_ENABLED
            	color *= tex2D(_MainTex, fragment.uv);
	#if VERTEX_COLORS_ENABLED
            	color = color.r * _ColorR + color.g * _ColorG + color.b * _ColorB;
	#endif
#endif
#if CUBEMAP_ENABLED
                color += texCUBE(_CubeMap, fragment.cubeNormal.xyz) * _CubeColor * _CubeIntensity;
#endif
				return color;
            }
            
            inline fixed4 GetLight(FragmentData fragment)
            {
#if HD_ENABLED
            	fixed3 normal = fragment.localNormal;
            	float light = ClampedLambert(normal);
    #if RIM_ENABLED
				light += Rim(fragment.viewDir, fragment.normal);
	#endif
#else
            	float light = fragment.light;
#endif
#if SPECULAR_ENABLED
	#if CUSTOM_SUN_ENABLED
            	light += Specular(fragment.normal, fragment.viewDir, _SunPosition);
    #else
            	light += Specular(fragment.normal, fragment.viewDir);
	#endif
#endif
            	return light;
            }
            
            fixed4 frag(FragmentData fragment) : SV_Target
            {
				float light = GetLight(fragment);
            	fixed4 color = GetColor(fragment);
            	color *= _LightColor0 * light;
#if EMISSION_ENABLED
            	color += tex2D(_EmissionMap, fragment.uv) * _EmissionColor * _EmissionIntensity;
#endif
#if RECEIVE_SHADOWS
            	color = ApplyShadow(color, fragment.depth, fragment.shadowCoord);
#endif
#if SATURATION_ENABLED
            	fixed4 desaturatedColor = Luminance(color);
            	color = lerp(desaturatedColor, color, _Saturation);
#endif
#if FOG_ENABLED
	#if CUBE_FOG_ENABLED
            	color = ApplyCubeFog(color, fragment.depth, fragment.fogViewDir);
    #else
            	color = ApplyFog(color, fragment.depth);
	#endif
#endif
            	return color;
            }
            ENDCG
        }
    	
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}