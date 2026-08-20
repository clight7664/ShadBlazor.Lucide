namespace ShadBlazor.LucideIcon.Base;

public record LucideIconModel(
    string Name,
    string PascalName,
    string[] Tags,
    string[] Categories,
    string SvgMarkup = ""
);
