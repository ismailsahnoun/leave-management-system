FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["leaveManagement.Presentation/leaveManagement.Presentation.csproj", "leaveManagement.Presentation/"]
COPY ["leaveManagement.Application/leaveManagement.Application.csproj", "leaveManagement.Application/"]
COPY ["leaveManagement.Domain/leaveManagement.Domain.csproj", "leaveManagement.Domain/"]
COPY ["leaveManagement.Infrastructure/leaveManagement.Infrastructure.csproj", "leaveManagement.Infrastructure/"]

RUN dotnet restore "leaveManagement.Presentation/leaveManagement.Presentation.csproj"

# Copy all source code
COPY . .
WORKDIR "/src/leaveManagement.Presentation"

# Build the application
RUN dotnet build "leaveManagement.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "leaveManagement.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "leaveManagement.Presentation.dll"]
