@echo off
REM 인자: %1 = tools 폴더 경로, %2 = .proto 파일 위치, %3 = Generated 코드 출력 위치

set TOOLS_PATH=%~1
set PROTO_PATH=%~2
set OUTPUT_PATH=%~3

REM protoc.exe와 Well-known types 경로 설정
set PROTOC_EXE=%TOOLS_PATH%\windows_x64\protoc.exe

set AUTH_OUTPUT_PATH=%OUTPUT_PATH%AuthServer\Packets\Models
set AUTH_DB_OUTPUT_PATH=%OUTPUT_PATH%AuthDBServer\Packets\Models
set WORLD_OUTPUT_PATH=%OUTPUT_PATH%WorldServer\Packets\Models
set WORLD_DB_OUTPUT_PATH=%OUTPUT_PATH%WorldDBServer\Packets\Models
set CLIENT_OUTPUT_PATH=%OUTPUT_PATH%Tools\DummyClient\Packets\Models

echo Starting protoc generation...
echo Using protoc.exe at: %PROTOC_EXE%
echo Scanning .proto files in: %PROTO_PATH%
echo Output will be generated in: %OUTPUT_PATH%
echo Well-known types path: %WELL_KNOWN_TYPES%

REM 초기화
for %%D in ("%AUTH_OUTPUT_PATH%" "%AUTH_DB_OUTPUT_PATH%" "%WORLD_OUTPUT_PATH%" "%WORLD_DB_OUTPUT_PATH%" "%CLIENT_OUTPUT_PATH%") do (
    if exist %%D (
        echo Deleting all contents in: %%D
        rmdir /s /q %%D
    )
    mkdir %%D
    echo Created output directory: %%D
)

REM 공통 함수로 프로토 처리

call :ProcessProto "ClientAuth.proto" "%CLIENT_OUTPUT_PATH%"
call :ProcessProto "ClientWorld.proto" "%CLIENT_OUTPUT_PATH%"

call :ProcessProto "WorldAuth.proto" "%AUTH_OUTPUT_PATH%"
call :ProcessProto "ClientAuth.proto" "%AUTH_OUTPUT_PATH%"
call :ProcessProto "AuthDb.proto" "%AUTH_OUTPUT_PATH%"

call :ProcessProto "AuthDb.proto" "%AUTH_DB_OUTPUT_PATH%"

call :ProcessProto "WorldAuth.proto" "%WORLD_OUTPUT_PATH%"
call :ProcessProto "ClientWorld.proto" "%WORLD_OUTPUT_PATH%"
call :ProcessProto "WorldDb.proto" "%WORLD_OUTPUT_PATH%"

call :ProcessProto "WorldDb.proto" "%WORLD_DB_OUTPUT_PATH%"

echo Protoc generation completed successfully.
exit /b 0

:ProcessProto
echo Processing file: %~1
echo Command: "%PROTOC_EXE%" --proto_path="%PROTO_PATH%" --csharp_out=%~2 %~n1.proto 2>&1
"%PROTOC_EXE%" --proto_path="%PROTO_PATH%" --csharp_out=%~2 %~n1.proto 2>&1
if %ERRORLEVEL% neq 0 (
    echo Error occurred while processing %~1
    exit /b %ERRORLEVEL%
)
exit /b 0
