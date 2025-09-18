$projectPath = "C:\Users\matth\RiderProjects\webcrafters.be-ASP.NET-Core-project"
$publishPath = "C:\out\webcrafters"  # nieuwe outputlocatie
$serverUser  = "matthias"
$serverHost  = "mgielen.zapto.org"
$serverPath  = "/var/www/asp.webcrafters.be"
$serviceName = "webcrafters-asp-web.service"

# Schoonmaken
if (Test-Path $publishPath) {
    Remove-Item -Recurse -Force $publishPath
}

# Build
dotnet publish $projectPath -c Release -o $publishPath

# Upload enkel inhoud
scp -r "${publishPath}\*" "${serverUser}@${serverHost}:${serverPath}/"

# Restart service
ssh "${serverUser}@${serverHost}" "sudo systemctl restart ${serviceName}"
