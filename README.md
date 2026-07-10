# Recycle Bin Taskbar
<p align="center">
<img width="440" height="267" alt="Capture d&#39;écran 2026-07-10 234528" src="https://github.com/user-attachments/assets/0f84faac-10a2-4dcf-ba2b-6c05c6ca2e8b" />
</p>

Pin the Recycle Bin to your Windows taskbar with a right-click jump list for
**Empty Recycle Bin** and **Open Recycle Bin** — with the jump list text
available in 14 languages. BUILT WITH CLAUDE.

## What's included

- **EmptyRecycleBin.exe** — creates a desktop shortcut and registers the
  taskbar jump list. Also handles the `/empty` and `/open` actions.
- **RecycleBinSettings.exe** — a small settings window for choosing the jump
  list language.

## Requirements

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (only needed to build)

## Build

```powershell
dotnet publish RecycleBinTaskbar -c Release -r win-x64 --self-contained false
dotnet publish RecycleBinSettings -c Release -r win-x64 --self-contained false
```

Copy **both** published `.exe` files into the same folder — `RecycleBinSettings.exe`
looks for `EmptyRecycleBin.exe` next to itself.

## Setup

1. Run `EmptyRecycleBin.exe` once. This creates a **Recycle Bin** shortcut on
   your Desktop.
2. Drag that shortcut onto your taskbar to pin it (or right-click it in the
   Start menu → *Pin to taskbar*).
3. Right-click the pinned icon to see **Empty Recycle Bin** and **Open Recycle Bin**
   in the jump list.

## Changing the language

Run `RecycleBinSettings.exe`, pick a language from the dropdown, and click
**Apply**. The shortcut and jump list update immediately — no need to re-run
`EmptyRecycleBin.exe` or re-pin anything.

Supported languages: English, Español, Français, Deutsch, Italiano,
Português, Русский, 简体中文, 日本語, 한국어, Nederlands, Polski, Türkçe, العربية.

## Project structure

```
RecycleBinTaskbar.Shared/   Shared code: localization strings, settings storage,
                             shortcut/jump-list creation logic
RecycleBinTaskbar/          Main app (EmptyRecycleBin.exe)
RecycleBinSettings/         Settings window (RecycleBinSettings.exe)
```

## License

MIT
