# Sirrine VR Menu System

Reusable Unity uGUI pieces for world-space VR menus:

- `MenuPageShell` reserves the same header, content, and footer regions on every page. Its title slot stays centered when a Back button is present.
- `MenuLayoutMetrics` centralizes canvas-space padding, spacing, target-size, and region measurements.
- `MenuButtonTheme` applies the same hover, pressed, selected, disabled, and normal-state behavior across app-defined button roles.
- `MenuLayoutValidator` checks region bounds, header/content/footer collisions, Back/title separation, button containment and minimum target sizing, plus visible button and text-box overlaps.

The package intentionally uses Unity's uGUI and has no runtime dependency on Meta sample assemblies. Meta's UI Set is itself built on uGUI Canvas and RectTransform and offers themed buttons, sliders, toggles, dropdowns, and dialogs. Skybound uses the same foundation and page/layout patterns while retaining its existing 3D controller-ray button path. A project with a compatible Meta Interaction SDK input setup can use UI Set controls inside this page shell; check for competing interaction providers before combining them with another pointer system.

## Use in another app

The package is maintained independently at [Sirrine-Programming/vr-menu-system](https://github.com/Sirrine-Programming/vr-menu-system). Add the tagged package to `Packages/manifest.json`:

```json
"com.sirrineprogramming.vr-menu-system": "https://github.com/Sirrine-Programming/vr-menu-system.git#v0.1.0"
```

Pin a release tag or full commit hash for reproducible builds. The consuming project still owns its app-specific navigation, labels, input wiring, and visual brand colors.

## Validation

Call `MenuLayoutValidator.Validate(shell)` from an EditMode test or run **VR Menu > Validate Active Scene Layouts** in the Unity Editor. Mark intentional layered controls with `MenuLayoutIgnore`; keep those exceptions narrow and documented.

The validator uses the current `RectTransform` geometry after `Canvas.ForceUpdateCanvases()`. Text checks detect text boxes that escape a page region or overlap another visible text box; they do not measure glyph clipping inside a text box. Add app-specific preferred-size checks for long dynamic labels and verify the result on device.
