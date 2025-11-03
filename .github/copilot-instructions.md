## Copilot / AI agent instructions for this repository

Purpose: give an AI coding agent immediate, actionable context for working in this single-project .NET solution so it can make safe, small, high-value changes.

High-level snapshot
- Project: single C# project in `DotNetSemanticAgent/` targeting .NET 8 (net8.0). See `DotNetSemanticAgent.csproj` and the `bin/Debug/net8.0/` output.
- Entry point and orchestration: `Program.cs` wires up the application and composes plugins/services.
- Primary domain files: `LightsPlugin.cs` (plugin/feature surface) and `LightModel.cs` (domain model for lights).

Build & run (developer workflows)
- Build solution from the repository root:
  - `dotnet build DotNetSemanticAgent.sln`
- Run the app from project directory:
  - `dotnet run --project DotNetSemanticAgent/DotNetSemanticAgent.csproj`
- Debugging: use Visual Studio / VS Code with the .NET debugger; the project targets net8.0 so ensure the .NET 8 SDK is installed.

Project-specific patterns and conventions (discoverable)
- Single-project console-style application where `Program.cs` composes a small plugin set. Changes to behavior are usually made by editing or adding plugin-like classes (e.g., `LightsPlugin.cs`).
- Domain models are simple POCOs — `LightModel.cs` contains the light properties/state. When adding fields, keep property names simple and immutable patterns minimal unless adjusting the plugin logic.
- No test project discovered in the repository root. Assume changes should be small and validated by running the program locally.

Integration & dependencies
- The project appears self-contained (no obvious external service configs). If you add NuGet packages, update `DotNetSemanticAgent.csproj` and run a build to verify.
- Outputs and runtime files live under `bin/` and `obj/` — do not modify these directly.

How to make safe changes (examples)
- To change runtime behavior: modify `LightsPlugin.cs` where the plugin implements the lighting logic, then run `dotnet run` and verify expected console/log output.
- To add a new property to lights: update `LightModel.cs` and adjust any serialization/usage in `LightsPlugin.cs` and `Program.cs`.

Minimal checks an agent should run before proposing edits
1. Confirm `DotNetSemanticAgent.csproj` target framework (net8.0) and adjust new code to be compatible.
2. Build after edits: `dotnet build DotNetSemanticAgent.sln` and fix compiler errors before submitting changes.
3. Avoid touching `bin/` and `obj/` files and generated artifacts.

Notes & limitations
- No existing `.github/copilot-instructions.md` or AGENT.md files were found in the repo root; this file is intended to be a compact, actionable guide only using discoverable information.
- If you need deeper domain knowledge (why certain plugin choices were made), ask the maintainer for design notes — they are not present in source.

If anything here is unclear or you'd like more examples (for instance, snippets from `LightsPlugin.cs` or `Program.cs`), say which file to inspect and I will refine this guidance.
