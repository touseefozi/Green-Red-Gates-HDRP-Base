Shader "Custom/Special/Gate"
{
    Properties
    {
        [MainColor] _Color("Color", Color) = (1,1,1,1)
		_AlphaStart("AlphaStart", Range(0, 1)) = 1
		_AlphaEnd("AlphaEnd", Range(0, 1)) = 1
        
        [Header(Blend Settings)]
    	[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrcMode ("SrcMode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDstMode ("DstMode", Float) = 0
    }
	
	SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True" 
        }
    	
        Blend [_BlendSrcMode] [_BlendDstMode]
    	
        Cull Off
        Lighting Off
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing
            #include "UnityCG.cginc" 
            #include "../Utils/LightUtils.cginc"
            #include "../Utils/FogUtils.cginc"
            
            struct VertexData
            {
            	UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 position : POSITION;
            	fixed4 color : COLOR;
            };
            
            struct FragmentData
            {
                float4 position : SV_POSITION;
            	fixed4 color : COLOR;
                float depth : TEXCOORD0;
            };
            
            fixed4 _Color;
            float _AlphaStart;
            float _AlphaEnd;
            
            void vert(VertexData vertex, out FragmentData output)
            {
                float4 position = vertex.position;

				UNITY_INITIALIZE_OUTPUT(FragmentData, output);
            	UNITY_SETUP_INSTANCE_ID(vertex);
                output.color = vertex.color;
                output.position = UnityObjectToClipPos(position);
                output.depth = abs(UnityObjectToViewPos(position).z);
            }
            
            fixed4 frag(FragmentData fragment) : SV_Target
            {
				fixed4 vertexColor = fragment.color;
            	fixed4 color = _Color;
            	float alpha = vertexColor.r * _AlphaStart + vertexColor.g * _AlphaEnd;
            	float blend = GetFogBlend(fragment.depth);
            	color = lerp(color, float4(0, 0, 0, 0), blend);
            	color.a = lerp(alpha, 0, blend);
            	return color;
            }
            ENDCG
        }
    }
}