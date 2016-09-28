#
# .SYNOPSIS
# Generate a .nuspec file for building a TestRunner NuGet package
#
# .DESCRIPTION
# Populates as much information as possible from TestRunner.exe metadata.
#


#
# Deploying NuGet Package
#
# 1. Increment version numbers in TestRunner\Properties\AssemblyInfo.cs
#
# 2. Rebuild the solution
#
# 3. Run the unit test suite
#
# 4. powershell -file Generate-Nuspec.ps1
#
# 5. Review / edit .nuspec, especially $releaseNotes
#
# 6. nuget.exe pack .nuspec
#
# 7. nuget.exe push TestRunner.<version>.nupkg <apikey> -Source https://www.nuget.org/api/v2/package
#


#
# Read metadata from TestRunner.exe
#
$exe = "TestRunner\bin\Debug\TestRunner.exe"
$exeInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($exe)
$exeName = $exeInfo.ProductName
$exeMajor = $exeInfo.ProductMajorPart
$exeMinor = $exeInfo.ProductMinorPart
$exeBuild = $exeInfo.ProductBuildPart
$exeAuthors = $exeInfo.CompanyName
$exeCopyright = $exeInfo.LegalCopyright
$exeDescription = $exeInfo.FileDescription


#
# Assemble .nuspec information
#
$id = $exeName
$version = "$exeMajor.$exeMinor.$exeBuild"
$summary = $exeDescription
$description = $summary
$owners = ($exeAuthors -split ",")[0].Trim()
$authors = $exeAuthors
$copyright = "$exeCopyright $exeAuthors"
$tags = "mstest test runner console"
$projectUrl = "https://github.com/macro187/testrunner"
$licenseUrl = "$projectUrl/blob/master/license.txt"

$releaseNotes =
@"
Initial release
"@

$files = @(
    "readme.md",
    "license.txt"
    )

$toolsFiles = @(
    "TestRunner\bin\Debug\TestRunner.exe",
    "TestRunner\bin\Debug\Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll"
    )


#
# Generate .nuspec content
#
function Generate()
{
@"
<?xml version="1.0"?>
<package>
    <metadata>
        <id>$id</id>
        <version>$version</version>
        <summary>$summary</summary>
        <description>$description</description>
        <tags>$tags</tags>
        <projectUrl>$projectUrl</projectUrl>
        <owners>$owners</owners>
        <authors>$authors</authors>
        <copyright>$copyright</copyright>
        <licenseUrl>$licenseUrl</licenseUrl>
        <requireLicenseAcceptance>false</requireLicenseAcceptance>
        <releaseNotes>$releaseNotes</releaseNotes>
    </metadata>
    <files>
"@

foreach ($file in $files) {
@"
        <file src="$file" target="" />
"@
}

foreach ($file in $toolsFiles) {
@"
        <file src="$file" target="tools" />
"@
}

@"
    </files>
</package>
"@
}


#
# Write .nuspec file
#
$s = ""
foreach ($line in (Generate)) {
    $line = $line.Replace("`r`n", "`n").Replace("`r", "`n").Replace("`n", [Environment]::NewLine)
    $s += $line + [Environment]::NewLine
}

[System.IO.File]::WriteAllText(".nuspec", $s)

