using DAL;
using DAL.Abstractions;
using Shared.Command;
using Shared.Models;

namespace BLL
{
    public class PersonsService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhysicalPersonRepository _repository;

        public PersonsService(IPhysicalPersonRepository repo, IUnitOfWork unitOfWork)
        {
            _repository = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddOrUpdateAsync(CreatePersonCommandDto personDto)
        {
            var entity = await _repository.GetByIdAsync(personDto.PersonalNumber);

            if (entity == null)
            {
                entity = new PhysicalPerson
                {
                    FirstName = personDto.FirstName,
                    LastName = personDto.LastName,
                    Gender = personDto.Gender,
                    PersonalNumber = personDto.PersonalNumber,
                    DateOfBirth = personDto.DateOfBirth,
                    PhoneNumbers = personDto.PhoneNumbers != null && personDto.PhoneNumbers.Any() ?
                    personDto.PhoneNumbers
                    .Where(p => p != null)
                    .Select(p => new PhoneNumber
                    {
                        Type = p!.Type ?? PhoneType.Mobile,
                        Number = !string.IsNullOrEmpty(p.Number) ? p.Number : "Unknown"
                    }).ToList() : new List<PhoneNumber>()
                };

                if (personDto.City is not null)
                {
                    if (personDto.City.Id == 0)
                    {
                        if (string.IsNullOrEmpty(personDto.City.Name))
                        {
                            throw new ArgumentException("City name is required if 'Other' is selected.");
                        }
                        entity.City = new City { Name = personDto.City.Name };
                    }
                    else if (personDto.City.Id.HasValue)
                    {
                        var city = await _repository.GetCityByIdAsync(personDto.City.Id.Value);
                        entity.City = city;
                    }
                }

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync(); // შევინახოთ ID-ის მისაღებად
            }

            // Update
            if (personDto.PhoneNumbers?.Any() == true)
            {
                entity.PhoneNumbers ??= new List<PhoneNumber>();

                var newPhoneNumbers = personDto.PhoneNumbers
                    .Where(p => p != null)
                    .Select(p => new PhoneNumber
                    {
                        Type = p!.Type ?? PhoneType.Mobile,
                        Number = !string.IsNullOrEmpty(p.Number) ? p.Number : "Unknown",
                        PhysicalPersonId = entity.Id
                    })
                    .Where(p => entity.PhoneNumbers!.All(ep => ep.Number != p.Number))
                    .ToList();

                if (newPhoneNumbers.Any())
                {
                    await _repository.AddPhoneNumbersAsync(newPhoneNumbers);
                }
            }

            await _repository.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<List<PersonWithRelationsDto>> GetRelatedInfoAsync(int id)
        {
            var persons = await _repository.GetRelatedInfoAsync(id);

            if (persons == null || !persons.Any())
                return new List<PersonWithRelationsDto>(); // ცარიელი სია, თუ ვერ მოიძებნა

            return persons.Select(person => new PersonWithRelationsDto
            {
                Name = person!.FirstName, 
                RelatedInfo = person.RelatedPersons?
                    .GroupBy(rp => rp.RelationType)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count()) ?? new Dictionary<string, int>()
            }).ToList();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.physicalPersonRepo.DeletePersonWithRelationshipsAsync(id);

            // შეცდომის იმიტაცია
            //throw new Exception("Test rollback");

            await _unitOfWork.CommitAsync();        
        }

        public async Task<GetPersonByIdDto> GetFullInfoByIdAsync(int id)
        {
            var person = await _repository.GetByIdAsync(id)
                             ?? throw new KeyNotFoundException($"Person with ID {id} not found.");

            return new GetPersonByIdDto
            {
                Id = person!.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Gender = person.Gender,
                PersonalNumber = person.PersonalNumber!,
                DateOfBirth = person.DateOfBirth,

                City = person.City,

                ImagePath = person.ImagePath ?? "",

                PhoneNumbers = person.PhoneNumbers?.Select(p => new PhoneNumberDto
                {
                    PhoneNumberType = p.Type, 
                    PhoneNumber = p.Number
                }).ToList(),

              
                RelatedPersons = person.RelatedPersons?.Select(rp => new PersonWithRelationsDto
                {
                    Name = rp.Related?.FirstName + " " + rp.Related?.LastName ?? "Unknown", 
                    RelatedInfo = new Dictionary<string, int>
                    {  
                          { rp.RelationType.ToString(), rp.RelatedPersonId }
                    }
                }).ToList()
            };
        }

        public async Task AddRelationshipAsync(int personId, int relatedPersonId, RelationType relationType)
        {
            await _repository.AddRelationshipAsync(personId, relatedPersonId, relationType);
        }
     

        public async Task<PagedResult<PhysicalPerson>> QuickSearchAsync(QuickSearchDto quickSearch)
        {          
            return await _repository.QuickSearchAsync(quickSearch);
        }

        public async Task<PagedResult<PhysicalPerson>> DetailedSearchAsync(DetailedSearchDto searchDto)
        {
            return await _repository.DetailedSearchAsync(searchDto);           
        }

        public async Task SetProfilePictureAsync(int personId, string imagePath)
        {
            await _repository.SetProfilePicture(personId, imagePath);
        }

        public async Task<string> GetProfilePictureAsync(int personId)
        {
            return await _repository.GetProfilePictureAsync(personId);
        }

        public async Task DeleteProfilePictureAsync(int personId)
        {
            await _repository.DeleteProfilePictureAsync(personId);
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _repository.GetAllCitiesAsync();
        }

        public async Task<IEnumerable<GetPersonByIdDto>> GetAllPersonAsync()
        {
            return await _repository.GetAllPersonAsync();
        }

        public async Task<bool> UpdateProfilePictureAsync(int personId, string imagePath)
        {
            return await _repository.UpdateProfilePictureAsync(personId, imagePath);
        }
    }
}
