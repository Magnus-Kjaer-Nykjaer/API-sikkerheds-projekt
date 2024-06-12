# API-sikkerheds-projekt

Velkommen til mit API sikkerheds projekt.

Dette er et projekt jeg har lavet som en demo, til min bachelor i IT-sikkerhed.

For at teste projektet af, er der to ting man kan vælge at gøre.

1. Hent projektet ned fra Github og køre det via en IDE som visual studio. Husk at kør det med HTTPS.

2. Jeg har også lavet det som en docker container, man kan hente ved at køre de fire neden stående kommandoer. Hvis man er interasseret er der et link til DockerHub reposeroriet her: https://hub.docker.com/repository/docker/skumbanan/apisikkerhedsprojekt/general 

De første to dotnet kommandoer opretter et dev TLS certifikat, som vi skal bruge til at indsætte i vores docker container, så vi har mulighed for at køre min API med HTTPS.

Kør disse i powershell:
```
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\apisikkerhedsprojekt.pfx -p testpass

dotnet dev-certs https --trust

docker pull skumbanan/apisikkerhedsprojekt

docker run --rm -it -p 8081:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORTS=8081 -e ASPNETCORE_Kestrel__Certificates__Default__Password="testpass" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/apisikkerhedsprojekt.pfx -v $env:USERPROFILE\.aspnet\https:/https/ skumbanan/apisikkerhedsprojekt:latest
```

Da det er en API, med authentication system på skal alle kald til den, indeholde en API-Key og en API-Secret i headeren samt sendes med content-typen: application/json

API-Key: Test

API-Secret: e52bbe31-bf30-4fb4-84b0-8345c5812a31

content-typen: application/json

Der er tre endpoints som kan kaldes og de er som følger:

/FejlTest

/GetRenterInformation

/TestLogging

Eksempel curl kommando man kan køre i powershell, dette kal skulle gerne retunere en 400 bad request, som man så i sit andet powershell vindue, kan se at giver en log besked om at brugerne ikke har rettighed til dataen.
```
$headers=@{}
$headers.Add("Content-Type", "application/json")
$headers.Add("Api-key", "Test")
$headers.Add("Api-secret", "e52bbe31-bf30-4fb4-84b0-8345c5812a31")
$response = Invoke-RestMethod -Uri 'https://localhost:8081/GetRenterInformation?RenterId=2' -Method GET -Headers $headers
```