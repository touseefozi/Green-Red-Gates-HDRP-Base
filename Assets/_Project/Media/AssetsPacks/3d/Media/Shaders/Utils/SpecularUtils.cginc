#ifndef SPECULAR_UTILS
#define SPECULAR_UTILS

#include <UnityCG.cginc>

uniform half _Shininess;
uniform fixed _SpecIntensity;

inline fixed4 Specular(float3 normal, float3 viewDir)
{
    float3 normalDir = normalize(normal);
    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
    float3 specNormal = normalize(lightDir + viewDir);
    return pow(max(dot(normalDir, specNormal), 0.0), _Shininess) * _SpecIntensity;
}

inline fixed4 Specular(float3 normal, float3 viewDir, float3 customSunPosition)
{
    float3 normalDir = normalize(normal);
    float3 lightDir = normalize(customSunPosition);
    float3 specNormal = normalize(lightDir + viewDir);
    return pow(max(dot(normalDir, specNormal), 0.0), _Shininess) * _SpecIntensity;
}

#endif