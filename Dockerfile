#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src
COPY ["./Mandible/Mandible.csproj", "./"]
RUN dotnet restore "Mandible.csproj"
COPY ./Mandible .
WORKDIR "/src/"
RUN dotnet build "Mandible.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mandible.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
RUN mkdir LoneAnt
RUN mkdir LoneAnt/Submissions
RUN mkdir LoneAnt/Backups
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeChallengeInc.Mandible.dll"]
