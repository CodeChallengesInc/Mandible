#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["SubmissionApi.csproj", "./"]
RUN dotnet restore "SubmissionApi.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "SubmissionApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubmissionApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
RUN mkdir LoneAnt
RUN mkdir LoneAnt/Submissions
RUN mkdir LoneAnt/Backups
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeChallengeInc.SubmissionApi.dll"]
