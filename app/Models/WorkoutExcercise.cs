using System.ComponentModel.DataAnnotations;
using Gymr.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gymr.Models;

public class WorkoutExercise : Exercise
{
    [Range(1.0, 100.0, ErrorMessage = "[1.0 ... 100.0]")]
    [BsonRequired]
    public double? Weight { get; set; }

    [Required]
    [BsonRequired]
    [NotEmpty]
    public List<int> Sets { get; set; } = new();

    [Required]
    [BsonRequired]
    [BsonRepresentation(BsonType.Int32)]
    public Feel Feel { get; set; }
}

public enum Feel : int
{
    Good = 1,
    Normal = 0,
    Bad = -1,
}