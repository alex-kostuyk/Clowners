# Assets/Scenes Codemap

## Overview
This directory contains the Unity scenes and scene-related lighting/baked assets for **clown jam** (`Clowners`).

## Scene Inventory & Status

| Scene File | Path | Status / Build Inclusion | Purpose / Role |
|---|---|---|---|
| `Main menu.unity` | `Assets/Scenes/Main menu.unity` | ❌ Not in `EditorBuildSettings.asset` | Main Menu UI screen / entry point interface for game startup. |
| `Test.unity` | `Assets/Scenes/Test.unity` | ❌ Not in `EditorBuildSettings.asset` | Test environment / sandbox scene for gameplay mechanics, character tests, console controls, and inventory pickup/throw testing (`Player/ExampleCamera`, `InventoryHoldPoint`, `InventoryTestPickup`). |

## Scene Test Wiring (`Test.unity`)
- **`Player/ExampleCamera`**: Holds `CameraLookAtEventTrigger` with camera transform reference scanning hit layers and handling contextual `E` key throws.
- **`InventoryHoldPoint`**: Position offset transform under camera view hierarchy used by `PlayerInventory` for critically damped spring lerping.
- **`InventoryTestPickup`**: Test pickable GameObjects configured with `Rigidbody`, `PickupItem`, `Event`, and `ActionPickupItem` components.

## Associated Scene Assets

- `Assets/Scenes/Test/`
  - `LightingData.asset` & `.exr`/`.png` lightmaps (`Lightmap-0` through `Lightmap-3`, `ReflectionProbe-0`): Baked lighting data generated for the `Test` scene.
- `Assets/Scenes/moons.lighting`: Custom lighting setting profile asset.
- `Assets/Scenes/Test_Profiles/GameObject Profile.asset`: Profile config asset stored alongside test scene data.

## Practical Integration Notes

1. **Build Settings Configuration Required**:
   - Currently, `ProjectSettings/EditorBuildSettings.asset` contains an empty `m_Scenes: []` list.
   - When building a stand-alone release or running in-editor multi-scene flows, add `Main menu.unity` (index 0) and `Test.unity` (index 1) to `EditorBuildSettings`.

2. **Render Pipeline Alignment**:
   - Scenes rely on Unity's **Built-in Render Pipeline** (Forward rendering path). Highlighting/shaders use standard built-in properties or custom shaders.

3. **Input & UI**:
   - New Input System package (`com.unity.inputsystem` v1.14.2) is active, alongside standard legacy InputManager mapping. UI elements in `Main menu` utilize TMPro / uGUI (`com.unity.textmeshpro`, `com.unity.ugui`).