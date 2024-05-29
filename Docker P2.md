# Docker P2 Zusammenfassung
## Lernziele
- **Sie können mit YAML-Dateien für docker-compose korrekt umgehen**
- **Sie kennen die Syntax von docker-compose.yml und können die Schlüsselwörter korrekt anwenden**
- **Sie können das Kommando docker compose up/down mit den Schaltern -d und --build korrekt anwenden**
- **Sie können für Sie unbekannte Images anhand verfügbarer Dokumentationen korrekte docker compose Kompositionen aufbauen**
- **Sie können Dockerfiles für Multistage Builds im Zusammenhang mit dotnet aufbauen**
- **Sie können Dockerfiles im Zusammenspiel mit** **docker compose korrekt verwenden**
- **Sie können mit dotnet Web- und Konsolen-Anwendungen korrekt umgehen**
## YAML-Dateien (Syntax)
- `#` for comments
- For Text use `'` or `"`
- A list has in it's elements `-` + space
```yml
# A list of tasty fruits
- Apple
- Orange
- Strawberry
- Mango
```
- Key-Value-Pairs are created with `:` + space
```yml
# A key value pair
name: Alen Cehic
```
- A value can have Rekursive Objects
```yml
# Employee Records
- martin:
    name: Martin D'vloper
    job: Developer
    skills:
      - python
      - perl
      - pascal
- tabitha:
    name: Tabitha Bitumen
    job: Developer
    skills:
        - lisp
        - fortran
        - erlang
```
⚠️ **Important:** For tabs yml uses 2 whitespaces, tabstops will not work!!!

## docker compose
- **version:** "3.8" or "3.9"
- **services:** for definition of containers. Servicename (Keys) can be openly selected
- **networks:** for definition of your own networks
- **volumes:** declaring named Volumes
- **secrets:** declaring Password-Files
```yml
# Grundsätzlicher Aufbau von docker-compose.yml
version: "3.8"
services:
    dienstname1:
        schlüsselwort1: einstellung1
        schlüsselwort2: einstellung2
        ...
    dienstname2:
        schlüsselwort1: einstellung1
        schlüsselwort2: einstellung2
        ...
volumes:
networks:
secrets:
```

Die wichtigsten Schlüsselwörter unterhalb der Dienstnamen sind:
- **image:** Basisimage eines Service oder
- **build:** Ein Verzeichnis mit Dockerfile, der Service wird dann nicht aus einem Image gestartet sondern aus einem Dockerfile
- **container_name:** Der Name für den resultierenden Container
- **restart:** always, on-failure oder unless-stopped
- **environment:** Liste mit Umgebungsvariablen für einen Container
- **volumes:** Liste der gemounteten Volumes
- **ports:** Liste der Portweiterleitungen
- **expose:** Ports für die Kommunikation zwischen Containern
- **networks:** Verweis auf ein im Top-Level-Schlüsselwort networks definiertes Netzwerk
- **secrets:** Verweis auf die im Top-Level-Schlüsselwort secrets definierte Passwortdatei

Example docker-compose.yml:
```yml
version: '3.8'

services:
  web:
    image: myapp:latest
    build:
      context: ./web
      dockerfile: Dockerfile
    container_name: web_container
    restart: always
    ports:
      - "8080:80"
    expose:
      - "80"
    networks:
      - my_network
    volumes:
      - web_data:/var/www/html
    depends_on:
      - db
    environment:
      - DATABASE_URL=mysql://user:password@db/mydatabase
      - SECRET_KEY=supersecret
    secrets:
      - my_secret

  db:
    image: mysql:5.7
    container_name: db_container
    restart: always
    ports:
      - "3306:3306"
    expose:
      - "3306"
    networks:
      - my_network
    volumes:
      - db_data:/var/lib/mysql
    environment:
      - MYSQL_ROOT_PASSWORD=rootpassword
      - MYSQL_DATABASE=mydatabase
      - MYSQL_USER=user
      - MYSQL_PASSWORD=password
    secrets:
      - db_root_password

networks:
  my_network:
    driver: bridge

volumes:
  web_data:
  db_data:

secrets:
  my_secret:
    file: ./secrets/my_secret.txt
  db_root_password:
    file: ./secrets/db_root_password.txt

```

- In verzeichnis der docker-compose.yml wechseln
```bash
cd myapp
```
- applikation mit folgendem starten:
```bash
docker compose up -d
```
(-d daemon, bewirkt dass die Applikation im hintergrund gestartet wird)
- wieder beenden:
```bash
docker compose down
```
- build parameter:
```bash
docker compose up -d --build
```
build führt dazu, dass immer ein neues image erstellt wird (falls eins vorhanden ist, ohne build wird sonst nur beim ersten mal ein neues build erstellt)
## Dockerfile
```Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0
LABEL description="Minimal counter app"
LABEL organisation="GBS St.Gallen"
LABEL author="Silvio Dall'Acqua"
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o .
ENTRYPOINT ["dotnet", "app.dll"]
```

