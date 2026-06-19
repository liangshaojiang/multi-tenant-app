$body = @{accountName="admin"; password="123456"} | ConvertTo-Json
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5220/api/auth/login" -Method Post -Body $body -ContentType "application/json" -UseBasicParsing
    Write-Host "Status: $($response.StatusCode)"
    Write-Host $response.Content
} catch {
    Write-Host "Status: $($_.Exception.Response.StatusCode)"
    $streamReader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
    $err = $streamReader.ReadToEnd()
    Write-Host "Body: $err"
}
