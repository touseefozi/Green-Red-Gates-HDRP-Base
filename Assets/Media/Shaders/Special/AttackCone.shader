Shader "Custom/Special/AttackCone"
{
    Properties
    {
        [Header(Diffuse Settings)]
        [MainColor] _Color("Color", Color) = (1,1,1,1)
		_Clamp("Clamp", Range(0, 1)) = 0.5
    	
        [Header(CubeMap Settings)]
        [NoScaleOffset] _CubeMap ("Cubemap", CUBE) = "" { }
        _CubeIntensity("Intensity", Range(0,1)) = 1
        _CubeColor("Color", Color) = (1,1,1,1)
		
        [Header(Rim Settings)]
        _RimIntensity("Intensity", Range(0,1)) = 0.5
    	_RimPower("Power", Range(0.5,8.0)) = 3.0
    	
        [Header(Stencil Settings)]
		_Stencil("Stencil Ref", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comp", Float)	= 8
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Op", Float) = 0
    }
	
	SubShader
    {
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
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
            #include "UnityCG.cginc" 
            #include "../Utils/LightUtils.cginc"
            #include "AutoLight.cginc"
            
            struct VertexData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };
            
            struct FragmentData
            {
                float light : TEXCOORD0;
                float4 position : SV_POSITION;
                fixed4 fresnel : TEXCOORD2;
                float3 cubeNormal : TEXCOORD6;
                float3 localNormal : TEXCOORD7;
            };
            
            fixed4 _Color;
            uniform float4 _CubeColor;
            uniform fixed _CubeIntensity;
            uniform samplerCUBE _CubeMap;
            
            void vert(VertexData vertex, out FragmentData output)
            {
                float4 position = vertex.position;
                float3 normal = vertex.normal;

                output.light = ClampedLambertWithRim(position, normal);
                output.position = UnityObjectToClipPos(position);
                output.cubeNormal = CubeNormal(normal);
            }
            
            fixed4 frag(FragmentData fragment) : SV_Target
            {
				fixed4 color = _Color;
                color += texCUBE(_CubeMap, fragment.cubeNormal.xyz) * _CubeColor * _CubeIntensity;
            	color *=  fragment.light;
            	return color;
            }
            ENDCG
        }
    	
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}