# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Blackout Reflection Probe

> Quick overview: Creates a child Reflection Probe that always returns a black cubemap inside its bounds, effectively killing environment reflections in that volume. Scale this GameObject to set the probe’s box size; optionally hide the probe in the hierarchy.

A tiny, drop‑in component to disable reflections for a region of space. It spawns a child `ReflectionProbe`, switches it to Custom mode, and assigns a generated all‑black cubemap. Useful for indoor rooms, staging areas, or volumes where you don’t want skybox or scene reflections to appear.

![screenshot](Documentation/Screenshot.png)

## Features
- Automatic probe management
  - Spawns a child `ReflectionProbe` named "Blackout Reflection Probe"
  - Custom mode with a generated black `Cubemap` (RGBA32), one texture for all faces
  - Refresh mode: OnAwake; blend distance fixed at 1.0
- Volume sized by transform
  - Probe size follows this GameObject’s local scale (centered, zero rotation/offset)
- Editor‑friendly
  - `ExecuteAlways`: active in Edit and Play
  - Optional "Hide Reflection Probe" to keep the child out of the hierarchy
  - Rebuilds the probe when scale changes; cleans up children on disable
  - Triggers `DynamicGI.UpdateEnvironment()` and marks scenes dirty in Edit Mode after changes
- SRP‑agnostic
  - Works with Built‑in, URP, and HDRP (standard `ReflectionProbe` API)

## Requirements
- Unity 6000.0+ (Runtime + Editor)
- Any render pipeline (Built‑in, URP, HDRP)

## Usage
1) Add `BlackoutReflectionProbe` to an empty GameObject
2) Position the GameObject at the center of the area where reflections should be disabled
3) Set the local Scale to the desired probe box size (X,Y,Z)
4) Optionally toggle "Hide Reflection Probe" to remove the child from the hierarchy view
5) For multiple rooms/areas, add one component per volume

Tip: Keep volumes reasonably tight to the area you want to affect for crisp transitions.

## Notes and Limitations
- Hard black reflections
  - Inside the box, reflective materials sample a black environment. If you need a dim or colored environment, fork and replace the generated cubemap
- Blend distance is fixed
  - The script sets a blend distance of 1.0. For softer boundaries, adjust the constant in code
- Edit‑time housekeeping
  - Changing scale destroys and recreates children; this is expected. The component also updates GI and marks scenes dirty in Edit Mode
- Cubemap size
  - The generated cubemap defaults to 16×16 per face. Increase the size in code only if you have a specific need; most cases don’t require higher resolution
- Hierarchy visibility
  - The child probe’s `hideFlags` are toggled by the component; disable hiding if you want to inspect settings directly

## Files in This Package
- `Runtime/BlackoutReflectionProbe.cs` – Component that manages the black reflection probe
- `Runtime/UnityEssentials.BlackoutReflectionProbe.asmdef` – Runtime assembly definition
- `Editor/UnityEssentials.BlackoutReflectionProbe.Editor.asmdef` – Editor assembly definition
- `package.json` – Package manifest metadata

## Tags
unity, reflection, reflection-probe, cubemap, black, indoor, environment, rendering, builtin, urp, hdrp
