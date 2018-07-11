
#<LastPublishVersion>1.0.2</LastPublishVersion>

function getRegex{ [regex][System.String]::Format("<{0}>(.*)</{0}>",$args); }
function getConfig{
  $reg=getRegex($args.Get(1));
  $reg.Matches($args.Get(0)).Groups[1].Value;
}
function putFile($baseDir, $dir,$targetPath,$script){
#   echo "base dir:" $baseDir
#   echo "dir:" $dir
    $path=$baseDir+$dir;
    "mkdir "+$targetPath | Out-File -Append $script
    $dirs = [System.IO.Directory]::GetDirectories($path)
    if($dirs.Length){
        foreach($name in $dirs){
           $sDir=([string]$name).Replace($baseDir,"")
           putFile $baseDir $sDir $targetPath$sDir $script
        }
    }
    $files = [System.IO.Directory]::GetFiles($path)
    if($files.Length){
        foreach($name in $files){
            "put "+$name+" "+$targetPath+([string]$name).Replace($path,"").Replace("\","/") | Out-File -Append $script   
        }
    }
}
$proj=Get-Content .\AirStandard.Core.csproj
$version=getConfig $proj "Version"
$ps= [System.IO.File]::ReadAllText(".\publish.ps1")
$lastVersion=getConfig $ps LastPublishVersion
if([string]::Equals($version,$lastVersion)){
   return;
}else{
   $ps = ([string]$ps).Replace($lastVersion,$version)
   [System.IO.File]::WriteAllText(".\publish.ps1",$ps)
}

$config=Get-Content .\Properties\PublishProfiles\linux-arm.pubxml
$buildConfig= getConfig $config "LastUsedBuildConfiguration"
$framework= getConfig $config "TargetFramework"
$ftp=getConfig $config "publishUrl"
$ftpPath=getConfig $config "FtpSitePath"
$ftpUser=getConfig $config "UserName"
$runtime=getConfig $config "RuntimeIdentifier"
$ftpPassword="123456"
$ftp=([string]$ftp).Replace("ftp://","")
$ftp=([string]$ftp).Replace("/","")
$ftp=([string]$ftp).Replace(":"," ")


dotnet publish -c $buildConfig -f $framework -r $runtime 

$script="ftpscript";
"open "+$ftp | out-file -append $script
$ftpUser | out-file -append $script
$ftpPassword | out-file -append $script
"prompt" | out-file -append $script

$baseDir=".\bin\"+$buildConfig+"\"+$framework+"\"+$runtime+"\publish\"

putFile $baseDir "" $ftpPath $script 
"bye" | out-file -append $script
"quit" | out-file -append $script
$log="./ftp.log"

ftp -s:$script -i | Out-File -Append $log
get-content $log
del $script
del $log


