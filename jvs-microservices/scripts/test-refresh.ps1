$body = @{accountName="admin"; password="123456"} | ConvertTo-Json
$login = Invoke-RestMethod -Uri "http://localhost:5220/api/auth/login" -Method Post -Body $body -ContentType "application/json"
Write-Host "Login: company=$($login.data.companyName), refresh exists: $($login.data.refreshToken.Length -gt 0)"
$parts = $login.data.accessToken.Split('.')
$pad = 4 - ($parts[0].Length % 4)
if ($pad -ne 4) { $parts[0] += '=' * $pad }
$hdr = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($parts[0]))
Write-Host "JWT: $hdr"
Write-Host "Groups: $($login.data.companyGroups.Count)"
foreach ($g in $login.data.companyGroups) {
    Write-Host "  $($g.tenantName) ($($g.deploymentType)): $($g.companies[0].companyName)"
}
$refreshBody = @{refreshToken=$login.data.refreshToken} | ConvertTo-Json
$refresh = Invoke-RestMethod -Uri "http://localhost:5220/api/auth/refresh" -Method Post - Body $refreshBody -ContentType "application/json"
Write-Host "Refresh OK: new company=$($refresh.data.companyName)"
