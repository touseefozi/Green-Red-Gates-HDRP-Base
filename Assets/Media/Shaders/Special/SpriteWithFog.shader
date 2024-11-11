Shader "Custom/Special/SpriteWithFog"
{
	Properties
	{
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		

		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma target 2.0

			#include "UnityCG.cginc"
            #include "../Utils/FogUtils.cginc"

			struct VertexData
			{
				float4 position   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct FragmentData
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
                float depth : TEXCOORD2;
            	float3 fogViewDir : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
			};
			
            sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
            float4 _MainTex_ST;

            void vert(VertexData vertex, out FragmentData output)
            {
                float4 position = vertex.position;

				UNITY_INITIALIZE_OUTPUT(FragmentData, output);
            	UNITY_SETUP_INSTANCE_ID(vertex);
            	
                output.worldPosition = position;
				output.vertex = UnityObjectToClipPos(output.worldPosition);
                output.texcoord = TRANSFORM_TEX(vertex.texcoord, _MainTex);
                output.color = vertex.color * _Color;
                output.depth = abs(UnityObjectToViewPos(position).z);
	            output.fogViewDir = GetFogViewDir(position);
			}

			fixed4 frag(FragmentData fragment) : SV_Target
			{
				half4 color = (tex2D(_MainTex, fragment.texcoord) + _TextureSampleAdd) * fragment.color;
				return ApplyCubeFog(color, fragment.depth, fragment.fogViewDir);
			}
			ENDCG
		}
	}
}
