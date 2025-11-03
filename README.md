# DotNetSemanticAgent

Small single-project .NET 8 console application demonstrating a plugin-style agent.

## Build & run

Requirements:
- .NET 8 SDK (net8.0)

Build:

```pwsh
dotnet build DotNetSemanticAgent.sln
```

Run from project folder:

```pwsh
dotnet run --project DotNetSemanticAgent/DotNetSemanticAgent.csproj
```

## Project layout

- `DotNetSemanticAgent/` - project source
  - `Program.cs` - application entry
  - `LightsPlugin.cs` - example plugin
  - `LightModel.cs` - domain model

## Notes

- This repository was prepared for local development. If you plan to publish, add an appropriate license and update the README.

## About

Building your own AI Agent using Semantic Kernel
-----------------------------------------------
Semantic Kernel is a lightweight, open-source development kit that lets you easily build AI agents and integrate the latest AI models into your C#, Python, or Java codebase. It serves as an efficient middleware that enables rapid delivery of enterprise-grade solutions.
