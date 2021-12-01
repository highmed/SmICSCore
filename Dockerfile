#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY RKIAlgorithm/Statistik.dod.zip RKIAlgorithm/Statistik.dod.zip
COPY RKIAlgorithm/Statistik.dod.tar.gz RKIAlgorithm/Statistik.dod.tar.gz
COPY SmICSWebApp/Resources/RRuntime/R_Script_00010.R Resources/RRuntime/
COPY SmICSWebApp/Resources/ Resources/

RUN apt-get update 
Run apt-get -y upgrade
RUN apt-get -y install ca-certificates wget
RUN apt-get -y install gnupg2 gnupg1
RUN apt-get -y install dirmngr --install-recommends
RUN apt-get -y install software-properties-common
RUN apt-get -y install apt-transport-https

RUN apt-key adv --keyserver keyserver.ubuntu.com --recv-key '95C0FAF38DB3CCAD0C080A7BDC78B2DDEABC47B7'

RUN add-apt-repository 'deb http://cloud.r-project.org/bin/linux/debian buster-cran40/'
RUN apt-get update
RUN apt-get -y install -t buster-cran40 r-base

RUN Rscript -e "options(repos = 'https://cran.r-project.org')"
RUN Rscript -e "install.packages('RJSONIO')"
RUN Rscript -e "install.packages('surveillance')"
RUN Rscript -e "install.packages('dplyr')"
RUN Rscript -e "install.packages('lubridate')"
RUN Rscript -e "install.packages('plotrix')"
RUN Rscript -e "install.packages('/app/RKIAlgorithm/Statistik.dod.tar.gz', repos=NULL, type='source')"

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

COPY Certificates/ /usr/local/share/ca-certificates/
RUN find /usr/local/share/ca-certificates -type f -exec chmod 644 {} \;
RUN update-ca-certificates



EXPOSE 80
EXPOSE 443
ENV SMICS_VISU_PORT=3231

ENTRYPOINT ["dotnet", "SmICSWebApp.dll"]
