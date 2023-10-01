FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TimeReport/TimeReport.csproj", "TimeReport/"]
RUN dotnet restore "TimeReport/TimeReport.csproj"
COPY . .
WORKDIR "/src/TimeReport"
RUN dotnet build "TimeReport.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeReport.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeReport.dll"]
