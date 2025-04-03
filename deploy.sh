## self contained executables
# windows
dotnet publish -r win-x64 -c release --self-contained
# linux 
dotnet publish --os linux -c release --self-contained

## framework dependent executable and cross-platform binary (.dll)
dotnet publish
