
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum ExtrusionMethod
{
    ScaleObject,
    ScaleObjectNormalized,
    ExtrudeAlongNormal1,
    ExtrudeAlongNormal2,
}

public enum DrawingMode
{
    OutlineAlways,
    HiddenFaces,
    DepthCulled,
    DepthCulledInverted,
}

public enum BlendingMode
{
    Alpha,
    Premultiply,
    Additive,
    SoftAdditive,
    Multiply
}

public class VertexExtrusionFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class OutlineSettings
    {
        public ExtrusionMethod method;
        public DrawingMode drawingMode;

        [Header("Visual")]
        [ColorUsage(true, true)] public Color color = new Color(0.2f, 0.4f, 1, 1f);
        [Range(0f, 1f)] public float width = 0.5f;
        public BlendingMode blendMode;

        [Header("Rendering")]
        [Range(0, 32)] public int layer = 4;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public bool writeDepth = true;
    }

    // render passes
    private StencilMaskPass stencilMaskPass;
    private VertexExtrusionPass outlinePass;

    // render pass settings
    public OutlineSettings outlineSettings = new OutlineSettings();
    public StencilMaskSettings stencilMaskSettings = new StencilMaskSettings();

    public override void Create()
    {
        stencilMaskPass = new StencilMaskPass(stencilMaskSettings);
        outlinePass = new VertexExtrusionPass(outlineSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (outlineSettings.drawingMode != DrawingMode.HiddenFaces) renderer.EnqueuePass(stencilMaskPass);
        renderer.EnqueuePass(outlinePass);
    }
}