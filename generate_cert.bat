@echo off
set CERT_NAME=testcert
set CERT_PASSWORD=1234
set CERT_PATH=%~dp0Certs

REM certs 폴더 없으면 생성
if not exist "%CERT_PATH%" (
    mkdir "%CERT_PATH%"
)

powershell -Command ^
 "$cert = New-SelfSignedCertificate -DnsName '127.0.0.1' -CertStoreLocation 'cert:\LocalMachine\My' -NotAfter (Get-Date).AddYears(10);" ^
 "$password = ConvertTo-SecureString -String '%CERT_PASSWORD%' -Force -AsPlainText;" ^
 "Export-PfxCertificate -Cert $cert -FilePath '%CERT_PATH%\\%CERT_NAME%.pfx' -Password $password;"

echo ---------------------------------------
echo 인증서 생성 완료: %CERT_PATH%\%CERT_NAME%.pfx
echo 비밀번호: %CERT_PASSWORD%
echo.
pause
