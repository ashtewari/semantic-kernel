Get-Process -Name "AzureAIStudioIndexExample" -ErrorAction SilentlyContinue | Stop-Process
dotnet build
dotnet run --no-build