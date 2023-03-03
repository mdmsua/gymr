using System.ComponentModel.DataAnnotations;
using Gymr.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gymr.Models;

public class Exercise
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonRequired]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
    public string Name { get; set; } = null!;

    [BsonRequired]
    [Required(AllowEmptyStrings = false)]
    public string Muscle { get; set; } = Constants.Muscles[0];

    [BsonRequired]
    [Required(AllowEmptyStrings = true)]
    public string Equipment { get; set; } = string.Empty;

    public override string ToString() => $"{Name} ({Muscle}, {(Equipment == string.Empty ? "Body" : Equipment)})";
}