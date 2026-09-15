@echo off

wt -w 0 new-tab --title "BlueCrown AI" -d "D:\253OU\DAN\BlueCrown.Api\BlueCrown.AI" powershell.exe -NoExit -ExecutionPolicy Bypass -Command "& '.\.venv\Scripts\Activate.ps1'; uvicorn app.main:app --host 127.0.0.1 --port 8001"

wt -w 0 new-tab --title "BlueCrown Backend" -d "D:\253OU\DAN\BlueCrown.Api\BlueCrown.Api" powershell.exe -NoExit -Command "dotnet run"

wt -w 0 new-tab --title "BlueCrown Frontend" -d "D:\253OU\DAN\BlueCrown.Api\BlueCrown.ClientApp" powershell.exe -NoExit -Command "npm start"