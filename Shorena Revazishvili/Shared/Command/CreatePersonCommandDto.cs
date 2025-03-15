using Microsoft.AspNetCore.Http;
using Shared.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;



namespace Shared.Command
{
    public class CreatePersonCommandDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^([ა-ჰ]+|[a-zA-Z]+)$", ErrorMessage = "The field FirstName must contain only letters.")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^([ა-ჰ]+|[a-zA-Z]+)$", ErrorMessage = "The field LastName must contain only letters.")]
        public required string LastName { get; set; }

        public Gender Gender { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "The field PersonalNumber must be exactly 11 digits.")]
        public required string PersonalNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomAgeValidation(18, ErrorMessage = "You must be 18 years old or more.")]
        public DateTime DateOfBirth { get; set; }
        public CreateCityDto? City { get; set; }
        public List<CreatePhoneNumberDto?> PhoneNumbers { get; set; } = new();
        public IFormFile? ProfileImage { get; set; }
    }
    public class CreateCityDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }

    public class CreatePhoneNumberDto
    {
        public PhoneType? Type { get; set; } = PhoneType.Mobile;
        public string? Number { get; set; }
    }


    public class PersonWithRelationsDto
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, int> RelatedInfo { get; set; } = new();
    }

    public class QuickSearchDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        [DefaultValue(1)]
        public int Page { get; set; } = 1;
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;
    }

    public class DetailedSearchDto 
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^([ა-ჰ]+|[a-zA-Z]+)$")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^([ა-ჰ]+|[a-zA-Z]+)$")]
        public string? LastName { get; set; }

        public Gender? Gender { get; set; }

        [StringLength(11, MinimumLength = 11)]
        [RegularExpression(@"^\d{11}$")]
        public string? PersonalNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? City { get; set; }

        public string? PhoneNumber { get; set; }

        public PhoneType? PhoneType { get; set; }

        [DefaultValue(1)]
        public int Page { get; init; } = 1;
        [DefaultValue(10)]
        public int PageSize { get; init; } = 10;
    }

    public class GetPersonByIdDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public Gender Gender { get; set; }
        public required string PersonalNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public City? City { get; set; }
        public string? ImagePath { get; set; }
        public List<PhoneNumberDto>? PhoneNumbers { get; set; }
        public List<PersonWithRelationsDto>? RelatedPersons { get; set; }
    }

    public class PhoneNumberDto
    {
        public PhoneType PhoneNumberType { get; set; }
        public string? PhoneNumber { get; set; }

    }
}