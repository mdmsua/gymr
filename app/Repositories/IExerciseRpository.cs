using Gymr.Models;

namespace Gymr.Repositories;

interface IExerciseRepository
{
    Task Add(Exercise exercise, CancellationToken token = default);

    Task<IList<Exercise>> SearchByName(string? name, CancellationToken token = default);

    Task<IDictionary<string, int>> GetGroupedByMuscle(CancellationToken token = default);

    Task<IDictionary<string, int>> GetGroupedByEquipment(CancellationToken token = default);
}