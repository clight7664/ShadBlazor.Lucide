# ShadBlazor.Lucide

<p align="center">
  <a href="https://github.com/clight7664/ShadBlazor.Lucide">
    <img src="https://raw.githubusercontent.com/lucide-icons/lucide/main/icons/sparkles.svg" width="64" height="64" alt="ShadBlazor.Lucide Logo" />
  </a>
</p>

<p align="center">
  <strong>基于 Lucide Icons 的高性能 Blazor 图标组件库</strong><br />
  <em>High-performance Lucide icon library for Blazor (.NET 6, 7, 8, 9) with 2,000+ icons, virtualization, and i18n support.</em>
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/ShadBlazor.LucideIcon"><img src="https://img.shields.io/nuget/v/ShadBlazor.LucideIcon.svg?style=flat-square" alt="NuGet Version" /></a>
  <a href="https://www.nuget.org/packages/ShadBlazor.LucideIcon"><img src="https://img.shields.io/nuget/dt/ShadBlazor.LucideIcon.svg?style=flat-square" alt="NuGet Downloads" /></a>
  <a href="https://github.com/clight7664/ShadBlazor.Lucide/blob/master/LICENSE"><img src="https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square" alt="License" /></a>
  <img src="https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0%20%7C%209.0-purple.svg?style=flat-square" alt=".NET Support" />
</p>

---

## 🌟 特性 / Features

- ⚡ **超高性能 (High Performance)**：基于纯 C# `RenderTreeBuilder` 代码生成与分类 Partial Class 架构，无多余 Razor 开销，完美适配 AOT 与 WebAssembly。
- 🎯 **多框架支持 (Multi-target)**：全面支持 `net6.0`, `net7.0`, `net8.0`, `net9.0`。
- 📦 **2,000+ 矢量图标 (2,000+ Icons)**：同步官方 Lucide 完整图标集，涵盖 40+ 业务分类与丰富元数据。
- 🚀 **虚拟化渲染 (Virtualized Rendering)**：内置 `<Virtualize>` 视口渲染，卡片 (Card) 与列表 (List) 视图 60fps 丝滑呈现。
- 🎨 **高度可定制 (Highly Customizable)**：自由调整 `Size`（尺寸）、`Color`（颜色）、`StrokeWidth`（线宽）、`AbsoluteStrokeWidth`（绝对线宽）及 `Class`（Tailwind CSS）。
- 🌐 **国际化支持 (i18n)**：全站支持中英文实时无缝切换。
- 🔍 **双重使用模式 (Strong & Dynamic Types)**：
  - 强类型组件：`<LucideCamera Size="24" />`
  - 动态组件：`<LucideIcon Name="camera" />`

---

## 🚀 快速开始 / Quick Start

### 1. 安装 NuGet 包 / Installation

```bash
dotnet add package ShadBlazor.LucideIcon
```

### 2. 引入命名空间 / Add Imports

在全局 `_Imports.razor` 中添加：

```razor
@using ShadBlazor.LucideIcon
@using ShadBlazor.LucideIcon.Base
```

### 3. 使用组件 / Component Usage

#### 强类型组件 (Strong-typed Component)
```razor
<LucideCamera Size="24" Color="currentColor" StrokeWidth="2.0" />
<LucideCheck Size="20" Color="#10b981" />
<LucideSparkles Size="24" Class="text-rose-500 animate-pulse" />
```

#### 动态组件 (Dynamic Component)
适用于菜单、数据库驱动配置或动态绑定场景：
```razor
<LucideIcon Name="camera" Size="24" Color="#3b82f6" />
<LucideIcon Icon="LucideIconName.Settings" Size="20" />
```

---

## 🖥️ 本地运行预览站点 / Documentation Site

```bash
# 启动文档站点 (Blazor WASM)
dotnet run --project samples/ShadBlazor.LucideIcon.Docs
```

在浏览器打开 `http://localhost:5000` 即可体验 1:1 还原 Lucide.dev 的图标画廊、粘性搜索、Customizer 实时调色盘与双语切换。

---

## 📦 NuGet 打包与发布教程 / NuGet Publishing Guide

本项目提供了一键式跨平台发布脚本与标准命令行流程。

### 方式一：使用一键自动化脚本 (推荐)

#### Windows (PowerShell)
```powershell
# 1. 仅构建、测试并生成 nupkg 包（不推送到源）
.\scripts\publish-nuget.ps1 -SkipPush

# 2. 一键打包并推送到 NuGet.org
.\scripts\publish-nuget.ps1 -ApiKey "YOUR_NUGET_API_KEY"

# 3. 指定版本号发布
.\scripts\publish-nuget.ps1 -Version "1.33.0" -ApiKey "YOUR_NUGET_API_KEY"
```

#### Linux / macOS (Bash)
```bash
chmod +x ./scripts/publish-nuget.sh

# 仅打包测试
./scripts/publish-nuget.sh --skip-push

# 一键打包并发布
./scripts/publish-nuget.sh --api-key "YOUR_NUGET_API_KEY"
```

---

### 方式二：手动执行标准 dotnet CLI 流程

#### 1. 更新版本号
在 `Directory.Build.props` 或 `src/ShadBlazor.LucideIcon/ShadBlazor.LucideIcon.csproj` 中更新版本号（例如 `1.33.0`）。

#### 2. 编译与单元测试
```bash
dotnet build -c Release
dotnet test tests/ShadBlazor.LucideIcon.Tests/ShadBlazor.LucideIcon.Tests.csproj -c Release
```

#### 3. 打包生成 Release NuGet 包
```bash
dotnet pack src/ShadBlazor.LucideIcon/ShadBlazor.LucideIcon.csproj -c Release -o ./artifacts
```

#### 4. 推送到 NuGet.org
```bash
dotnet nuget push ./artifacts/ShadBlazor.LucideIcon.*.nupkg \
  --api-key YOUR_NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate
```

---

## 🤝 贡献与规范 / Contributing

本项目采用 [Conventional Commits](https://www.conventionalcommits.org/) 提交规范，并通过 Husky 与 Commitlint 实施自动校验。

常用前缀：
- `feat:` 新增功能
- `fix:` 修复缺陷
- `docs:` 文档更新
- `style:` 代码格式/样式调整
- `refactor:` 代码重构
- `perf:` 性能优化
- `test:` 测试用例补充
- `chore:` 构建或辅助工具变动

---

## 📄 开源许可证 / License

MIT License © [ShadBlazor Team](https://github.com/clight7664/ShadBlazor.Lucide)
