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

FROM rocker/r-ver:latest as rbuild
WORKDIR /app/out
COPY RKIAlgorithm/Statistik.dod.zip RKIAlgorithm/
RUN R -e "options(repos = 'https://cran.r-project.org')"
RUN R -e ".libPaths('/app/out/R')"
RUN R -e "install.packages('RJSONIO')"
RUN R -e "install.packages('surveillance')"
RUN R -e "install.packages('dplyr')"
RUN R -e "install.packages('lubridate')"
RUN R -e "install.packages('RKIAlgorithm/Statistik.dod.zip')"

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app

COPY --from=publish /app/out .

EXPOSE 80
EXPOSE 443
ENV SMICS_VISU_PORT=3231
ENV R_HOME=home

ENTRYPOINT ["dotnet", "SmICSWebApp.dll"]