FROM mcr.microsoft.com/windows/servercore:ltsc2019

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN $cert = New-SelfSignedCertificate -DnsName "dontcare" -CertStoreLocation Cert:\LocalMachine\My; \
    winrm create winrm/config/Listener?Address=*+Transport=HTTPS ('@{Hostname=\"dontcare\"; CertificateThumbprint=\"' + $cert.Thumbprint + '\"}'); \
    winrm set winrm/config/service/Auth '@{Basic=\"true\"}'; \
    winrm set winrm/config/service '@{AllowUnencrypted=\"true\"}';

RUN net user User Password12345! /add ; \
    net localgroup Administrators User /add

CMD ["ping", "-t", "localhost"]
