@echo off

cd ..\src\Isotope\Isotope

del /f /q ".\Data\Migrations\*.cs"
dotnet ef database drop -f

dotnet ef migrations add Initial -o "Data\Migrations" --no-build
dotnet ef database update --no-build