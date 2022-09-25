using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class StencilMaskSettings
{
    [Header("Rendering")]
    [Range(0, 32)] public int layer = 4;
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
}

