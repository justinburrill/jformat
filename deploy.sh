echo "Make sure to run this from WINDOWS, doesn't work properly on linux for some reason"

config="--tl:on -c release"
sc="--self-contained=true"
sf="$config -p:PublishSingleFile=true"
nosc="--self-contained=false"
trim="-p:PublishTrimmed=true"
notrim="-p:PublishTrimmed=false"

p="./jformat/bin/Release/net8.0"
proj="./jformat/jformat.csproj"

win="-r win-x64"
linux="-r linux-x64"


## remove old
echo "Removing old builds..."
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
dotnet publish $proj $sf $notrim $nosc
cp "${p}/win-x64/publish/jformat.exe" ./deploy/jformat-win-x64-dep.exe
dotnet publish $proj $sf $notrim $nosc --os linux
cp "${p}/linux-x64/publish/jformat" ./deploy/jformat-linux-x64-dep
## ======================


## copy examples over
cp "${p}/win-x64/publish/examples/"* ./deploy/

## lemme see em
echo "Files in ./deploy:"
ls -lah ./deploy
