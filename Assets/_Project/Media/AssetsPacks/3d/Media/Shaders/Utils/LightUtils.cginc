#ifndef LIGHT_UTILS
#define LIGHT_UTILS

#include <AutoLight.cginc>
#include <UnityCG.cginc>
#include <UnityShaderVariables.cginc>

uniform fixed _Clamp;
uniform float _RimPower;
uniform fixed _RimIntensity;

inline float3 GetWorldNormal(sampler2D normalTexture, fixed2 uv, fixed intensity, fixed3 normal, fixed3 binormal, fixed3 tangent)
{
    float3 zeroNormal = float3(0,0,1);
    float3 textureNormal = tex2D(normalTexture, uv) * 2 - 1;
    textureNormal = lerp(zeroNormal, textureNormal, intensity);
    float3x3 TBN = float3x3(tangent, binormal, normal);
    return normalize(mul(textureNormal, TBN));
}

inline float Fresnel(float4 position, float3 normal)
{
    float3 viewDir = normalize(ObjSpaceViewDir(position));
    return 1 - saturate(dot(normal, viewDir));
}

inline float Rim(float4 position, float3 normal)
{
    float3 viewDir = normalize(ObjSpaceViewDir(position));
    fixed4 fresnel = 1 - saturate(dot(normal, viewDir));
    return pow(fresnel, _RimPower) * _RimIntensity;
}

inline float Rim(float3 viewDir, float3 normal)
{
    fixed4 fresnel = 1 - saturate(dot(normal, viewDir));
    return pow(fresnel, _RimPower) * _RimIntensity;
}

inline half Lambert(float3 normal)
{
    float3 worldNormal = UnityObjectToWorldNormal(normal);
    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
    return dot(worldNormal, lightDir);
}

inline half LambertWorldNormal(float3 worldNormal)
{
    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
    return dot(worldNormal, lightDir);
}

inline half ClampedLambert(float3 normal)
{
    float3 worldNormal = UnityObjectToWorldNormal(normal);
    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
    return dot(worldNormal, lightDir) * _Clamp + (1 - _Clamp);
}

inline float LambertWithRim(float4 position, float3 normal)
{
    half diffuseLight = Lambert(normal);
    half rimLight = Rim(position, normal);

    return diffuseLight + rimLight;
}

inline float ClampedLambertWithRim(float4 position, float3 normal)
{
    half diffuseLight = ClampedLambert(normal);
    half rimLight = Rim(position, normal);

    return diffuseLight + rimLight;
}

inline float SmartReflex(float3 normal, float3 viewDir, fixed amount, fixed smoothness, float threshold, fixed intensity)
{
    fixed3 worldNormal = normalize(normal);
    float diffuseLight = LambertWorldNormal(worldNormal);
    fixed4 fresnel = 1 - dot(viewDir, worldNormal);
    float reflex = fresnel * pow(1 - diffuseLight, threshold);
    return smoothstep(amount - smoothness, amount + smoothness, reflex) * intensity;
}

inline fixed3 CubeNormal(float3 normal)
{
    return mul(UNITY_MATRIX_V, mul(unity_ObjectToWorld, float4(normal, 0))).xyz;
}

#endif