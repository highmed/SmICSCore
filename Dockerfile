#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY ["AQLWebInterface/SmICSWebApp/SmICSWebApp.csproj", "AQLWebInterface/SmICSWebApp/"]
COPY ["SmICS/SmICS/SmICS.csproj", "SmICS/SmICS/"]
COPY ["AQLWebInterface/SmICSConnection.Test/SmICSConnection.Test.csproj", "AQLWebInterface/SmICSConnection.Test/"]
RUN dotnet restore "AQLWebInterface/SmICSWebApp/SmICSWebApp.csproj"

#COPY . .
#WORKDIR "AQLWebInterface/SmICSWebApp"
#RUN dotnet build "SmICSWebApp.csproj" -c Release -o /app/build
ARG repo=default_value
ENV OPENEHRDB=$repo

FROM build AS publish
COPY . ./
RUN dotnet test "AQLWebInterface/SmICSConnection.Test" --logger:trx -c Release
RUN dotnet publish "AQLWebInterface/SmICSWebApp/SmICSWebApp.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "SmICSWebApp.dll"]
