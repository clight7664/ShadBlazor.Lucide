using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace ShadBlazor.LucideIcon.Base;

public abstract class LucideIconBase : ComponentBase
{
    [Parameter] public int Size { get; set; } = 24;
    [Parameter] public string Color { get; set; } = "currentColor";
    [Parameter] public double StrokeWidth { get; set; } = 2.0;
    [Parameter] public bool AbsoluteStrokeWidth { get; set; } = false;
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", Size);
        builder.AddAttribute(3, "height", Size);
        builder.AddAttribute(4, "viewBox", "0 0 24 24");
        builder.AddAttribute(5, "fill", "none");
        builder.AddAttribute(6, "stroke", Color);

        var calculatedStroke = AbsoluteStrokeWidth ? (StrokeWidth * 24.0) / Size : StrokeWidth;
        builder.AddAttribute(7, "stroke-width", calculatedStroke.ToString("0.##", CultureInfo.InvariantCulture));
        builder.AddAttribute(8, "stroke-linecap", "round");
        builder.AddAttribute(9, "stroke-linejoin", "round");

        if (!string.IsNullOrWhiteSpace(Class))
            builder.AddAttribute(10, "class", Class);

        if (!string.IsNullOrWhiteSpace(Style))
            builder.AddAttribute(11, "style", Style);

        builder.AddMultipleAttributes(12, AdditionalAttributes);

        RenderChildElements(builder, 13);

        builder.CloseElement();
    }

    protected abstract void RenderChildElements(RenderTreeBuilder builder, int sequence);
}
