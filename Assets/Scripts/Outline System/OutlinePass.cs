using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlinePass : ScriptableRenderPass
{
    private const string profilerTag = "Outline Pass";
    private static readonly ProfilingSampler profilingSampler = new ProfilingSampler(profilerTag);

    private const string shaderName = "Hidden/Outline";

    static readonly ShaderTagId SRPDefaultUnlit = new ShaderTagId("SRPDefaultUnlit");
    static readonly ShaderTagId UniversalForward = new ShaderTagId("UniversalForward");
    static readonly ShaderTagId LightweightForward = new ShaderTagId("LightweightForward");
    static readonly List<ShaderTagId> shaderTags = new List<ShaderTagId>() { SRPDefaultUnlit, UniversalForward, LightweightForward };

    private static readonly int silhouetteBufferID = Shader.PropertyToID("_SilhouetteBuffer");
    private static readonly int nearestPointID = Shader.PropertyToID("_NearestPoint");
    private static readonly int nearestPointPingPongID = Shader.PropertyToID("_NearestPointPingPong");

    private OutlineFeature.OutlineSettings settings;
    private FilteringSettings filteringSettings;

    private int axisWidthID = Shader.PropertyToID("_AxisWidth");

    private Material outlineMaterial;

    private RenderTextureDescriptor descriptor;

    public RenderTargetIdentifier cameraColor;

    public OutlinePass(OutlineFeature.OutlineSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;

        filteringSettings = new FilteringSettings(RenderQueueRange.all, -1, 1u << settings.layer - 1);

        outlineMaterial = settings.material;

        outlineMaterial.SetColor("_OutlineColor", settings.color);
        outlineMaterial.SetFloat("_OutlineWidth", Mathf.Max(1f, settings.width));
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        RenderTextureDescriptor descriptor = cameraTextureDescriptor;

        descriptor.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8_UNorm;
        descriptor.msaaSamples = 1;
        descriptor.depthBufferBits = 0;
        descriptor.sRGB = false;
        descriptor.useMipMap = false;
        descriptor.autoGenerateMips = false;
        cmd.GetTemporaryRT(silhouetteBufferID, descriptor, FilterMode.Point);

        descriptor.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16_SNorm;
        cmd.GetTemporaryRT(nearestPointID, descriptor, FilterMode.Point);
        cmd.GetTemporaryRT(nearestPointPingPongID, descriptor, FilterMode.Point);

        ConfigureTarget(silhouetteBufferID);
        ConfigureClear(ClearFlag.Color, Color.clear);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        DrawingSettings drawingSettings = CreateDrawingSettings(shaderTags, ref renderingData, SortingCriteria.CommonOpaque);
        drawingSettings.overrideMaterial = outlineMaterial;
        drawingSettings.overrideMaterialPassIndex = 1;

        int numMips = Mathf.CeilToInt(Mathf.Log(settings.width + 1.0f, 2f));
        int jfaIter = numMips - 1;

        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
        using (new ProfilingScope(cmd, profilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            Blit(cmd, silhouetteBufferID, nearestPointID, outlineMaterial, 2);

            for (int i = jfaIter; i >= 0; i--)

            {
                float stepWidth = Mathf.Pow(2, i) + 0.5f;

                cmd.SetGlobalVector(axisWidthID, new Vector2(stepWidth, 0f));
                Blit(cmd, nearestPointID, nearestPointPingPongID, outlineMaterial, 3);
                cmd.SetGlobalVector(axisWidthID, new Vector2(0f, stepWidth));
                Blit(cmd, nearestPointPingPongID, nearestPointID, outlineMaterial, 3);
            }

            cmd.Blit(nearestPointID, cameraColor, outlineMaterial, 4);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new ArgumentNullException("cmd");

        cmd.ReleaseTemporaryRT(silhouetteBufferID);
        cmd.ReleaseTemporaryRT(nearestPointID);
        cmd.ReleaseTemporaryRT(nearestPointPingPongID);
    }
}

public class StencilPass : ScriptableRenderPass
{
    private const string profilerTag = "Stencil Pass";
    private static readonly ProfilingSampler profilingSampler = new ProfilingSampler(profilerTag);

    private const string stencilShaderName = "Hidden/Outline";

    static readonly ShaderTagId SRPDefaultUnlit = new ShaderTagId("SRPDefaultUnlit");
    static readonly ShaderTagId UniversalForward = new ShaderTagId("UniversalForward");
    static readonly ShaderTagId LightweightForward = new ShaderTagId("LightweightForward");
    static readonly List<ShaderTagId> shaderTags = new List<ShaderTagId>() { SRPDefaultUnlit, UniversalForward, LightweightForward };

    private OutlineFeature.StencilSettings settings;
    private FilteringSettings filteringSettings;

    private Material stencilMaterial;

    public StencilPass(OutlineFeature.StencilSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;

        filteringSettings = new FilteringSettings(RenderQueueRange.all, -1, 1u << settings.layer - 1);

        stencilMaterial = settings.material; 
        stencilMaterial.enableInstancing = true;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        DrawingSettings drawingSettings = CreateDrawingSettings(shaderTags, ref renderingData, SortingCriteria.CommonOpaque);
        drawingSettings.overrideMaterial = stencilMaterial;
        drawingSettings.overrideMaterialPassIndex = 0;

        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
        using (new ProfilingScope(cmd, profilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new ArgumentNullException("cmd");
    }
}