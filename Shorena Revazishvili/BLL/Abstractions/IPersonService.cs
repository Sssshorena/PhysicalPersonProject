using Shared.Command;
using Shared.Models;

namespace BLL
{
    public interface IPersonService
    {
        Task<int> AddOrUpdateAsync(CreatePersonCommandDto personDto);
        Task<IEnumerable<GetPersonByIdDto>> GetAllPersonAsync();
        Task AddRelationshipAsync(int personId, int relatedPersonId, RelationType relationType);
        Task<List<PersonWithRelationsDto>> GetRelatedInfoAsync(int id);
        Task DeleteAsync(int id);
        Task SetProfilePictureAsync(int personId, string imagePath);
        Task<string> GetProfilePictureAsync(int personId);
        Task<bool> UpdateProfilePictureAsync (int personId, string imagePath);
        Task<GetPersonByIdDto> GetFullInfoByIdAsync(int id);
        Task DeleteProfilePictureAsync(int personId);
        Task<IEnumerable<City>> GetAllCitiesAsync();


        Task<PagedResult<PhysicalPerson>> QuickSearchAsync(QuickSearchDto quickSearch);
        Task<PagedResult<PhysicalPerson>> DetailedSearchAsync(DetailedSearchDto searchParams);

    }
}
