using System.Collections.Generic;
using Aarthificial.PixelGraphics.Common;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aarthificial.PixelGraphics.Forward
{
    public class VelocityRenderPass : ScriptableRenderPass
    {
        private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();
        private readonly ProfilingSampler _profilingSampler;
        private readonly Material _emitterMaterial;
        private readonly Material _blitMaterial;

        private RTHandle _temporaryVelocityTarget;
        private RTHandle _velocityTarget;
        private RTHandle _cameraColorTarget;

        private VelocityPassSettings _passSettings;
        private SimulationSettings _simulationSettings;
        private FilteringSettings _filteringSettings;
        private Vector2 _previousPosition;

        public VelocityRenderPass(Material emitterMaterial, Material blitMaterial)
        {
            _emitterMaterial = emitterMaterial;
            _blitMaterial = blitMaterial;

            _shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            _shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            _shaderTagIdList.Add(new ShaderTagId("Universal2D"));
            _shaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
            _shaderTagIdList.Add(new ShaderTagId("LightweightForward"));

            _filteringSettings = new FilteringSettings(RenderQueueRange.transparent);
            _profilingSampler = new ProfilingSampler(nameof(VelocityRenderPass));

            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        public void Setup(
            VelocityPassSettings passSettings,
            SimulationSettings simulationSettings
        )
        {
            _passSettings = passSettings;
            _simulationSettings = simulationSettings;
        }

        public void SetupCameraTargets(RTHandle cameraColorTarget)
        {
            _cameraColorTarget = cameraColorTarget;
            ConfigureTarget(cameraColorTarget);
        }

        public void Dispose()
        {
            _temporaryVelocityTarget?.Release();
            _temporaryVelocityTarget = null;
            _velocityTarget?.Release();
            _velocityTarget = null;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            desc.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;

            float textureScale = _passSettings != null ? _passSettings.textureScale : 1f;
            desc.width = Mathf.Max(1, Mathf.FloorToInt(desc.width * textureScale));
            desc.height = Mathf.Max(1, Mathf.FloorToInt(desc.height * textureScale));

            RenderingUtils.ReAllocateIfNeeded(
                ref _temporaryVelocityTarget,
                desc,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_PG_TemporaryVelocityTextureTarget"
            );
            RenderingUtils.ReAllocateIfNeeded(
                ref _velocityTarget,
                desc,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_VelocityTarget"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Clear();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                ref var cameraData = ref renderingData.cameraData;

                int textureWidth = _velocityTarget.rt.width;
                int textureHeight = _velocityTarget.rt.height;

                float height = 2 * cameraData.camera.orthographicSize * _passSettings.pixelsPerUnit;
                float width = height * cameraData.camera.aspect;

                var cameraPosition = (Vector2) cameraData.GetViewMatrix().GetColumn(3);
                var delta = cameraPosition - _previousPosition;
                var screenDelta = cameraData.GetProjectionMatrix() * cameraData.GetViewMatrix() * delta;
                _previousPosition = cameraPosition;

                cmd.SetGlobalVector(ShaderIds.CameraPositionDelta, screenDelta / 2);
                cmd.SetGlobalTexture(ShaderIds.VelocityTexture, _velocityTarget.nameID);
                cmd.SetGlobalTexture(ShaderIds.PreviousVelocityTexture, _temporaryVelocityTarget.nameID);
                cmd.SetGlobalVector(ShaderIds.VelocitySimulationParams, _simulationSettings.Value);
                cmd.SetGlobalVector(
                    ShaderIds.PixelScreenParams,
                    new Vector4(
                        width,
                        height,
                        _passSettings.pixelsPerUnit,
                        1 / _passSettings.pixelsPerUnit
                    )
                );

                CoreUtils.SetRenderTarget(cmd, _velocityTarget);
                cmd.SetViewport(new Rect(0, 0, textureWidth, textureHeight));
                Blitter.BlitTexture(cmd, new Vector4(1f, 1f, 0f, 0f), _blitMaterial, 0);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                if (!cameraData.isPreviewCamera && !cameraData.isSceneViewCamera)
                {
                    var drawingSettings = CreateDrawingSettings(
                        _shaderTagIdList,
                        ref renderingData,
                        SortingCriteria.CommonTransparent
                    );

                    if (_passSettings.layerMask != 0)
                    {
                        _filteringSettings.layerMask = _passSettings.layerMask;
                        _filteringSettings.renderingLayerMask = uint.MaxValue;
                        drawingSettings.overrideMaterial = null;
                        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
                    }

                    if (_passSettings.renderingLayerMask != 0)
                    {
                        _filteringSettings.layerMask = -1;
                        _filteringSettings.renderingLayerMask = _passSettings.renderingLayerMask;
                        drawingSettings.overrideMaterial = _emitterMaterial;
                        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
                    }
                }

                // TODO Implement proper double buffering
                Blitter.BlitCameraTexture(cmd, _velocityTarget, _temporaryVelocityTarget);
#if UNITY_EDITOR
                if (_passSettings.preview && _cameraColorTarget != null)
                    Blitter.BlitCameraTexture(cmd, _velocityTarget, _cameraColorTarget);
#endif
                if (_cameraColorTarget != null)
                    CoreUtils.SetRenderTarget(cmd, _cameraColorTarget);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            _cameraColorTarget = null;
        }
    }
}
