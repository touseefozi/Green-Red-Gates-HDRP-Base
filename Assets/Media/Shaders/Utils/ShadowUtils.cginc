#ifndef SMART_SHADOW_UTILS
#define SMART_SHADOW_UTILS

#include <UnityShaderVariables.cginc>

uniform fixed4 _ShadowColor;
uniform fixed _ShadowIntensity;
static const float _ShadowMinDistance = 15;
static const float _ShadowMaxDistance = 25;

inline float4 TransferShadows(float4 position)
{
    return mul(unity_WorldToShadow[0], mul(unity_ObjectToWorld, position));
}

inline fixed SampleShadow(float4 shadowCoord)
{
    #if defined(SHADOWS_SCREEN)
    fixed shadow = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, shadowCoord.xyz);
    return _LightShadowData.r + shadow * (1 - _LightShadowData.r);
    #endif
    return 1.0;
}

inline fixed4 ApplyShadow(fixed4 color, float depth, float4 shadowCoord)
{
    fixed attenuation = SampleShadow(shadowCoord);
    fixed shadow = 1 - attenuation;

    float blend = 1 - (depth - _ShadowMinDistance) / (_ShadowMaxDistance - _ShadowMinDistance);
    shadow = shadow * saturate(blend) * _ShadowIntensity;

    return lerp(color, _ShadowColor, shadow);
}

#endif