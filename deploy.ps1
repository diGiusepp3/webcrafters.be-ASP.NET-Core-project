# FILE: deploy.ps1
# Powershell script voor automatische deploy naar asp.webcrafters.be

# Config
$projectPath = "C:\Users\matth\RiderProjects\webcrafters.be-ASP.NET-Core-project"
$publishDir  = "$projectPath\publish"
$serverUser  = "matthias"
$serverHost  = "mgielen.zapto.org"
$serverPath  = "/var/www/asp.webcrafters.be"
$serviceName = "asp-webcrafters"

Write-Host "🚀 Stap 1: Clean & Publish..."
dotnet publish "$projectPath" -c Release -o $publishDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Publish gefaald!"
    exit 1
}

Write-Host "📦 Stap 2: Upload naar server..."
scp -r "$publishDir\*" "${serverUser}@${serverHost}:${serverPath}/"

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Upload gefaald!"
    exit 1
}

Write-Host "🔄 Stap 3: Restart systemd service..."
ssh "$serverUser@$serverHost" "sudo systemctl restart $serviceName && sudo systemctl status $serviceName --no-pager -l"

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Restart gefaald!"
    exit 1
}

Write-Host "✅ Deploy succesvol afgerond! Bezoek https://asp.webcrafters.be"
