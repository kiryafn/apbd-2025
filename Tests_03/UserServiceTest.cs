
using LegacyApp;
using LegacyApp.Interfaces;
using NSubstitute;
using Shouldly;

namespace Tests_03
{   
    public class UserServiceTest
    {
        private readonly IUserService _userService;
        private readonly IClientRepository _clientRepository;
        private readonly IUserCreditService _userCreditService;
        private readonly IUserValidator _userValidator;

        public UserServiceTest()
        {
            _clientRepository = Substitute.For<IClientRepository>();
            _userCreditService = Substitute.For<IUserCreditService>();
            _userValidator = Substitute.For<IUserValidator>();

            _userService = new UserService(_clientRepository, _userCreditService, _userValidator);
        }

        [Theory]
        [InlineData("", "LastName", "email@mail.com", "1990-01-01")]
        [InlineData("FirstName", "", "email@mail.com", "1990-01-01")]
        [InlineData("FirstName", "LastName", "invalid_email", "1990-01-01")]
        [InlineData("FirstName", "LastName", "email@mail.com", "2005-01-01")]
        public void AddUser_InvalidUserInfo_ShouldReturnFalse(string firstName, string lastName, string email, string dateOfBirth)
        {
            _userValidator.ValidateBasicUserInfo(firstName, lastName, email, DateTime.Parse(dateOfBirth)).Returns(false);

            var result = _userService.AddUser(firstName, lastName, email, DateTime.Parse(dateOfBirth), 1);

            result.ShouldBeFalse();
        }

        [Fact]
        public void AddUser_ClientDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var firstName = "FirstName";
            var lastName = "LastName";
            var email = "email@mail.com";
            var dateOfBirth = new DateTime(1990, 1, 1);

            _userValidator.ValidateBasicUserInfo(firstName, lastName, email, dateOfBirth).Returns(true);
            _clientRepository.GetById(Arg.Any<int>()).Returns((Client)null);

            // Act
            var result = _userService.AddUser(firstName, lastName, email, dateOfBirth, 1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void AddUser_CreditLimitBelow500_ShouldReturnFalse()
        {
            // Arrange
            var firstName = "FirstName";
            var lastName = "LastName";
            var email = "email@mail.com";
            var dateOfBirth = new DateTime(1990, 1, 1);

            // Явно создаем клиента
            var client = new Client
            {
                ClientId = 1,
                Name = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                Type = "NormalClient"
            };

            _userValidator.ValidateBasicUserInfo(firstName, lastName, email, dateOfBirth).Returns(true);
            _clientRepository.GetById(Arg.Any<int>()).Returns(client);
            _userCreditService.GetCreditLimit(lastName, dateOfBirth).Returns(400);

            // Act
            var result = _userService.AddUser(firstName, lastName, email, dateOfBirth, 1);

            // Assert
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData("VeryImportantClient", false, 0, true)]
        [InlineData("ImportantClient", true, 1000, true)]
        [InlineData("ImportantClient", true, 499, true)]
        [InlineData("NormalClient", true, 600, true)]
        [InlineData("NormalClient", true, 400, false)]
        public void AddUser_DifferentClientTypesAndCreditLimits_ShouldReturnExpectedResult(string clientType, bool hasCreditLimit, int creditLimit, bool expectedResult)
        {
            // Arrange
            var firstName = "FirstName";
            var lastName = "LastName";
            var email = "email@mail.com";
            var dateOfBirth = new DateTime(1990, 1, 1);
            var client = new Client
            {
                ClientId = 2,
                Name = "Jane Smith",
                Email = "janesmith@example.com",
                Address = "456 Secondary St",
                Type = clientType
            };

            _userValidator.ValidateBasicUserInfo(firstName, lastName, email, dateOfBirth).Returns(true);
            _clientRepository.GetById(Arg.Any<int>()).Returns(client);
            _userCreditService.GetCreditLimit(lastName, dateOfBirth).Returns(creditLimit);

            // Act
            var result = _userService.AddUser(firstName, lastName, email, dateOfBirth, 1);

            // Assert
            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void AddUser_ValidData_NewUserIsCreated()
        {
            // Arrange
            var firstName = "FirstName";
            var lastName = "LastName";
            var email = "email@mail.com";
            var dateOfBirth = new DateTime(1990, 1, 1);

            // Явно создаем клиента
            var client = new Client
            {
                ClientId = 3,
                Name = "Michael Johnson",
                Email = "michaelj@example.com",
                Address = "789 Tertiary St",
                Type = "NormalClient"
            };

            _userValidator.ValidateBasicUserInfo(firstName, lastName, email, dateOfBirth).Returns(true);
            _clientRepository.GetById(Arg.Any<int>()).Returns(client);
            _userCreditService.GetCreditLimit(lastName, dateOfBirth).Returns(1000);

            // Act
            var result = _userService.AddUser(firstName, lastName, email, dateOfBirth, 1);

            // Assert
            result.ShouldBeTrue();
        }
    }
}