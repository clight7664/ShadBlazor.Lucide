namespace ShadBlazor.LucideIcon.Docs.Services;

public class I18nService
{
    public string CurrentLanguage { get; private set; } = "zh";

    public bool IsZh => CurrentLanguage == "zh";

    public event Action? OnLanguageChanged;

    public void SetLanguage(string lang)
    {
        if (lang != "zh" && lang != "en") lang = "zh";
        if (CurrentLanguage != lang)
        {
            CurrentLanguage = lang;
            OnLanguageChanged?.Invoke();
        }
    }

    public void ToggleLanguage()
    {
        SetLanguage(CurrentLanguage == "zh" ? "en" : "zh");
    }

    public string T(string key, params object[] args)
    {
        if (_translations.TryGetValue(key, out var dict))
        {
            if (dict.TryGetValue(CurrentLanguage, out var text))
            {
                return args.Length > 0 ? string.Format(text, args) : text;
            }
        }
        return key;
    }

    public string GetCategoryName(string category)
    {
        if (CurrentLanguage == "en") return category;
        return _categoryTranslations.TryGetValue(category, out var zh) ? zh : category;
    }

    private static readonly Dictionary<string, string> _categoryTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        { "All", "全部" },
        { "Accessibility", "无障碍" },
        { "Account", "账户与用户" },
        { "Animals", "动物生态" },
        { "Arrows", "箭头方向" },
        { "Brands", "品牌标志" },
        { "Buildings", "建筑设施" },
        { "Charts", "图表统计" },
        { "Communication", "通讯社交" },
        { "Connectivity", "连接网络" },
        { "Cursors", "光标指针" },
        { "Design", "设计工具" },
        { "Development", "开发编程" },
        { "Devices", "硬件设备" },
        { "Emoji", "表情符号" },
        { "Files", "文件文档" },
        { "Finance", "财务金融" },
        { "Food & beverage", "餐饮美食" },
        { "Gaming", "游戏娱乐" },
        { "General", "通用基础" },
        { "Hardware", "硬件设施" },
        { "Home", "家居生活" },
        { "Layout", "布局排版" },
        { "Mail", "邮件消息" },
        { "Maps", "地图定位" },
        { "Math", "数学符号" },
        { "Media", "多媒体" },
        { "Medical", "医疗健康" },
        { "Multimedia", "影音媒体" },
        { "Navigation", "导航出行" },
        { "Network", "网络互联" },
        { "Notifications", "通知提醒" },
        { "Photography", "摄影拍照" },
        { "Science", "科学探索" },
        { "Security", "安全隐私" },
        { "Shapes", "几何形状" },
        { "Shopping", "购物电商" },
        { "Social", "社交媒体" },
        { "Sports", "运动健身" },
        { "Sustainability", "绿色环保" },
        { "System", "系统设置" },
        { "Text", "文本字体" },
        { "Time", "时间日期" },
        { "Tools", "实用工具" },
        { "Transportation", "交通出行" },
        { "Travel", "旅行度假" },
        { "Weather", "天气气候" }
    };

    private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        // Header
        { "Nav.Icons", new() { { "zh", "图标" }, { "en", "Icons" } } },
        { "Nav.Guide", new() { { "zh", "指南" }, { "en", "Guide" } } },
        { "Nav.Packages", new() { { "zh", "包与生态" }, { "en", "Packages" } } },
        { "Nav.Showcase", new() { { "zh", "展示" }, { "en", "Showcase" } } },
        { "Nav.ToggleTheme", new() { { "zh", "切换主题" }, { "en", "Toggle Theme" } } },
        { "Nav.Language", new() { { "zh", "EN" }, { "en", "中" } } },

        // Customizer
        { "Customizer.Title", new() { { "zh", "自定义面板" }, { "en", "Customizer" } } },
        { "Customizer.Reset", new() { { "zh", "重置默认" }, { "en", "Reset to defaults" } } },
        { "Customizer.Color", new() { { "zh", "颜色" }, { "en", "Color" } } },
        { "Customizer.StrokeWidth", new() { { "zh", "线条粗细" }, { "en", "Stroke width" } } },
        { "Customizer.Size", new() { { "zh", "图标尺寸" }, { "en", "Size" } } },
        { "Customizer.AbsoluteStroke", new() { { "zh", "绝对线宽" }, { "en", "Absolute stroke width" } } },

        // Sidebar
        { "Sidebar.View", new() { { "zh", "视图" }, { "en", "View" } } },
        { "Sidebar.All", new() { { "zh", "全部图标" }, { "en", "All" } } },
        { "Sidebar.Categories", new() { { "zh", "分类" }, { "en", "Categories" } } },

        // Search & Controls
        { "Search.Placeholder", new() { { "zh", "搜索 2,000+ 个图标（按名称、关键词或标签）... (Ctrl+K)" }, { "en", "Search 2,000+ icons by name, keyword or tags... (Ctrl+K)" } } },
        { "Search.Clear", new() { { "zh", "清除搜索" }, { "en", "Clear search" } } },
        { "Sort.Popularity", new() { { "zh", "热度推荐" }, { "en", "Popularity" } } },
        { "Sort.NameAZ", new() { { "zh", "名称 (A-Z)" }, { "en", "Name (A-Z)" } } },
        { "Sort.Category", new() { { "zh", "分类排序" }, { "en", "Category" } } },
        { "View.Card", new() { { "zh", "卡片网格" }, { "en", "Card Grid" } } },
        { "View.List", new() { { "zh", "列表视图" }, { "en", "Table List" } } },

        // Stats & Filter
        { "Stats.Showing", new() { { "zh", "正在显示 <strong>{0}</strong> / <strong>{1}</strong> 个图标" }, { "en", "Showing <strong>{0}</strong> of <strong>{1}</strong> icons" } } },
        { "Stats.CategoryFilter", new() { { "zh", "分类: {0}" }, { "en", "Category: {0}" } } },
        { "Stats.SearchFilter", new() { { "zh", "搜索: \"{0}\"" }, { "en", "Search: \"{0}\"" } } },
        { "Stats.EmptyTitle", new() { { "zh", "未找到匹配的图标。" }, { "en", "No icons found matching your query." } } },
        { "Stats.EmptyDesc", new() { { "zh", "请尝试搜索其他关键词或切换分类。" }, { "en", "Try searching for another keyword or switch category." } } },

        // List View Table
        { "List.ColIcon", new() { { "zh", "图标" }, { "en", "Icon" } } },
        { "List.ColName", new() { { "zh", "名称与组件" }, { "en", "Name & Component" } } },
        { "List.ColTags", new() { { "zh", "分类与标签" }, { "en", "Categories & Tags" } } },
        { "List.ColActions", new() { { "zh", "快捷操作" }, { "en", "Actions" } } },
        { "List.ActionTag", new() { { "zh", "组件" }, { "en", "Tag" } } },
        { "List.ActionName", new() { { "zh", "名称" }, { "en", "Name" } } },
        { "List.ActionSvg", new() { { "zh", "SVG" }, { "en", "SVG" } } },

        // Toasts
        { "Toast.CopiedTag", new() { { "zh", "已复制组件: <Lucide{0} />" }, { "en", "Copied component: <Lucide{0} />" } } },
        { "Toast.CopiedName", new() { { "zh", "已复制图标名: {0}" }, { "en", "Copied icon name: {0}" } } },
        { "Toast.CopiedSvg", new() { { "zh", "已复制 {0} SVG 代码！" }, { "en", "Copied {0} SVG code!" } } },

        // Detail Drawer
        { "Drawer.Title", new() { { "zh", "图标详情" }, { "en", "Icon Details" } } },
        { "Drawer.CopyStrongTag", new() { { "zh", "复制强类型组件代码" }, { "en", "Copy Component Code" } } },
        { "Drawer.CopyDynamicTag", new() { { "zh", "复制通用组件代码" }, { "en", "Copy Dynamic Component" } } },
        { "Drawer.CopySvg", new() { { "zh", "复制原始 SVG 代码" }, { "en", "Copy Raw SVG Markup" } } },
        { "Drawer.Tags", new() { { "zh", "搜索关键词 / 标签" }, { "en", "Tags / Keywords" } } },
        { "Drawer.Categories", new() { { "zh", "所属分类" }, { "en", "Categories" } } },

        // Guide Page Navigation
        { "Guide.Nav.Intro", new() { { "zh", "介绍" }, { "en", "Introduction" } } },
        { "Guide.Nav.WhatIs", new() { { "zh", "什么是 ShadBlazor.Lucide？" }, { "en", "What is ShadBlazor.Lucide?" } } },
        { "Guide.Nav.Installation", new() { { "zh", "安装指引" }, { "en", "Installation" } } },
        { "Guide.Nav.Basics", new() { { "zh", "基础使用" }, { "en", "Basics" } } },
        { "Guide.Nav.StrongTyped", new() { { "zh", "强类型组件" }, { "en", "Strong-typed Component" } } },
        { "Guide.Nav.Dynamic", new() { { "zh", "动态图标组件" }, { "en", "Dynamic Component" } } },
        { "Guide.Nav.Advanced", new() { { "zh", "进阶特性" }, { "en", "Advanced" } } },
        { "Guide.Nav.Tailwind", new() { { "zh", "Tailwind / Shadcn 样式融合" }, { "en", "Tailwind / Shadcn Styling" } } },
        { "Guide.Nav.AbsoluteStroke", new() { { "zh", "绝对线宽与比例缩放" }, { "en", "Absolute Stroke Width" } } },
        { "Guide.Nav.NugetPublish", new() { { "zh", "NuGet 发布流程教程" }, { "en", "NuGet Publishing Tutorial" } } },

        // Guide Page Content
        { "Guide.Title", new() { { "zh", "使用指南与技术文档" }, { "en", "Guide & Documentation" } } },
        { "Guide.Lead", new() { { "zh", "ShadBlazor.LucideIcon 是针对 Blazor 生态封装的高性能矢量图标库，提供超过 2,000+ 个高度一致且精美的矢量图标。最低支持 .NET 6.0，同时完美兼容 .NET 7.0 / 8.0 / 9.0。" }, { "en", "ShadBlazor.LucideIcon is a high-performance vector icon library for the Blazor ecosystem, providing over 2,000+ consistent and beautiful icons. Supporting .NET 6.0+, .NET 7.0, .NET 8.0, and .NET 9.0." } } },
        { "Guide.Install.Title", new() { { "zh", "安装与引用" }, { "en", "Installation & Import" } } },
        { "Guide.Install.Desc", new() { { "zh", "通过 NuGet 包管理器安装核心库：" }, { "en", "Install the package via NuGet package manager:" } } },
        { "Guide.Install.ImportsDesc", new() { { "zh", "在全局命名空间 _Imports.razor 中引入：" }, { "en", "Add the namespaces to your _Imports.razor:" } } },
        { "Guide.Basics.Title", new() { { "zh", "基础使用" }, { "en", "Usage & Basics" } } },
        { "Guide.Basics.StrongTitle", new() { { "zh", "1. 强类型组件使用" }, { "en", "1. Strong-typed Components" } } },
        { "Guide.Basics.DynamicTitle", new() { { "zh", "2. 动态组件使用（适用于数据库配置菜单）" }, { "en", "2. Dynamic Components (Great for database-driven menus)" } } },
        { "Guide.Tailwind.Title", new() { { "zh", "Tailwind CSS & Shadcn 深度集成" }, { "en", "Tailwind CSS & Shadcn Integration" } } },
        { "Guide.Tailwind.Desc", new() { { "zh", "直接通过 Class 参数传递 Tailwind CSS 工具类，尺寸与颜色将自动与样式系统融合：" }, { "en", "Pass Tailwind CSS utility classes directly via the Class parameter:" } } },
        { "Guide.Nuget.Title", new() { { "zh", "NuGet 包发布流程教程" }, { "en", "NuGet Publishing Tutorial" } } },
        { "Guide.Nuget.Step1", new() { { "zh", "步骤一：配置版本号与元数据" }, { "en", "Step 1: Configure Version & Metadata" } } },
        { "Guide.Nuget.Step1Desc", new() { { "zh", "在 Directory.Build.props 或 ShadBlazor.LucideIcon.csproj 中更新版本号（例如 1.33.0）与作者信息。" }, { "en", "Update the version (e.g. 1.33.0) and author info in Directory.Build.props or ShadBlazor.LucideIcon.csproj." } } },
        { "Guide.Nuget.Step2", new() { { "zh", "步骤二：编译与自动化测试" }, { "en", "Step 2: Compile & Run Unit Tests" } } },
        { "Guide.Nuget.Step2Desc", new() { { "zh", "确保所有框架（.NET 6/7/8/9）编译与测试均通过：" }, { "en", "Ensure all target frameworks pass build and tests:" } } },
        { "Guide.Nuget.Step3", new() { { "zh", "步骤三：生成 Release NuGet 包" }, { "en", "Step 3: Pack Release NuGet Package" } } },
        { "Guide.Nuget.Step3Desc", new() { { "zh", "打包生成 .nupkg 文件到输出目录：" }, { "en", "Pack and output .nupkg to artifacts folder:" } } },
        { "Guide.Nuget.Step4", new() { { "zh", "步骤四：推送到 NuGet.org 官方源" }, { "en", "Step 4: Push to NuGet.org" } } },
        { "Guide.Nuget.Step4Desc", new() { { "zh", "使用 API Key 发布 NuGet 包：" }, { "en", "Publish the package using your NuGet API Key:" } } },
        { "Guide.Nuget.Step5", new() { { "zh", "自动化一键脚本发布" }, { "en", "Step 5: One-Click Automated Script" } } },
        { "Guide.Nuget.Step5Desc", new() { { "zh", "直接运行项目内置的 PowerShell 自动化发布脚本：" }, { "en", "Run the built-in PowerShell publishing script:" } } },

        // Packages Page
        { "Packages.Title", new() { { "zh", "官方生态包矩阵" }, { "en", "Official Packages Matrix" } } },
        { "Packages.Desc", new() { { "zh", "ShadBlazor 官方生态包与周边工具" }, { "en", "ShadBlazor official ecosystem packages and tools" } } },
        { "Packages.Core.Desc", new() { { "zh", "2,000+ 核心矢量图标组件库，兼容 .NET 6.0/7.0/8.0/9.0 与 AOT 编译，提供极致性能。" }, { "en", "2,000+ core vector icon components for .NET 6.0/7.0/8.0/9.0 with AOT and blazing performance." } } },
        { "Packages.Gen.Desc", new() { { "zh", "自动同步官方最新 Lucide SVG 资源的 CLI 自动化代码生成引擎。" }, { "en", "CLI code generator engine to automatically sync latest Lucide SVG icons." } } },

        // Showcase Page
        { "Showcase.Title", new() { { "zh", "社区项目展示" }, { "en", "Community Showcase" } } },
        { "Showcase.Desc", new() { { "zh", "基于 ShadBlazor 与 Lucide Icons 构建的优秀项目与实践" }, { "en", "Inspiring projects and templates built with ShadBlazor and Lucide Icons" } } },
        { "Showcase.Admin.Title", new() { { "zh", "ShadBlazor Admin Pro" }, { "en", "ShadBlazor Admin Pro" } } },
        { "Showcase.Admin.Desc", new() { { "zh", "基于 Tailwind CSS 与 Lucide 图标打造的企业级极简后台管理系统，响应式与深色模式完美支持。" }, { "en", "Enterprise-ready clean admin dashboard built with Tailwind CSS and Lucide Icons." } } },
        { "Showcase.Canvas.Title", new() { { "zh", "Lucide Blazor Canvas" }, { "en", "Lucide Blazor Canvas" } } },
        { "Showcase.Canvas.Desc", new() { { "zh", "在线可交互式矢量图元编辑器与图标设计工具，支持 SVG 实时导出与组件代码生成。" }, { "en", "Interactive vector canvas editor and design tool with real-time SVG export." } } }
    };
}
