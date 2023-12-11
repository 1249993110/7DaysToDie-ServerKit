@echo off
echo "try restart server"
if %1.==. (goto error) ELSE (goto restart)
:restart
taskkill /f /pid %1 >nul
timeout /t 3 /nobreak >nul
start "" %2 >nul
echo "restart complete"
goto exit
:error
echo "Oops... something went wrong!"
:exit