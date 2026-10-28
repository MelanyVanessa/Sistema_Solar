// Shader liviano para Quest: textura + luz que sale de la posición del Sol (sin luces de Unity).
Shader "SistemaSolar/Planeta"
{
    Properties
    {
        _BaseMap ("Textura", 2D) = "white" {}
        _Tinte ("Tinte", Color) = (1,1,1,1)
        _Ambiente ("Luz ambiente", Range(0,1)) = 0.06
        _Emisivo ("Emisivo (Sol)", Range(0,1)) = 0
        _Fuego ("Fuego", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _Main_Space_Light_Position;
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _Tinte;
                half _Ambiente;
                half _Emisivo;
                half4 _Fuego;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes i)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
                o.positionCS = TransformWorldToHClip(o.positionWS);
                o.normalWS = TransformObjectToWorldNormal(i.normalOS);
                o.uv = TRANSFORM_TEX(i.uv, _BaseMap);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half3 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).rgb * _Tinte.rgb;
                float3 haciaSol = normalize(_Main_Space_Light_Position.xyz - i.positionWS);
                half luz = saturate(dot(normalize(i.normalWS), haciaSol));
                luz = lerp(saturate(luz + _Ambiente), 1.0, _Emisivo);
                color = lerp(color * luz, _Fuego.rgb, _Fuego.a);
                return half4(color, 1);
            }
            ENDHLSL
        }
    }
}
