FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ARG PROJECT_PATH=ECommerce.ApiGateway.Solution/ApiGateway.Presentation/ApiGateway.Presentation.csproj

COPY . .
RUN dotnet restore "$PROJECT_PATH"
RUN dotnet publish "$PROJECT_PATH" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app

ARG APP_DLL=ApiGateway.Presentation.dll
ENV APP_DLL=$APP_DLL

COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet \"$APP_DLL\""]
