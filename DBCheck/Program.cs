using MongoDB.Driver;
using MongoDB.Bson;

Console.WriteLine("Starting DB Check...");
var connectionString = "mongodb://[::1]:27017/?directConnection=true";
Console.WriteLine($"Connecting to: {connectionString}");

try
{
    var client = new MongoClient(connectionString);
    var db = client.GetDatabase("eClaimsDb");
    var collection = db.GetCollection<BsonDocument>("Probe");

    Console.WriteLine("Attempting to insert document...");
    collection.InsertOne(new BsonDocument { { "status", "alive" }, { "date", DateTime.UtcNow } });
    
    Console.WriteLine("SUCCESS: Document inserted into 'eClaimsDb.Probe'.");
}
catch (Exception ex)
{
    var msg = $"FAILURE: {ex.Message}\n{ex.StackTrace}";
    Console.WriteLine(msg);
    File.WriteAllText("error.log", msg);
}
