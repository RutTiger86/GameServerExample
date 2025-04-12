@echo off
echo ==============================
echo Starting Redis via Docker...
echo ==============================

REM 현재 디렉토리를 기준으로 docker-compose 실행
docker-compose -f redis-auth.yml up -d

echo ==============================
echo Redis container is running!
echo ==============================
pause
