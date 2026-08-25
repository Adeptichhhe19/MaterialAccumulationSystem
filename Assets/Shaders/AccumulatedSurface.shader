Shader "Material Accumulation/Surface"
{
    Properties
    {
        _LowColor("Ground Color", Color) = (0.12, 0.09, 0.06, 1)
        _HighColor("Material Color", Color) = (0.92, 0.42, 0.12, 1)
        _PeakColor("Peak Color", Color) = (1.0, 0.76, 0.32, 1)
        _HeightRange("Height Range", Float) = 2.5
        _GridScale("Grid Scale", Float) = 1.0
        _GridStrength("Grid Strength", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _LowColor;
                half4 _HighColor;
                half4 _PeakColor;
                float _HeightRange;
                float _GridScale;
                float _GridStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positions = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positions.positionCS;
                output.positionWS = positions.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half GridLine(float value)
            {
                float scaled = value / max(_GridScale, 0.001);
                float distanceToLine = abs(frac(scaled + 0.5) - 0.5);
                return 1.0 - smoothstep(0.0, fwidth(scaled) * 1.35, distanceToLine);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half height01 = saturate(input.positionWS.y / max(_HeightRange, 0.001));
                half3 baseColor = lerp(_LowColor.rgb, _HighColor.rgb, smoothstep(0.0, 0.45, height01));
                baseColor = lerp(baseColor, _PeakColor.rgb, smoothstep(0.55, 1.0, height01));

                half3 normal = normalize(input.normalWS);
                half3 lightDirection = normalize(half3(-0.45, 0.85, -0.3));
                half diffuse = 0.36 + 0.64 * saturate(dot(normal, lightDirection));
                half grid = max(GridLine(input.positionWS.x), GridLine(input.positionWS.z));
                baseColor *= lerp(1.0, 0.62, grid * _GridStrength);

                return half4(baseColor * diffuse, 1.0);
            }
            ENDHLSL
        }
    }
}
