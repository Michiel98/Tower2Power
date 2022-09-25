// DRAWING MODES
// OUTLINE ALWAYS: STENCIL NOTEQUAL, ZTEST ALWAYS
// HIDDEN SURFACES: STENCIL OFF, ZTEST GREATER, OUTLINE WIDTH 0

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VertexExtrusionPass : ScriptableRenderPass
{
    private const string profilerTag = "Outline Pass";
    private static readonly ProfilingSampler profilingSampler = new ProfilingSampler(profilerTag);

    int shaderPass = 0;

    static readonly ShaderTagId SRPDefaultUnlit = new ShaderTagId("SRPDefaultUnlit");
    static readonly ShaderTagId UniversalForward = new ShaderTagId("UniversalForward");
    static readonly ShaderTagId LightweightForward = new ShaderTagId("LightweightForward");
    static readonly List<ShaderTagId> shaderTags = new List<ShaderTagId>() { SRPDefaultUnlit, UniversalForward, LightweightForward };

    private VertexExtrusionFeature.OutlineSettings settings;
    private FilteringSettings filteringSettings;
    private RenderStateBlock renderStateBlock;

    private Material outlineMaterial;

    public CompareFunction GetCompareFunction(DrawingMode drawingMode)
    {
        CompareFunction function = CompareFunction.Always;

        switch (drawingMode)
        {
            case DrawingMode.DepthCulled: function = CompareFunction.LessEqual; break;
            case DrawingMode.DepthCulledInverted: function = CompareFunction.GreaterEqual; break;
            case DrawingMode.OutlineAlways: function = CompareFunction.Always; break;
            case DrawingMode.HiddenFaces: function = CompareFunction.Greater; break;
        }

        return function;
    }

    public VertexExtrusionPass(VertexExtrusionFeature.OutlineSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;

        filteringSettings = new FilteringSettings(RenderQueueRange.all, -1, 1u << settings.layer - 1);

        if (outlineMaterial) CoreUtils.Destroy(outlineMaterial);
        outlineMaterial = CoreUtils.CreateEngineMaterial("Hidden/Vertex Extrusion");
        outlineMaterial.SetColor("_Color", settings.color);
        if(settings.drawingMode == DrawingMode.HiddenFaces) outlineMaterial.SetFloat("_Width", 0);
        else outlineMaterial.SetFloat("_Width", settings.width);
        outlineMaterial.enableInstancing = true;

        CompareFunction function = GetCompareFunction(settings.drawingMode);
        renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        renderStateBlock.mask |= RenderStateMask.Depth;
        renderStateBlock.depthState = new DepthState(settings.writeDepth, function);

        int srcBlend = 0, dstBlend = 0;

        // Set Blend Mode
        switch (settings.blendMode)
        {
            case BlendingMode.Alpha: srcBlend = (int)BlendMode.SrcAlpha; dstBlend = (int)BlendMode.OneMinusSrcAlpha; break;
            case BlendingMode.Premultiply: srcBlend = (int)BlendMode.One; dstBlend = (int)BlendMode.OneMinusSrcAlpha; break;
            case BlendingMode.Additive: srcBlend = (int)BlendMode.One; dstBlend = (int)BlendMode.One; break;
            case BlendingMode.SoftAdditive: srcBlend = (int)BlendMode.OneMinusDstColor; dstBlend = (int)BlendMode.One; break;
            case BlendingMode.Multiply: srcBlend = (int)BlendMode.DstColor; dstBlend = (int)BlendMode.Zero; break;
        }

        outlineMaterial.SetInt("_SrcBlend", srcBlend);
        outlineMaterial.SetInt("_DstBlend", dstBlend);

        switch (settings.method)
        {
            case ExtrusionMethod.ScaleObject: shaderPass = 0; break;
            case ExtrusionMethod.ScaleObjectNormalized: shaderPass = 1; break;
            case ExtrusionMethod.ExtrudeAlongNormal1: shaderPass = 2; break;
            case ExtrusionMethod.ExtrudeAlongNormal2: shaderPass = 3; break;
        }
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        DrawingSettings drawingSettings = CreateDrawingSettings(shaderTags, ref renderingData, SortingCriteria.CommonOpaque);
        drawingSettings.overrideMaterial = outlineMaterial;
        drawingSettings.overrideMaterialPassIndex = shaderPass;

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