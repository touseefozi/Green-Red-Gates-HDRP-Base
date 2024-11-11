#ifndef FOG_UTILS
#define FOG_UTILS

#include <UnityCG.cginc>
#include <UnityShaderVariables.cginc>

inline fixed4 ApplyFog(fixed4 sourceColor, float4 position)
{
    float depth = abs(UnityObjectToViewPos(position).z);
    float blend = 1 - saturate(depth * unity_FogParams.z + unity_FogParams.w);
    blend = lerp(blend, 0, 1 - saturate(unity_FogParams.w * 100));
    fixed4 color = lerp(sourceColor, unity_FogColor, blend);
    color.a = sourceColor.a;
    return color;
}

inline fixed4 ApplyFog(fixed4 sourceColor, float depth)
{
    float blend = 1 - saturate(depth * unity_FogParams.z + unity_FogParams.w);
    blend = lerp(blend, 0, 1 - saturate(unity_FogParams.w * 100));
    blend = lerp(blend, 0, 1 - saturate(unity_FogParams.w * 100));
    fixed4 color = lerp(sourceColor, unity_FogColor, blend);
    color.a = sourceColor.a;
    return color;
}

inline fixed4 GetFogBlend(float depth)
{
    float blend = 1 - saturate(depth * unity_FogParams.z + unity_FogParams.w);
    blend = lerp(blend, 0, 1 - saturate(unity_FogParams.w * 100));
    blend = lerp(blend, 0, 1 - saturate(unity_FogParams.w * 100));
    return blend;
}

inline fixed4 ApplyCubeFog(fixed4 sourceColor, float depth, float3 viewDir)
{
    float blend = 1 - saturate(depth * unity_FogParams.z + unity_FogParams.w);
    half4 fogCube = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, viewDir);
    blend = lerp(blend, 0, 1 - saturate(unity_FogParams.w * 100));
    fixed4 color = lerp(sourceColor, fogCube, blend);
    color.a = sourceColor.a;
    return color;
}

inline float3 GetFogViewDir(float4 position)
{
    float4 worldPosition = mul(unity_ObjectToWorld, position);
    return -normalize(UnityWorldSpaceViewDir(worldPosition.xyz));
}

#endif