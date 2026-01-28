# ⚡ OmegaSSH v1.1.0-Stable

**OmegaSSH** is a high-performance, premium SSH terminal and toolchain built for the modern era. Designed with a clean, professional aesthetic, it combines the power of raw SSH/SFTP with a seamless, resilient user experience.

---

## 🚀 Key Features

### 🛠️ Integrated Toolchain
- **Multi-Tab Terminal**: Manage multiple sessions simultaneously with high-speed rendering.
- **Integrated SFTP Browser**: Built-in file management with background upload/download capabilities.
- **Comprehensive Session Manager**: Organize connections into folders, duplicate existing profiles, and search instantly.
- **Key Manager**: Generate, export, and manage SSH keys (RSA/Ed25519) in a secure environment.

### 🎨 User Interface
- **Responsive Layout**: Resizable sidebar with GridSplitter and adaptive window scaling.
- **Professional Themes**: Switch between **Default (Dark)**, **Retro (Classic)**, and **Nord (Frost)** themes.
- **Glassmorphism UI**: Modern, translucent interfaces with smooth transitions and subtle animations.

### ⚙️ Full Settings Menu
- **Terminal Customization**: Change font families (Consolas, Cascadia, etc.) and font sizes.
- **Global Preferences**: Configure auto-connection, keep-alive intervals, and custom session logging paths.
- **ANSI Engine**: Full support for 256 ANSI colors with toggle capabilities.

### 🛡️ Resilience & Performance
- **Smart Data Buffering**: High-speed terminal rendering with batch-processing to prevent UI freezes.
- **Detailed Logging**: Boot sequence logs and timestamped session recording for audit trails.
- **Global Error Handling**: Robust "Black Box" exception management to prevent application crashes.

---

## 🛠️ Technical Stack
- **Platform**: .NET 8.0 WPF
- **Architecture**: MVVM (CommunityToolkit.Mvvm)
- **Protocols**: SSH.NET (SSH, SFTP, Port Forwarding)
- **Styling**: Vanilla XAML + DynamicResource Theme Engine

---

## 📂 Project Structure
- `OmegaSSH/`: Main application source.
- `OmegaSSH/Infrastructure/`: ANSI Parsers, Binding Proxies, and UI Helpers.
- `OmegaSSH/Services/`: SSH, Session Management, Settings, and Logging logic.
- `OmegaSSH/ViewModels/`: Pure logic for terminals, SFTP, and application state.

---

## 🔧 Getting Started
1. Clone the repository: `git clone https://github.com/houssemdub/OmegaSSH.git`
2. Open the solution in **Visual Studio 2022**.
3. Restore NuGet packages and Build/Run.

---

## 📜 Version History
- **v1.1.0-Stable**: Added resizable sidebar, complete Session Manager, full Settings Menu, and finalized UI design.
- **v1.0.0-Beta**: Initial Release - Architecture Foundation, SSH core, and Theme Engine.

*Developed with ❤️ by Antigravity*
