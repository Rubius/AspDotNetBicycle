SETLOCAL
SET SwaggerDirectory=bin/Debug/net6.0
SET TestDirectory=../tests/IntegrationTests
SET FrontDirectory=bin/Debug/net6.0
SET SwaggerURL=http://localhost:5000/api/swagger/v1/swagger.json
dotnet build SwaggerGen.csproj ^
	&& echo start copying files... ^
	&& start /wait /min %SwaggerDirectory%/SwaggerGen.exe %TestDirectory% %FrontDirectory% %SwaggerURL% ^