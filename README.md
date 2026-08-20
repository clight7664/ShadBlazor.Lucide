# ShadBlazor.LucideIcon

基于 [Lucide Icons](https://lucide.dev/) 的高性能 Blazor 图标组件库，**最低支持 .NET 6.0**，同时完美兼容 **.NET 7.0 / 8.0 / 9.0**。

## 🌟 特性

- ⚡ **超高性能**：基于原生 `RenderTreeBuilder` C# 代码生成，无额外 Razor 开销，支持 AOT。
- 🎯 **多框架支持**：支持 `net6.0`, `net7.0`, `net8.0`, `net9.0`。
- 🎨 **高度可定制**：灵活调整 `Size`、`Color`、`StrokeWidth`、`AbsoluteStrokeWidth` 与 `Class`（Tailwind）。
- 🔍 **智能感知**：提供全部强类型组件（如 `<LucideCamera />`）与动态渲染组件（`<LucideIcon Name="..." />`）。
- 🖥️ **现代化站点**：内置 1:1 复刻 Lucide.dev 风格的在线预览与代码复制站点。

## 🚀 快速开始

### 1. 安装与引用
```razor
<!-- _Imports.razor -->
@using ShadBlazor.LucideIcon
@using ShadBlazor.LucideIcon.Base
```

### 2. 使用强类型组件
```razor
<LucideCamera Size="24" Color="currentColor" StrokeWidth="2" />
<LucideSparkles Size="20" Class="text-rose-500 animate-pulse" />
```

### 3. 使用动态组件
```razor
<LucideIcon Name="camera" Size="24" />
<LucideIcon Icon="LucideIconName.Settings" Size="20" />
```

## 🛠️ 运行预览站点
```bash
cd samples/ShadBlazor.LucideIcon.Docs
dotnet run
```
打开浏览器访问 `http://localhost:5000` 即可浏览画廊。
