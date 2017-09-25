![Logo](https://raw.githubusercontent.com/helluvamatt/ModMonitor/master/ModMonitor/Resources/Icons/icon.png "Mod Monitor")

# Mod Monitor

An alternative to Device Monitor from the Evolv Escribe Suite for monitoring Evolve DNA devices through a serial connection.

## Releases

[Latest Releases](https://github.com/helluvamatt/ModMonitor/releases)

**Please note**: Only one version (the latest version) is supported for bug fixes and new features.

Please open an Issue if you find a bug or are requesting a new feature.

## Contributing

1. Get [Visual Studio 2017 Community](https://www.visualstudio.com/downloads/)
2. Fork/clone project
3. Open ModMonitor.sln
4. Run either `LibDnaSerial.Test` or `ModMonitor`

This project uses [Semantic Versioning](http://semver.org/) mapped into Microsoft 4-value version numbers as follows:

* Major -> MAJOR
* Minor -> MINOR
* Patch -> BUILD
* "0" -> REVISION

All other rules of Semantic Versioning should be followed. Any API breaking features to `LibDnaSerial` should be developed on a feature branch with the `AssemblyInfo.cs` updated accordingly and will likely be tracked and merged on a `MAJOR-dev` branch.

The main application (`ModMonitor`) will update "Major" with full rewrites, "Minor" with new features", and "Patch" with bug fixes. New features should be developed on feature branches. Bug fixes can be developed on `master`.

Pull requests for fixing bugs _must_ have an open "Issue" on GitHub. The "Issue" is for discussing the issue and possible fixes. The "Pull Request" is for discussing that particular proposed fix. Pull requests for new features do not need an associated Issue, but one may already be present if requested by a user. All off-topic discussion in Issues or Pull Requests may be subject to moderation at any time.

