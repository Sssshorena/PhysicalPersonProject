using BLL;
using DAL;
using DAL.Abstractions;
using Shared.Models;
using Moq;
using Shared.Command;

namespace BusinessUnitTests
{
    public class PersonServiceTests
    {
        private readonly Mock<IPhysicalPersonRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PersonsService _service;

        public PersonServiceTests() 
        {
            _repositoryMock = new Mock<IPhysicalPersonRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service  = new PersonsService( _repositoryMock.Object, _unitOfWorkMock.Object );
        }


        [Fact]
        public async Task GetFullInfoByIdAsync_ReturnsPerson_WhenPersonFound()
        {
            //Arrange
            var person = new PhysicalPerson
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Gender  = Gender.Male,
                PersonalNumber = "12345678901",
                DateOfBirth = new DateTime(1991, 1, 31),
                City = new City { Name = "City"},
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber {Type = PhoneType.Mobile, Number = "555001001"}
                },
                RelatedPersons = new List<RelatedPerson>
                {
                    new RelatedPerson 
                    { 
                        RelationType = RelationType.Family, RelatedPersonId = 1,

                        Related = new PhysicalPerson 
                        {
                            FirstName = "Jane", 
                            LastName = "Doe", 
                            PersonalNumber = "11111111111"
                        } 
                    }
                }
            };
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(person);

            //Act
            var result = await _service.GetFullInfoByIdAsync(1);

            //Assert
            Assert.Equal(1, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal(Gender.Male, result.Gender);
            Assert.Equal("12345678901", result.PersonalNumber);
            Assert.Equal(new DateTime(1991, 1, 31), result.DateOfBirth);
            Assert.Equal("City", result?.City?.Name);
            Assert.Single(result?.PhoneNumbers!);
            Assert.Single(result?.RelatedPersons!);
        }

        [Fact]
        public async Task AddOrUpdateAsync_WhenPersonDoesNotExist_ShouldAddNewPerson()
        {
            // Arrange
            var personDto = new CreatePersonCommandDto
            {
                FirstName = "Giorgi",
                LastName = "Giorgadze",
                Gender = Gender.Male,
                PersonalNumber = "12345678901",
                DateOfBirth = new DateTime(1990, 1, 31),
                PhoneNumbers = new List<CreatePhoneNumberDto?>
            {
                new CreatePhoneNumberDto { Type = PhoneType.Mobile, Number = "555123456" }
            }
                
            };
            _repositoryMock.Setup(r => r.GetByIdAsync(personDto.PersonalNumber)).ReturnsAsync((PhysicalPerson?)null);

            var newPerson = new PhysicalPerson 
            { 
                Id = 1, 
                FirstName = "Giorgi", 
                LastName = "Giorgadze", 
                PersonalNumber = "12345678901" 
            };
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<PhysicalPerson>())).Callback<PhysicalPerson>(p => p.Id = 1);

            // Act
            var result = await _service.AddOrUpdateAsync(personDto);

            // Assert
            Assert.Equal(1, result);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PhysicalPerson>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
        }
    }
}