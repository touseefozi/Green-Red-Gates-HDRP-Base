Shader "Custom/Additive" 
{
    Properties 
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Z Test", Float) = 2
    }
    
    Category 
    {
        Tags 
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }
        
        Blend SrcAlpha One
        
        Cull Off
        ZTest [_ZTest]
        Lighting Off
        ZWrite Off
        Fog { Color (0,0,0,0) }
    
        BindChannels 
        {
            Bind "Color", color
            Bind "Vertex", vertex
            Bind "TexCoord", texcoord
        }
    
        SubShader 
        {
            Pass 
            {
                SetTexture [_MainTex] 
                {
                    combine texture * primary
                }
            }
        }
    }
}
