using Shared.Command;
using Shared.Models;

namespace DAL
{
    public interface IPhysicalPersonRepository
    {
        Task AddAsync(PhysicalPerson person);
        Task<IEnumerable<GetPersonByIdDto>> GetAllPersonAsync();
        Task AddPhoneNumbersAsync(IEnumerable<PhoneNumber> phoneNumbers);
        Task<PhysicalPerson> GetByIdAsync(int id);
        Task<PhysicalPerson?> GetByIdAsync(string personalNumber);
        Task AddRelationshipAsync(int personId, int relatedPersonId, RelationType relationType);
        Task<City?> GetCityByNameAsync(string cityName);
        Task<List<PhysicalPerson?>> GetRelatedInfoAsync(int id);
        Task DeletePersonWithRelationshipsAsync(int personId);
        Task<int> SaveChangesAsync();
        Task SetProfilePicture(int personId, string imagePath);
        Task<string> GetProfilePictureAsync(int personId);
        Task<bool> UpdateProfilePictureAsync (int personId, string imagePath);
        Task DeleteProfilePictureAsync(int personId);
        Task<IEnumerable<City>> GetAllCitiesAsync();
        Task<City?> GetCityByIdAsync(int id);



        Task<PagedResult<PhysicalPerson>> QuickSearchAsync(QuickSearchDto quickSearch);
        Task<PagedResult<PhysicalPerson>> DetailedSearchAsync(DetailedSearchDto searchParams);
        
    }
}

