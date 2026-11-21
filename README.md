# Sync Anki with Google Phrasebook

A Windows-oriented .NET application with PowerShell scripts for exporting Google Phrasebook and synchronizing translations with Anki.

## Repository layout

- [SyncAnkiWithGooglePhraseBook.slnx](./SyncAnkiWithGooglePhraseBook.slnx): solution entry point.
- [src](./src): application and library projects.
- [tests](./tests): unit tests and shared test utilities.
- [scripts](./scripts): synchronization, URL handling, and protocol registration scripts.
- [external/ExportGooglePhraseBook](./external/ExportGooglePhraseBook): exporter Git submodule, with its own solution, build configuration, source projects, scripts, and analyzer policy.
- [Directory.Build.props](./Directory.Build.props) and [Directory.Packages.props](./Directory.Packages.props): shared build settings and central package versions for this repository. The submodule's own build files govern its projects.
- `artifacts\Output` and `artifacts\logs`: ignored phrasebook exports and runtime logs. Build output remains in each project's ignored `bin` and `obj` directories.

## Checkout and build

Initialize the submodule after cloning or updating the repository:

```powershell
git submodule update --init --recursive
dotnet build .\SyncAnkiWithGooglePhraseBook.slnx --configuration Release
dotnet test .\SyncAnkiWithGooglePhraseBook.slnx --configuration Release --no-build
```

Install .NET SDK 10.0.404 or a newer .NET 10 SDK. [global.json](./global.json) keeps builds on .NET 10 even when .NET 11 is installed. See the [exporter setup instructions](./external/ExportGooglePhraseBook/README.md) for credentials and exporter prerequisites.

## Code analysis

[.editorconfig](./.editorconfig) owns analyzer severities for this repository's source and tests without changing the exporter submodule's policy. Builds use the SDK's .NET 10 recommended analysis baseline, Sonar C# analyzers, threading analyzers, and the xUnit analyzers supplied transitively by xUnit. Menees and the separate .NET analyzer package are no longer referenced by this repository.

Sonar line-length (`S103`) and method-length (`S138`) violations remain warnings and fail the build because warnings are treated as errors. Magic-number checks (`S109`), selected performance/style recommendations, and library-only `CA2007` await guidance are suggestions. Disposal and cancellation-token checks are build-blocking warnings. Other migrated Sonar severities are preserved.

Install the reviewed PowerShell analyzer version and run the same check used by [CI](./.github/workflows/code-analysis.yml):

```powershell
Install-Module PSScriptAnalyzer -RequiredVersion 1.24.0 -Scope CurrentUser
.\scripts\Test-CodeAnalysis.ps1
```

[PSScriptAnalyzerSettings.psd1](./PSScriptAnalyzerSettings.psd1) checks warnings and errors while preserving console output, existing switch defaults, and the custom `-what-if` contract. Formatting verification remains deferred until a formatting baseline is established.

## Script entry points

With PowerShell 7.0 or later, Anki, and AnkiConnect configured:

```powershell
.\scripts\ExportGooglePhrasebookAndSyncAnki.ps1 -spreadSheetId '<spreadsheet-id>'
.\scripts\CustomUrlProtocol\RegisterCustomUrlProtocolForAnki.ps1
```

Use `-what-if` for a side-effect-free preview of the workflow:

```powershell
.\scripts\ExportGooglePhrasebookAndSyncAnki.ps1 -spreadSheetId '<spreadsheet-id>' -what-if
```

For valid input, dry-run prints the planned operations and returns without accessing Google or Anki, starting or stopping Anki, running .NET commands, or writing exports, logs, or build output. It does not calculate individual Anki changes. The [URL handler](./scripts/ExportGooglePhrasebookAndSyncAnkiCustomUrlHandler.ps1) also accepts `-what-if` and preserves the `PLATFORM` environment variable in that mode.

The synchronization script reuses a responsive AnkiConnect instance; otherwise it starts Anki once and waits up to 30 seconds for an AnkiConnect version response. Each probe times out after 2 seconds, so the final probe can extend the wait slightly. It does not stop Anki on failure. Check that AnkiConnect is installed and responding on `127.0.0.1:8765` if startup fails.

The URL handler requires an absolute `exportGooglePhrasebookToAnki` URL with exactly one non-empty `spreadSheetId` query parameter, for example `exportGooglePhrasebookToAnki://open?spreadSheetId=<spreadsheet-id>`. Invalid URLs are rejected before the environment is changed.

Workflow and input-validation errors are written to the console and appended to `artifacts\logs\ScriptErrors.log`, including error location and stack trace. The synchronization script and URL handler pause on errors by default when console input is available, including when launched through the registered protocol, then exit with code 1. Pass `-interactive:$false` for unattended execution; redirected console input also disables the pause. If the log cannot be written, that failure is reported in the console before pausing.

The [protocol registration script](./scripts/CustomUrlProtocol/RegisterCustomUrlProtocolForAnki.ps1) accepts `-what-if` to preview registration without writing to the registry. These guarantees apply to this repository's entry points, not direct invocations of the exporter submodule's scripts.

Run the PowerShell regression tests with the installed Pester module:

```powershell
Invoke-Pester .\tests\PowerShell
```

After upgrading from the old layout, rerun protocol registration to replace the stored handler path. Move any existing root-level `Output` and `logs` directories into `artifacts`, and update local shortcuts to the new script paths. Projects previously under `src\src` and `src\tests` are now under `src` and `tests`; rebuild rather than copying old build caches. The submodule has moved from the repository root to `external\ExportGooglePhraseBook` without changing its pinned commit or internal layout.