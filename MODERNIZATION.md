# Modernization plan

## Context and baseline

This is a Windows-oriented .NET command-line application with PowerShell orchestration. The priorities are reproducible builds, safe execution, testing, and distribution rather than cloud infrastructure.

The existing project already uses central package management, nullable reference types, warnings as errors, separate library and test projects, and multiple analyzers.

The assessment found that a Release build succeeded with no warnings or errors and all 30 tests passed using SDK 11.0.100. Google and Anki operations were not executed.

## 1. Improve operational safety

- Dry-run safety implemented: the [orchestration script](./scripts/ExportGooglePhrasebookAndSyncAnki.ps1) prints the planned workflow without remote access, file writes, or process operations. The URL handler preserves the environment in dry-run mode.
- Reuse an already responsive Anki instance, check an actual AnkiConnect response, and use bounded timeouts (implemented).
- Track processes started by the script; do not terminate unrelated or pre-existing Anki instances.
- Declare the required PowerShell version, validate typed parameters, and use `Join-Path` and `$PSScriptRoot` for script-relative paths.
- Validate the scheme and required query parameters in the [URL handler](./scripts/ExportGooglePhrasebookAndSyncAnkiCustomUrlHandler.ps1).
- Separate interactive error prompts from unattended execution.

## 2. Document setup and usage

Expand the [README](./README.md) with instructions for:

- Prerequisites and recursive submodule checkout.
- Credentials, AnkiConnect, configuration, and protocol registration.
- Normal execution, dry-run semantics, troubleshooting, and recovery.
- Building, testing, and publishing.

## 3. Stabilize build infrastructure

- Pin the .NET SDK and roll-forward policy in `global.json`.
- Repository-owned analyzer policy implemented; the exporter submodule retains its separate policy.
- Preserve central package management and consider NuGet lock files with locked restore in CI.
- Set `ContinuousIntegrationBuild` in CI and provide a shared local build and test entry point.
- Account for the submodule's own build and package configuration; root configuration does not automatically override it.

## 4. Modernize analyzer and formatting policy

- Migrated this repository's Sonar severities to [.editorconfig](./.editorconfig); line and method length remain build blockers, while magic numbers are suggestions.
- Selected the SDK's .NET 10 recommended analysis baseline and removed this repository's separate .NET analyzer package and overlapping Menees suite.
- Add `dotnet format --verify-no-changes` after establishing a formatting baseline.
- Added [PSScriptAnalyzer validation](./scripts/Test-CodeAnalysis.ps1) locally and in CI; xUnit analyzers are already supplied transitively by xUnit.
- Removed the root import of stale StyleCop/Menees configuration; retained the submodule's configuration and avoided additional general-purpose suites.

## 5. Align tests and CI

- Retain the currently working xUnit v2 and VSTest setup initially and remove misleading Microsoft Testing Platform properties. Treat migration to xUnit v3 and Microsoft Testing Platform as a separate change.
- Add AnkiConnect contract tests with a fake HTTP handler and CLI validation and exit-code tests.
- Add Pester tests for orchestration with Google, Anki, and process operations mocked, including dry-run and failure paths.
- Added a [Windows GitHub Actions workflow](./.github/workflows/code-analysis.yml) with recursive submodule checkout, the pinned SDK, Release build, tests, and PowerShell analysis. Locked restore and formatting verification remain deferred.
- Publish test results and release artifacts. Pin actions to reviewed commit SHAs and use minimal workflow permissions.

## 6. Directory structure implemented

The repository now follows this layout while retaining existing project boundaries and matching the exporter's source and script conventions:

```text
repository\
  scripts\
    ExportGooglePhrasebookAndSyncAnki.ps1
    ExportGooglePhrasebookAndSyncAnkiCustomUrlHandler.ps1
    CustomUrlProtocol\
      RegisterCustomUrlProtocolForAnki.ps1
  src\
    application and library projects
  tests\
    test projects
  external\
    ExportGooglePhraseBook\
  artifacts\                    # Ignored generated output
  Directory.Build.props
  Directory.Packages.props
  SyncAnkiWithGooglePhraseBook.slnx
  README.md
  MODERNIZATION.md
```

Solution and project references, script paths, and submodule configuration use the new locations. Root build settings and analyzer policy apply to this repository's source and tests; the submodule retains its own build, package, and analyzer configuration. See the [README](./README.md) for build commands, analysis checks, and migration steps.

## 7. Improve distribution and maintenance

- Publish a versioned Windows bundle containing the application, required exporter tools, scripts, example configuration, setup instructions, and checksums.
- Choose framework-dependent publishing for smaller packages or self-contained publishing for installation without a preinstalled runtime. Defer trimming and Native AOT until compatibility is tested.
- Allow configuration outside the application directory and define retention and access rules for logs and phrasebook dumps.
- Add Dependabot for NuGet and GitHub Actions. Review package maintenance and licensing, including Accord.Math, without replacing algorithms before adding regression tests.

## Suggested implementation order

1. Operational safety and README.
2. SDK pinning and repository-owned analyzer policy.
3. CI and consistent test configuration.
4. Directory restructuring (implemented).
5. Published releases and dependency cleanup.
