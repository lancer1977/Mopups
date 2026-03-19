# Mopups

**Location:** `~/code/Mopups`

**Purpose:** Popup/modal library for .NET MAUI applications. A replacement for Rg.Plugins.Popups (Xamarin) with similar API for easy migration.

**NuGet:** [Mopups](https://www.nuget.org/packages/Mopups/)

## Tech Stack

- **Framework:** .NET MAUI (net8.0, net9.0)
- **Platforms:** Android, iOS, Windows, Mac Catalyst
- **Language:** C#

## Project Structure

```
Mopups/
├── Mopups.sln                    # Solution file
├── Mopups/
│   └── Mopups.Maui/              # Main library
│       ├── Animations/           # Popup animation definitions
│       ├── Contracts/            # Interfaces/contracts
│       ├── Enums/                # Enumerations
│       ├── Events/               # Event definitions
│       ├── Hosting/              # DI extension methods
│       ├── Pages/                # PopupPage base class
│       ├── Platforms/            # Platform-specific implementations
│       └── Services/             # MopupsService
├── SampleMaui/                   # Demo app
└── tests/                         # Unit tests
```

## Entry Points

### Registration
```csharp
// In MauiProgram.cs
builder.Services.AddMopups();
```

### Usage
```csharp
// Show popup
await MopupsService.Instance.PushAsync(new MyPopupPage());

// Dismiss
await MopupsService.Instance.PopAsync();
```

## Dependencies

- `AsyncAwaitBestPractices` v6.0.4

## Build

```bash
dotnet build Mopups.sln
dotnet test tests/Mopups.Tests.csproj
```

## CI/CD

- Azure Pipelines: `azure-pipelines.yml`
- NuGet package auto-generated on build

## Notes

- Platform code uses suffix convention: `*.Android.cs`, `*.iOS.cs`, `*.Windows.cs`, `*.MacCatalyst.cs`
- Supports .NET 8 and .NET 9
- Version 1.1.0 added Windows and MacOS support