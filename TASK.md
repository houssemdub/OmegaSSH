# OmegaSSH Development Plan

## Project Overview
OmegaSSH is a modern, premium SSH terminal and session manager built with .NET 8 and WPF. It aims to rival tools like Termius and MobaXterm by offering intelligent session management, a multitasking interface, and an integrated toolchain, wrapped in a high-end "Glassmorphism/Cyberpunk" aesthetic.

## Feature Roadmap

### Phase 1: Foundation & Architecture
- [x] **1.1 Project Initialization**: Create .NET 8 WPF solution, configure DI (Dependency Injection), and Logging.
- [x] **1.2 MVVM Setup**: Implement `CommunityToolkit.Mvvm` for robust state management.
- [x] **1.3 UI/UX Design System**: 
    - Create a modern Theme manager (Dark Mode default).
    - Implement "Glassmorphism" window styles (Acrylic/Mica where supported).
    - Define core control styles (Buttons, Inputs, Scrollbars) to look premium.
- [x] **1.4 Application Shell**: 
    - Build `MainWindow` with a custom chromeless window title bar.
    - Implement the main layout: Sidebar (Sessions), Main Content (Terminal/Tabs), Status Bar.

### Phase 2: Intelligent Session Management
- [x] **2.1 Data Models**: Define `Session`, `Folder`, `Tag`, `Credential` models.
- [x] **2.2 Persistence Layer**: Implement a JSON-based or SQLite local database for storing sessions securely.
- [ ] **2.3 Session Browser**: 
    - Create a Sidebar TreeView with support for nested folders and drag-and-drop.
    - Implement Search/Filter functionality (Fuzzy matching).
- [x] **2.4 Connection Logic**: 
    - Abstract SSH connection logic (using `SSH.NET` or similar).
    - Handle connection states (Connecting, Connected, Disconnected, Error).

### Phase 3: The Terminal Experience (Multitasking)
- [x] **3.1 Terminal Emulator Control**: 
    - Implement or integrate a WPF-compatible Terminal emulator (handling ANSI escape sequences, xterm colors).
- [x] **3.2 Tabs & Panes**: 
    - Implement a robust TabManager (Draggable tabs).
- [x] **3.3 Input Broadcasting**: Logic to send keystrokes to multiple active terminal instances.

### Phase 4: Integrated Toolchain
- [x] **4.1 SFTP Browser**: 
    - dedicated SFTP view alongside the terminal.
    - Drag-and-drop file upload/download core logic.
- [x] **4.2 Port Forwarding Manager**: Visual UI and background logic for managing SSH Tunnels.
- [x] **4.3 Local Terminal**: Integration of PowerShell as local session type.

### Phase 5: Security & Credentials
- [x] **5.1 Credential Vault**: 
    - Secure storage using AES-256.
- [x] **5.2 SSH Key Manager**: 
    - Generator for RSA-2048 keys with vault encryption.

### Phase 6: Polish & Advanced Features
- [x] **6.1 Scripting/Snippets**: Library for saved snippets and quick execution.
- [x] **6.2 Theming**: User-configurable themes (Cyberpunk, Retro, Nord).
- [x] **6.3 Visual Polish**: Added transitions and hover animations.

### Phase 7: Stability & Advanced Integration
- [x] **7.1 Splash Screen**: Cyberpunk initialization screen with dependency checks.
- [x] **7.2 Settings Engine**: Persistence of user preferences (Theme, Window Size) via JSON.
- [x] **7.3 Global Error Handling**: Custom-styled message boxes and crash logging.

## Current Progress
1. Hierarchical Session Browser.
2. ANSI Terminal with RichText rendering.
3. Tabbed Multitasking & Broadcast.
4. Integrated SFTP & Key Manager.
5. Command Snippets & Theme Engine.
6. AES Vault & JSON Settings.
7. Resilience: Global Error Handling & Splash Screen.

## Next Steps
1. Performance: Optimize rendering for high-output streams.
2. Advanced Scripting: Python/JS scripting for automated tasks.
3. Cloud Sync: Encrypted sync for sessions/snippets.
