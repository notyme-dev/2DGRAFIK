Shader "Hidden/PixelGraphics/Velocity/Blit"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "../VelocitySimulation.hlsl"

            float4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                return SimulateVelocity(input.texcoord);
            }
            ENDHLSL
        }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "../VelocitySimulation.hlsl"

            float4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 result = SimulateVelocity(input.texcoord);

                float2 temporaryVelocity = SAMPLE_TEXTURE2D(_PG_TemporaryVelocityTexture, sampler_PG_TemporaryVelocityTexture, input.texcoord).zw;
                if (abs(temporaryVelocity.x) > 0)
                    result.z = temporaryVelocity.x;
                if (abs(temporaryVelocity.y) > 0)
                    result.w = temporaryVelocity.y;

                return result;
            }
            ENDHLSL
        }
    }
}
