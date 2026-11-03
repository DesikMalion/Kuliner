Shader "Custom/Rotten Food Detailed URP"
{
    Properties
    {
        [Header(Base Food)]
        _BaseMap ("Food Texture", 2D) = "white" {}
        _BaseColor ("Food Color", Color) = (1, 1, 1, 1)

        [Header(Overall Decay)]
        _DecayLevel ("Decay Level", Range(0, 1)) = 1

        [Header(Rotten Color)]
        _RotColor ("Rotten Green Brown", Color) = (0.30, 0.38, 0.15, 1)
        _RotAmount ("Rot Amount", Range(0, 1)) = 0.8
        _RotScale ("Rot Pattern Scale", Range(1, 30)) = 5

        [Header(Mold)]
        _MoldColor ("Mold Main Color", Color) = (0.42, 0.58, 0.24, 1)
        _MoldLightColor ("Mold Light Color", Color) = (0.73, 0.79, 0.54, 1)
        _MoldAmount ("Mold Amount", Range(0, 1)) = 0.8
        _MoldScale ("Mold Pattern Scale", Range(1, 40)) = 10
        _MoldDetail ("Mold Detail", Range(0, 1)) = 0.7

        [Header(Dark Spots)]
        _SpotColor ("Dark Spot Color", Color) = (0.06, 0.07, 0.025, 1)
        _SpotAmount ("Dark Spot Amount", Range(0, 1)) = 0.65
        _SpotScale ("Dark Spot Scale", Range(1, 40)) = 13

        [Header(Slime)]
        _SlimeColor ("Slime Color", Color) = (0.38, 0.65, 0.13, 1)
        _SlimeAmount ("Slime Amount", Range(0, 1)) = 0.8
        _SlimeScale ("Slime Pattern Scale", Range(1, 30)) = 6
        _SlimeSpeed ("Slime Animation Speed", Range(0, 0.5)) = 0.05
        _SlimeGloss ("Slime Gloss", Range(0, 1)) = 0.85
        _SlimeShine ("Slime Shine", Range(0, 3)) = 1.5

        [Header(Drip Streaks)]
        _DripAmount ("Drip Streak Amount", Range(0, 1)) = 0.5
        _DripScale ("Drip Streak Scale", Range(1, 30)) = 8
        _DripLength ("Drip Streak Length", Range(1, 10)) = 5

        [Header(Lighting)]
        _AmbientStrength ("Minimum Brightness", Range(0, 1)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"

            Tags
            {
                "LightMode" = "UniversalForwardOnly"
            }

            Blend One Zero
            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)

                float4 _BaseMap_ST;
                half4 _BaseColor;

                float _DecayLevel;

                half4 _RotColor;
                float _RotAmount;
                float _RotScale;

                half4 _MoldColor;
                half4 _MoldLightColor;
                float _MoldAmount;
                float _MoldScale;
                float _MoldDetail;

                half4 _SpotColor;
                float _SpotAmount;
                float _SpotScale;

                half4 _SlimeColor;
                float _SlimeAmount;
                float _SlimeScale;
                float _SlimeSpeed;
                float _SlimeGloss;
                float _SlimeShine;

                float _DripAmount;
                float _DripScale;
                float _DripLength;

                float _AmbientStrength;

            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float Noise2D(float2 p)
            {
                float2 cell = floor(p);
                float2 f = frac(p);

                float2 u = f * f * (3.0 - 2.0 * f);

                float a = Hash21(cell);
                float b = Hash21(cell + float2(1.0, 0.0));
                float c = Hash21(cell + float2(0.0, 1.0));
                float d = Hash21(cell + float2(1.0, 1.0));

                return lerp(
                    lerp(a, b, u.x),
                    lerp(c, d, u.x),
                    u.y
                );
            }

            float OrganicNoise(float2 p)
            {
                float largeNoise = Noise2D(p);
                float mediumNoise = Noise2D(p * 2.1 + 13.7);
                float smallNoise = Noise2D(p * 4.2 + 29.3);

                return (
                    largeNoise * 0.55 +
                    mediumNoise * 0.30 +
                    smallNoise * 0.15
                );
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs positions =
                    GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionCS = positions.positionCS;
                OUT.positionWS = positions.positionWS;

                OUT.normalWS = TransformObjectToWorldNormal(
                    IN.normalOS
                );

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                half3 normalWS = normalize(IN.normalWS);

                half3 viewDirection = normalize(
                    GetWorldSpaceViewDir(IN.positionWS)
                );

                half3 foodTexture = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    uv
                ).rgb;

                half3 color = foodTexture * _BaseColor.rgb;

                // -----------------------------------------
                // 1. WARNA BUSUK
                // -----------------------------------------

                float rotNoise = OrganicNoise(
                    uv * _RotScale
                );

                float rotMask = smoothstep(
                    0.22,
                    0.68,
                    rotNoise
                );

                rotMask *= _RotAmount * _DecayLevel;

                // Sedikit mempertahankan tekstur asli makanan.
                half3 rottenFood =
                    color * _RotColor.rgb * 1.25;

                color = lerp(
                    color,
                    rottenFood,
                    saturate(rotMask)
                );

                // -----------------------------------------
                // 2. BINTIK BUSUK GELAP
                // -----------------------------------------

                float spotNoise = OrganicNoise(
                    uv * _SpotScale + float2(7.3, 13.1)
                );

                float spotMask = smoothstep(
                    0.52,
                    0.72,
                    spotNoise
                );

                spotMask *= _SpotAmount * _DecayLevel;

                color = lerp(
                    color,
                    _SpotColor.rgb,
                    saturate(spotMask)
                );

                // -----------------------------------------
                // 3. JAMUR DENGAN DUA WARNA
                // -----------------------------------------

                float moldNoise = OrganicNoise(
                    uv * _MoldScale + float2(21.5, 11.8)
                );

                float moldMask = smoothstep(
                    0.43,
                    0.65,
                    moldNoise
                );

                moldMask *= _MoldAmount * _DecayLevel;

                // Variasi warna dalam bercak jamur.
                float moldColorNoise = Noise2D(
                    uv * _MoldScale * 2.5 + 9.0
                );

                half3 moldColor = lerp(
                    _MoldColor.rgb,
                    _MoldLightColor.rgb,
                    moldColorNoise
                );

                // Bintik kecil agar jamur terlihat lebih kasar.
                float moldSpeckles = Noise2D(
                    uv * _MoldScale * 9.0
                );

                float moldBrightness = lerp(
                    1.0,
                    0.55 + moldSpeckles,
                    _MoldDetail
                );

                moldColor *= moldBrightness;

                color = lerp(
                    color,
                    moldColor,
                    saturate(moldMask)
                );

                // -----------------------------------------
                // 4. LENDIR BERGERAK
                // -----------------------------------------

                float2 slimeUV = uv * _SlimeScale;

                // Menggerakkan pola secara perlahan.
                slimeUV.y += _Time.y * _SlimeSpeed;

                float slimeNoise = OrganicNoise(
                    slimeUV + float2(4.2, 9.7)
                );

                float slimeMask = smoothstep(
                    0.34,
                    0.69,
                    slimeNoise
                );

                slimeMask *= _SlimeAmount * _DecayLevel;

                // Jamur lebih kering daripada area berlendir.
                slimeMask *= 1.0 - moldMask * 0.45;

                half3 slimeTint =
                    color * _SlimeColor.rgb * 1.45;

                color = lerp(
                    color,
                    slimeTint,
                    saturate(slimeMask * 0.5)
                );

                // -----------------------------------------
                // 5. GARIS LENDIR PADA PERMUKAAN
                // -----------------------------------------

                // Pola memanjang mengikuti koordinat UV.
                float2 dripUV = float2(
                    uv.x * _DripScale,
                    uv.y * (_DripScale / _DripLength)
                );

                dripUV.y += _Time.y * _SlimeSpeed * 0.5;

                float dripNoise = OrganicNoise(
                    dripUV + float2(12.4, 3.6)
                );

                float dripMask = smoothstep(
                    0.53,
                    0.7,
                    dripNoise
                );

                dripMask *= _DripAmount * _DecayLevel;

                color = lerp(
                    color,
                    color * _SlimeColor.rgb * 1.3,
                    saturate(dripMask * 0.5)
                );

                // -----------------------------------------
                // 6. PENCAHAYAAN DAN KILAU BASAH
                // -----------------------------------------

                Light mainLight = GetMainLight();

                half diffuse = saturate(
                    dot(normalWS, mainLight.direction)
                );

                half3 lightColor =
                    _AmbientStrength.xxx +
                    mainLight.color * diffuse;

                half3 finalColor = color * lightColor;

                half3 halfDirection = normalize(
                    mainLight.direction + viewDirection
                );

                half specularAngle = saturate(
                    dot(normalWS, halfDirection)
                );

                float specularPower = lerp(
                    8.0,
                    120.0,
                    _SlimeGloss
                );

                float wetMask = saturate(
                    slimeMask + dripMask * 0.6
                );

                half specular = pow(
                    specularAngle,
                    specularPower
                );

                specular *= wetMask;
                specular *= diffuse;
                specular *= _SlimeShine;

                finalColor += specular * mainLight.color;

                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}