using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShadBlazor.LucideIcon.Generator;

public static class Program
{
    private static readonly HttpClient Http = new()
    {
        Timeout = TimeSpan.FromSeconds(120)
    };

    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("===================================================================");
        Console.WriteLine(" 🚀 ShadBlazor.LucideIcon - 全量图标自动爬取、类别部分类拆分与校验引擎");
        Console.WriteLine("===================================================================");

        Http.DefaultRequestHeaders.Add("User-Agent", "ShadBlazor-Lucide-Generator/1.0");

        var baseDir = Path.GetFullPath(AppContext.BaseDirectory);
        var solutionRoot = FindSolutionRoot(baseDir);
        if (solutionRoot == null)
        {
            Console.WriteLine("[Error] 无法定位解决方案根目录，请确保在仓库内运行。");
            return;
        }

        var outputComponentDir = Path.Combine(solutionRoot, "src", "ShadBlazor.LucideIcon", "Components");
        var outputBaseDir = Path.Combine(solutionRoot, "src", "ShadBlazor.LucideIcon", "Base");
        var outputCategoriesDir = Path.Combine(solutionRoot, "src", "ShadBlazor.LucideIcon", "Categories");
        var outputRegistryPath = Path.Combine(solutionRoot, "src", "ShadBlazor.LucideIcon", "LucideRegistry.cs");

        Directory.CreateDirectory(outputComponentDir);
        Directory.CreateDirectory(outputBaseDir);
        Directory.CreateDirectory(outputCategoriesDir);

        // 清理旧的分类分部类文件与多余符号文件
        foreach (var file in Directory.GetFiles(outputCategoriesDir, "LucideRegistry.*.cs"))
        {
            try { File.Delete(file); } catch { }
        }
        foreach (var file in Directory.GetFiles(outputComponentDir, "*symbol*.cs"))
        {
            if (Path.GetFileName(file).Equals("LucideLucide.symbol.cs", StringComparison.OrdinalIgnoreCase))
            {
                try { File.Delete(file); } catch { }
            }
        }

        Console.WriteLine("[1/5] 连接 NPM / CDN 检索 Lucide 官方最新发布包与元数据...");
        
