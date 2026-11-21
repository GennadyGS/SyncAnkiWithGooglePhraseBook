# Sync Anki with Google Phrasebook

A Windows-oriented .NET application with PowerShell scripts for exporting Google Phrasebook and synchronizing translations with Anki.

## Repository layout

- [SyncAnkiWithGooglePhraseBook.slnx](./SyncAnkiWithGooglePhraseBook.slnx): solution entry point.
- [src](./src): application and library projects.
- [tests](./tests): unit tests and shared test utilities.
- [scripts](./scripts): synchronization, URL handling, and protocol registration scripts.
- [external/ExportGooglePhraseBook](./external/ExportGooglePhraseBook): exporter Git submodule, with its own solution, build configuration, source projects, scripts, and analyzer policy.
- [Directory.Build.props](./Directory.Build.props) and [Directory.Packages.props](./Directory.Packages.props): shared build settings and central package versions for this repository. The analyzer import still uses the exporter submodule; the submodule's own build files govern its projects.
- `artifacts\Output` and `artifacts\logs`: ignored phrasebook exports and runtime logs. Build output remains in each project's ignored `bin` and `obj` directories.

## Checkout and build

Initialize the submodule after cloning or updating the repository:

```powershell
git submodule update --init --recursive
dotnet build .\SyncAnkiWithGooglePhraseBook.slnx --configuration Release
dotnet test .\SyncAnkiWithGooglePhraseBook.slnx --configuration Release --no-build
```

Install .NET SDK 10.0.404 or a newer .NET 10 SDK. [global.json](./global.json) keeps builds on .NET 10 even when .NET 11 is installed. See the [exporter setup instructions](./external/ExportGooglePhraseBook/README.md) for credentials and exporter prerequisites.

## Script entry points

With PowerShell 7, Anki, and AnkiConnect configured:

```powershell
.\scripts\ExportGooglePhrasebookAndSyncAnki.ps1 -spreadSheetId '<spreadsheet-id>'
.\scripts\CustomUrlProtocol\RegisterCustomUrlProtocolForAnki.ps1
```

Use `-what-if` for a side-effect-free preview of the workflow:

```powershell
.\scripts\ExportGooglePhrasebookAndSyncAnki.ps1 -spreadSheetId '<spreadsheet-id>' -what-if
```

Dry-run prints the planned operations and returns without accessing Google or Anki, starting or stopping Anki, running .NET commands, or writing exports, logs, or build output. It does not calculate individual Anki changes. The [URL handler](./scripts/ExportGooglePhrasebookAndSyncAnkiCustomUrlHandler.ps1) also accepts `-what-if` and preserves the `PLATFORM` environment variable in that mode.

The [protocol registration script](./scripts/CustomUrlProtocol/RegisterCustomUrlProtocolForAnki.ps1) accepts `-what-if` to preview registration without writing to the registry. These guarantees apply to this repository's entry points, not direct invocations of the exporter submodule's scripts.

Run the PowerShell regression tests with the installed Pester module:

```powershell
Invoke-Pester .\tests\PowerShell\DryRun.Tests.ps1
```

After upgrading from the old layout, rerun protocol registration to replace the stored handler path. Move any existing root-level `Output` and `logs` directories into `artifacts`, and update local shortcuts to the new script paths. Projects previously under `src\src` and `src\tests` are now under `src` and `tests`; rebuild rather than copying old build caches. The submodule has moved from the repository root to `external\ExportGooglePhraseBook` without changing its pinned commit or internal layout.