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
        { "Drawer.Categories", new() { { "zh", "所属分类" }, { "en", "Categories" } } }
    };
}
