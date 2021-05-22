# RadLine

This is a preview of the RadLine library.  
At this point, we will not be accepting pull requests for new functionality.

_[![RadLine NuGet Version](https://img.shields.io/nuget/v/radline.svg?style=flat&label=NuGet%3ARadLine)](https://www.nuget.org/packages/RadLine)_

# Usage

See the `RadLine.Sandbox` project for usage examples.

# Building

```
> dotnet tool restore
> dotnet cake
```

# Known issues

* Terminal is not set in raw mode, so some key combinations might not
  work in certain terminals.
* Any modifier with `ENTER` key does not register on macOS, due to a bug
  in the System.Console.ReadKey implementation. We will be moving away
  from using this before release.
* Lines do not update properly when moving past vertical screen buffer boundaries.