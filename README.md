# Unity Variables Package - cz.xprees.variables

[![NPM Version](https://img.shields.io/npm/v/cz.xprees.variables)](https://www.npmjs.com/package/cz.xprees.variables)

ScriptableObject-based variable architecture for decoupled state management, event observation, and multi-scene data sharing in Unity.

## Features

- **ScriptableObject Variables** - Store and share state without coupling systems together.
    - Extend [`VariableBaseSO<T>`](Runtime/Base/VariableBaseSO.cs) to author custom typed variables.
    - Observe runtime changes via the `onValueChanged` event.
- **Standalone Default State** - Every variable manages its own authored `defaultValue` and deep-clones it into `currentValue` on `OnEnable()` and
  `ResetToDefault()`.
- **PlayMode Transition Resets** - Declarative `[ResetOnPlayMode]` attribute controls automatic reset when entering or exiting Play Mode.
- **State Lifetime Control** - Inherits `DescriptionBaseSO.lifetime` (`Scenario`, `Session`, `Persistent`) for boundary-aware resets.
- **Persistent Variables** - `PersistentVariableBaseSO<T>` backs a variable with `PlayerPrefs` so its value survives across sessions.
- **References** - Inspector-switchable references (`ReferenceBase<T>`) supporting both inlined values and shared variable assets.
- **Variable Modifiers & Aggregations** - Compose and transform variable streams dynamically.

## Installation

Install the package using npm scoped registry in `Project Settings > Package Manager > Scoped Registries`:

```json
{
    "name": "NPM - xprees",
    "url": "https://registry.npmjs.org",
    "scopes": [
        "cz.xprees",
        "com.dbrizov.naughtyattributes"
    ]
}
```

Then install `cz.xprees.variables` via the Unity Package Manager.

---

## State Lifecycle & PlayMode Reset (DX)

Variables are completely self-contained and integrate with the centralized `StateSnapshotService` without requiring bespoke registration systems.

### 1. Default Behavior

By default, `VariableBaseSO<T>`:

1. Clones `defaultValue` into `currentValue` on `OnEnable()`.
2. Automatically resets to default on entering Play Mode via `[ResetOnPlayMode(PlayModeResetTiming.EnterPlayMode)]`.
3. In Unity Editor, captures its baseline snapshot before the first runtime mutation via `StateSnapshotService.EnsureCaptured(this)`.
4. Reverts cleanly to baseline upon exiting Play Mode without dirtying asset files on disk.

### 2. Customizing PlayMode Resets via `[ResetOnPlayMode]`

Use `[ResetOnPlayMode]` to customize when custom variable classes reset:

```csharp
using Xprees.Core;
using Xprees.Variables.Base;

// 1. Default: resets on Play Mode entry
public class AmmoVariable : IntVariable { }

// 2. Prevent Play Mode reset (e.g. for persistent editor values or external saves)
[ResetOnPlayMode(PlayModeResetTiming.None)]
public class PlayerProfileVariable : StringVariable { }

// 3. Reset on both entry and exit
[ResetOnPlayMode(PlayModeResetTiming.Both)]
public class TransientDebugFlagVariable : BoolVariable { }
```

### 3. Setting State Lifetime per Variable

Every variable ScriptableObject exposes the **Lifetime** dropdown in the Inspector (inherited from `DescriptionBaseSO`):

- **`Scenario` (Default)**:
  Resets automatically whenever a scenario starts or restarts via `ScenarioStateRegistrySO` and `StateSnapshotService.RestoreAll()`.
- **`Session`**:
  Persists across scenarios, resetting only when returning to the Main Menu or quitting.
- **`Persistent`**:
  **Never reset** by scenario resets or Play Mode entry (e.g. Audio volume, resolution settings).

```csharp
// You can also enforce class-level lifetime via attribute:
[StatefulLifetime(StateLifetime.Persistent)]
public class MasterVolumeVariable : FloatVariable { }
```

---

## Persistent Variables (PlayerPrefs)

[`PersistentVariableBaseSO<T>`](Runtime/Base/PersistentVariableBaseSO.cs) stores its value in `PlayerPrefs` instead of keeping it purely in memory. It
is declared `[StatefulLifetime(StateLifetime.Persistent)]` and `[ResetOnPlayMode(PlayModeResetTiming.None)]`, so it is skipped by both the scenario
snapshot restore and the Play Mode reset, and loads from `PlayerPrefs` on `OnEnable()` instead of cloning `defaultValue`.

Ready-made asset types live under `Assets > Create > Variables/Persistent`:

| Type                       | Backing store                 |
|----------------------------|-------------------------------|
| `PersistentBoolVariable`   | `PlayerPrefs` int (`0` / `1`) |
| `PersistentIntVariable`    | `PlayerPrefs` int             |
| `PersistentFloatVariable`  | `PlayerPrefs` float           |
| `PersistentStringVariable` | `PlayerPrefs` string          |

Key points:

- **`playerPrefsKey` is required.** There is no implicit key - an empty key logs an error and the value is not stored.
- **Write-through.** Every `CurrentValue` set writes to `PlayerPrefs` before `onValueChanged` fires. Unity flushes to disk on quit and focus loss;
  call `Flush()` when an immediate write is required.
- **`defaultValue` is the fallback** used when the key is not present yet. `ResetToDefault()` also overwrites the stored value, while
  `ClearStoredValue()` deletes the key.
- **Other types** can extend [`PersistentJsonVariableSO<T>`](Runtime/Base/PersistentJsonVariableSO.cs), which stores `JsonUtility` JSON in a string
  key. `JsonUtility` only round-trips `[Serializable]` classes and structs - top level primitives, enums and arrays need their own
  `PersistentVariableBaseSO<T>` subclass.
- The inherited **Lifetime** dropdown is inert on these assets, because the class-level `[StatefulLifetime]` attribute takes precedence.
