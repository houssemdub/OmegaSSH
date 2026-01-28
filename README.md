# ⚡ OmegaSSH v1.2.0-Alpha

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

### 🎨 User Interface (Corporate Cyberpunk)
- **Fluid Layout**: Resizable sidebar with GridSplitter and adaptive window scaling using WindowChrome.
- **Dynamic Theme Engine**: Select from **Dracula**, **Nord**, **Monokai**, **CyberNeon**, and **Retro** using dedicated "Theme Orbs".
- **Glassmorphism UI**: Premium Translucent interfaces with smooth gradients, glow effects, and subtle animations.
- **Liquid Scrollbars**: Integrated, ultra-thin scrollbars that expand and highlight on interaction.

### ⚙️ Full Settings Menu
- **Terminal Customization**: Change font families (Consolas, Cascadia, etc.) and font sizes.
- **Global Preferences**: Configure auto-connection, keep-alive intervals, and custom session logging paths.
- **ANSI Engine**: Full support for 256 ANSI colors with batch-processing performance.

---

## 🛠️ Technical Stack
- **Platform**: .NET 8.0 WPF
- **Architecture**: MVVM (CommunityToolkit.Mvvm)
- **Protocols**: SSH.NET (SSH, SFTP, Port Forwarding)
- **Rendering**: Custom ANSI-to-WPF Parser with high-performance buffering.

---

## 🔧 Getting Started
1. Clone the repository: `git clone https://github.com/houssemdub/OmegaSSH.git`
2. Open the solution in **Visual Studio 2022**.
3. Restore NuGet packages and Build/Run.

---

## 📜 Version History
- **v1.2.0-Alpha**: "The Orchestration Update" - Local Shell support, Multi-Commander, Broadcast Mode, and a complete UI overhaul with Corporate Cyberpunk aesthetics.
- **v1.1.0-Stable**: Added resizable sidebar, complete Session Manager, and full Settings Menu.
- **v1.0.0-Beta**: Initial Release - Architecture Foundation, SSH core, and Theme Engine.

*Developed with ❤️ by Antigravity*
