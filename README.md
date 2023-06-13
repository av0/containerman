# Container management service
The REST API service for docker container management

## TODO
* Move implementation from controller to service
* Add more configuration
* Handle more scenarios

## Run
`dotnet run --project ./containerman`

or

`docker run -it $(docker build -q . -f ./containerman/Dockerfile)`
## Build
`dotnet build`
## Test
`dotnet test`