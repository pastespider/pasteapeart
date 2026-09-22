@echo off
for /R "." %%G in (*.csproj) DO (
    dotnet pack "%%G" -c Release -o ".\nupkgs"
)
echo.
echo ==== 全部打包完成 ====
pause