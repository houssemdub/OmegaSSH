# ⚡ OmegaSSH v1.0-Beta

**OmegaSSH** is a high-performance, premium SSH terminal and toolchain built for the modern era. Designed with a sleek **Cyberpunk aesthetic**, it combines the power of raw SSH/SFTP with a seamless, resilient user experience.

---

## 🚀 Key Features

### 🛠️ Integrated Toolchain
- **Multi-Tab Terminal**: Manage multiple sessions simultaneously with broadcast capabilities.
- **Neural SFTP Browser**: Built-in file management with rapid up/down capabilities.
- **Key Manager**: Generate, export, and manage SSH keys (RSA/Ed25519) within a secure vault.
- **AES Vault**: Secure encrypted storage for all your credentials and snippets.

### 🎨 Premium Aesthetics
- **Theme Engine**: Switch between **Cyberpunk (Neon)**, **Retro**, and **Nord** themes on the fly.
- **Glassmorphism UI**: Modern, translucent interfaces with smooth transitions.
- **Micro-Animations**: Tactile hover effects and slide-fade view switching.

### 🛡️ Resilience & Performance
- **Neural Buffering**: High-speed terminal rendering with batch-processing to prevent UI freezes.
- **Stability Engine**: Global exception handling, boot logging, and persistent session recovery.
- **Smart Virtualization**: Efficient line-limiting to manage large terminal histories without RAM overhead.

---

## 🛠️ Technical Stack
- **Platform**: .NET 8.0 WPF
- **Architecture**: MVVM (CommunityToolkit.Mvvm)
- **Protocols**: SSH.NET (SSH, SFTP, Port Forwarding)
- **Styling**: Vanilla XAML + DynamicResource Theme Engine

---

## 📂 Project Structure
- `OmegaSSH/`: Main application source.
- `OmegaSSH/Infrastructure/`: ANSI Parsers, Crypto Helpers, and Converters.
- `OmegaSSH/Services/`: SSH, Vault, Settings, and Theme logic.
- `OmegaSSH/ViewModels/`: Logic for sessions, terminal tabs, and SFTP.

---

## 🔧 Getting Started
1. Clone the repository:
   ```bash
   git clone https://github.com/houssemdub/OmegaSSH.git
   ```
2. Open the solution in **Visual Studio 2022**.
3. Restore NuGet packages and Build/Run.

---

## 📜 Version History
- **v1.0.0-Beta**: Initial Release - Stability, Themes, SFTP, and Architecture Foundation.

*Developed with ❤️ by Antigravity*
