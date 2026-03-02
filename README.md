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
    <img src="https://img.shields.io/badge/Platform-Windows_x64-blue?style=flat-square&logo=windows" alt="Windows">
    <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="License">
    <img src="https://img.shields.io/badge/Version-V1.6.0-orange?style=flat-square" alt="Version">
  </p>
</div>

## 📖 简介

**NZM 战绩助手** 是一款专为逆战玩家打造的战绩查询与数据分析工具。
本项目是 **基于原 Python 版本进行的完全 C# 重构版**。相比原脚本，C# WPF 版本拥有原生的图形界面、更低的内存占用以及更快的并发处理速度，且已打包为纯净单文件，无需配置 Python 环境，开箱即用。

> **🎉 V1.6.0 重大更新**：因官方封堵扫码登录，本版本迎来底层逻辑重构！新增**原生无感代理抓包引擎**，一键自动拦截小程序凭证，并加入**智能区服识别**，彻底告别登录烦恼！

## ✨ 核心功能

### 🛡️ 全新免抓包登录系统 (V1.6.0 核心)
* **⚡ 原生无感抓包**：内置原生代理嗅探器，点击按钮即可一键拦截微信小程序流量并提取 Cookie，完美绕过官方网页登录限制。
* **🧠 智能区服识别**：无论是自动抓取还是手动粘贴，系统均能根据凭证特征自动精准区分 **QQ 区** 与 **微信区**，极速登入。
* **🔒 安全会话管理**：登录凭证本地加密存储，24小时自动下线机制，保障账号绝对安全。

### 🎨 现代化 UI/UX
* **💎 磨砂玻璃特效**：主界面与详情页采用类 iOS/Win11 的动态磨砂玻璃背景，视觉体验大幅提升。
* **🌗 双主题切换**：内置 **“碳黑极简”** 与 **“炫彩玻璃”** 等多套主题，支持一键无缝热切换。

### 📊 深度数据查询
* **官方战绩同步**：展示历史总场次、总时长及近期 KDA 数据。
* **近期走势分析**：可视化展示近期场次的详细胜率、场均得分与伤害趋势。
* **地图/模式偏好**：自动统计常玩地图（僵尸猎场/塔防/时空追猎）的各难度通关率，一眼看穿你的“本命图”。

### ⚔️ 对局与图鉴增强
* **👤 专属数据栏**：详情页高亮显示本人积分、BOSS伤害、金币及 KDA，自动去重队友数据。
* **🔫 配装可视化**：以 **图标 + 品质边框** 直观展示单局所有玩家的武器及镶嵌插件配置。
* **🗃️ 碎片悬浮监控**：支持图鉴收集进度查询，并提供可置顶的碎片实时监控悬浮窗。

## 📸 界面预览

| 极简登录 (支持自动抓包) | 战绩概览 |
|:---:|:---:|
| <img src="./screenshots/login.png" alt="登录" width="400"/> | <img src="./screenshots/战绩概览.png" alt="主页" width="400"/> |

| 详细数据 (专属数据栏) | 图鉴系统 |
|:---:|:---:|
| <img src="./screenshots/详细数据.png" alt="详情" width="400"/> | <img src="./screenshots/图鉴系统.png" alt="图鉴" width="400"/> |

## 🚀 快速开始

### 运行环境
* Windows 10 / 11 (64-bit)
* 软件已采用单文件独立压缩打包，通常**无需手动安装任何环境**即可运行。

### 使用说明 (必读)
因 V1.6.0 引入了底层网络代理抓包功能，使用前请仔细阅读以下步骤：

1. 前往 [Releases](../../releases/latest) 页面下载最新的 `NZMHelper_v1.6.0.exe`。
2. ⚠️ **【非常重要】** 请右键点击软件，选择 **“以管理员身份运行”** (否则拦截器将因权限不足而启动失败)。
3. 在登录界面选择你要查询的区服（QQ区 / 微信区）。
4. 点击大按钮 **“开启自动抓取 (推荐)”**。
5. 在电脑上打开微信，进入 **“逆战未来工具箱”** 小程序。
6. 点击小程序内的 **“战绩”** 或任意数据页面，软件将瞬间捕获凭证并自动进入数据大盘！

> **排错指南 (FAQ)**：
> * **抓不到数据/一直转圈**：请检查您的电脑是否开启了 **VPN、梯子、游戏加速器 (如 UU、Clash 的 TUN 全局模式)**。代理软件会劫持本地流量导致工具失效，请**暂时关闭它们**后再试。
> * 如果自动抓取因特殊网络环境始终失败，您依然可以使用老方法：通过 Fiddler/Charles 抓包后，将 Cookie 手动粘贴至下方输入框进行登录。

## 🛠️ 开发与编译

如果你想自己编译代码：
1. 克隆本仓库：`git clone https://github.com/MoBai-Curt/NZM-Stats-Tool.git`
2. 使用 **Visual Studio 2022** 打开 `NZMHelper.sln`。
3. 确保安装了 **.NET 8 SDK** 和 **WPF 开发工作负载**。
4. 核心依赖：`Titanium.Web.Proxy` (原生 MITM 抓包) & `Newtonsoft.Json`。
5. 还原 NuGet 包并运行。

## ⚠️ 免责声明

* 本软件为纯本地客户端工具，所有数据均直接请求官方公开 API，**绝不上传、收集或存储用户的任何个人隐私及账号密码**。
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
* 本项目在此基础上进行了 C# 本地化重构、UI 全新设计、底层网络库更换以及功能全面扩展。

---
💡 如果觉得这个项目不错，请给一个 ⭐️ **Star** 鼓励一下作者吧！
