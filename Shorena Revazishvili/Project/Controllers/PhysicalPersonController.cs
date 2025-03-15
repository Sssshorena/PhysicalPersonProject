using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Filters;
using Shared.Command;
using Shared.Models;


namespace Project.Controllers
{
    //[Route("api/[controller]")]
    [CustomActionFilter]
    [Authorize]
    public class PhysicalPersonController : Controller
    {
        private readonly IPersonService _service;

        public PhysicalPersonController(IPersonService physicalPersonService)
        {
            _service = physicalPersonService;
        }

        [HttpPost("[controller]/create-person")]
        public async Task<IActionResult> CreatePhysicalPerson([FromForm] CreatePersonCommandDto physicalPerson, [FromServices] IFileService fileService)
        {
            var personId = await _service.AddOrUpdateAsync(physicalPerson);

            // Image 
            if (physicalPerson.ProfileImage != null && physicalPerson.ProfileImage.Length > 0)
            {
                var currentFilePath = await _service.GetProfilePictureAsync(personId);
                if (!string.IsNullOrEmpty(currentFilePath))
                {
                    fileService.DeleteFile(currentFilePath);
                }

                var filePath = await fileService.SaveFileAsync(physicalPerson.ProfileImage);
                await _service.SetProfilePictureAsync(personId, filePath);
            }


            return Ok(new { Id = personId, Message = "Person created successfully" });
        }

        [AllowAnonymous]
        [HttpPost("{personId}/relationship/{relatedPersonId}")]
        public async Task<IActionResult> AddRelationship(int personId, int relatedPersonId, RelationType relationType)
        {

            await _service.AddRelationshipAsync(personId, relatedPersonId, relationType);
            return Ok($"Relationship added between Person {personId} and Person {relatedPersonId}.");
        }

        [HttpGet("persons/{personId}/relations")]
        public async Task<IActionResult> GetRelatedInfo(int personId)
        {
            var people = await _service.GetRelatedInfoAsync(personId);

            if (people == null || !people.Any())
            {
                return NotFound($"No matching records found ID {personId}.");
            }

            return Ok(people);
        }

        [HttpGet("persons/{personId}/details")]
        public async Task<IActionResult> GetPersonById(int personId)
        {
            var person = await _service.GetFullInfoByIdAsync(personId);

            if (person == null)
                return NotFound($"Person with ID {personId} not found.");

            return Ok(person);
        }

        [HttpGet("persons-search-quick")]
        public async Task<IActionResult> QuickSearch([FromQuery] QuickSearchDto quickSearch)
        {
            var result = await _service.QuickSearchAsync(quickSearch);

            // თუ ბაზიდან დაბრუნდა null ან მონაცემები არ მოიძებნა
            if (result == null || result.Items == null || !result.Items.Any())
            {
                return NotFound("No matching records found.");
            }

            return Ok(result);
        }

        [HttpGet("persons-search-detailed")]
        public async Task<IActionResult> DetailedSearch([FromQuery] DetailedSearchDto searchDto)
        {
            var result = await _service.DetailedSearchAsync(searchDto);

            if (result == null || result.Items == null || result.Items.Count == 0)
            {
                return NotFound("No results found.");
            }

            return Ok(result);
        }

        [HttpGet("persons/all")]
        public async Task<IActionResult> GetAllPerson()
        {
            var persons = await _service.GetAllPersonAsync();
            return View(persons);
        }

        //[HttpGet("{pesronId}")]
        public async Task<IActionResult> Details(int? personId = null)
        {
            var allPersons = await _service.GetAllPersonAsync();

            if (!allPersons.Any())
            {
                return NotFound("The List is empty.");
            }

            var person = allPersons.FirstOrDefault(p => p.Id == personId) ?? allPersons.FirstOrDefault() ?? allPersons.FirstOrDefault(p => p.Id == 1);

            if (person == null)
            {
                return NotFound("Person not found.");
            }

            var personIndex = allPersons.ToList().FindIndex(p => p.Id == person.Id);

            int? prevPersonId = personIndex > 0 ? allPersons.ElementAt(personIndex - 1).Id : (int?)null;
            int? nextPersonId = personIndex < allPersons.Count() - 1 ? allPersons.ElementAt(personIndex + 1).Id : (int?)null;

            if (!personId.HasValue || personId != person.Id)
            {
                return RedirectToAction("Details", new { personId = person.Id });
            }

            ViewData["Person"] = person;
            ViewData["PrevPersonId"] = prevPersonId;
            ViewData["NextPersonId"] = nextPersonId;

            return View();
        }

        public async Task<IActionResult> Add()
        {
            var cities = await _service.GetAllCitiesAsync() ?? new List<City>();
            ViewBag.Cities = cities;

            return View();
        }

        [HttpDelete("persons/{personId}/relations")]
        public async Task<IActionResult> Delete(int personId)
        {
            await _service.DeleteAsync(personId);
            return Ok($"Person with ID:{personId} has been deleted.");
        }

        [HttpPost("persons/{personId}/picture")]
        public async Task<IActionResult> AddProfilePicture(int personId, IFormFile image, [FromServices] IFileService fileService)
        {
            var createImage = await fileService.SaveFileAsync(image);
            await _service.SetProfilePictureAsync(personId, createImage);
            return Ok();
        }

        [HttpPut("persons/{personId}/picture")]
        public async Task<IActionResult> UpdateProfilePicture(int personId, IFormFile file, [FromServices] IFileService fileService)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            var currentFilePath = await _service.GetProfilePictureAsync(personId);

            if (!string.IsNullOrEmpty(currentFilePath))
            {
                fileService.DeleteFile(currentFilePath);
            }

            var filePath = await fileService.SaveFileAsync(file);

            var success = await _service.UpdateProfilePictureAsync(personId, filePath);

            if (!success)
            {
                return NotFound("Person not found or could not update profile picture.");
            }

            return Ok(new { Message = "Profile picture updated successfully.", FilePath = filePath });
        }

        [HttpGet("persons/{personId}/picture")]
        public async Task<IActionResult> GetProfilePicture(int personId, [FromServices] IFileService fileService)
        {
            var imagePath = await _service.GetProfilePictureAsync(personId);
            var image = await fileService.GetFileAsync(imagePath);

            return File(image, "image/jpg");

        }

        [HttpDelete("persons/{personId}/picture")]
        public async Task<IActionResult> DeleteProfilePicture(int personId, [FromServices] IFileService fileService)
        {
            var imagePath = await _service.GetProfilePictureAsync(personId);

            if (string.IsNullOrEmpty(imagePath))
            {
                return NotFound("Profile picture not found.");
            }

            fileService.DeleteFile(imagePath);
            await _service.DeleteProfilePictureAsync(personId);

            return Ok("The picture has been deleted.");
        }
    }
}

