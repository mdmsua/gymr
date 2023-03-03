using Gymr.Models;

namespace Gymr.Repositories;

interface IWorkoutRepository
{
    Task<Workout> Add(Workout workout, CancellationToken token = default);
    
    Task<Workout?> GetById(string id, CancellationToken token = default);

    Task<IReadOnlyList<Workout>> Get(string user, int? limit = null, int? skip = null, CancellationToken token = default);

    Task Update(Workout workout, CancellationToken token = default);

    Task<long> GetCount(string user, CancellationToken token = default);

    Task<double> GetTotalWeight(string user, CancellationToken token = default);

    Task<TimeSpan> GetAverageDuration(string user, CancellationToken token = default);

    Task<TimeSpan> GetTotalDuration(string user, CancellationToken token = default);

    Task<Workout?> GetCurrent(string user, CancellationToken token = default);
}