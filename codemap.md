# Repository Atlas & Architecture (`Clowners`)

## Responsibility & Overview
`Clowners` (**clown jam**) is a Unity game project built on **Unity 2022.3.62f3** using the **Built-in Render Pipeline** (Forward rendering path). The project implements an interactive space/facility environment featuring a single-slot inventory state machine, event-driven object interaction, control console UI state management with dynamic cooldowns, character locomotion via KinematicCharacterController, and meteorite entity request mechanics.

---

## Directory Map & Sub-Codemaps

| Directory | Role / Primary Content | Sub-Codemap Link |
| :--- | :--- | :--- |
| `Assets/` | All runtime assets: scripts, scenes, models, textures, audio, plugins | [`Assets/codemap.md`](Assets/codemap.md) |
| `Assets/Scripts/` | First-party C# gameplay scripts & architectural core | [`Assets/Scripts/codemap.md`](Assets/Scripts/codemap.md) |
| `Assets/Scripts/Inventory/` | Single-slot inventory state machine, pickup action, camera-synchronized hold, and item lifecycle | [`Assets/Scripts/Inventory/codemap.md`](Assets/Scripts/Inventory/codemap.md) |
| `Assets/Scripts/interactable events/` | Interaction event triggers and action execution bridge | [`Assets/Scripts/interactable events/codemap.md`](Assets/Scripts/interactable%20events/codemap.md) |
| `Assets/Scripts/interactable events/Events/` | Interaction triggers (camera raycast, triggers, hold events) and contextual throw interception | [`Assets/Scripts/interactable events/Events/codemap.md`](Assets/Scripts/interactable%20events/Events/codemap.md) |
| `Assets/Scripts/interactable events/Actions/` | Modular leaf actions implementing `IAction` interface | [`Assets/Scripts/interactable events/Actions/codemap.md`](Assets/Scripts/interactable%20events/Actions/codemap.md) |
| `Assets/Scripts/Control Console/` | Console button shuffling, cooldown timer, and screen display UI | [`Assets/Scripts/Control Console/codemap.md`](Assets/Scripts/Control%20Console/codemap.md) |
| `Assets/Scripts/Meteorite entity/` | Meteorite entity singleton, food tags, request color mappings | [`Assets/Scripts/Meteorite entity/codemap.md`](Assets/Scripts/Meteorite%20entity/codemap.md) |
| `Assets/Scenes/` | Scene files (`Main menu.unity`, `Test.unity`) and baked lightmaps | [`Assets/Scenes/codemap.md`](Assets/Scenes/codemap.md) |
| `Packages/` | Unity Package Manager dependencies, embedded Hot Reload tool | [`Packages/codemap.md`](Packages/codemap.md) |
| `ProjectSettings/` | Unity project configuration files (ProjectVersion, BuildSettings, etc.) | [`ProjectSettings/codemap.md`](ProjectSettings/codemap.md) |

---

## Key Entry Points & Key Systems

1. **Inventory Subsystem**:
   - `PlayerInventory.cs`: Singleton state machine (`Empty`, `PickingUp`, `Holding`, `Throwing`) handling smooth pickup, kinematic `LateUpdate` hold synchronization with bounded procedural motion, throw impulse calculation, and ownership lifecycle cleanup (`NotifyItemDisabledOrDestroyed`, `NotifyItemConsumed`).
   - `PickupItem.cs`: Pickable item component with cooldown tracking (`CanBePickedUp()`) and consumption notification hook (`NotifyConsumed()`).
   - `ActionPickupItem.cs`: `IAction` implementation bridging event triggers to `PlayerInventory.Pickup()`.
2. **Player Interaction & Contextual Controls**:
   - `CameraLookAtEventTrigger.cs`: Drives player view raycasts. When holding an item, pressing `E` intercepts key input to call `PlayerInventory.ThrowItem()` along camera forward; when unheld, fires targeted `Event.Action()`.
3. **Event & Action Execution Framework**:
   - `Event.cs` / `WaitEvent.cs` / `OnTriggerEvent.cs`: Base triggers calling `CallActions()`.
   - `AbstractAction.cs` (`IAction`): Leaf actions performing audio playback, teleportation, scene loading, animator toggles, item pickups, and delayed cascading actions.
4. **Console Management Subsystem**:
   - `ControlConsoles.cs`: Singleton handling console activity lifecycle and cooldown countdowns.
   - `ConsoleScreen.cs`: UI updating countdown timer, recharging slider, and binary matrix stream.
   - `ConsoleButton.cs`: Button state handling using `MaterialPropertyBlock` for efficient visual dimming and request color matching.
5. **Meteorite Entity Data Subsystem**:
   - `MeteoriteEntity.cs`: Singleton holding `RequestProperties` mapping `RequestType` enums to `RequestColor` visual indicators. Integrates with `PickupItem.NotifyConsumed()`.

---

## Runtime & Control Flow Summary

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

## Integration & Dependency Notes

- **Engine Version**: Unity 2022.3.62f3 (built-in forward rendering pipeline).
- **Package Stack**: Input System (`com.unity.inputsystem` v1.14.2), TextMeshPro (`com.unity.textmeshpro` v3.0.9), uGUI (`com.unity.ugui` v1.0.0), Post Processing (`com.unity.postprocessing` v3.4.0), Unity MCP (`com.coplaydev.unity-mcp`).
- **Embedded Packages**: Singularity Group Hot Reload (`Packages/com.singularitygroup.hotreload`) for runtime C# code patching.
- **Third-Party Assets**: KinematicCharacterController (`Assets/KinematicCharacterController`), SC Post Effects (`Assets/Packs/SC Post Effects`), Volumetric Light Beam (`Assets/Packs/VolumetricLightBeam`), DOTween (`Assets/Plugins/Demigiant`).

---

## Known Project State

- **Build Settings Scene Registration**: `ProjectSettings/EditorBuildSettings.asset` currently has **no scenes registered** (`m_Scenes: []`). Scenes `Assets/Scenes/Main menu.unity` and `Assets/Scenes/Test.unity` exist in the repository but must be added to build settings prior to standalone builds.
