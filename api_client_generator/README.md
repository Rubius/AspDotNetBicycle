# [SwaggerGen] Генератор API

## Входные параметры
- сваггеровский json
- выходной .cs-файл с C#-клиентом

## Как обычно используем
SwaggerGen.exe "C:\projects\gpdm-backend\utils\SwaggerGen\bin\Debug\netcoreapp3.1\swagger.json" "C:\projects\gpdm-backend\tests\Endpoint.IntegrationTests\ApiClient.g.cs"
Либо запустить UpdateApiClient.bat, который автоматически обновит необходимые файлы как на бэкенде, так и на фронтенде

## Где потом искать typescript-файл с API
C:\projects\gpdm-backend\utils\SwaggerGen\bin\Debug\netcoreapp3.1\ApiClient.ts