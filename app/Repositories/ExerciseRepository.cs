using System.Text.RegularExpressions;
using Gymr.Models;
using MongoDB.Driver;

namespace Gymr.Repositories;

internal class ExerciseRepository : IExerciseRepository
{
    private readonly IMongoDatabase database;

    public ExerciseRepository(IMongoDatabase database)
    {
        this.database = database;
    }

    public async Task Add(Exercise exercise, CancellationToken token)
    {
        IMongoCollection<Exercise> collection = await GetCollection(token);
        try
        {
            await collection.InsertOneAsync(exercise, default, token);
        }
        catch (MongoWriteException exception) when (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new ExerciseAlreadyExistsException(exercise);
        }
    }

    public async Task<IList<Exercise>> SearchByName(string? name, CancellationToken token = default)
    {
        IMongoCollection<Exercise> collection = await GetCollection(token);
        var filterDefinition = name is { Length: > 0 } ? Builders<Exercise>.Filter.Regex(x => x.Name, name) : Builders<Exercise>.Filter.Empty;
        var sortDefinition = Builders<Exercise>.Sort.Ascending(x => x.Name);
        return await (await collection.FindAsync(filterDefinition, new FindOptions<Exercise> { Sort = sortDefinition }, cancellationToken: token)).ToListAsync();
    }

    public async Task<IDictionary<string, int>> GetGroupedByEquipment(CancellationToken token = default)
    {
        IMongoCollection<Exercise> collection = await GetCollection(token);
        return new Dictionary<string, int>(collection.AsQueryable().GroupBy(x => x.Equipment).Select(x => new KeyValuePair<string, int>(x.Key, x.Count())).ToList());
    }

    public async Task<IDictionary<string, int>> GetGroupedByMuscle(CancellationToken token = default)
    {
        IMongoCollection<Exercise> collection = await GetCollection(token);
        return new Dictionary<string, int>(collection.AsQueryable().GroupBy(x => x.Muscle).Select(x => new KeyValuePair<string, int>(x.Key, x.Count())).ToList());
    }

    private async Task<IMongoCollection<Exercise>> GetCollection(CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<Exercise>("Exercises");
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<Exercise>(Builders<Exercise>.IndexKeys.Text(exercise => exercise.Name)), cancellationToken: cancellationToken);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<Exercise>(Builders<Exercise>.IndexKeys.Combine(
            Builders<Exercise>.IndexKeys.Ascending(exercise => exercise.Name),
            Builders<Exercise>.IndexKeys.Ascending(exercise => exercise.Muscle),
            Builders<Exercise>.IndexKeys.Ascending(exercise => exercise.Equipment)), new CreateIndexOptions { Unique = true}), cancellationToken: cancellationToken);
        return collection;
    }
}