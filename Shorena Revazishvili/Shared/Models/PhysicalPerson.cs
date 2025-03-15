using System.Text.Json.Serialization;

namespace Shared.Models
{
    public enum Gender
    {
       Female,
       Male
    }
    public class PhysicalPerson
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public Gender Gender { get; set; }
        public required string PersonalNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public virtual City? City { get; set; }

        [JsonIgnore]
        public virtual List<PhoneNumber>? PhoneNumbers { get; set; }
        [JsonIgnore]
        public virtual List<RelatedPerson>? RelatedPersons { get; set; }

        public string? ImagePath { get; set; }
    }
}
