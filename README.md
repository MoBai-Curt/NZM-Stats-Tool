<div align="center">
  <img src="./screenshots/logo.png" alt="Logo" width="120" height="120">
  <h1 align="center">NZM 战绩助手 (NZM Helper)</h1>

  <p align="center">
    一款基于 .NET 8 + WPF 构建的现代化逆战战绩查询工具。
    <br />
    <span style="color: #3b82f6; font-weight: bold;">(由原 Python 版本完全重构迁移至 C#)</span>
    <br />
    <br />
    <a href="http://mobaiya.icu/"><strong>作者博客</strong></a>
    ·
    <a href="https://github.com/MoBai-Curt/NZM-Stats-Tool/releases">下载最新版</a>
    ·
    <a href="https://github.com/MoBai-Curt/NZM-Stats-Tool/issues">报告 Bug</a>
  </p>

  <p align="center">
    <img src="https://img.shields.io/badge/Refactored-Python%20%E2%86%92%20C%23-blueviolet?style=flat-square&logo=csharp" alt="Python to C#">
    <img src="https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet" alt=".NET 8">
    <img src="https://img.shields.io/badge/Platform-Windows-blue?style=flat-square&logo=windows" alt="Windows">
    <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="License">
    <img src="https://img.shields.io/badge/Version-V1.5.9-orange?style=flat-square" alt="Version">
  </p>
</div>

## 📖 简介

**NZM 战绩助手** 是一款专为逆战玩家打造的战绩查询与数据分析工具。
本项目是 **基于原 Python 版本进行的完全 C# 重构版**。相比原脚本，C# WPF 版本拥有原生的图形界面、更低的内存占用以及更快的并发处理速度，无需配置 Python 环境，开箱即用。

> **V1.5.0 重大更新**：全新 UI 重构，引入磨砂玻璃特效、图标化侧边栏、双主题切换及配装可视化增强。

## ✨ 核心功能与 V1.5.0 升级

### 🎨 现代化 UI/UX (New!)
* **💎 磨砂玻璃特效**：主界面与详情页采用类 iOS/Win11 的动态磨砂玻璃背景，配合流光动画，视觉体验大幅提升。
* **📱 图标化侧边栏**：全新的窄边导航栏设计（80px），集成悬浮提示（Tooltip），最大化内容展示空间。
* **🌗 双主题切换**：内置 **“碳黑极简”** 与 **“炫彩玻璃”** 两套主题，支持一键热切换。
* **📐 自适应布局**：严格对齐的卡片式设计，数据展示更加整洁直观。

### 📊 深度数据查询
* **官方战绩同步**：展示历史总场次、总时长及 KDA 数据。
* **近期走势分析**：可视化展示 **近 30 天 / 100 场** 的详细胜率、场均伤害趋势。
* **地图/模式偏好**：自动统计常玩地图与模式胜率（僵尸猎场/塔防/机甲），一眼看穿你的“本命图”。

### ⚔️ 对局详情增强 (New!)
* **👤 专属数据栏**：详情页底部新增 **“个人专属数据栏”**，高亮显示本人积分、BOSS伤害及金币，自动去重。
* **🔫 配装可视化**：修复代码显示问题，现在以 **图标 + 品质边框** 直观展示武器及镶嵌插件（配件），一键查看队友配置。

### 🛡️ 系统与安全
* **🔐 多方式登录**：支持 QQ 扫码登录与手动 Cookie 录入（双区支持）。
* **⏳ 会话管理**：24小时自动下线机制，保障账号安全。
* **☁️ 远程更新**：启动自动检测版本与公告（源自 `mobaiya.icu`），支持一键跳转更新。
* **🛡️ 防崩溃设计**：全线引入安全类型转换，杜绝因 API 返回空数据导致的闪退。

## 📸 界面预览

| 登录界面 | 战绩概览 |
|:---:|:---:|
| <img src="./screenshots/login.png" alt="登录" width="400"/> | <img src="./screenshots/战绩概览.png" alt="主页" width="400"/> |

| 详细数据 (新版) | 图鉴系统 |
|:---:|:---:|
| <img src="./screenshots/详细数据.png" alt="详情" width="400"/> | <img src="./screenshots/图鉴系统.png" alt="图鉴" width="400"/> |

## 🚀 快速开始

### 环境要求
* Windows 10 / 11 (64-bit)
* [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (通常 Win10/11 系统已内置或自动安装)

### 安装运行
1.  前往 [Releases](../../releases) 页面下载最新的 `NZM 战绩助手 V1.5.9.zip`。
2.  解压后直接双击 `NZMHelper.exe` 运行即可（绿色免安装）。

### 开发构建
如果你想自己编译代码：
1.  克隆本仓库：
    ```bash
    git clone [https://github.com/MoBai-Curt/NZM-Stats-Tool.git](https://github.com/MoBai-Curt/NZM-Stats-Tool.git)
    ```
2.  使用 **Visual Studio 2022** 打开 `NZMHelper.sln` 解决方案。
3.  确保安装了 **.NET 8 SDK** 和 **WPF 开发工作负载**。
4.  还原 NuGet 包并运行。

## ⚠️ 免责声明

* 本工具仅供编程学习与技术交流使用，**严禁用于任何商业或非法用途**。
* 如果用户将本工具用于非法用途，产生的一切后果由用户自行承担，与作者无关。
* 本工具不包含任何破坏游戏平衡的功能（如外挂、脚本），仅为数据查询工具。
* **本项目完全免费，严禁倒卖！**

## 👤 作者信息

* **作者**: MoBai
* **Blog**: [http://mobaiya.icu/](http://mobaiya.icu/)
* **GitHub**: [@MoBai-Curt](https://github.com/MoBai-Curt)

## 🙏 致谢

本项目灵感与核心逻辑参考了原网页版本：
* **原网页作者**: HaMan412 ([GitHub](https://github.com/HaMan412))
* 本项目在此基础上进行了 C# 本地化重构、UI 全新设计以及功能扩展。

---
如果觉得这个项目不错，请给一个 ⭐️ **Star** 吧！
