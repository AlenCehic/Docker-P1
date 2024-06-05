using MongoDB.Driver;

MongoClient client = new MongoClient("mongodb://localhost:27017");

// Aufgabe a: Alle vorhandenen Datenbanken auflisten
var databaseNames = client.ListDatabaseNames().ToList();
Console.WriteLine("a.) Alle Datenbanken: ");
Console.WriteLine("Databases: " + string.Join("," , databaseNames));
Console.WriteLine("----------------------------------------------\n");

// Aufgabe b: Alle Collections in M165 auflisten
var m165Db = client.GetDatabase("M165");
var collections = m165Db.ListCollectionNames().ToList();
Console.WriteLine("b.) Alle Collections in M165: ");
Console.WriteLine("Collections: " + string.Join(",", collections));
Console.WriteLine("----------------------------------------------\n");


// Aufgabe c: Find erster Film aus Jahr 2012 (FirstOrDefault)
// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/findOne
var moviesCollection = m165Db.GetCollection<Movie>("Movies");
var findFilterYear = Builders<Movie>.Filter.Eq(f => f.Year, 2012);
var findResultFirstOrDefault = moviesCollection.Find(findFilterYear).FirstOrDefault();
Console.WriteLine("c.) Erster Film im Jahr 2012 (FirstOrDefault):");
Console.WriteLine(findResultFirstOrDefault.Title);
Console.WriteLine("----------------------------------------------\n");

// Aufgabe D: Find alle Filme mit Pierce Brosnan
// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/findMany
var findFilterActor = Builders<Movie>.Filter.AnyEq(f => f.Actors, "Pierce Brosnan");
var findResultsActor = moviesCollection.Find(findFilterActor).ToList();
Console.WriteLine("d.) Filme mit Pierce Brosnan (Liste):");
foreach (var item in findResultsActor)
{
    Console.WriteLine("- " + item.Title);
}
Console.WriteLine("----------------------------------------------\n");


// Insert The Da Vinci Code
// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/insertOne

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
Console.WriteLine("----------------------------------------------\n");


// Insert Many
// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/insertMany

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
Console.WriteLine("----------------------------------------------\n");

// Update One / Update Many
// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/updateOne
// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/updateMany
Console.WriteLine("g.) Skyfall -007 zu Skyfall umbenennen:");

var updateFilter = Builders<Movie>.Filter.Eq(f => f.Title, "Skyfall - 007");
var update = Builders<Movie>.Update
    .Set(d => d.Title, "Skyfall");

var result = moviesCollection.UpdateMany(updateFilter, update);
Console.WriteLine("Update 'Skyfall - 007' -> 'Skyfall' (Anzahl): " + result.ModifiedCount); 
Console.WriteLine("----------------------------------------------\n");

// Delete one / Delete Many
// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/deleteOne
// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/deleteMany
var deleteFilter = Builders<Movie>.Filter.Lte(f => f.Year, 1995);
var deleteResult = moviesCollection.DeleteMany(deleteFilter);
Console.WriteLine("h.) Alle Filme von 1995 und früher löschen:");
Console.WriteLine("Delete Year <= 1995 (Anzahl): " + deleteResult.DeletedCount); 
Console.WriteLine("----------------------------------------------\n");

//Aggregate
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
Console.WriteLine("----------------------------------------------\n");