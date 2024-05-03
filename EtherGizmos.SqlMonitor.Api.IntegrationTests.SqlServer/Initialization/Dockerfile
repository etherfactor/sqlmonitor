FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=b@y4PJFfkiOIK1Ma6tXs

COPY entry.sh .
COPY init.sh .
COPY init.sql .

USER root
RUN chmod 777 entry.sh
RUN chmod 777 init.sh
USER mssql

CMD /bin/bash ./entry.sh
