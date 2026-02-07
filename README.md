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
    <a href="#下载">下载最新版</a>
    ·
    <a href="https://github.com/MoBai-Curt/NZM-Stats-Tool/issues">报告 Bug</a>
  </p>

  <p align="center">
    <img src="https://img.shields.io/badge/Refactored-Python%20%E2%86%92%20C%23-blueviolet?style=flat-square&logo=csharp" alt="Python to C#">
    <img src="https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet" alt=".NET 8">
    <img src="https://img.shields.io/badge/Platform-Windows-blue?style=flat-square&logo=windows" alt="Windows">
    <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="License">
  </p>
</div>

## 📖 简介

**NZM 战绩助手** 是一款专为逆战玩家打造的战绩查询与数据分析工具。
本项目是 **基于原 Python 版本进行的完全 C# 重构版**。相比原脚本，C# WPF 版本拥有原生的图形界面、更低的内存占用以及更快的并发处理速度，无需配置 Python 环境，开箱即用。

## ✨ 核心功能与技术升级

* **⚡ 技术栈迁移 (Python → C#)**：
    * 告别繁琐的 Python 环境配置，单文件 exe 直接运行。
    * 利用 .NET 8 的高性能 `HttpClient` 和 `Task` 并发，数据加载速度飞跃提升。
    * 更加稳定、流畅的原生 WPF 交互体验。
* **🔐 双区支持**：完美支持 QQ (扫码登录) 与 微信 (Cookie 登录) 双大区。
* **📊 深度统计**：
    * 展示历史总场次、时长。
    * **近 30 天 / 100 场** 的详细胜率、场均伤害分析。
* **🔍 对局细节重制**：
    * 重新设计的 UI 布局，直观展示队友数据。
    * **一键查看大佬配装**（支持查看具体插件名称与图标）。


## 🙏 致谢

本项目灵感与核心逻辑参考了原 Python 版本：
* **原 Python 版作者**: HaMan412 ([GitHub](https://github.com/HaMan412))
* 本项目在此基础上进行了 C# 本地化重构与 UI 升级。
