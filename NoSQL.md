# NoSQL Prüfung 3

## Lernziele

- Konsolenanwendung erstellen (.net / C#)
- MongoDB-Treiber für C# integrieren und in der Anwendung eine Verbindung zu einer MongoDB erstellen
- Datenbanken und Collections aufrufen
- CRUD-Operationen aus der Anwendung über den MongoDB-Treiber abrufen
    - Find (One und Many)
    - Insert (One und Many)
    - Update (One und Many)
    - Delete (One und Many)
- einfache Aggregationen aus der Anwendung durchführen

## Konsolenanwendung erstellen

Vorbereiteteten MongoDB-Container starten:

```bash
docker run --name m165 -p:27017:27017 -d frm1971/m165-15
```

Neuses .NET 8 Konsolenprojekt

```bash
dotnet new console --name m165-15
```

## MongoDB-Treiber integrieren und Verbindung zu einer MongoDB

In Projekt wechseln, MongoDB-Treiber hinzufügen und Applikation starten:

```bash
cd m165-15
dotnet add package MongoDB.Driver
dotnet run
```

Eine Datei für einer Klasse um als Behälter für Datenbankobjekte zu fungieren:

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Movie
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public string Title { get; set; } = "";
    public int Year { get; set; }
    public string Summary { get; set; } = "";
    public List<string> Actors { get; set; } = new List<string>();
}
```

Program.cs MongoDB Verbinden:

```csharp
using MongoDB.Driver;

MongoClient client = new MongoClient("mongodb://localhost:27017");
```

## Datenbanken und Collections aufrufen

Datenbanken auflisten:

```csharp
var databaseNames = client.ListDatabaseNames().ToList();
Console.WriteLine("Alle Datenbanken: ");
Console.WriteLine("Databases: " + string.Join("," , databaseNames));
Console.WriteLine("----------------------------------\n");
```

Alle Collections auflisten:

```csharp
var m165Db = client.GetDatabase("M165");
var collections = m165Db.ListCollectionNames().ToList();
Console.WriteLine(" Alle Collections in M165: ");
Console.WriteLine("Collections: " + string.Join(",", collections));
```

## CRUD Operationen

find one:

```csharp
var moviesCollection = m165Db.GetCollection<Movie>("Movies");
var findFilterYear = Builders<Movie>.Filter.Eq(f => f.Year, 2012);
var findResultFirstOrDefault = moviesCollection.Find(findFilterYear).FirstOrDefault();
Console.WriteLine("c.) Erster Film im Jahr 2012 (FirstOrDefault):");
Console.WriteLine(findResultFirstOrDefault.Title);
```

Find many:

```csharp
var findFilterActor = Builders<Movie>.Filter.AnyEq(f => f.Actors, "Pierce Brosnan");
var findResultsActor = moviesCollection.Find(findFilterActor).ToList();
Console.WriteLine("d.) Filme mit Pierce Brosnan (Liste):");
foreach (var item in findResultsActor)
{
    Console.WriteLine("- " + item.Title);
}
```

Insert One:

```csharp
var myMovie = new Movie();
myMovie.Title = "The Da Vinci Code";
myMovie.Actors = new List<string>(){"Tom Hanks","Audrey Tautou"};
myMovie.Summary = "So dunkel ist der Betrug \n\t\tan der Menschheit";
myMovie.Year = 2006;

moviesCollection.InsertOne(myMovie);

var filterInsertedItem = Builders<Movie>.Filter.Eq(f => f.Id, myMovie.Id);
var insertedMovie = moviesCollection.Find(filterInsertedItem).Single();

Console.WriteLine("e.) Eingefügter Film (Insert One): ");
Console.WriteLine("Id:\t\t" + insertedMovie.Id);
Console.WriteLine("Title:\t\t" + insertedMovie.Title);
Console.WriteLine("Summary:\t" + insertedMovie.Summary);
Console.WriteLine("Actors:\t\t" + string.Join(", " , insertedMovie.Actors));
Console.WriteLine("Year:\t\t" + insertedMovie.Year);
```

Insert Many:

```csharp
var newMovies = new List<Movie>();

var myMovie1 = new Movie();
myMovie1.Title = "Ocean's Eleven";
myMovie1.Actors = new List<string>(){"George Clooney", "Brad Pitt", "Julia Roberts"};
myMovie1.Summary = "Bist du drin oder draussen?";
myMovie1.Year = 2001;

newMovies.Add(myMovie1);

var myMovie2 = new Movie();
myMovie2.Title = "Ocean's Twelve";
myMovie2.Actors = new List<string>(){"George Clooney", "Brad Pitt", "Julia Roberts", "Andy Garcia"};
myMovie2.Summary = "Die Elf sind jetzt Zwölf.";
myMovie2.Year = 2004;

newMovies.Add(myMovie2);

moviesCollection.InsertMany(newMovies);

var filterNewInsertedMovies = Builders<Movie>.Filter.In(m => m.Id, new[]{myMovie1.Id, myMovie2.Id});
var insertedMovies = moviesCollection.Find(filterNewInsertedMovies).ToList();

Console.WriteLine("f.) Eingefügte Filme (Insert Many): ");


foreach (var movie in insertedMovies)
{
    Console.WriteLine("Id:\t\t" + movie.Id);
    Console.WriteLine("Title:\t\t" + movie.Title);
    Console.WriteLine("Summary:\t" + movie.Summary);
    Console.WriteLine("Actors:\t\t" + string.Join(", " , movie.Actors));
    Console.WriteLine("Year:\t\t" + movie.Year);
    Console.WriteLine("");
}
```

Update One / Many:

```csharp
// Build Filter
var updateFilter = Builders<Movie>.Filter.Eq(f => f.Title, "Skyfall - 007");
// Update One
var update = Builders<Movie>.Update
    .Set(d => d.Title, "Skyfall");

// Update Many
var result = moviesCollection.UpdateMany(updateFilter, update);
Console.WriteLine("Update 'Skyfall - 007' -> 'Skyfall' (Anzahl): " + result.ModifiedCount);
```

Delete One / Many:

```csharp
// Filter
var deleteFilter = Builders<Movie>.Filter.Lte(f => f.Year, 1995);
// Delete Many
var deleteResult = moviesCollection.DeleteMany(deleteFilter);
Console.WriteLine("h.) Alle Filme von 1995 und früher löschen:");
Console.WriteLine("Delete Year <= 1995 (Anzahl): " + deleteResult.DeletedCount);
```

## Einfache Aggregationen

Beispiel Aggregate (Liste Sie ab 2000 die Anzahl Filme pro Jahr auf.):

```csharp
var aggregateResult = moviesCollection.Aggregate()
    .Match(m => m.Year >= 2000)
    .Group( m => m.Year, g => new{ Jahr = g.Key, Anzahl=g.Count()})
    .SortBy(m => m.Jahr)
    .ToList();

Console.WriteLine("i.) Anzahl Filme pro Jahr ab 2000:");
foreach (var item in aggregateResult)
{
    Console.WriteLine("- " + item.Jahr + ": " + item.Anzahl);
}
```
https://learn.mongodb.com/learn/course/mongodb-aggregation-with-c-sharp/lesson-2-using-mongodb-aggregation-stages-with-c-match-and-group/learn?page=2