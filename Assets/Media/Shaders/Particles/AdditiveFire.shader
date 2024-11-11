Shader "Custom/AdditiveFire" 
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Z Test", Float) = 2
    }
    
    Category 
    {
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }
        
        Blend SrcAlpha One
        
        Cull Off
        ZTest [_ZTest]
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
    
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
