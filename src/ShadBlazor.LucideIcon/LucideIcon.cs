using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ShadBlazor.LucideIcon.Base;

namespace ShadBlazor.LucideIcon;

public class LucideIcon : ComponentBase
{
    [Parameter] public string? Name { get; set; }
    [Parameter] public LucideIconName? Icon { get; set; }
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
        var targetType = ResolveComponentType();
        if (targetType != null)
        {
            builder.OpenComponent(0, targetType);
            builder.AddAttribute(1, nameof(LucideIconBase.Size), Size);
            builder.AddAttribute(2, nameof(LucideIconBase.Color), Color);
            builder.AddAttribute(3, nameof(LucideIconBase.StrokeWidth), StrokeWidth);
            builder.AddAttribute(4, nameof(LucideIconBase.AbsoluteStrokeWidth), AbsoluteStrokeWidth);
            builder.AddAttribute(5, nameof(LucideIconBase.Class), Class);
            builder.AddAttribute(6, nameof(LucideIconBase.Style), Style);
            builder.AddMultipleAttributes(7, AdditionalAttributes);
            builder.CloseComponent();
            return;
        }

        var model = ResolveModel();
        if (model != null && !string.IsNullOrEmpty(model.SvgMarkup))
        {
            builder.OpenElement(0, "svg");
            builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
            builder.AddAttribute(2, "width", Size);
            builder.AddAttribute(3, "height", Size);
            builder.AddAttribute(4, "viewBox", "0 0 24 24");
            builder.AddAttribute(5, "fill", "none");
            builder.AddAttribute(6, "stroke", Color);
            builder.AddAttribute(7, "stroke-width", StrokeWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));
            builder.AddAttribute(8, "stroke-linecap", "round");
            builder.AddAttribute(9, "stroke-linejoin", "round");
            if (!string.IsNullOrWhiteSpace(Class)) builder.AddAttribute(10, "class", Class);
            if (!string.IsNullOrWhiteSpace(Style)) builder.AddAttribute(11, "style", Style);
            builder.AddMultipleAttributes(12, AdditionalAttributes);
            builder.AddMarkupContent(13, model.SvgMarkup);
            builder.CloseElement();
        }
    }

    private Type? ResolveComponentType()
    {
        if (Icon.HasValue)
            return LucideRegistry.GetTypeByName(Icon.Value.ToString());

        if (!string.IsNullOrWhiteSpace(Name))
            return LucideRegistry.GetTypeByName(Name);

        return null;
    }

    private LucideIconModel? ResolveModel()
    {
        var key = Icon.HasValue ? Icon.Value.ToString() : Name;
        if (string.IsNullOrEmpty(key)) return null;
        return LucideRegistry.GetModelByName(key);
    }
}
