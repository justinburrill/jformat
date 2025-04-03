config="--tl:on -c release"
sc="--self-contained=true"
sf="$config -p:PublishSingleFile=true"
trim="-p:PublishTrimmed=true"
notrim="-p:PublishTrimmed=false"

p="./jformat/bin/Release/net8.0"
proj="./jformat/jformat.csproj"

win="-r win-x64"
linux="-r linux-x64"


## remove old
rm -rf ./deploy
mkdir deploy


## ===== self contained executables =====
# windows
dotnet publish $proj $win $sc $sf $trim
cp "${p}/win-x64/publish/jformat.exe" ./deploy/jformat-win-x64-sc.exe
# linux
dotnet publish $proj $linux $sc $sf $trim
cp "${p}/linux-x64/publish/jformat" ./deploy/jformat-linux-x64-sc

## ======================

## ===== framework dependent executable and cross-platform binary (.dll) =====
dotnet publish $proj $sf $notrim
cp "${p}/linux-x64/publish/jformat" ./deploy/jformat-linux-x64-dep
cp "${p}/win-x64/publish/jformat.exe" ./deploy/jformat-win-x64-dep.exe
## ======================

## lemme see em
ls -lah ./deploy
