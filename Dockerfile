#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

WORKDIR /src
COPY ["SmICSWebApp/SmICSWebApp.csproj", "SmICSWebApp/"]
COPY ["SmICSCoreLib/SmICSCoreLib.csproj", "SmICSCoreLib/"]
RUN dotnet restore "SmICSWebApp/SmICSWebApp.csproj"

WORKDIR /src/.
COPY . .
RUN dotnet build "SmICSWebApp/SmICSWebApp.csproj" -c Release -o /app/build

FROM build AS publish
COPY . ./
RUN dotnet publish "SmICSWebApp/SmICSWebApp.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app

COPY --from=publish /app/out .

COPY pub/jranke.asc jranke.asc

RUN apt-get update
#RUN apt-get -y install ca-certificates wget
RUN apt-get -y install gnupg2 gnupg
RUN apt-get -y install dirmngr --install-recommends
RUN apt-get -y install software-properties-common
RUN apt-get -y install apt-transport-https

#RUN gpg --keyserver keyserver.ubuntu.com --recv-key E19F5F87128899B192B1A2C2AD5F960A256A04AF
#RUN gpg -a --export 381BA480 > jranke_cran.asc
#RUN apt-key add jranke.asc
RUN apt-key adv --keyserver keyserver.ubuntu.com -http_proxy=http://proxy.mh-hannover.de:8080 --recv-key E298A3A825C0D65DFD57CBB651716619E084DAB9
RUN add-apt-repository 'deb http://cloud.r-project.org/bin/linux/debian buster-cran40/'
RUN apt-get update
RUN apt-get -y install -t buster-cran40 r-base

#RUN Rscript -e "options(repos = 'https://cran.r-project.org')"
#RUN Rscript -e "install.packages('RJSONIO')"
#RUN Rscript -e "install.packages('surveillance')"
#RUN Rscript -e "install.packages('dplyr')"
#RUN Rscript -e "install.packages('lubridate')"
#RUN Rscript -e "install.packages('RKIAlgorithm/Statistik.dod.zip')"

EXPOSE 80
EXPOSE 443
ENV SMICS_VISU_PORT=3231
ENV R_HOME=home

ENTRYPOINT ["dotnet", "SmICSWebApp.dll"]