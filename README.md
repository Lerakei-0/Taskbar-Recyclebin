# Recycle Bin Taskbar
<p align="center">
<img alt="Screenshot of the program" src=RecycleBin_Screenshot.png />
</p>

Pin the Recycle Bin to your Windows taskbar with a right-click jump list for
**Empty Recycle Bin** and **Open Recycle Bin** — complete with a live item
count/size header, optional confirmation and sound, toast notifications, and
a Fluent-styled settings app for choosing the jump list's language.

## Features

- Pin the Recycle Bin to your taskbar with a proper jump list — no more
  right-click → Empty Recycle Bin from the desktop icon.
- **Live status header** showing the current item count and total size
  (e.g. *"17 items, 1.6 GB"*), refreshed automatically every time you use
  the jump list or click the pinned icon.
- **14 languages** for the jump list text: English, Español, Français,
  Deutsch, Italiano, Português, Русский, 简体中文, 日本語, 한국어, Nederlands,
  Polski, Türkçe, العربية .
- **Optional confirmation prompt** and **sound** before emptying (off by
  default, matching classic silent behavior).
- **Toast notification** after emptying (on by default).
- A small **Fluent/Windows 11-styled settings app** (built with
  [WPF-UI](https://github.com/lepoco/wpfui)) to change the language and
  toggle the behaviors above, with a live "current status" readout.

## What's included

- **EmptyRecycleBin.exe** — creates the desktop/Start Menu shortcuts and
  registers the taskbar jump list. Also handles the `/empty` and `/open`
  actions triggered from the jump list.
- **RecycleBinSettings.exe** — the settings window.

## Requirements

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (only needed to build)

## Build

```powershell
dotnet publish RecycleBinTaskbar -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
dotnet publish RecycleBinSettings -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Each command produces a single self-contained `.exe` in that project's
`bin\Release\net8.0-windows*\win-x64\publish\` folder. Copy **both** exes
into the same folder — `RecycleBinSettings.exe` looks for
`EmptyRecycleBin.exe` right next to itself.

> Prefer a smaller download over a no-dependencies exe? Drop
> `--self-contained true` (and the two `-p:` flags) and use
> `--self-contained false` instead. This needs the
> [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
> installed on the machine running it, but produces a much smaller build.

## Setup

1. Run `EmptyRecycleBin.exe` once. This creates a **Recycle Bin** shortcut
   on your Desktop (and one in your Start Menu, used for notification
   delivery).
2. Drag the Desktop shortcut onto your taskbar to pin it (or right-click it
   in the Start menu → *Pin to taskbar*).
3. Right-click the pinned icon to see the live item count header along with
   **Empty Recycle Bin** and **Open Recycle Bin**.

## Changing settings

Run `RecycleBinSettings.exe` to:

- Pick the jump list language
- Toggle the confirmation prompt, sound, and post-empty notification
- Check the current item count/size, with a manual refresh option

Click **Apply** — changes take effect immediately, no need to re-run
`EmptyRecycleBin.exe` or re-pin anything.

## Project structure

```
RecycleBinTaskbar.Shared/   Shared code: localization strings, settings storage,
                             Recycle Bin status queries, shortcut/jump-list logic
RecycleBinTaskbar/          Main app (EmptyRecycleBin.exe)
RecycleBinSettings/         Settings window (RecycleBinSettings.exe)
```

## Contributing

Adding a language is just a matter of adding an entry to the dictionary in
`RecycleBinTaskbar.Shared/Localization.cs`. PRs welcome.

## License

MIT
