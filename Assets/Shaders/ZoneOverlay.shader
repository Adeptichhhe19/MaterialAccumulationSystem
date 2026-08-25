Shader "Material Accumulation/Zone Overlay"
{
    Properties
    {
        _Color("Color", Color) = (0.1, 0.9, 1.0, 0.12)
        _LineColor("Line Color", Color) = (0.25, 1.0, 1.0, 0.8)
        _LongitudeLines("Longitude Lines", Float) = 16
        _LatitudeLines("Latitude Lines", Float) = 6
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+20"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ZoneOverlay"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _LineColor;
                float _LongitudeLines;
                float _LatitudeLines;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                half3 viewDirectionWS : TEXCOORD2;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positions = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positions.positionCS;
                output.positionOS = input.positionOS.xyz;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirectionWS = GetWorldSpaceNormalizeViewDir(positions.positionWS);
                return output;
            }

            half PeriodicLine(float coordinate, float count)
            {
                float value = coordinate * count;
                float distanceToLine = abs(frac(value + 0.5) - 0.5);
                return 1.0 - smoothstep(0.0, fwidth(value) * 1.5, distanceToLine);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 direction = normalize(input.positionOS);
                float longitude = atan2(direction.z, direction.x) / (2.0 * PI) + 0.5;
                float latitude = asin(saturate(direction.y)) / (0.5 * PI);
                half lines = max(
                    PeriodicLine(longitude, _LongitudeLines),
                    PeriodicLine(latitude, _LatitudeLines));
                half rim = pow(1.0 - saturate(abs(dot(normalize(input.normalWS),
                    normalize(input.viewDirectionWS)))), 2.0);
                half4 color = lerp(_Color, _LineColor, saturate(lines + rim * 0.4));
                color.a = saturate(_Color.a + lines * _LineColor.a + rim * 0.18);
                return color;
            }
            ENDHLSL
        }
    }
}
