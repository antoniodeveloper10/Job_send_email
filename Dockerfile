#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Job_send_emails.csproj", ""]
RUN dotnet restore "./Job_send_emails.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Job_send_emails.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Job_send_emails.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Job_send_emails.dll"]