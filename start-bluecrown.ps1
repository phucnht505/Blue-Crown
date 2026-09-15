$root = "D:\253OU\DAN\BlueCrown.Api"

Write-Host "Starting Blue Crown..."

Start-Process -FilePath "$root\BlueCrown.AI\.venv\Scripts\python.exe" `
    -ArgumentList "-m","uvicorn","app.main:app","--host","127.0.0.1","--port","8001" `
    -WorkingDirectory "$root\BlueCrown.AI" `
    -NoNewWindow

Start-Sleep -Seconds 2

Start-Process -FilePath "dotnet" `
    -ArgumentList "run" `
    -WorkingDirectory "$root\BlueCrown.Api" `
    -NoNewWindow

Start-Sleep -Seconds 2

Start-Process -FilePath "cmd.exe" `
    -ArgumentList "/c","npm start" `
    -WorkingDirectory "$root\BlueCrown.ClientApp" `
    -NoNewWindow

Write-Host "Blue Crown started."