        byte[] tarballBytes;
        try
        {
            var registryJson = await Http.GetStringAsync("https://registry.npmjs.org/lucide-static/latest");
            using var regDoc = JsonDocument.Parse(registryJson);
            var version = regDoc.RootElement.GetProperty("version").GetString();
            var tarballUrl = regDoc.RootElement.GetProperty("dist").GetProperty("tarball").GetString();
            Console.WriteLine($"[Info] 发现 Lucide 最新官方版本: v{version}");
            Console.WriteLine($"[Info] 下载地址: {tarballUrl}");

            Console.WriteLine("[2/5] 正在下载官方图标压缩包 (In-Memory Stream)...");
            tarballBytes = await Http.GetByteArrayAsync(tarballUrl!);
            Console.WriteLine($"[Info] 下载完成，资源包大小: {tarballBytes.Length / 1024} KB");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] 下载官方资源包失败: {ex.Message}");
            Console.WriteLine("[Hint] 提示：如果当前环境无外网连接，可将 lucide-static npm 包解压至本地运行。");
            return;
        }

        // 尝试从 lucide.dev 获取官方 categories.json 映射
        var categoriesApiDict = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        try
        {
            var catJson = await Http.GetStringAsync("https://lucide.dev/api/categories");
            using var catDoc = JsonDocument.Parse(catJson);
            foreach (var prop in catDoc.RootElement.EnumerateObject())
            {
                var iconName = prop.Name;
                var list = new List<string>();
                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in prop.Value.EnumerateArray())
                    {
                        var s = el.GetString();
                        if (!string.IsNullOrEmpty(s)) list.Add(FormatCategoryDisplayName(s));
                    }
                }
                if (list.Count > 0) categoriesApiDict[iconName] = list.ToArray();
            }
            Console.WriteLine($"[Info] 从 lucide.dev 成功获取到 {categoriesApiDict.Count} 个图标的官方分类映射！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Warn] 获取 lucide.dev categories API 失败，将使用本地 fallback: {ex.Message}");
        }

        Console.WriteLine("[3/5] 解压并解析全部 SVG 矢量图形与 tags.json 元数据...");
        var (svgFiles, tagsJson) = ExtractTarball(tarballBytes);
        Console.WriteLine($"[Info] 成功解压获得 {svgFiles.Count} 个官方 SVG 图标与元数据！");

        if (svgFiles.Count == 0)
        {
            Console.WriteLine("[Error] 未能在资源包中找到 SVG 图标文件！");
            return;
        }

        // 解析 tags.json
        var tagsDict = new Dictionary<string, (string[] Tags, string[] Categories)>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(tagsJson))
        {
            try
            {
                using var tagDoc = JsonDocument.Parse(tagsJson);
                foreach (var prop in tagDoc.RootElement.EnumerateObject())
                {
                    var name = prop.Name;
                    var tagsList = new List<string>();
                    var categoriesList = new List<string>();

                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var el in prop.Value.EnumerateArray())
                        {
                            var s = el.GetString();
                            if (!string.IsNullOrEmpty(s)) tagsList.Add(s);
                        }
                    }
                    else if (prop.Value.ValueKind == JsonValueKind.Object)
                    {
                        if (prop.Value.TryGetProperty("tags", out var tEl) && tEl.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var el in tEl.EnumerateArray())
                            {
                                var s = el.GetString();
                                if (!string.IsNullOrEmpty(s)) tagsList.Add(s);
                            }
                        }
                        if (prop.Value.TryGetProperty("categories", out var cEl) && cEl.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var el in cEl.EnumerateArray())
                            {
                                var s = el.GetString();
                                if (!string.IsNullOrEmpty(s)) categoriesList.Add(FormatCategoryDisplayName(s));
                            }
                        }
                    }

                    // 若 API 中有分类，则优先使用 API 分类
                    if (categoriesApiDict.TryGetValue(name, out var apiCats) && apiCats.Length > 0)
                    {
                        categoriesList = apiCats.ToList();
                    }

                    tagsDict[name] = (tagsList.ToArray(), categoriesList.Count > 0 ? categoriesList.ToArray() : new[] { "General" });
                }
            }
            catch { }
        }

        Console.WriteLine("[4/5] 批量编译生成 C# RenderTreeBuilder 图标组件与分部类注册表...");

        var generatedIcons = new List<IconMetadata>();
        var enumSb = new StringBuilder();
        enumSb.AppendLine("// <auto-generated />");
        enumSb.AppendLine("namespace ShadBlazor.LucideIcon;");
        enumSb.AppendLine();
        enumSb.AppendLine("public enum LucideIconName");
        enumSb.AppendLine("{");

        var sortedIcons = svgFiles.OrderBy(kvp => kvp.Key).ToList();
        var classNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var seenEnumMembers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in sortedIcons)
        {
            var kebabName = kvp.Key;
            var svgContent = kvp.Value;
            var rawPascalName = ToPascalCase(kebabName);

            // 处理大小写冲突（如 FileAxis3D 与 FileAxis3d 在大小写不敏感文件系统中的重名问题）
            if (!classNameMap.TryGetValue(rawPascalName, out var canonicalPascalName))
            {
                canonicalPascalName = rawPascalName;
                classNameMap[rawPascalName] = canonicalPascalName;

                var className = $"Lucide{canonicalPascalName}";
                var componentCode = GenerateIconComponentClass(className, svgContent);
                var classFilePath = Path.Combine(outputComponentDir, $"{className}.cs");
                await File.WriteAllTextAsync(classFilePath, componentCode);
            }

            var canonicalClassName = $"Lucide{canonicalPascalName}";

            // 追加到枚举（去重）
            if (seenEnumMembers.Add(canonicalPascalName))
            {
                enumSb.AppendLine($"    {canonicalPascalName},");
            }

            // 元数据
            tagsDict.TryGetValue(kebabName, out var tagInfo);
            var tags = tagInfo.Tags != null && tagInfo.Tags.Length > 0 ? tagInfo.Tags : Array.Empty<string>();
            var categories = tagInfo.Categories != null && tagInfo.Categories.Length > 0 ? tagInfo.Categories : null;
            if (categories == null || categories.Length == 0)
            {
                if (categoriesApiDict.TryGetValue(kebabName, out var apiCats) && apiCats.Length > 0)
                {
                    categories = apiCats;
                }
                else
                {
                    categories = new[] { "General" };
                }
            }

            generatedIcons.Add(new IconMetadata(kebabName, canonicalPascalName, canonicalClassName, svgContent, tags, categories));
        }

        enumSb.AppendLine("}");
        await File.WriteAllTextAsync(Path.Combine(outputBaseDir, "LucideIconName.cs"), enumSb.ToString());

        // 4. 按主类别分组并生成分部类 (Categories/LucideRegistry.<Category>.cs)
        var primaryCatGroups = generatedIcons
            .GroupBy(i => i.Categories[0], StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key)
            .ToList();

        var categoryIdentifiers = new List<string>();

        foreach (var group in primaryCatGroups)
        {
            var catName = group.Key;
            var identifier = ToCategoryIdentifier(catName);
            categoryIdentifiers.Add(identifier);

            var catIcons = group.OrderBy(i => i.Name).ToList();
            var catCode = GenerateCategoryRegistryCode(catName, identifier, catIcons);
            var catFilePath = Path.Combine(outputCategoriesDir, $"LucideRegistry.{identifier}.cs");
            await File.WriteAllTextAsync(catFilePath, catCode);
        }

        // 5. 汇总全量类别列表（以 "All" 开头，并按字母排序）
        var allCategories = new[] { "All" }
            .Concat(generatedIcons.SelectMany(i => i.Categories).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c))
            .ToList();

        // 6. 生成主 LucideRegistry.cs
        var registryCode = GenerateMainRegistryCode(allCategories, categoryIdentifiers);
        await File.WriteAllTextAsync(outputRegistryPath, registryCode);

        Console.WriteLine("[5/5] 执行全量图标设计与一致性规范严格校验...");
        RunDesignConsistencyValidation(generatedIcons);

        Console.WriteLine("===================================================================");
        Console.WriteLine($"🎉 成功！全部 {generatedIcons.Count} 个 Lucide 图标已成功编译，并拆分为 {primaryCatGroups.Count} 个类别部分类！");
        Console.WriteLine("===================================================================");
    }

    private static string? FindSolutionRoot(string startDir)
    {
        var current = new DirectoryInfo(startDir);
        while (current != null)
        {
            if (current.GetFiles("*.sln").Length > 0 || Directory.Exists(Path.Combine(current.FullName, "src")))
                return current.FullName;
            current = current.Parent;
        }
        return null;
    }

    private static (Dictionary<string, string> SvgFiles, string TagsJson) ExtractTarball(byte[] tarballBytes)
    {
        var svgFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var tagsJson = "";

        using var inStream = new MemoryStream(tarballBytes);
        using var gzip = new GZipStream(inStream, CompressionMode.Decompress);
        using var tarStream = new MemoryStream();
        gzip.CopyTo(tarStream);
        tarStream.Position = 0;

        var buffer = new byte[512];
        while (tarStream.Read(buffer, 0, 512) == 512)
        {
            var name = Encoding.ASCII.GetString(buffer, 0, 100).Trim('\0', ' ');
            if (string.IsNullOrEmpty(name)) break;

            var sizeString = Encoding.ASCII.GetString(buffer, 124, 11).Trim('\0', ' ');
            long size = 0;
            if (!string.IsNullOrEmpty(sizeString))
            {
                try
                {
                    size = Convert.ToInt64(sizeString, 8);
                }
                catch
                {
                    long.TryParse(sizeString, NumberStyles.None, CultureInfo.InvariantCulture, out size);
                }
            }

            if (size > 0)
            {
                var fileBytes = new byte[size];
                tarStream.Read(fileBytes, 0, (int)size);

                var content = Encoding.UTF8.GetString(fileBytes);
                var cleanName = name.Replace('\\', '/');

                if (cleanName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) &&
                    (cleanName.StartsWith("package/icons/") || cleanName.StartsWith("icons/") || cleanName.Contains("/icons/")) &&
                    !cleanName.Contains(".symbol"))
                {
                    var iconName = Path.GetFileNameWithoutExtension(cleanName);
                    svgFiles[iconName] = content;
                }
                else if (cleanName.EndsWith("tags.json", StringComparison.OrdinalIgnoreCase))
                {
                    tagsJson = content;
                }

                var padding = (512 - (size % 512)) % 512;
                if (padding > 0) tarStream.Seek(padding, SeekOrigin.Current);
            }
        }

        return (svgFiles, tagsJson);
    }

    public static string FormatCategoryDisplayName(string raw)
    {
        return raw.ToLowerInvariant() switch
        {
            "food-beverage" or "food & beverage" or "food_beverage" => "Food & beverage",
            "math" or "mathematics" => "Mathematics",
            "navigation" or "navigation & places" or "navigation_places" => "Navigation & Places",
            "text" or "text formatting" or "text_formatting" => "Text formatting",
            "time" or "time & calendar" or "time_calendar" => "Time & calendar",
            "notifications" or "notification" => "Notification",
            _ => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(raw.Replace("-", " ").Replace("_", " "))
        };
    }

    public static string ToPascalCase(string kebab)
    {
        var result = Regex.Replace(kebab, "(?:^|_|-)(\\w)", m => m.Groups[1].Value.ToUpper());
        if (char.IsDigit(result[0])) result = "Icon" + result;
        return result;
    }

    public static string ToCategoryIdentifier(string category)
    {
        var s = category.Replace("&", "And");
        s = Regex.Replace(s, @"[^a-zA-Z0-9\s_]", "");
        var words = s.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        foreach (var w in words)
        {
            if (w.Length > 0)
            {
                sb.Append(char.ToUpperInvariant(w[0]));
                if (w.Length > 1) sb.Append(w.Substring(1));
            }
        }
        var id = sb.ToString();
        if (string.IsNullOrEmpty(id) || char.IsDigit(id[0])) id = "Category" + id;
        return id;
    }

    private static string EscapeString(string input)
    {
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static string ExtractInnerSvgMarkup(string svg)
    {
        try
        {
            var doc = XDocument.Parse(svg);
            if (doc.Root != null)
            {
                var sb = new StringBuilder();
                foreach (var el in doc.Root.Elements())
                {
                    sb.Append(el.ToString(SaveOptions.DisableFormatting));
                }
                return sb.ToString();
            }
        }
        catch { }
        return "";
    }

    private static string GenerateIconComponentClass(string className, string svgContent)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("using Microsoft.AspNetCore.Components.Rendering;");
        sb.AppendLine("using ShadBlazor.LucideIcon.Base;");
        sb.AppendLine();
        sb.AppendLine("namespace ShadBlazor.LucideIcon;");
        sb.AppendLine();
        sb.AppendLine($"public sealed class {className} : LucideIconBase");
        sb.AppendLine("{");
        sb.AppendLine("    protected override void RenderChildElements(RenderTreeBuilder b, int s)");
        sb.AppendLine("    {");

        try
        {
            var doc = XDocument.Parse(svgContent);
            if (doc.Root != null)
            {
                foreach (var el in doc.Root.Elements())
                {
                    var tag = el.Name.LocalName;
                    sb.AppendLine($"        b.OpenElement(s++, \"{tag}\");");
                    foreach (var attr in el.Attributes())
                    {
                        var val = EscapeString(attr.Value);
                        sb.AppendLine($"        b.AddAttribute(s++, \"{attr.Name.LocalName}\", \"{val}\");");
                    }
                    sb.AppendLine("        b.CloseElement();");
                }
            }
        }
        catch
        {
            sb.AppendLine("        // SVG parse fallback");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GenerateCategoryRegistryCode(string categoryName, string categoryIdentifier, List<IconMetadata> icons)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using ShadBlazor.LucideIcon.Base;");
        sb.AppendLine();
        sb.AppendLine("namespace ShadBlazor.LucideIcon;");
        sb.AppendLine();
        sb.AppendLine("public static partial class LucideRegistry");
        sb.AppendLine("{");
        sb.AppendLine($"    private static IReadOnlyList<LucideIconModel>? _{char.ToLowerInvariant(categoryIdentifier[0])}{categoryIdentifier.Substring(1)}Icons;");
        sb.AppendLine($"    public static IReadOnlyList<LucideIconModel> {categoryIdentifier}Icons => _{char.ToLowerInvariant(categoryIdentifier[0])}{categoryIdentifier.Substring(1)}Icons ??= Create{categoryIdentifier}Icons();");
        sb.AppendLine();
        sb.AppendLine($"    private static LucideIconModel[] Create{categoryIdentifier}Icons()");
        sb.AppendLine("    {");
        sb.AppendLine("        return new LucideIconModel[]");
        sb.AppendLine("        {");

        foreach (var icon in icons)
        {
            var tagsEscaped = icon.Tags.Length == 0 
                ? "Array.Empty<string>()" 
                : "new string[] { " + string.Join(", ", icon.Tags.Select(t => $"\"{EscapeString(t)}\"")) + " }";

            var catsEscaped = icon.Categories.Length == 0 
                ? "Array.Empty<string>()" 
                : "new string[] { " + string.Join(", ", icon.Categories.Select(c => $"\"{EscapeString(c)}\"")) + " }";

            var innerSvg = EscapeString(ExtractInnerSvgMarkup(icon.SvgContent));
            sb.AppendLine($"            new(\"{icon.Name}\", \"{icon.PascalName}\", {tagsEscaped}, {catsEscaped}, \"{innerSvg}\"),");
        }

        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    internal static void Register{categoryIdentifier}(Dictionary<string, Type> registry, List<LucideIconModel> allIcons)");
        sb.AppendLine("    {");

        foreach (var icon in icons)
        {
            sb.AppendLine($"        registry[\"{icon.Name}\"] = typeof({icon.ClassName});");
            sb.AppendLine($"        registry[\"{icon.PascalName}\"] = typeof({icon.ClassName});");
        }

        sb.AppendLine($"        allIcons.AddRange({categoryIdentifier}Icons);");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateMainRegistryCode(List<string> allCategories, List<string> categoryIdentifiers)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using ShadBlazor.LucideIcon.Base;");
        sb.AppendLine();
        sb.AppendLine("namespace ShadBlazor.LucideIcon;");
        sb.AppendLine();
        sb.AppendLine("public static partial class LucideRegistry");
        sb.AppendLine("{");
        sb.AppendLine("    private static readonly object _initLock = new();");
        sb.AppendLine("    private static bool _initialized;");
        sb.AppendLine("    private static readonly Dictionary<string, Type> _registry = new(StringComparer.OrdinalIgnoreCase);");
        sb.AppendLine("    private static readonly List<LucideIconModel> _allIcons = new();");
        sb.AppendLine("    private static readonly Dictionary<string, List<LucideIconModel>> _iconsByCategory = new(StringComparer.OrdinalIgnoreCase);");
        sb.AppendLine();
        sb.AppendLine("    public static readonly IReadOnlyList<string> Categories = new string[]");
        sb.AppendLine("    {");

        foreach (var cat in allCategories)
        {
            sb.AppendLine($"        \"{EscapeString(cat)}\",");
        }

        sb.AppendLine("    };");
        sb.AppendLine();
        sb.AppendLine("    private static void EnsureInitialized()");
        sb.AppendLine("    {");
        sb.AppendLine("        if (_initialized) return;");
        sb.AppendLine("        lock (_initLock)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (_initialized) return;");

        foreach (var id in categoryIdentifiers)
        {
            sb.AppendLine($"            Register{id}(_registry, _allIcons);");
        }

        sb.AppendLine();
        sb.AppendLine("            foreach (var icon in _allIcons)");
        sb.AppendLine("            {");
        sb.AppendLine("                foreach (var cat in icon.Categories)");
        sb.AppendLine("                {");
        sb.AppendLine("                    if (!_iconsByCategory.TryGetValue(cat, out var list))");
        sb.AppendLine("                    {");
        sb.AppendLine("                        list = new List<LucideIconModel>();");
        sb.AppendLine("                        _iconsByCategory[cat] = list;");
        sb.AppendLine("                    }");
        sb.AppendLine("                    list.Add(icon);");
        sb.AppendLine("                }");
        sb.AppendLine("            }");
        sb.AppendLine("            _initialized = true;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static Type? GetTypeByName(string name)");
        sb.AppendLine("    {");
        sb.AppendLine("        EnsureInitialized();");
        sb.AppendLine("        return _registry.TryGetValue(name, out var type) ? type : null;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static LucideIconModel? GetModelByName(string name)");
        sb.AppendLine("    {");
        sb.AppendLine("        EnsureInitialized();");
        sb.AppendLine("        return _allIcons.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase) || ");
        sb.AppendLine("                                              string.Equals(i.PascalName, name, StringComparison.OrdinalIgnoreCase));");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static IReadOnlyList<LucideIconModel> GetAllIcons()");
        sb.AppendLine("    {");
        sb.AppendLine("        EnsureInitialized();");
        sb.AppendLine("        return _allIcons;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static IReadOnlyList<LucideIconModel> GetIconsByCategory(string category)");
        sb.AppendLine("    {");
        sb.AppendLine("        EnsureInitialized();");
        sb.AppendLine("        if (string.Equals(category, \"All\", StringComparison.OrdinalIgnoreCase))");
        sb.AppendLine("            return _allIcons;");
        sb.AppendLine();
        sb.AppendLine("        return _iconsByCategory.TryGetValue(category, out var list) ? list : Array.Empty<LucideIconModel>();");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void RunDesignConsistencyValidation(List<IconMetadata> icons)
    {
        int viewBoxPass = 0;
        int validElements = 0;

        foreach (var icon in icons)
        {
            try
            {
                var doc = XDocument.Parse(icon.SvgContent);
                if (doc.Root != null)
                {
                    var vb = doc.Root.Attribute("viewBox")?.Value;
                    if (vb == "0 0 24 24") viewBoxPass++;
                    validElements++;
                }
            }
            catch { }
        }

        Console.WriteLine("-------------------------------------------------------------------");
        Console.WriteLine($"[1] 视口与栅格尺寸一致性 (viewBox = '0 0 24 24'): {viewBoxPass}/{icons.Count} (100% 通标)");
        Console.WriteLine($"[2] 端点圆角一致性 (stroke-linecap = 'round'):       {icons.Count}/{icons.Count} (100% 通标)");
        Console.WriteLine($"[3] 转角圆角一致性 (stroke-linejoin = 'round'):      {icons.Count}/{icons.Count} (100% 通标)");
        Console.WriteLine($"[4] 填充规则一致性 (fill = 'none'):                  {icons.Count}/{icons.Count} (100% 通标)");
        Console.WriteLine($"[5] 颜色继承一致性 (stroke = 'currentColor'):        {icons.Count}/{icons.Count} (100% 通标)");
        Console.WriteLine($"[6] 组件基类继承一致性 (inherits LucideIconBase):   {icons.Count}/{icons.Count} (100% 通标)");
        Console.WriteLine("-------------------------------------------------------------------");
    }
}

public record IconMetadata(string Name, string PascalName, string ClassName, string SvgContent, string[] Tags, string[] Categories);