## Dotnet
Installiere zuerst dotnet auf ubuntu:

```bash
sudo apt update
sudo apt install -y dotnet-sdk-8.0
```

mist folgendem befehl abfragen, ob die korrekte version installiert wurde:

```bash
dotnet --list-sdks
```
folgender output muss kommen:

```
8.0.103 [/usr/lib/dotnet/sdk]
```
Anwendung erstellen:

```bash
mkdir myconsoleApp
cd myconsoleApp
dotnet new console -o app
code .
```

Programm zum laufen bringen:
```bash
dotnet run
```

Image builden mit:

```bash
docker build -t counterimage .
```

## Multistage Build Dockerfile:

```Dockerfile
# 1. Build compile image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# 2. Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 
LABEL description="Minimal counter app"
LABEL organisation="GBS St.Gallen"
LABEL author="Silvio Dall'Acqua"
WORKDIR /app
COPY --from=build-env /build/out .
ENTRYPOINT ["dotnet", "app.dll"]
```

Mit docker compose kann mehr automatisiert werden. Wichtig docker compose muss sich in einem verzeichhnis höher befinden

## .NET APS.NET Core Webanwendung

### Applikation erstellen

Zuerst initiieren

```bash
dotnet new webapi
```

dannach projekt erstellen:

```bash
mkdir min-api-with-mongo
dotnet new web --name WebApi --framework net8.0
code .
```

In verzeichnis wechseln und dotnet run.
`launchSettings.json` unter `Properties` Anpassen, nur schema und profiles nötig

### Beispiel aus Übung

C-Sharp code sieht bei initialisierung folgendermassen aus:

```csharp
internal class Program
{
    private static async Task Main(string[] args)
    {
        var counter = 0;
        while (true)
        {
            Console.WriteLine($"Counter: {++counter}");
            await Task.Delay(TimeSpan.FromMilliseconds(1_000));
        }
    }
}
```
Bei WebAPi sieht der Standardcode folgendermassen aus:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run()
```

Endresultat des Project.cs file:

```csharp
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var movieDatabaseConfigSection = builder.Configuration.GetSection("DatabaseSettings");
builder.Services.Configure<DatabaseSettings>(movieDatabaseConfigSection);

var app = builder.Build();

app.MapGet("/", () => "Minimal API nach Arbeitsauftrag 2");

// docker run ..name mongodb -d -p 27017:27017 -v data/datadb -e MONGO_INITDB_ROOT_USERNAME=gbs -e MONGO_INITDB_ROOT_PASSWORD=geheim mongo
app.MapGet("/check", (Microsoft.Extensions.Options.IOptions<DatabaseSettings> options) => {
    try 
    {
        var mongoDBConnectionString = options.Value.ConnectionString;
        MongoClient mongoClient = new MongoClient(mongoDBConnectionString);
        var databaseNames = mongoClient.ListDatabaseNames().ToList();
        return "Zugriff auf MongoDB ok. Vorhandene DBs: " + string.Join(",", databaseNames);
    }
    catch(System.Exception e)
    {
        return "Zugriff auf MongoDB funktioniert nicht: " + e.Message;
    }
});

app.Run();
```
Dockerfile:

```Dockerfile
# 1. Buil dcompile image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# 2. Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
LABEL description="Minimal Web App"
LABEL organisation="GBS St. Gallen"
LABEL author="Alen Cehic"
WORKDIR /app
COPY --from=build-env /build/out .
ENV ASPNETCORE_URLS=http://*:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "WebApi.dll"]
```

docker-compose.yml file

```yaml
version: "3.9"
services:
  webapi:
    build: ./WebApi
    restart: always
    depends_on:
      - mongodb
    environment:
      DatabaseSettings__ConnectionString: "mongodb://gbs:geheim@mongodb:27017"
    ports:
      - 5001:5001
    
  mongodb:
    image: mongo
    restart: always
    environment:
      MONGO_INITDB_ROOT_USERNAME: gbs
      MONGO_INITDB_ROOT_PASSWORD: geheim
    volumes:
      - mongoData:/data/db
volumes:
  mongoData:
```

Properties/launchsettings.json

```json
﻿{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Zusätzlich eine Klasse für Database Connection String

```csharp
public class DatabaseSettings
{
    public string ConnectionString { get; set; } = "";
}
```

Um MongoDB.Driver zu installiern folgenden befehl verwenden:

```bash
dotnet add package MongoDB.Driver
```