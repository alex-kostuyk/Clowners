# Assets/Scripts Codemap

## Responsibility
`Assets/Scripts` contains the first-party game logic and runtime subsystems for **clown jam** (`Clowners`). Its primary job is to drive player interactions, single-slot inventory mechanics, console UI/cooldown management, event-driven gameplay flow, and meteorite entity mechanics.

---

## Architecture & Subsystems

The codebase is organized into four principal first-party domain modules and child codemaps:

1. **Inventory Subsystem** (`/Inventory`)
   - (`Inventory/codemap.md`): Single-slot inventory state machine (`PlayerInventory`: `Empty`, `PickingUp`, `Holding`, `Throwing`), `IAction` pickup handler (`ActionPickupItem`), item entity wrapper (`PickupItem`), physics-guided spring holding, contextual `E` key throw logic, ownership cleanup, and `NotifyConsumed()` meteorite integration hook.

2. **Interactable Events System** (`/interactable events`)
   - **`Events/`** (`interactable events/Events/codemap.md`): Trigger components (`CameraLookAtEventTrigger`, `Event`, `WaitEvent`, `OnTriggerEvent`) that detect player aim raycasts, physical volumes, or lifecycle triggers and execute `CallActions()`. Intercepts `E` input to trigger inventory throw when holding an item.
   - **`Actions/`** (`interactable events/Actions/codemap.md`): Decoupled atomic execution handlers implementing `IAction` (`StartAction()`) for sound, player movement/teleportation, animations, GameObject toggles, item pickups (`ActionPickupItem`), delays, scene loading, and cascading event execution.

3. **Control Console Subsystem** (`/Control Console`)
   - (`Control Console/codemap.md`): Manages console button interaction, Fisher-Yates button position shuffling, global console cooldown state lifecycle (`ControlConsoles`), screen display UI/slider/binary Matrix effect stream (`ConsoleScreen`), and dynamic button coloring using `MaterialPropertyBlock` (`ConsoleButton`).

4. **Meteorite Entity Subsystem** (`/Meteorite entity`)
   - (`Meteorite entity/codemap.md`): Singleton data holder (`MeteoriteEntity`) exposing `RequestProperties` mapping `RequestType` enums to `RequestColor` visual indicators, along with food tags (`MeteoriteFoodTag`), mouth triggers (`MeteoriteMouthTrigger`), and request actions (`ActionSendMeteoriteRequest`). Integrates with `PickupItem.NotifyConsumed()`.

---

## Control & Event Data Flow

```
                                  [ Player View / Physical Trigger ]
                                                  │
                                                  ▼
                                     [ CameraLookAtEventTrigger / OnTriggerEvent ]
                                                  │
                         ┌────────────────────────┴────────────────────────┐
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
                                    - Audio / Particles / Animations                                 │
                                    - Teleportation / Camera Shake                                   ▼
                                    - GameObject / Script Toggles                       [ ControlConsoles Cooldown Routine ]
                                                                                                     │
                                                                                     ┌───────────────┴───────────────┐
                                                                                     ▼                               ▼
                                                                           [ ConsoleScreen UI ]          [ ConsoleButton Materials ]
                                                                           - Recharging slider           - MaterialPropertyBlock dimming
                                                                           - TextMeshPro countdown       - Meteorite RequestType color
```

---

## Integration & Child Codemaps

- **[Inventory Codemap](Inventory/codemap.md)**: Single-slot inventory state machine, spring-guided hold physics, pickup action integration, and item lifecycle.
- **[interactable events Codemap](interactable%20events/codemap.md)**: Main architecture of event detection and action execution bridge.
  - **[Events Codemap](interactable%20events/Events/codemap.md)**: Triggers, raycasting, contextual throw interception, and hold-to-interact behavior.
  - **[Actions Codemap](interactable%20events/Actions/codemap.md)**: `IAction` interface definition and concrete leaf action inventory.
- **[Control Console Codemap](Control%20Console/codemap.md)**: Console state machine, screen updates, button shuffling, and `MaterialPropertyBlock` styling.
- **[Meteorite Entity Codemap](Meteorite%20entity/codemap.md)**: Central meteorite data singleton, request type definitions, mouth triggers, and `NotifyConsumed` hook.