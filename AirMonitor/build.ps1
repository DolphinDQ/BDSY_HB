$source=get-content -path ./Properties/AssemblyInfo.cs
$reg=[regex]"(\w+\.){3}\w+"
$version  = $reg.Matches($source).Value
#echo $source
echo "read version :" $version
$reg=[regex]"(<ApplicationVersion>)((\w+\.){3}\w+)(</ApplicationVersion>)"
$targetPath="./AirMonitor.csproj"
$target= get-content -path $targetPath
$targetVersion = $reg.Matches($target).Groups[2].Value
 if(!$version.Equals($targetVersion)) 
 {
    echo "target version:" $targetVersion
    $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding($False)
    [System.IO.File]::WriteAllLines($targetPath ,$target.Replace($targetVersion,$version), $Utf8NoBomEncoding)
 } 
