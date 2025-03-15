using Microsoft.EntityFrameworkCore;
using Project.Infrastructure;
using Shared.Command;
using Shared.Models;

namespace DAL
{
    public class PhysicalPersonRepository : IPhysicalPersonRepository
    {
        private readonly PhysicalPersonsDbContext _dbContext;
        public PhysicalPersonRepository(PhysicalPersonsDbContext physicalPersonsDbContext)
        {
            _dbContext = physicalPersonsDbContext;
        }

        public async Task AddAsync(PhysicalPerson person)
        {
            if (person.City != null && person.City.Id == 0)
            {
                _dbContext.Cities.Add(person.City);
            }

            if (person.PhoneNumbers is not null && person.PhoneNumbers.Count != 0)
            {
                _dbContext.PhoneNumbers.AddRange(person.PhoneNumbers);
            }

            await _dbContext.PhysicalPersons.AddAsync(person);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PhysicalPerson> GetByIdAsync(int id)
        {
            return await _dbContext.PhysicalPersons
             .Where(p => p.Id == id)
             .Include(p => p.RelatedPersons!)
             .ThenInclude(rp => rp.Related) 
             .Include(p => p.PhoneNumbers) 
             .Include(p => p.City) 
             .FirstOrDefaultAsync()
              ?? throw new KeyNotFoundException("No matching records found.");
        }

        public async Task<PhysicalPerson?> GetByIdAsync(string personalNumber)
        {
            return await _dbContext.PhysicalPersons
                .Include(p => p.PhoneNumbers)
                .FirstOrDefaultAsync(p => p.PersonalNumber == personalNumber);
        }

        public async Task AddRelationshipAsync(int personId, int relatedPersonId, RelationType relationType)
        {
            var relationship = new RelatedPerson
            {
                PersonId = personId,
                RelatedPersonId = relatedPersonId,
                RelationType = relationType
            };

            await _dbContext.RelatedPersons.AddAsync(relationship);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<City?> GetCityByNameAsync(string cityName)
        {
            return await _dbContext.Cities.FirstOrDefaultAsync(c => c.Name == cityName);
        }

        public async Task<PagedResult<PhysicalPerson>> QuickSearchAsync(QuickSearchDto quickSearch)
        {
            var query = _dbContext.PhysicalPersons
                       .Where(p => p.FirstName.Contains(quickSearch.SearchTerm) || p.LastName.Contains(quickSearch.SearchTerm) || p.PersonalNumber.Contains(quickSearch.SearchTerm))
                       .OrderBy(p => p.FirstName);

            // მთლიანი ჩანაწერების რაოდენობა
            int totalCount = await query.CountAsync();

            // გვერდების დამატება (Pagination)
            var items = await query
                .Skip((quickSearch.Page - 1) * quickSearch.PageSize)
                .Take(quickSearch.PageSize)
                .ToListAsync();

            return new PagedResult<PhysicalPerson>
            {
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<PagedResult<PhysicalPerson>> DetailedSearchAsync(DetailedSearchDto searchParams)
        {
            var query = _dbContext.PhysicalPersons.AsQueryable();

            if (!string.IsNullOrEmpty(searchParams.FirstName))
                query = query.Where(p => p.FirstName == searchParams.FirstName);

            if (!string.IsNullOrEmpty(searchParams.LastName))
                query = query.Where(p => p.LastName == searchParams.LastName);

            if (!string.IsNullOrEmpty(searchParams.PersonalNumber))
                query = query.Where(p => p.PersonalNumber == searchParams.PersonalNumber);

            if (searchParams.Gender.HasValue)
                query = query.Where(p => p.Gender == searchParams.Gender);

            if (searchParams.DateOfBirth.HasValue)
                query = query.Where(p => p.DateOfBirth >= searchParams.DateOfBirth);

            if (!string.IsNullOrEmpty(searchParams.City))
                query = query.Where(p => p.City!.Name == searchParams.City);

            if (!string.IsNullOrEmpty(searchParams.PhoneNumber))
                query = query.Where(p => p.PhoneNumbers!.Any(ph => ph.Number == searchParams.PhoneNumber));

            if (searchParams.PhoneType.HasValue)
                query = query.Where(p => p.PhoneNumbers!.Any(ph => ph.Type == searchParams.PhoneType));

            // მთლიანი ჩანაწერების რაოდენობა
            int totalCount = await query.CountAsync();

            // გვერდების (Pagination) დამატება
            var items = await query
                .OrderBy(p => p.FirstName)
                .Skip((searchParams.Page - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .ToListAsync();

            return new PagedResult<PhysicalPerson>
            {
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<int> SaveChangesAsync()
        {
            var result = await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task AddPhoneNumbersAsync(IEnumerable<PhoneNumber> phoneNumbers)
        {
            foreach (var phone in phoneNumbers)
            {
                _dbContext.Entry(phone).State = EntityState.Added;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PhysicalPerson?>> GetRelatedInfoAsync(int id)
        {
            return await _dbContext.PhysicalPersons
             .Where(p => p.Id == id)
             .Include(p => p.RelatedPersons!) 
             .ThenInclude(rp => rp.Related) 
             .DefaultIfEmpty() // ამოიღებს NullReferenceException-ის რისკს
             .ToListAsync();
        }

        public async Task DeletePersonWithRelationshipsAsync(int personId)
        {
            var person = await _dbContext.PhysicalPersons
                .Include(p => p.PhoneNumbers)
                .Include(p => p.RelatedPersons)
                .FirstOrDefaultAsync(p => p.Id == personId) ?? throw new KeyNotFoundException($"Person with ID {personId} not found.");

            _dbContext.RelatedPersons.RemoveRange(person.RelatedPersons!);
            _dbContext.PhoneNumbers.RemoveRange(person.PhoneNumbers!);
            _dbContext.PhysicalPersons.Remove(person);
        }

        public async Task SetProfilePicture(int personId, string imagePath)
        {
            var person = await _dbContext.PhysicalPersons.FirstOrDefaultAsync(p => p.Id == personId) ?? throw new KeyNotFoundException($"Person with ID {personId} not found.");
            person.ImagePath = imagePath;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GetProfilePictureAsync(int personId)
        {
            var person = await _dbContext.PhysicalPersons
                .Where (p => p.Id == personId)
                .FirstOrDefaultAsync();

            return person?.ImagePath ?? string.Empty; ;
        }

        public async Task DeleteProfilePictureAsync(int personId)
        {
            var person = await _dbContext.PhysicalPersons.FindAsync(personId);
            if (person != null)
            {
                person.ImagePath = null;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _dbContext.Cities.ToListAsync();
        }

        public async Task<City?> GetCityByIdAsync(int id)
        {
            return await _dbContext.Cities.FindAsync(id);
        }

        public async Task<IEnumerable<GetPersonByIdDto>> GetAllPersonAsync()
        {
            var persons = await _dbContext.PhysicalPersons
           .Select(p => new GetPersonByIdDto
             {
                 Id = p.Id,
                 FirstName = p.FirstName,
                 LastName = p.LastName,
                 Gender = p.Gender,
                 PersonalNumber = p.PersonalNumber!,
                 DateOfBirth = p.DateOfBirth,
                 City = p.City,
                 ImagePath = p.ImagePath,
                 PhoneNumbers = p.PhoneNumbers!.Select(ph => new PhoneNumberDto
                 {
                     PhoneNumber = ph.Number,
                     PhoneNumberType = ph.Type
                 }).ToList(),
                 RelatedPersons = p.RelatedPersons!.Select(rp => new PersonWithRelationsDto
                 {
                     Name = rp.Related!.FirstName + " " + rp.Related.LastName,
                     RelatedInfo = new Dictionary<string, int>
                {
                    { rp.RelationType.ToString(), rp.RelatedPersonId }
                }
                 }).ToList()
             }).ToListAsync();

            return persons;
        }

        public async Task<bool> UpdateProfilePictureAsync(int personId, string imagePath)
        {
            var person = await _dbContext.PhysicalPersons.FindAsync(personId);
            if (person == null)
            {
                return false; 
            }
            person!.ImagePath = imagePath;

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

