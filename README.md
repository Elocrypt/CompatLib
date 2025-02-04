# CompatLib

**CompatLib** is a comprehensive compatibility management library for **Vintage Story** modding. It offers tools to detect mod dependencies, handle compatibility conflicts, and streamline integration between mods, ensuring a harmonious modding experience. 

## Features

- ✅ **Mod Detection**: Verify the presence of specific mods and retrieve their versions.
- 🔄 **Compatibility Management**: Register, process, and manage compatibility handlers for mods.
- 📊 **Conflict Tracking**: Dynamically detect and log compatibility conflicts.
- 🔧 **Server & Client Commands**: In-game commands for debugging compatibility issues.
- 🔒 **Thread Safety**: Utilizes thread-safe operations to ensure data integrity in multithreaded environments.
- 🛠️ **Dynamic Handler Control**: Enable or disable specific handlers at runtime for flexible compatibility management.
- 🕒 **Version Compatibility Checks**: Ensure handlers are compatible with specific mod versions to prevent conflicts.
- 📣 **User Feedback Mechanism**: Inform users about active handlers and detected conflicts, enhancing transparency.
- 📄 **JSON Export for Compatibility Logs**: Export compatibility logs in a structured JSON format for easier debugging and analysis.
- 📈 **Logging Granularity**: Introduced different log levels (INFO, WARNING, ERROR) to categorize messages, allowing for more granular control over logging output.

## Installation

1. **Download**: Obtain the latest **CompatLib** release from the [Releases](https://github.com/Elocrypt/CompatLib/releases) page.
2. **Placement**: Place the `.dll` file in your **Vintage Story Mods** directory.
3. **Reference**: Ensure your mod's `.csproj` references **CompatLib** for compatibility handling.

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
CompatibilityManager.RegisterHandler(
    targetmodid: "targetmodid",
    priority: 100,
    handler: () => {
        // Custom compatibility logic
        api.Logger.Notification("Applying compatibility fix for targetmodid.");
    },
    description: "Handler for targetmodid",
    mutuallyExclusive: false,
    dependencies: new List<string> { "dependencyModId" },
    compatibleVersion: "1.0.0"
);
```

- **`priority`**: Determines the execution order of handlers; higher values indicate higher priority.
- **`description`**: A brief description of the handler.
- **`mutuallyExclusive`**: Indicates if the handler should not run alongside others for the same mod.
- **`dependencies`**: A list of mod IDs that this handler depends on.
- **`compatibleVersion`**: Specifies the mod version this handler is compatible with.

---

### 3️⃣ Processing Compatibility Handlers

CompatLib automatically processes registered handlers when mods are detected on the server. To manually process handlers:

```csharp
CompatibilityManager.ProcessHandlers("targetmodid", api);
```

---

### 4️⃣ Enabling or Disabling Handlers at Runtime

To dynamically enable or disable a specific handler:

```csharp
CompatibilityManager.SetHandlerEnabled("targetmodid", "Handler for targetmodid", isEnabled: false);
```

---

### 5️⃣ Checking Compatibility Logs (In-Game)

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

### 6️⃣ Version Compatibility Ranges

Demonstrate how to register a handler with a version range:

```csharp
CompatibilityManager.RegisterHandler(
    targetmodid: "targetmodid",
    priority: 100,
    handler: () => {
        // Custom compatibility logic
        api.Logger.Notification("Applying compatibility fix for targetmodid.");
    },
    description: "Handler for targetmodid",
    mutuallyExclusive: false,
    dependencies: new List<string> { "dependencyModId" },
    compatibleVersionRange: "[1.0.0,2.0.0)" // Compatible with versions 1.0.0 (inclusive) to 2.0.0 (exclusive)
);
```

---

### 7️⃣ Exporting Compatibility Logs as JSON

To export the compatibility logs in JSON format:

```csharp
string jsonLogs = AnalyticsManager.ExportLogsAsJson();
// You can then save jsonLogs to a file or process it as needed.
```

---

### 8️⃣ Logging Granularity 

Explain how to log messages with different severity levels:

```csharp
// Log an informational message
AnalyticsManager.LogInfo("This is an informational message.");

// Log a warning message
AnalyticsManager.LogWarning("This is a warning message.");

// Log an error message
AnalyticsManager.LogError("This is an error message.");
```

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
| `RegisterHandler(modid, priority, handler, description, mutuallyExclusive, dependencies, compatibleVersion)` | Registers a compatibility handler with detailed parameters. |
| `ProcessHandlers(modid, api)` | Executes the appropriate handlers for a mod. |
| `GetRegistrations()` | Returns all registered handlers. |
| `SetHandlerEnabled(modid, description, isEnabled)` | Enables or disables a specific handler at runtime. |

### **AnalyticsManager**
| Method | Description |
|--------|------------|
| `LogConflict(message)` | Logs compatibility conflicts. |
| `LogInfo(message)` | Logs an informational message. |
| `LogWarning(message)` | Logs a warning message. |
| `LogError(message)` | Logs an error message. |
| `ConflictCount` | Retrieves the number of logged conflicts. |
| `GetConflictLogs()` | Returns a thread-safe snapshot of conflict logs. |
| `ExportLogsAsJson()` | Exports the conflict logs as a JSON-formatted string. |

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
