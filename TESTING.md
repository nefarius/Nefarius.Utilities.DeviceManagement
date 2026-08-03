# Testing

This repository mixes hermetic unit tests, Windows CI integration tests against always-present devices, and an Explicit hardware/lab suite that mutates drivers or needs special peripherals.

## Quick start (safe default)

On Windows:

```powershell
dotnet test Tests/Tests.csproj -c Release --filter "Category=Unit|Category=CI"
```

A bare `dotnet test` also skips `[Explicit]` tests, so Hardware / Destructive / Interactive cases will not run unless selected.

## Categories

| Category | Purpose | Runs on GitHub-hosted CI |
|----------|---------|--------------------------|
| `Unit` | Pure helpers and fakes (no live PnP required for matching/filter/virtual logic) | Yes (`windows-latest`) |
| `CI` | Live Windows, non-destructive, always-available devices (HID, HPET, Driver Store read) | Yes (`windows-latest`) |
| `Hardware` | Special devices/drivers (Xbox, DualSense, BthPS3, Bluetooth props, HidHide, USBPcap) | No — Explicit |
| `Interactive` | Human plug/unplug within a timeout | No — Explicit |
| `Destructive` | Null/custom driver install, class filter mutation | No — Explicit |
| `Admin` | Needs an elevated process | No — Explicit |

## Hardware / lab suite

Run elevated when exercising Destructive / Admin tests:

```powershell
dotnet test Tests/Tests.csproj -c Release --filter "Category=Hardware|Category=Destructive|Category=Interactive"
```

### Prerequisites

| Test area | Needs |
|-----------|--------|
| Xbox XUSB find / instance ID | One or two Xbox 360/One-compatible controllers |
| Virtual X360 / `IsVirtual` | Emulated Xbox 360 controller |
| DualSense custom driver | DualSense (`USB\VID_054C&PID_0CE6`), elevated |
| Null driver install | Emulated X360, elevated |
| Class filters | HidHide + USBPcap services installed, elevated |
| BthPS3 partial hardware ID | BthPS3 stack present |
| Device notification listener | Interactive HID plug then unplug within 10s each |

**Warning:** Destructive tests install/remove drivers and rewrite device-class Upper/Lower filters. Use a lab machine, not a daily driver, unless you know how to recover.

## Self-hosted device lab (optional)

A commented job template lives in [`.github/workflows/build.yml`](.github/workflows/build.yml). To use it:

1. Register a Windows self-hosted runner with labels `self-hosted`, `windows`, and `device-lab`.
2. Install required lab software/devices from the table above.
3. Uncomment / enable the `test-hardware-lab` job.

## CI layout

- **Ubuntu `build` job** — multi-TFM compile gate.
- **Windows `test-windows` job** — `Category=Unit|Category=CI` with TRX + coverage artifacts.
- **NuGet publish** — same Windows test filter must pass before pack/push.
