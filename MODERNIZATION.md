# Modernization plan

## Context and baseline

This is a Windows-oriented .NET command-line application with PowerShell orchestration. The priorities are reproducible builds, safe execution, testing, and distribution rather than cloud infrastructure.

The existing project already uses central package management, nullable reference types, warnings as errors, separate library and test projects, and multiple analyzers.

The assessment found that a Release build succeeded with no warnings or errors and all 30 tests passed using SDK 11.0.100. Google and Anki operations were not executed.

## 1. Improve operational safety

- Decide and document whether dry-run mode means no remote mutations or only an Anki preview. The current [orchestration script](./ExportGooglePhrasebookAndSyncAnki.ps1) still exports and deletes the spreadsheet in dry-run mode.
- Reuse an already responsive Anki instance, check an actual AnkiConnect response, and use bounded timeouts.
- Track processes started by the script; do not terminate unrelated or pre-existing Anki instances.
- Declare the required PowerShell version, validate typed parameters, and use `Join-Path` and `$PSScriptRoot` for script-relative paths.
- Validate the scheme and required query parameters in the [URL handler](./ExportGooglePhrasebookAndSyncAnkiCustomUrlHandler.ps1).
- Separate interactive error prompts from unattended execution.

## 2. Document setup and usage

Replace the empty [README](./README.md) with instructions for:

- Prerequisites and recursive submodule checkout.
- Credentials, AnkiConnect, configuration, and protocol registration.
- Normal execution, dry-run semantics, troubleshooting, and recovery.
- Building, testing, and publishing.

## 3. Stabilize build infrastructure

- Pin the .NET SDK and roll-forward policy in `global.json`.
- Make analyzer policy owned by this repository rather than importing it from the exporter submodule.
- Preserve central package management and consider NuGet lock files with locked restore in CI.
- Set `ContinuousIntegrationBuild` in CI and provide a shared local build and test entry point.
- Account for the submodule's own build and package configuration; root configuration does not automatically override it.

## 4. Modernize analyzer and formatting policy

- Migrate the legacy ruleset to `.editorconfig`, preserving existing severities first.
- Explicitly set the analysis level and review whether the separate .NET analyzer package is needed for deliberate version pinning.
- Add `dotnet format --verify-no-changes` after establishing a formatting baseline.
- Add PSScriptAnalyzer for scripts and xUnit analyzers for tests.
- Remove stale analyzer configuration only after confirming it is unused; avoid adding overlapping general-purpose analyzer suites.

## 5. Align tests and CI

- Retain the currently working xUnit v2 and VSTest setup initially and remove misleading Microsoft Testing Platform properties. Treat migration to xUnit v3 and Microsoft Testing Platform as a separate change.
- Add AnkiConnect contract tests with a fake HTTP handler and CLI validation and exit-code tests.
- Add Pester tests for orchestration with Google, Anki, and process operations mocked, including dry-run and failure paths.
- Add a Windows GitHub Actions workflow that checks out submodules recursively, installs the pinned SDK, restores in locked mode, builds Release, runs tests, and checks formatting and PowerShell analysis.
- Publish test results and release artifacts. Pin actions to reviewed commit SHAs and use minimal workflow permissions.

## 6. Simplify the directory structure

Consider this layout while retaining existing project boundaries:

```text
repository\
  .github\
    workflows\
    dependabot.yml
  eng\
    build.ps1
    analyzers\
  scripts\
    Sync-Phrasebook.ps1
    Invoke-PhrasebookUrl.ps1
    Register-PhrasebookProtocol.ps1
  src\
    application and library projects
  tests\
    test projects
  external\
    ExportGooglePhraseBook\
  artifacts\                    # Ignored generated output
  .editorconfig
  .gitattributes
  global.json
  Directory.Build.props
  Directory.Build.targets
  Directory.Packages.props
  SyncAnkiWithGooglePhraseBook.slnx
  README.md
```

Update solution and project references, script paths, submodule configuration, and analyzer imports together when moving files.

## 7. Improve distribution and maintenance

- Publish a versioned Windows bundle containing the application, required exporter tools, scripts, example configuration, setup instructions, and checksums.
- Choose framework-dependent publishing for smaller packages or self-contained publishing for installation without a preinstalled runtime. Defer trimming and Native AOT until compatibility is tested.
- Allow configuration outside the application directory and define retention and access rules for logs and phrasebook dumps.
- Add Dependabot for NuGet and GitHub Actions. Review package maintenance and licensing, including Accord.Math, without replacing algorithms before adding regression tests.

## Suggested implementation order

1. Operational safety and README.
2. SDK pinning and repository-owned analyzer policy.
3. CI and consistent test configuration.
4. Directory restructuring.
5. Published releases and dependency cleanup.
