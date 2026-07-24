# Inventory Subsystem Codemap (`Assets/Scripts/Inventory`)

## Overview & Core Responsibility
The `Inventory` subsystem manages a **single-slot player inventory state machine**, item pickup triggers via the `IAction` interface, camera-synchronized kinematic holding, deterministic throwing, and ownership/lifecycle cleanup.

---

## State Machine & Component Model

### `PlayerInventory.cs` (Singleton State Machine)
- **`InventoryState` Enum**:
  - `Empty`: No item held. Ready to pick up items.
  - `PickingUp`: Item becomes kinematic and follows a smoothstep interpolation toward `holdPoint` over `pickupDuration` (default `0.25s`) without snapping.
  - `Holding`: `LateUpdate()` synchronizes the kinematic item with the camera-relative hold point. `PlayerInventory` uses `DefaultExecutionOrder(1000)` so this runs after the KCC camera's `LateUpdate`, preventing one-frame camera-pose lag. Only bounded local X/Y bob and turn-sway offsets are applied; no world obstruction or forward/back correction changes the hold distance.
  - **Procedural motion**: Subtle frame-rate-independent walking bob and camera-turn sway are applied as bounded local offsets while `Holding`; motion state resets on pickup/release and settles to the exact hold pose while idle.
  - `Throwing`: Transient state that restores colliders and dynamic `Rigidbody` properties before assigning a mass-independent launch velocity. The launch combines cached KCC motor velocity, camera-forward `throwSpeed` (`15 m/s`), and `throwUpwardModifier` (`1.5 m/s`), then starts the repickup cooldown and returns to `Empty`.
  - **Swap flow**: If `E` targets a different valid `ActionPickupItem` while holding an item, `TrySwap()` validates the target, throws the current item through the normal restoration/launch path, and immediately starts the new item's smooth pickup. Non-pickup targets retain contextual throw behavior.
- **Ownership & Lifecycle Cleanup**:
  - `NotifyItemDisabledOrDestroyed(PickupItem)`: Restores physics state and clears inventory reference if the held item is disabled or destroyed.
  - `NotifyItemConsumed(PickupItem)`: Clears held reference without restoring physics and destroys the target `GameObject`.
  - `ConsumeOrReleaseItem()`: Releases item from inventory, restores original `Rigidbody` properties, and returns the `PickupItem` instance (e.g. for meteorite feeding).

### `PickupItem.cs` (Item Entity Component)
- **Role**: Required on pickable GameObjects alongside a `Rigidbody`.
- **State & Cooldown**: Tracks `owningInventory` reference and `lastDropOrThrowTime`. Enforces pickability check (`CanBePickedUp()`) requiring no current owner and expiration of `cooldownTime = 0.5f`.
- **Remaining Meteorite Integration Point**: Provides `NotifyConsumed()`. When called, invokes `owningInventory.NotifyItemConsumed(this)` if held, or directly `Destroy(gameObject)` if unheld. This serves as the integration hook for meteorite entity consumption (e.g. `MeteoritMouthTrigger`).

### `ActionPickupItem.cs` (Event-System Action Bridge)
- **Role**: Concrete `IAction` leaf component.
- **Invocation**: When triggered by an `Event` via `StartAction()`, acquires `PlayerInventory.Instance` and executes `cachedInventory.Pickup(targetItem)`.

---

## Data & Mechanics Flow

```
[ CameraLookAtEventTrigger ] ──('E' Input)──> [ Event.Action() ]
                                                    │
                                                    ▼
                                      [ ActionPickupItem.StartAction() ]
                                                    │
                                                    ▼
                                      [ PlayerInventory.Pickup(item) ]
                                                    │
                                                    ├─ Saved original Rigidbody properties
                                                    ├─ Configures continuous interpolation / gravity OFF
                                                    └─ Sets state to PickingUp
                                                    │
                                                    ▼
                                        [ LateUpdate Kinematic Follow ]
                                                    │
                                                    ▼
                                          [ InventoryState.Holding ]
```

When an item is held (`HasItem == true`), pressing `E` on `CameraLookAtEventTrigger` contextually intercepts key presses to call `PlayerInventory.ThrowItem(throwDir)` along camera forward, bypassing standard raycast target actions.

---

## Scene Integration & Test Wiring (`Test.unity`)
- `Player/ExampleCamera`: Hosts `CameraLookAtEventTrigger` with `playerCamera` reference, scanning interactive hit layers.
- `InventoryHoldPoint`: Attached to camera view hierarchy as target transform for `PlayerInventory.holdPoint`.
- `InventoryTestPickup`: GameObjects configured with `Rigidbody`, `PickupItem`, `Event`, and `ActionPickupItem` to enable pick-and-throw sandbox testing.
