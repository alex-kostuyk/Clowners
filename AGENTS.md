# Project Overview & Contributor Guide (`AGENTS.md`)

## Repository Map

For architectural understanding and navigation, contributors and AI agents MUST consult the root and directory-specific codemaps:

- **[Root Repository Atlas](codemap.md)**: Main architecture, engine setup, entry points, complete directory map table, runtime flow, dependency notes, and current build state.
- **[Assets Codemap](Assets/codemap.md)**: Breakdown of first-party assets versus third-party/plugin folders.
- **[Scripts Codemap](Assets/Scripts/codemap.md)**: First-party C# gameplay architecture, inventory state machine, events, console controls, and meteorite entities.
  - **[Inventory Codemap](Assets/Scripts/Inventory/codemap.md)**: Single-slot state machine, smooth pickup, camera-synchronized hold, contextual throw, and lifecycle cleanup.
  - **[Interactable Events Codemap](Assets/Scripts/interactable%20events/codemap.md)**: Interaction framework overview.
    - **[Events Codemap](Assets/Scripts/interactable%20events/Events/codemap.md)**: Raycast target discovery, triggers, contextual `E` throw interception, and hold interaction components.
    - **[Actions Codemap](Assets/Scripts/interactable%20events/Actions/codemap.md)**: `IAction` interface and concrete leaf actions.
  - **[Control Console Codemap](Assets/Scripts/Control%20Console/codemap.md)**: Console state machine, screen feedback, and button shaders.
  - **[Meteorite Entity Codemap](Assets/Scripts/Meteorite%20entity/codemap.md)**: Meteorite singleton data, food tags, request types, and `NotifyConsumed` integration.
- **[Scenes Codemap](Assets/Scenes/codemap.md)**: Scene inventory, status, test scene wiring (`Player/ExampleCamera`, `InventoryHoldPoint`, `InventoryTestPickup`), and baked lightmaps.
- **[Packages Codemap](Packages/codemap.md)**: Unity Package Manager dependencies and embedded Hot Reload package.
- **[ProjectSettings Codemap](ProjectSettings/codemap.md)**: Project configuration YAML settings and pipeline setup.

---

## Technical Stack & Engine Environment

- **Unity Version**: Unity 2022.3.62f3.
- **Render Pipeline**: Built-in Render Pipeline (Forward Rendering).
- **Core Packages**: Unity Input System (`com.unity.inputsystem` v1.14.2), TextMeshPro (`com.unity.textmeshpro`), uGUI (`com.unity.ugui`), Post Processing (`com.unity.postprocessing`), Unity MCP (`com.coplaydev.unity-mcp`).
- **Locomotion & Plugins**: KinematicCharacterController (`Assets/KinematicCharacterController`), DOTween (`Assets/Plugins/Demigiant`), Hot Reload (`Packages/com.singularitygroup.hotreload`).

---

## First-Party Code Boundaries & Conventions

- **Code Boundaries**: First-party gameplay C# code resides strictly under `Assets/Scripts/`.
- **Scenes**: Available scenes in repo are `Assets/Scenes/Main menu.unity` and `Assets/Scenes/Test.unity`. Note that `ProjectSettings/EditorBuildSettings.asset` currently has no scenes registered (`m_Scenes: []`).
- **Architecture & Conventions**:
  - **Inspector Composition**: MonoBehaviours are authored to be attached to GameObjects and configured in the Unity Inspector via serialized fields (`[SerializeField]`).
  - **Event-Action & Inventory Architecture**: Single-slot inventory (`PlayerInventory`) state machine (`Empty`, `PickingUp`, `Holding`, `Throwing`). Interaction triggers derive from `Event` and dispatch execution to modular action components implementing `IAction` (`StartAction()`), including `ActionPickupItem`.
  - **Contextual Input Interception**: `CameraLookAtEventTrigger` inspects `PlayerInventory`. When an item is held, pressing `E` intercepts interaction to throw the held item along camera forward.
  - **Singletons & Dynamic Shaders**: `PlayerInventory`, `ControlConsoles`, and `MeteoriteEntity` use singleton patterns (`Instance`). `ConsoleButton` uses `MaterialPropertyBlock` for performant runtime color/emission updates.

---

## Runtime & Interaction Flow

```
                                  [ Player View / Physical Trigger ]
                                                  │
                                                  ▼
                                     [ CameraLookAtEventTrigger / OnTriggerEvent ]
                                                  │
                         ┌────────────────────────┴───────────────────────┐
                         │ (Press 'E' while holding item)                  │ (Standard interaction)
                         ▼                                                 ▼
             [ PlayerInventory.ThrowItem ]                         [ Event.Action() ]
                         │                                                 │
                         ▼                                                 ▼
            Restores physics & applies force                       [ Event.CallActions() ]
                                                                           │
               ┌─────────────────────────────────┬─────────────────────────┴─────────────────────────┐
               ▼                                 ▼                                                   ▼
   [ ActionPickupItem ]             [ Generic Gameplay Actions ]                         [ Control Console Actions ]
   (Pickup item into spring hold)   (IAction.StartAction())                              (ConsoleButton.PressButton())
```

---

## Safe Editing & Modification Guidance

1. **First-Party Code Focus**: Modify files within `Assets/Scripts/`. Do not edit third-party plugins (`KinematicCharacterController`, `Demigiant`, `SC Post Effects`, `VolumetricLightBeam`, `TextMesh Pro`) unless explicitly instructed.
2. **Ignored / Generated Paths**: Do not edit generated project files (`*.csproj`, `*.sln`) or Unity system directories (`Library/`, `Temp/`, `Logs/`, `obj/`, `Build/`, `Builds/`).
3. **Meta Files**: Ensure every new or renamed asset/script in `Assets/` retains its accompanying `.meta` file to preserve Unity GUID references.

---

## Verification & Unity Workflow

When making modifications in this repository:

1. **Unity MCP Tool Integration**:
   - Use the Unity MCP plugin (`com.coplaydev.unity-mcp`) when connected to trigger domain refresh, inspect compilation logs, check console output, or execute EditMode/PlayMode unit tests.
2. **Compilation & Console Checks**:
   - Verify that all C# script changes compile without errors or critical warnings.
3. **Scene & Asset State**:
   - Check scene dirty state and verify Inspector references after editing serialized MonoBehaviour fields.
