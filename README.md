Here is a `README.md` file for your **CompatLib** project. You can add it to your GitHub repository to provide clear documentation for users and contributors. 

```markdown
# CompatLib

**CompatLib** is a compatibility management library for **Vintage Story** modding. It provides tools to detect mod dependencies, handle compatibility conflicts, and streamline integration between mods. 

## Features

- ✅ **Mod Detection**: Check if specific mods are installed and retrieve their versions.
- 🔄 **Compatibility Management**: Register and process compatibility handlers for mods.
- 📊 **Conflict Tracking**: Detect and log compatibility conflicts dynamically.
- 🔧 **Server & Client Commands**: In-game commands for debugging compatibility issues.

## Installation

1. Download the latest **CompatLib** release from the [Releases](https://github.com/Elocrypt/CompatLib/releases) page.
2. Place the `.dll` file in your **Vintage Story Mods** directory.
3. Ensure your mod's `.csproj` references **CompatLib** for compatibility handling.

## Usage

### 1️⃣ Checking if a Mod is Loaded

Use `ModChecker` to verify if a mod is installed:

```csharp
bool isLoaded = ModChecker.IsModLoaded(api, "targetmodid");
```

Retrieve a mod’s version:

```csharp
string version = ModChecker.GetModVersion(api, "targetmodid");
```

---

### 2️⃣ Registering a Compatibility Handler

To register a compatibility handler for a specific mod:

```csharp
CompatibilityManager.RegisterHandler("targetmodid", priority: 100, () => {
    // Custom compatibility logic
    api.Logger.Notification("Applying compatibility fix for targetmodid.");
});
```

- **`priority`**: Determines which handler takes precedence if multiple are registered.

---

### 3️⃣ Processing Compatibility Handlers

CompatLib automatically processes registered handlers when mods are detected on the server:

```csharp
CompatibilityManager.ProcessHandlers("targetmodid", api);
```

---

### 4️⃣ Checking Compatibility Logs (In-Game)

#### **Client Command**
```text
/compatlog
```
- Displays conflict logs in the chat.

#### **Server Command**
```text
/compatlog
```
- Shows logs in the server console.

---

## API Reference

### **ModChecker**
| Method | Description |
|--------|------------|
| `IsModLoaded(api, modid)` | Checks if a mod is installed. |
| `GetModVersion(api, modid)` | Retrieves the version of a mod. |

### **CompatibilityManager**
| Method | Description |
|--------|------------|
| `RegisterHandler(modid, priority, handler, description)` | Registers a compatibility handler. |
| `ProcessHandlers(modid, api)` | Executes the highest-priority handler for a mod. |
| `GetRegistrations()` | Returns all registered handlers. |

### **AnalyticsManager**
| Method | Description |
|--------|------------|
| `LogConflict(message)` | Logs compatibility conflicts. |
| `ConflictCount` | Retrieves the number of logged conflicts. |
| `ConflictLogs` | Returns a list of conflict logs. |

---

## Contributing

Contributions are welcome! To get started:

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature-name`).
3. Commit your changes (`git commit -m "Add feature"`).
4. Push to the branch (`git push origin feature-name`).
5. Open a Pull Request.

---

## License

This project is licensed under the **MIT License**. See [LICENSE](LICENSE) for details.

---

## Credits

Developed by **Elocrypt** (elo).

---
```