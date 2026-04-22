Shader "Custom/Atmosphere"
{
    Properties
    {
        [MainColor]
        _AtmosphereSunColor("Atmosphere Sun Color", Color) = (1.0, 0.35, 0.08, 1.0)
        _AtmosphereDarkColor("Atmosphere Dark Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _SunLocation("Sun Location", Vector) = (0.0, 0.0, 0.0, 1.0)
        _FresnelPower("Fresnel Power", Range(0.5, 10.0)) = 3.0
        _FresnelStrength("Fresnel Strength", Range(0.0, 1.0)) = 1.0
        _DisplacementAmount("Displacement Amount", Float) = 0.1
        _FadeStartDistance("Fade Start Distance", Float) = 5.0
        _FadeEndDistance("Fade End Distance", Float) = 20.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
                float  cameraDist  : TEXCOORD2;
                float3 positionWS  : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float3 _AtmosphereSunColor;
                float3 _AtmosphereDarkColor;
                float3 _SunLocation;
                float _FresnelPower;
                float _FresnelStrength;
                float _DisplacementAmount;
                float _FadeStartDistance;
                float _FadeEndDistance;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // Displace vertex along its object-space normal before transforming
                float3 displacedPositionOS = IN.positionOS.xyz + IN.normalOS * _DisplacementAmount;

                float3 positionWS = TransformObjectToWorld(displacedPositionOS);
                OUT.positionHCS   = TransformWorldToHClip(positionWS);
                OUT.normalWS      = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS     = GetWorldSpaceViewDir(positionWS);
                OUT.cameraDist    = distance(positionWS, _WorldSpaceCameraPos);
                OUT.positionWS    = positionWS;

                return OUT;
            }

            half4 frag(Varyings IN, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                float3 normal  = normalize(IN.normalWS);
                float3 viewDir = normalize(IN.viewDirWS);

                // Flip normal on back faces so Fresnel works correctly for both sides
                normal = isFrontFace ? normal : -normal;

                // Fresnel: 0 when looking straight at surface, 1 at grazing angles
                float fresnel = 1.0 - saturate(dot(normal, viewDir));
                fresnel = pow(fresnel, _FresnelPower) * _FresnelStrength;

                // Fade to fully transparent when camera is closer than _FadeStartDistance,
                // full opacity reached at _FadeEndDistance
                float distanceFade = smoothstep(_FadeStartDistance, _FadeEndDistance, IN.cameraDist);

                float3 sunDir = normalize(_SunLocation - IN.positionWS);
                float sunDot = dot(normal, sunDir);
                if (isFrontFace) {
                    sunDot = 1.0 - sunDot;
                }
                sunDot -= 0.9;
                if (sunDot < 0.0) {
                    sunDot = 0.0;
                }
                sunDot = saturate(sunDot);
                sunDot = pow(sunDot, 2.0);
                sunDot *= 10.0;
                float3 atmosphereColor = lerp(_AtmosphereDarkColor, _AtmosphereSunColor, sunDot);

                return half4(atmosphereColor, fresnel * distanceFade);
            }

            ENDHLSL
        }
    }
}
