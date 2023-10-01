FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/TimeReport.Generator/TimeReport.Generator.csproj", "src/TimeReport.Generator/"]
RUN dotnet restore "src/TimeReport.Generator/TimeReport.Generator.csproj"
COPY . .
WORKDIR "/src/src/TimeReport.Generator"
RUN dotnet build "TimeReport.Generator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeReport.Generator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeReport.Generator.dll"]
