#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

WORKDIR /src
COPY ["SmICSWebApp/SmICSWebApp.csproj", "SmICSWebApp/"]
COPY ["SmICSCoreLib/SmICSCoreLib.csproj", "SmICSCoreLib/"]
#COPY ["SmICSConnection.Test/SmICSConnection.Test.csproj", "SmICSConnection.Test/"]
#COPY ["SmICSDataGenerator.Test/SmICSDataGenerator.Test.csproj", "SmICSDataGenerator.Test/"]
#COPY ["SmICS.Tests/SmICSFactory.Tests.csproj", "SmICS.Tests/"]
#COPY ["TestData/", "TestData/"]
RUN dotnet restore "SmICSWebApp/SmICSWebApp.csproj"

WORKDIR /src/.
COPY . .
RUN dotnet build "SmICSWebApp/SmICSWebApp.csproj" -c Release -o /app/build

#ENV OPENEHR_DB=$repo
#ENV OPENEHR_USER=$user
#ENV OPENEHR_PASSWD=$passwd
#ENV AUTH=GENERIC
#ENV AUTHORITY=authority
#ENV CLIENT_ID=clientID
#ENV CLIENT_SECRET=null

FROM build AS publish
COPY . ./
RUN dotnet publish "SmICSWebApp/SmICSWebApp.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
COPY Certificates/ /usr/local/share/ca-certificates/
RUN find /usr/local/share/ca-certificates -type f -exec chmod 644 {} \;
RUN update-ca-certificates

WORKDIR /app
COPY --from=publish /app/out .
EXPOSE 80
EXPOSE 443
ENV SMICS_VISU_PORT=8443
ENV ASPNETCORE_URLS=https://+;http://+
ENV ASPNETCORE_HTTPS_PORT=443
ENTRYPOINT ["dotnet", "SmICSWebApp.dll"]
