using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class StencilSettings
    {
        [Header("Rendering")]
        [Range(0, 32)] public int layer = 4;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
           public Material material;
    }

    [System.Serializable]
    public class OutlineSettings
    {
        [Header("Visual")]
        [ColorUsage(true, true)] public Color color = new Color(0.2f, 0.4f, 1, 1f);
        [Range(0.0f, 50.0f)] public float width = 4f;

        [Header("Rendering")]
        [Range(0, 32)] public int layer = 4;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        public Material material;
    }

    private OutlinePass outlinePass;
    private StencilPass stencilPass;
    
    public OutlineSettings outlineSettings = new OutlineSettings();
    public StencilSettings stencilSettings = new StencilSettings();

    public override void Create()
    {
        stencilPass = new StencilPass(stencilSettings);
        outlinePass = new OutlinePass(outlineSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        outlinePass.cameraColor = renderer.cameraColorTarget;

        renderer.EnqueuePass(stencilPass);
        renderer.EnqueuePass(outlinePass);
    }
}