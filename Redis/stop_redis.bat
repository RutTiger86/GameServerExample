@echo off
echo ==============================
echo Stopping Redis container...
echo ==============================

docker-compose -f redis-auth.yml down

echo ==============================
echo Redis container stopped!
echo ==============================
pause