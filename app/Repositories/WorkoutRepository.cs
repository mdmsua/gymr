using Gymr.Models;
using MongoDB.Driver;

namespace Gymr.Repositories;

internal class WorkoutRepository : IWorkoutRepository
{
    private readonly IMongoDatabase database;

    public WorkoutRepository(IMongoDatabase database)
    {
        this.database = database;
    }

    public async Task<Workout> Add(Workout workout, CancellationToken token)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        await collection.InsertOneAsync(workout, default, token);
        return workout;
    }

    public async Task<Workout?> GetById(string id, CancellationToken token)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        return await (await collection.FindAsync(Builders<Workout>.Filter.Eq(x => x.Id, id), cancellationToken: token)).SingleOrDefaultAsync();
    }

    public async Task Update(Workout workout, CancellationToken token)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        await collection.ReplaceOneAsync(Builders<Workout>.Filter.Eq(x => x.Id, workout.Id), workout, cancellationToken: token);
    }

    public async Task<long> GetCount(string user, CancellationToken token)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        return await collection.CountDocumentsAsync(Builders<Workout>.Filter.Eq(x => x.User, user), cancellationToken: token);
    }

    public async Task<IReadOnlyList<Workout>> Get(string user, int? limit = null, int? skip = null, CancellationToken token = default)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        var filterDefinition = Builders<Workout>.Filter.And(
            Builders<Workout>.Filter.Ne(x => x.Finish, null),
            Builders<Workout>.Filter.Eq(x => x.User, user)
        );
        var sortDefinition = Builders<Workout>.Sort.Descending(x => x.Date);
        var findOptions = new FindOptions<Workout>() { Limit = limit, Skip = skip, Sort = sortDefinition };
        return await (await collection.FindAsync(filterDefinition, findOptions, token)).ToListAsync();
    }

    public async Task<double> GetTotalWeight(string user, CancellationToken token)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        var filterDefinition = Builders<Workout>.Filter.And(
            Builders<Workout>.Filter.Ne(x => x.Finish, null),
            Builders<Workout>.Filter.Eq(x => x.User, user)
        );
        var pipelineDefinition = new EmptyPipelineDefinition<Workout>().Match(filterDefinition).Group(x => x.Id, x => x.Sum(y => y.Exercises.Where(e => e.Weight.HasValue).Sum(z => z.Weight!.Value)));
        return (await (await collection.AggregateAsync(pipelineDefinition, cancellationToken: token)).ToListAsync()).Sum();
    }

    public async Task<TimeSpan> GetAverageDuration(string user, CancellationToken token)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        var filterDefinition = Builders<Workout>.Filter.And(
            Builders<Workout>.Filter.Ne(x => x.Finish, null),
            Builders<Workout>.Filter.Eq(x => x.User, user)
        );
        var pipelineDefinition = new EmptyPipelineDefinition<Workout>()
            .Match(filterDefinition)
            .Group(x => x.Id, x => x.Average(y => y.Finish!.Value - y.Start));
        var list = await (await collection.AggregateAsync(pipelineDefinition, cancellationToken: token)).ToListAsync();
        if (list.Count == 0)
        {
            return TimeSpan.Zero;
        }
        return TimeSpan.FromTicks((long)Math.Round(list.Average()));
    }

    public async Task<TimeSpan> GetTotalDuration(string user, CancellationToken token)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        var filterDefinition = Builders<Workout>.Filter.And(
            Builders<Workout>.Filter.Ne(x => x.Finish, null),
            Builders<Workout>.Filter.Eq(x => x.User, user)
        );
        var pipelineDefinition = new EmptyPipelineDefinition<Workout>()
            .Match(filterDefinition)
            .Group(x => x.Id, x => x.Sum(y => y.Finish!.Value - y.Start));
        var total = (await (await collection.AggregateAsync(pipelineDefinition, cancellationToken: token)).ToListAsync()).Sum();
        return TimeSpan.FromTicks(total);
    }

    public async Task<Workout?> GetCurrent(string user, CancellationToken token = default)
    {
        IMongoCollection<Workout> collection = await GetCollection(token);
        var filterDefinition = Builders<Workout>.Filter.And(
            Builders<Workout>.Filter.Eq(x => x.Finish, null),
            Builders<Workout>.Filter.Eq(x => x.User, user)
        );
        return await (await collection.FindAsync(filterDefinition)).SingleOrDefaultAsync();
    }

    private async Task<IMongoCollection<Workout>> GetCollection(CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<Workout>("Workouts");
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<Workout>(Builders<Workout>.IndexKeys.Combine(
            Builders<Workout>.IndexKeys.Descending(workout => workout.Date),
            Builders<Workout>.IndexKeys.Descending(workout => workout.User),
            Builders<Workout>.IndexKeys.Descending(workout => workout.Exercises)
        )), cancellationToken: cancellationToken);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<Workout>(Builders<Workout>.IndexKeys.Ascending(workout => workout.Finish), new CreateIndexOptions() { Sparse = true }), cancellationToken: cancellationToken);
        return collection;
    }
}