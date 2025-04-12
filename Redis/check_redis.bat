@echo off
echo ==============================
echo Checking Redis container status...
echo ==============================

docker ps --filter "name=redis-auth"

echo ==============================
echo Done.
echo ==============================
pause