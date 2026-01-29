# ⚡ OmegaSSH v1.3.0-Alpha

**OmegaSSH** is a high-performance, premium SSH terminal and toolchain built for the modern era. Designed with a "Corporate Cyberpunk" aesthetic, it combines the power of raw SSH/SFTP with a seamless, resilient user experience and advanced command orchestration.

---

## 🚀 Key Features

### 🛠️ Integrated Toolchain
- **Multi-Tab Terminal**: Manage multiple sessions simultaneously with high-speed rendering.
- **Hybrid Shell Support**: Native integration for local terminals (PowerShell, CMD) alongside SSH.
- **Integrated SFTP Browser**: Built-in file management with background upload/download capabilities.
- **Multi-Commander Orchestrator**: Execute commands across multiple sessions in parallel with aggregated output.
- **Broadcast Mode**: Mirror real-time input to all active terminal tabs.
- **Key Manager**: Generate, export, and manage SSH keys (RSA/Ed25519) in a secure environment.

### 🧠 Smart Interface & Productivity
- **Workspace Persistence**: Remembers your preferred sidebar, snippets panel, and status bar visibility across sessions.
- **Smart Shortcuts**:
    - `Ctrl + C`: Context-aware - Copies selected text or sends an **Interrupt Signal** to stop running processes (like `ping`).
    - `Ctrl + V`: High-speed Paste directly into the terminal stream.
    - `Ctrl + L`: Instant terminal clear.
- **Improved ANSI Engine**: Advanced support for interactive Prompts (Alpine Linux, busybox) and escape sequences.
- **Zen Mode**: Instant distraction-free workspace that intelligently restores your previous layout when disabled.

### 🎨 User Interface (Corporate Cyberpunk)
- **Fluid Layout**: Resizable sidebar and adaptive window scaling using WindowChrome.
- **Instant Theme Engine**: Select from **OmegaSSH**, **MobaXtrem**, **GitHub**, and **WinXP**. Themes now apply instantly across all windows, transforming colors, fonts (e.g., Tahoma for XP), and corner rounding.
- **Glassmorphism UI**: Premium translucent interfaces with smooth gradients, glow effects, and subtle animations.
- **Liquid Scrollbars**: Integrated, ultra-thin scrollbars that expand and highlight on interaction.
- **Always Ready**: The application now starts **Maximized** by default for an immediate full-screen workspace.

---

## 🏛️ Heritage Themes
- **💾 WinXP**: A perfect recreation of the Luna Blue interface with Tahoma fonts and sharp corners.
- **💻 MobaXtrem**: Dark professional admin theme with high-contrast electric blue highlights.
- **🏢 GitHub**: Clean, developer-focused aesthetic based on GitHub's dark design system.
- **⚡ OmegaSSH**: The signature futuristic cyberpunk experience with deep rounding and neon glows.

## 🛠️ Technical Stack
- **Platform**: .NET 8.0 WPF
- **Architecture**: MVVM (CommunityToolkit.Mvvm)
- **Protocols**: SSH.NET (SSH, SFTP, Port Forwarding)
- **Rendering**: Enhanced ANSI-to-WPF Parser with device-status filtering and screen clearing logic.

---

## 🔧 Getting Started
1. Clone the repository: `git clone https://github.com/houssemdub/OmegaSSH.git`
2. Open the solution in **Visual Studio 2022**.
3. Restore NuGet packages and Build/Run.

---

## 📜 Version History
- **v1.3.0-Alpha**: "The Smart Overhaul" - Smart layout persistence, context-aware shortcuts (Ctrl+C/V/L), improved Alpine Linux prompt compatibility, and instant theme switching.
- **v1.2.0-Alpha**: "The Orchestration Update" - Local Shell support, Multi-Commander, Broadcast Mode, and a complete UI overhaul with Corporate Cyberpunk aesthetics.
- **v1.1.0-Stable**: Added resizable sidebar, complete Session Manager, and full Settings Menu.
- **v1.0.0-Beta**: Initial Release - Architecture Foundation, SSH core, and Theme Engine.

*Developed with ❤️ by Antigravity*
