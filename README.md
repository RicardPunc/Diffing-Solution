# Diffing-Solution

### How to start the API

Open Windows Powershell or CMD window in the solution directory and execute next command:
`docker-compose up`

### Testing the API

Open another Windows Powershell window in the solution directory and execute next commands:
`cd '.\Diffing API.Tests\'`
`docker build -t apptests .`
`docker run --network=host apptests`
