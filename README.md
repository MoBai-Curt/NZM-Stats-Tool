<div align="center">
  <img src="./screenshots/logo.png" alt="Logo" width="120" height="120">
  <h1 align="center">NZM 战绩助手 (NZM Helper)</h1>

  <p align="center">
    一款基于 .NET 8 + WPF 构建的现代化逆战战绩查询工具。
    <br />
    <a href="http://mobaiya.icu/"><strong>作者博客</strong></a>
    <br />
    <br />
    <a href="#下载">下载最新版</a>
    ·
    <a href="https://github.com/MoBai-Curt/NZM-Stats-Tool/issues">报告 Bug</a>
    ·
    <a href="https://github.com/MoBai-Curt/NZM-Stats-Tool/pulls">发起请求</a>
  </p>

  <p align="center">
    <img src="https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet" alt=".NET 8">
    <img src="https://img.shields.io/badge/Platform-Windows-blue?style=flat-square&logo=windows" alt="Windows">
    <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="License">
    <img src="https://img.shields.io/badge/Version-1.3.0-orange?style=flat-square" alt="Version">
  </p>
</div>

## 📖 简介

**NZM 战绩助手** 是一款专为逆战玩家打造的战绩查询与数据分析工具。告别繁琐的网页查询，我们提供了一个极速、美观且功能强大的桌面客户端。

支持 **QQ / 微信** 双区登录，采用最新的并发技术，瞬间加载您的历史战绩、图鉴进度及详细对局数据。

## ✨ 核心功能

* **⚡ 极速加载**：采用异步并发请求，数据加载速度提升 500%。
* **🔐 双区支持**：完美支持 QQ (扫码登录) 与 微信 (Cookie 登录) 双大区。
* **📊 深度统计**：
    * 展示历史总场次、时长。
    * **近 30 天 / 100 场** 的详细胜率、场均伤害分析。
    * **模式细分**：僵尸猎场、塔防、机甲战等模式独立统计。
* **🗺️ 地图详情**：精确到每张地图的胜率，以及**普通/英雄/炼狱/折磨**各难度的通关统计。
* **🔫 全面图鉴**：武器、插件、塔防陷阱图鉴一键查看，缺什么一目了然。
* **🔍 对局细节**：
    * 查看单局队友的详细数据（伤害、击杀、金币）。
    * **一键查看大佬配装**（武器+插件搭配）。
    * 展示个人详细战斗数据（Boss伤害、小怪伤害等）。

## 📸 界面预览

| 登录界面 | 战绩概览 |
|:---:|:---:|
| <img src="./screenshots/login.png" alt="登录" width="400"/> | <img src="./screenshots/战绩概览.png" alt="主页" width="400"/> |
| 详细数据 | 图鉴系统 |
|:---:|:---:|
| <img src="./screenshots/详细数据.png" alt="详情" width="400"/> | <img src="./screenshots/图鉴系统.png" alt="图鉴" width="400"/> |

## 🚀 快速开始

### 安装运行
1.  前往 [Releases](../../releases) 页面下载最新的 `NZM 战绩助手 V1.3.exe`。
2.  **无需安装**，直接双击运行即可（基于 .NET 8 独立发布）。

### 开发构建
如果你想自己编译代码：
1.  克隆本仓库：
    ```sh
    git clone [https://github.com/MoBai-Curt/NZM-Stats-Tool.git](https://github.com/MoBai-Curt/NZM-Stats-Tool.git)
    ```
2.  使用 **Visual Studio 2022** 打开解决方案。
3.  确保安装了 **.NET 8 SDK**。
4.  选择 `Release` 模式进行生成。

## ⚠️ 免责声明

* 本工具仅供学习与交流使用，**严禁用于任何非法用途**。
* 如果用户将本工具用于非法用途，产生的一切后果由用户自行承担，与作者无关。
* 本工具不包含任何破坏游戏平衡的功能（如外挂、脚本），仅为数据查询工具。
* **倒卖死全家！** 本项目完全免费。

## 👤 作者信息

* **作者**: MoBai
* **Blog**: [http://mobaiya.icu/](http://mobaiya.icu/)
* **GitHub**: [@MoBai-Curt](https://github.com/MoBai-Curt)

## 🙏 致谢

感谢原作者提供的灵感与基础：
* **原作者**: HaMan412 ([GitHub](https://github.com/HaMan412))

---
如果觉得这个项目不错，请给一个 ⭐️ **Star** 吧！
