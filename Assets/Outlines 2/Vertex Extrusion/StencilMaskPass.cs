using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StencilMaskPass : ScriptableRenderPass
{
    private const string profilerTag = "Stencil Mask";
    private static readonly ProfilingSampler profilingSampler = new ProfilingSampler(profilerTag);

    static readonly ShaderTagId SRPDefaultUnlit = new ShaderTagId("SRPDefaultUnlit");
    static readonly ShaderTagId UniversalForward = new ShaderTagId("UniversalForward");
    static readonly ShaderTagId LightweightForward = new ShaderTagId("LightweightForward");
    static readonly List<ShaderTagId> shaderTags = new List<ShaderTagId>() { SRPDefaultUnlit, UniversalForward, LightweightForward };

    private StencilMaskSettings settings;
    private FilteringSettings filteringSettings;
    private RenderStateBlock renderStateBlock;

    private Material stencilMaterial;

    public StencilMaskPass(StencilMaskSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;

        filteringSettings = new FilteringSettings(RenderQueueRange.all, -1, 1u << settings.layer - 1);

        if (stencilMaterial) CoreUtils.Destroy(stencilMaterial);
        stencilMaterial = CoreUtils.CreateEngineMaterial("Hidden/Stencil Mask");
        stencilMaterial.enableInstancing = true;

        renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
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

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}