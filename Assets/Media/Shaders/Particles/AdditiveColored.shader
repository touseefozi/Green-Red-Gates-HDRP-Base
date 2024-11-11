Shader "Custom/AdditiveColored"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        
		[Enum(Off,0,On,1)] _ZWrite ("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 1

        [Header(Stencil Settings)]
		_Stencil("Stencil Ref", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comp", Float)	= 8
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Op", Float) = 0
    }
    Category
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        
        Blend SrcAlpha One
        ColorMask RGB
        Cull Off
		Lighting Off 
		ZWrite [_ZWrite]
		ZTest [_ZTest]
        
        SubShader
        {
			Stencil
			{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp]
			}
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_particles
                #pragma multi_compile_fog
                #include "UnityCG.cginc"
                    
                sampler2D _MainTex;
                fixed4 _TintColor;
                
                struct VertexData
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 uv : TEXCOORD0;
                };
                
                struct FragmentData
                {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 uv : TEXCOORD0;
                };
                
                float4 _MainTex_ST;
                
                void vert(VertexData vertex, out FragmentData output)
                {
                    output.color = vertex.color;
                    output.vertex = UnityObjectToClipPos(vertex.vertex);
                    output.uv = TRANSFORM_TEX(vertex.uv, _MainTex);
                }
                
                fixed4 frag(FragmentData fragment) : SV_Target
                {
                    fixed4 col = 2.0f * _TintColor * tex2D(_MainTex, fragment.uv) * fragment.color;
                    col.a = saturate(col.a);
                    return col;
                }
                ENDCG
            }
        }
    }
}