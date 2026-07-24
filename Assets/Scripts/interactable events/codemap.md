# Interactable Events Subsystem Code Map

## Architecture Overview
The `interactable events` system is an event-driven interaction framework in Unity structured into two main layers: **Events** (triggers/sources) and **Actions** (behaviors/responses). It also integrates with the **Inventory** subsystem to route contextual `E` key interaction vs. item throwing.

```
   +-------------------------------------------------------------+
   |                       EVENT TRIGGERS                        |
   |                                                             |
   |  [ CameraLookAtEventTrigger ]   [ OnTriggerEvent ]   ...    |
   |             |                          |                    |
   |             v                          v                    |
   |       [ WaitEvent ]  ------->    [ Event ] (Base)           |
   +--------------------------------------|----------------------+
                                          |
                                          | CallActions()
                                          v
   +-------------------------------------------------------------+
   |                      ACTION HANDLERS                        |
   |                                                             |
   |                      [ IAction ] (Interface)                |
   |                                |                            |
   |     +--------------------------+--------------------+       |
   |     |                          |                    |       |
   | [ActionPlaySound]      [ActionPickupItem]   [TpPlayerToPosition]
   |     |                          |                    |       |
   |    ...                        ...                  ...      |
   +-------------------------------------------------------------+
```

1. **Events Layer (`/Events`)**: Defines interaction sources and trigger detection logic (player aim raycast, trigger zones, lifecycle events).
2. **Actions Layer (`/Actions`)**: Defines decoupled atomic execution logic adhering to the `IAction` interface, including `ActionPickupItem`.
3. **Dispatch Bridge**: `Event.cs` holds direct references to action components via `MonoBehaviour[] actionsToPerform`. When an event fires, `CallActions()` dynamically dispatches `StartAction()` calls to each configured component implementing `IAction`.
4. **Inventory Interception**: `CameraLookAtEventTrigger` inspects `PlayerInventory`. If an item is currently held, pressing `E` calls `PlayerInventory.ThrowItem()` directly instead of triggering target events.

---

## Folder Structure & Subsystems

* **`/Events`**: Event triggers and player interaction management.
  * `Event.cs`: Base event class handling action execution iteration.
  * `CameraLookAtEventTrigger.cs`: Raycast target scanner, user key input driver (`E`), and inventory throw interceptor.
  * `WaitEvent.cs`: Progressive hold-to-interact event subclass.
  * `OnTriggerEvent.cs`: Physics trigger volume enter/exit event driver.
  * `OnCollisionEvent.cs`: Physical collision event component.
  * `codemap.md`: Dedicated architectural documentation for the Events folder.

* **`/Actions`**: Concrete action implementations performing game responses.
  * `AbstractAction.cs`: Core interface definition (`IAction` with `StartAction()`).
  * *Inventory*: `ActionPickupItem.cs` (triggers `PlayerInventory.Pickup()`).
  * *Audio*: `ActionPlaySound.cs`, `ActionPlayRandomeSound.cs`, `ActionStopAudioSource.cs`.
  * *Transform & Movement*: `TpPlayerToPosition.cs`, `ActionSmoothPLayerTp.cs`, `ActionLerpToPosition.cs`, `ActionMakeChildOffTransform.cs`, `ActionExplosionPush.cs`, `ActionForceStandUp.cs`.
  * *Visual & Animation*: `ActionPlayAnimation.cs`, `ActionSetAnimatorTrigger.cs`, `ActionSetAnimatorState.cs`, `ActionSwapMaterials.cs`, `ActionPlayParticleSystem.cs`, `ActionShakeCamera.cs`.
  * *Lifecycle & Scene*: `ActionSetGameobjectActivity.cs`, `ActionSetMonobiheaviorActivity.cs`, `ActionInstantiateGameObject.cs`, `ActionDestroyGameObjects.cs`, `ActionLoadScene.cs`, `ActionFP_ControllerActivity.cs`.
  * *Control & Meta-Actions*: `ActionRunEvents.cs`, `ActionStartActionWithDelay.cs`, `ActionCallEventWithInterval.cs`, `ActionCallEventBasedOnCameraDistance.cs`, `ActionRunUnityEvents.cs`, `ActionRunButtonOnClick.cs`, `ActionDebugLog.cs`.
  * `codemap.md`: Dedicated documentation for the Actions folder.

---

## High-Level Dispatch Flow

1. **Detection & Raycasting**: `CameraLookAtEventTrigger` scans player view raycasts for objects containing `Event` components.
2. **Input / Inventory Interception**:
   - Player presses interaction key (`E`).
   - **If holding item (`PlayerInventory.HasItem == true`)**: Invokes `PlayerInventory.ThrowItem(throwDir)` along player camera forward vector.
   - **If no item held**: Calls `CameraLookAtEventTrigger.EventAction()`, which invokes `lastEvent.Action()`.
3. **Dispatch**: The triggered `Event` invokes `CallActions()`.
4. **Execution**: Each `MonoBehaviour` in `actionsToPerform` cast to `IAction` executes its custom `StartAction()` behavior (e.g. `ActionPickupItem` picking up item, teleporting player, playing sound, toggling GameObjects).