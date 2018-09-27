# Changelog

## v1.3 - 2018.09.27
- Add `NoOpGraphiteClient` and `NoOpStatsDClient` for DI-container integration convenience in cases 
  when you do not want any graphite or statsd reporting.
- Add more `StatsDClient` extensions. Note breaking change in `Timing(..., Action f)` and `Timing(..., Func<T> f)` overloads. 
  They now send metrics to statsd also in case of exception from delegate `f`.
- Make RootNamespace = AssemblyName and simplify code directory structure, hence several namespace changes.
- Rename `IGraphiteTopology` to `IGraphiteClientSettings` and enrich it with `GlobalPathPrefix` property which is convinent 
  to set project-wide common prefix for all your metrics being reported (e.g. `MyProject.Prod` and `MyProject.Stage`).

## v1.2 - 2018.09.20
- Support .NET Standard 2.0.
- Switch to SDK-style project format and dotnet core build tooling.
- Use [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning) to automate generation of assembly 
  and nuget package versions.
