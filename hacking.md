Building
========

.NET Framework
```
dotnet publish -f net461
```

.NET Core
```
dotnet publish -f netcoreapp2.0
```

Mono
```
msbuild /p:TargetFramework=net461 /t:Restore
msbuild /p:TargetFramework=net461 /t:Rebuild
msbuild /p:TargetFramework=net461 /t:Publish
```



Continuous Integration
======================


Appveyor (Windows)
------------------

[![Build status](https://ci.appveyor.com/api/projects/status/v8s72ij64an7kr87?svg=true)](https://ci.appveyor.com/project/macro187/testrunner)

net461 build and tests

netcoreapp2.0 build and tests


Travis (Linux)
--------------

[![Build Status](https://travis-ci.org/macro187/testrunner.svg?branch=master)](https://travis-ci.org/macro187/testrunner)

net461 build and tests (Mono)

netcoreapp2.0 build and tests (.NET Core)
