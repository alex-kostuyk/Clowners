# Assets Codemap

## Overview & Responsibility
`Assets/` contains all project runtime resources, including first-party C# scripts, single-slot inventory state mechanics, scene definitions, custom shaders, visual/audio assets, and integrated third-party/plugin packages.

---

## First-Party vs Third-Party Asset Boundaries

| Category | Path / Folder | Description & Responsibilities |
| :--- | :--- | :--- |
| **First-Party Code** | `Assets/Scripts/` | First-party game logic including single-slot inventory, interactable events, control consoles, and meteorite entities. Linked in [`Assets/Scripts/codemap.md`](Scripts/codemap.md). |
| **First-Party Scenes** | `Assets/Scenes/` | Game startup and testing environments (`Main menu.unity`, `Test.unity`) and baked lightmap assets. Linked in [`Assets/Scenes/codemap.md`](Scenes/codemap.md). |
| **First-Party Visual & Audio** | `Assets/3d models/`<br>`Assets/Animations/`<br>`Assets/Audio/`<br>`Assets/Fonts/`<br>`Assets/Prefabs/`<br>`Assets/Sprites/` | 3D FBX models (`main facility.fbx`), animation clips, audio clips (`Foot steps`), custom fonts (`Doto`, `GeistPixel`), prefab templates (`wall lamp`), and UI sprites (`progress bar`). |
| **Third-Party & Plugins** | `Assets/KinematicCharacterController/`<br>`Assets/Packs/`<br>`Assets/Plugins/`<br>`Assets/Resources/`<br>`Assets/TextMesh Pro/` | Player movement engine (KinematicCharacterController), plugin packs (`SC Post Effects`, `VolumetricLightBeam`), DOTween plugin DLLs (`Plugins/Demigiant`), DOTween/VLB configuration settings (`Resources/`), and TextMesh Pro shaders, fonts, and assets. |

---

## Detailed Directory Codemaps

- **[Scripts Codemap](Scripts/codemap.md)**: Detailed breakdown of first-party C# scripts, inventory state machine, interactable events, console controls, and meteorite entities.
- **[Scenes Codemap](Scenes/codemap.md)**: Scene inventory, test scene wiring (`Player/ExampleCamera`, `InventoryHoldPoint`, `InventoryTestPickup`), baked lighting data, status, and build inclusion guidance.