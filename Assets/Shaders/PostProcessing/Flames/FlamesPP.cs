using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(FlamesPPRenderer), PostProcessEvent.AfterStack, "Custom/Flames")]
public sealed class FlamesPP : PostProcessEffectSettings {
    [Range(0.0f, 1.0f), Tooltip("The amount of flames on the screen.")]
    public FloatParameter flameAmount = new FloatParameter { value = 1.0f };

    public ColorParameter baseColor =   new ColorParameter { value = new Color(0.9f, 0.0f, 0.0f) };
    public ColorParameter midColor =    new ColorParameter { value = new Color(1.0f, 0.5f, 0.0f) };
    public ColorParameter tipColor =    new ColorParameter { value = new Color(1.0f, 0.9f, 0.0f) };
}


public sealed class FlamesPPRenderer : PostProcessEffectRenderer<FlamesPP> {
    public override void Render(PostProcessRenderContext context) {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/FlamesPP"));
        sheet.properties.SetFloat("_FlameAmount", settings.flameAmount);
        sheet.properties.SetColor("_FlameBaseColor", settings.baseColor);
        sheet.properties.SetColor("_FlameMidColor", settings.midColor);
        sheet.properties.SetColor("_FlameTipColor", settings.tipColor);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}