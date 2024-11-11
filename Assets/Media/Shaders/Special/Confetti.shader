Shader "Custom/Special/Confetti"
{
    Properties
    {
		_Clamp("Clamp", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Cull Off
        
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "LightMode" = "ForwardBase" 
            "IgnoreProjector" = "True" 
            "PassFlags" = "OnlyDirectional"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" 
            #include "../Utils/LightUtils.cginc" 
            
            struct VertexData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };
            
            struct FragmentData
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };
            
            void vert(VertexData vertex, out FragmentData output)
            {
                float4 position = vertex.position;
                float light = ClampedLambert(vertex.normal) * 1.5;
                
                output.color = vertex.color * light;
                output.pos = UnityObjectToClipPos(position);
            }
            
            fixed4 frag(FragmentData fragment) : SV_Target
            {
                return fragment.color;
            }
            ENDCG
        }
    }
}