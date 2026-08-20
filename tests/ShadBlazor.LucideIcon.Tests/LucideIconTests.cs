using Bunit;
using ShadBlazor.LucideIcon;
using ShadBlazor.LucideIcon.Base;
using Xunit;

namespace ShadBlazor.LucideIcon.Tests;

public class LucideIconTests : TestContext
{
    [Fact]
    public void TypedIcon_RendersCorrectStandardSvgStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<LucideCamera>(parameters => parameters
            .Add(p => p.Size, 32)
            .Add(p => p.Color, "#ff0000")
            .Add(p => p.StrokeWidth, 1.5)
            .Add(p => p.Class, "test-camera-class")
            .Add(p => p.Style, "margin: 10px;")
        );

        // Assert
        var svg = cut.Find("svg");
        Assert.Equal("http://www.w3.org/2000/svg", svg.GetAttribute("xmlns"));
        Assert.Equal("32", svg.GetAttribute("width"));
        Assert.Equal("32", svg.GetAttribute("height"));
        Assert.Equal("0 0 24 24", svg.GetAttribute("viewBox"));
        Assert.Equal("none", svg.GetAttribute("fill"));
        Assert.Equal("#ff0000", svg.GetAttribute("stroke"));
        Assert.Equal("1.5", svg.GetAttribute("stroke-width"));
        Assert.Equal("round", svg.GetAttribute("stroke-linecap"));
        Assert.Equal("round", svg.GetAttribute("stroke-linejoin"));
        Assert.Equal("test-camera-class", svg.GetAttribute("class"));
        Assert.Equal("margin: 10px;", svg.GetAttribute("style"));

        // Verify child elements exist
        Assert.NotNull(svg.QuerySelector("path"));
        Assert.NotNull(svg.QuerySelector("circle"));
    }

    [Fact]
    public void DynamicIcon_RendersCorrectlyByName()
    {
        // Arrange & Act
        var cut = RenderComponent<LucideIcon>(parameters => parameters
            .Add(p => p.Name, "camera")
            .Add(p => p.Size, 28)
            .Add(p => p.Color, "blue")
        );

        // Assert
        var svg = cut.Find("svg");
        Assert.Equal("28", svg.GetAttribute("width"));
        Assert.Equal("blue", svg.GetAttribute("stroke"));
        Assert.NotNull(svg.QuerySelector("path"));
    }

    [Fact]
    public void DynamicIcon_RendersCorrectlyByEnum()
    {
        // Arrange & Act
        var cut = RenderComponent<LucideIcon>(parameters => parameters
            .Add(p => p.Icon, LucideIconName.Check)
            .Add(p => p.Size, 20)
            .Add(p => p.StrokeWidth, 2.5)
        );

        // Assert
        var svg = cut.Find("svg");
        Assert.Equal("20", svg.GetAttribute("width"));
        Assert.Equal("2.5", svg.GetAttribute("stroke-width"));
        Assert.NotNull(svg.QuerySelector("path"));
    }

    [Fact]
    public void AbsoluteStrokeWidth_CalculatesNormalizedStroke()
    {
        // Arrange: size = 48, StrokeWidth = 2 -> (24 / 48) * 2 = 1.0
        var cut = RenderComponent<LucideCamera>(parameters => parameters
            .Add(p => p.Size, 48)
            .Add(p => p.StrokeWidth, 2.0)
            .Add(p => p.AbsoluteStrokeWidth, true)
        );

        // Assert
        var svg = cut.Find("svg");
        Assert.Equal("1", svg.GetAttribute("stroke-width"));
    }

    [Fact]
    public void AdditionalAttributes_AreCapturedAndRendered()
    {
        // Arrange & Act
        var cut = RenderComponent<LucideCamera>(parameters => parameters
            .AddUnmatched("data-testid", "camera-icon")
            .AddUnmatched("aria-hidden", "true")
            .AddUnmatched("role", "img")
        );

        // Assert
        var svg = cut.Find("svg");
        Assert.Equal("camera-icon", svg.GetAttribute("data-testid"));
        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
        Assert.Equal("img", svg.GetAttribute("role"));
    }

    [Fact]
    public void LucideRegistry_ContainsCategoriesAndModels()
    {
        var icons = LucideRegistry.GetAllIcons();
        Assert.NotEmpty(icons);
        Assert.NotEmpty(LucideRegistry.Categories);
        Assert.Contains("All", LucideRegistry.Categories);
        Assert.Contains("Finance", LucideRegistry.Categories);
        Assert.NotNull(LucideRegistry.GetTypeByName("camera"));
        Assert.NotNull(LucideRegistry.GetTypeByName("check"));
    }

    [Fact]
    public void LucideRegistry_CategoryPartialClasses_ProvideDedicatedIconLists()
    {
        // Assert category partial class static properties
        Assert.NotEmpty(LucideRegistry.FinanceIcons);
        Assert.NotEmpty(LucideRegistry.CommunicationIcons);
        Assert.NotEmpty(LucideRegistry.ArrowsIcons);

        // Verify GetIconsByCategory method
        var financeIcons = LucideRegistry.GetIconsByCategory("Finance");
        Assert.NotEmpty(financeIcons);
        Assert.Contains(financeIcons, i => i.Name == "banknote");

        var allIcons = LucideRegistry.GetIconsByCategory("All");
        Assert.Equal(LucideRegistry.GetAllIcons().Count, allIcons.Count);

        // Verify model metadata
        var banknoteModel = LucideRegistry.GetModelByName("banknote");
        Assert.NotNull(banknoteModel);
        Assert.Equal("banknote", banknoteModel.Name);
        Assert.Equal("Banknote", banknoteModel.PascalName);
        Assert.NotEmpty(banknoteModel.Tags);
        Assert.NotEmpty(banknoteModel.Categories);
        Assert.NotEmpty(banknoteModel.SvgMarkup);
    }
}
