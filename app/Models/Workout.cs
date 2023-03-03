using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gymr.Models;

public class Workout
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    [Required]
    [BsonRequired]
    public string User { get; set; } = null!;

    [Required]
    [BsonRequired]
    public long Date { get; set; }

    [Required]
    [BsonRequired]
    public IList<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();

    [Required]
    [BsonRequired]
    public long Start { get; set; }

    [Required]
    [BsonRequired]
    public long? Finish { get; set; }

    [BsonIgnore]
    public DateOnly DateAsDateOnly => DateOnly.FromDateTime(new(Date)); 
    
    [BsonIgnore]
    public TimeOnly StartAsTimeOnly => TimeOnly.FromDateTime(new(Start)); 
    
    [BsonIgnore]
    public TimeOnly? FinishAsTimeOnly => IsCurrent ? null : TimeOnly.FromDateTime(new(Finish!.Value)); 

    [BsonIgnore]
    public bool IsCurrent => Finish is null;
    
    [BsonIgnore]
    public TimeSpan? Duration => IsCurrent ? null : TimeSpan.FromTicks(Finish!.Value - Start);
    
    [BsonIgnore]
    public string? DurationAsHumanReadable => Duration?.ToString(@"h\:mm\:ss");
}