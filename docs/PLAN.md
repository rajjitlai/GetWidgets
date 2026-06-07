# Vetra Plan

## Product Shape

Vetra is a standalone Windows widget app, not a Rainmeter skin. It should feel like a compact glass control layer for the desktop: useful at a glance, visually quiet, and configurable without becoming a full dashboard.

## V1 Scope

1. Now Playing widget
   - Detect current Windows media session.
   - Show title, artist, source app, and thumbnail.
   - Provide play/pause, previous, and next controls when available.
   - Support resize and drag.
   - Support overlay and desktop placement modes.
   - Desktop mode should sit above desktop icons but below ordinary app windows.
   - Compact controls only.
   - Save position, size, opacity, placement, and click-through immediately when changed.

2. Background shell
   - Start into tray.
   - Restore the widget automatically.
   - Provide tray commands for showing/hiding, settings, and exiting.

3. Visual settings
   - Glass background.
   - Opacity.
   - Rounded corners.
   - Shadow.
   - Click-through toggle.

## V2 Scope

1. System usage widget
   - CPU usage.
   - RAM usage.
   - Disk and network activity.
   - GPU stats through LibreHardwareMonitor or vendor-specific APIs.
   - Tiny live graphs as the primary display style, with compact numeric labels as secondary data.

2. Widget persistence
   - Save layout, size, opacity, and placement mode per widget.
   - Restore all widgets on app launch.

3. Widget manager
   - Add/remove widgets.
   - Configure widgets.
   - Export/import layout.

## Architecture

```text
Vetra
├─ App shell
│  ├─ Tray service
│  ├─ Widget lifecycle
│  └─ Settings persistence
├─ Services
│  ├─ Media session service
│  ├─ System stats service
│  └─ Hardware stats service
├─ Native
│  ├─ Window placement
│  ├─ Desktop attachment
│  └─ Effects
└─ Widgets
   ├─ Now Playing
   ├─ Clock
   └─ System Usage
```

## Known Constraints

- YouTube, Spotify, and browser playback are discoverable only when they expose a Windows media session.
- Browser thumbnails and metadata can vary by browser and site.
- Desktop-attached widgets require Windows shell interop and need careful testing across monitor setups.
- GPU stats are not standardized across all devices; a fallback path is required.
