@echo off
REM 인자: %1 = tools 폴더 경로, %2 = .proto 파일 위치, %3 = Generated 코드 출력 위치

set TOOLS_PATH=%~1
set IMPORT_PATH=%~2
set PROTO_PATH=%~3
set OUTPUT_PATH=%~4

REM protoc.exe와 Well-known types 경로 설정
set PROTOC_EXE=%TOOLS_PATH%\windows_x64\protoc.exe
set WELL_KNOWN_TYPES=%IMPORT_PATH%

echo Starting protoc generation...
echo Using protoc.exe at: %PROTOC_EXE%
echo Scanning .proto files in: %PROTO_PATH%
echo Output will be generated in: %OUTPUT_PATH%
echo Well-known types path: %WELL_KNOWN_TYPES%

REM OUTPUT_PATH 초기화 (폴더가 존재하면 삭제)
if exist "%OUTPUT_PATH%" (
    echo Deleting all contents in: %OUTPUT_PATH%
    rmdir /s /q "%OUTPUT_PATH%"
)

REM Generated 폴더가 없으면 생성
if not exist "%OUTPUT_PATH%" (
    mkdir "%OUTPUT_PATH%"
    echo Created output directory: %OUTPUT_PATH%
)

REM protoc 명령어 실행 로그 및 에러 출력
for %%f in ("%PROTO_PATH%\*.proto") do (
    echo Processing file: %%f
    echo Command: "%PROTOC_EXE%" --proto_path="%PROTO_PATH%" --proto_path="%WELL_KNOWN_TYPES%" --csharp_out="%OUTPUT_PATH%" "%%f"

    "%PROTOC_EXE%" --proto_path="%PROTO_PATH%" --proto_path="%WELL_KNOWN_TYPES%" --csharp_out="%OUTPUT_PATH%" "%%f" 2>&1

    if %ERRORLEVEL% neq 0 (
        echo Error occurred while processing %%f
        echo Command: "%PROTOC_EXE%" --proto_path="%PROTO_PATH%" --proto_path="%WELL_KNOWN_TYPES%" --csharp_out="%OUTPUT_PATH%" "%%f"
        exit /b %ERRORLEVEL%
    )
)

echo Protoc generation completed successfully.
exit /b 0
