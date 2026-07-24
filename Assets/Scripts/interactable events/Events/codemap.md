# Events Subsystem Code Map

## Responsibility
The `Events` folder contains the event triggering components for the interactable events system in Unity. This subsystem is responsible for detecting interaction conditions (raycasts from player camera, trigger volume bounds, lifecycle events), tracking active target events, presenting context UI feedback, intercepting input when the player is holding an inventory item, and dispatching execution calls to associated actions implementing `IAction`.

---

## Architecture & Design Patterns
* **Polymorphism / Inheritance**: `Event` acts as the base `MonoBehaviour` event trigger. Subclasses such as `WaitEvent` and `OnTriggerEvent` customize interaction check behaviors or event responses while reusing core action dispatch functionality (`CallActions()`).
* **Command / Strategy Pattern**: Events reference arrays of `MonoBehaviour` implementations of `IAction`. On trigger, `CallActions()` iterates over the referenced components and invokes `IAction.StartAction()`.
* **Raycast & Target Discovery**: `CameraLookAtEventTrigger` uses periodic sphere/line raycasting from the player camera to discover interactive objects carrying an `Event` component.
* **Contextual Inventory Interception**: `CameraLookAtEventTrigger` inspects `PlayerInventory`. If an item is held, key input (`E`) bypasses raycast event targets to execute a directional item throw.
* **Progressive Hold Interaction**: `WaitEvent` interacts with `CameraLookAtEventTrigger`'s progress bar slider state, executing incremental checks (`IsActionReady()`) until a hold threshold is reached.

---

## Concrete Trigger Flow & Integration

```
[ CameraLookAtEventTrigger ] -- (Physics Raycast) --> [ Event / WaitEvent ]
            │                                                 │
    (Press key 'E')                                    CallActions()
            │                                                 │
  ┌─────────┴────────────────────────┐                        │
  ▼                                  ▼                        v
[Item Held? ThrowItem(dir)]  [lastEvent.Action()] ----> [ IAction (Actions) ]
                                                        .StartAction()
```

1. **Detection**: `CameraLookAtEventTrigger` runs a periodic `checkForAction` coroutine using `Physics.Raycast` along the player camera forward vector against configured `hitLayer` targets within `maxDistance`.
2. **Target Tracking & UI**: When an `Event` collider is hit, `lastEvent` is populated, the action UI elements are activated, and the interaction description text (`Discription`) is shown.
3. **Execution Trigger & Inventory Interception**:
   - **Contextual E Key**: When pressing `E`, `EventAction()` checks `PlayerInventory.HasItem`.
     - **Item Held**: Calls `PlayerInventory.ThrowItem(throwDir)` with force along `playerCamera.forward`, restoring item physics and setting throw cooldown.
     - **Item Not Held**: Key input (`E`) calls `lastEvent.Action()`. In `Event.cs`, `Action()` checks `gameObject.activeInHierarchy` and invokes `CallActions()`.
   - **Hold / Wait Event**: For `WaitEvent`, interaction progress increments `progressBarr.value` via `IsActionReady()`. Upon reaching `maxValue`, actions are fired and progress resets via `StopEventProgress()`.
   - **Trigger Volume Event**: `OnTriggerEvent` evaluates `OnTriggerEnter` / `OnTriggerExit` for player presence (`ExampleCharacterController`) and invokes UnityEvents (`_onEnterEvent`, `_onExitEvent`).
   - **Lifecycle Event**: `Event` can also trigger immediately `OnEnable` (`actionOnStart`) or `OnDisable` (`actionsOnDisable`).

---

## Key Symbols & Files

### `Event.cs`
* **Base Class**: `Event : MonoBehaviour`
* **Fields**:
  * `MonoBehaviour[] actionsToPerform`: Array of components implementing `IAction`.
  * `bool actionOnStart`: Automatically triggers actions when object enables.
  * `bool actionsOnDisable`: Automatically triggers actions when object disables.
  * `string Discription`: UI display prompt text shown when target is focused.
  * `bool limitForHeavy`: Flag for heavy item carrying restrictions.
* **Methods**:
  * `Action()`: Base virtual method calling `CallActions()` if the game object is active in hierarchy.
  * `CallActions()`: Iterates `actionsToPerform` and invokes `StartAction()` on each valid `IAction`.
  * `IsWaitEvent()`: Virtual indicator returning `false` by default.

### `CameraLookAtEventTrigger.cs`
* **Controller Class**: `CameraLookAtEventTrigger : MonoBehaviour`
* **Fields**: `timeBetweenCheck`, `maxDistance`, `hitLayer`, `EventUI`, `progressBarr`, `description`, `playerCamera`, `cachedInventory`, `lastEvent`.
* **Methods**:
  * `checkForAction()`: Coroutine evaluating raycasts continuously.
  * `EventAction()`: Checks `PlayerInventory.HasItem` to trigger `ThrowItem(dir)` or calls `lastEvent.Action()`.
  * `GetInventory()`: Lazy acquisition/caching of `PlayerInventory` instance.
  * `IsActionReady()`: Increments and checks hold progress on `progressBarr`.
  * `StartEventProgress()` / `StopEventProgress()` / `eventProgress()`: Coroutines and helpers for hold-to-interact behaviors.

### `WaitEvent.cs`
* **Subclass**: `WaitEvent : Event`
* **Methods**:
  * `Action()`: Checks `CameraLookAtEventTrigger.IsActionReady()` and triggers actions when interaction hold completes.
  * `IsWaitEvent()`: Returns `true`.

### `OnTriggerEvent.cs`
* **Subclass**: `OnTriggerEvent : Event`
* **Fields**: `UnityEvent _onEnterEvent`, `_onExitEvent`.
* **Methods**:
  * `OnTriggerEnter(Collider other)` / `OnTriggerExit(Collider other)`: Filters by `ExampleCharacterController` component and fires respective `UnityEvent`.

### `OnCollisionEvent.cs`
* Stub/empty script placeholder for collision-driven event triggers.