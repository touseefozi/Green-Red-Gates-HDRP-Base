Shader "Custom/Special/Chest"
{
    Properties
    {
        [Header(Diffuse Settings)]
        [Space]
        _ColorR("Gold Color", Color) = (1,1,1,1)
        _ColorG("Wood Color", Color) = (1,1,1,1)
    	_BrightnessR("Gold Brightness", Range(0, 1)) = 0.5
    	_BrightnessG("Wood Brightness", Range(0, 1)) = 0.5
		_Clamp("Clamp", Range(0, 1)) = 0.5
    	
        [Space]
        [NoScaleOffset] _ReflectionMap ("Reflection Map", CUBE) = "" { }
        _ReflectionColor("Reflection Color", Color) = (1,1,1,1)
		_Reflection("Reflection", Range(0, 1)) = 1
    	
        [Space]
        [NoScaleOffset] _CubeMap ("Cubemap", CUBE) = "" { }
        _CubeIntensity("Cube Intensity", Range(0,1)) = 1
        _CubeColor("Cube Color", Color) = (1,1,1,1)
		
        [Space]
        _RimIntensity("Rim Intensity", Range(0,1)) = 0.5
        _RimPower("Rim Power", Range(0.5,8.0)) = 3.0
        [Space]
        _SpecColor("Specular Color", Color) = (1,1,1,1)
        _SpecIntensity("Specular Intensity", Range(0, 1)) = 1
        _Shininess("Specular Pow", Float) = 10
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
            #include "../Utils/LightUtils.cginc"
            #include "AutoLight.cginc"
            #include "../Utils/SpecularUtils.cginc"
            #include "../Utils/FogUtils.cginc"
            
            struct VertexData
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 position : POSITION;
            	fixed4 color : COLOR;
                float3 normal : NORMAL;
            };
            
            struct FragmentData
            {
            	fixed4 color : COLOR;
                float4 position : SV_POSITION;
                fixed3 normal : NORMAL;
                float light : TEXCOORD0;
                float depth : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 cubeNormal : TEXCOORD3;
                float3 localNormal : TEXCOORD4;
            	float3 fogViewDir : TEXCOORD5;
            };
            
            uniform fixed4 _ColorR;
            uniform fixed4 _ColorG;
            uniform fixed _BrightnessR;
            uniform fixed _BrightnessG;
            
            uniform fixed4 _SpecColor;
            
            uniform samplerCUBE _ReflectionMap;
            uniform fixed4 _ReflectionColor;
            uniform fixed _Reflection;
            
            uniform float4 _CubeColor;
            uniform fixed _CubeIntensity;
            uniform samplerCUBE _CubeMap;
            
            void vert(VertexData vertex, out FragmentData output)
            {
                float4 position = vertex.position;
                float3 normal = vertex.normal;

				UNITY_INITIALIZE_OUTPUT(FragmentData, output);
            	UNITY_SETUP_INSTANCE_ID(vertex);
            	
            	output.color = vertex.color;
                output.position = UnityObjectToClipPos(position);
                output.normal = UnityObjectToWorldNormal(normal);
                output.light = ClampedLambertWithRim(position, normal);
                output.depth = abs(UnityObjectToViewPos(position).z);
                output.viewDir = normalize(WorldSpaceViewDir(position));
                output.cubeNormal = CubeNormal(normal);
	            output.fogViewDir = GetFogViewDir(position);
            }
            
			inline half3 HDRDecode(half4 data)
            {
            	return unity_SpecCube0_HDR.x * (unity_SpecCube0_HDR.w * (data.a - 1.0) + 1.0) * data.rgb;
            }

            fixed4 frag(FragmentData fragment) : SV_Target
            {
				fixed4 vertexColor = fragment.color;
            	float brightness = vertexColor.b * (vertexColor.r * _BrightnessR + vertexColor.g * _BrightnessG);
            	fixed4 color = (vertexColor.r * _ColorR + vertexColor.g * _ColorG) * saturate(1 - brightness);

                fixed4 sample = texCUBE(_ReflectionMap, reflect(-fragment.viewDir, fragment.normal));
                fixed3 reflection = HDRDecode(sample) * vertexColor.r * _Reflection * _ReflectionColor;
            	
            	fixed4 specular = _SpecColor * Specular(fragment.normal, fragment.viewDir) * (1 - vertexColor.g * 0.7);
				fixed3 cube = texCUBE(_CubeMap, fragment.cubeNormal.xyz) * _CubeIntensity * _CubeColor;
				color.rgb += cube;
            	color.rgb *= fragment.light + specular + reflection;
            	color.a = 1.0;

            	return ApplyCubeFog(color, fragment.depth, fragment.fogViewDir);
            }
            ENDCG
        }
    	
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}