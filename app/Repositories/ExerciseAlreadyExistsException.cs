using Gymr.Models;

namespace Gymr.Repositories;

internal class ExerciseAlreadyExistsException : Exception
{
    public ExerciseAlreadyExistsException(Exercise exercise) : base(message: $"{exercise.Name} ({exercise.Muscle}) already exists")
    {
    }
}