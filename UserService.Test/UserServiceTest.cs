using AutoFixture;
using LegacyApp;
using LegacyApp.Interfaces;
using NSubstitute;
using Shouldly;

namespace Tests
{
    public class UserServiceTest
    {
        private readonly IUserService _userService;
        private readonly IClientRepository _repository;
        private readonly IUserCreditService _userCreditService;
        private readonly IClock _clock;
        private readonly Fixture _fixture;

        public UserServiceTest()
        {
            _fixture = new Fixture();
            _repository = Substitute.For<IClientRepository>();
            _userCreditService = Substitute.For<IUserCreditService>();

            _clock = Substitute.For<IClock>();
            _clock.Now().Returns(new DateTime(2021, 1, 1));

            _userService = new UserService(_repository, _userCreditService, _clock);
        }

        [Theory]
        [InlineData("","Surname","email@mail.com", 30)]
        [InlineData("Name", "", "email@mail.com", 30)]
        [InlineData("Name", "Surname", "", 30)]
        [InlineData("Name", "Surname", "email", 30)]
        [InlineData("Name", "Surname", "email@mail.com", 20)]
        public void AddUser_InputIncorrectValues_ShouldReturnFalseBecauseOfValidation(string firstName, string lastName, string email, int years)
        {
            //Act
            var result = _userService.AddUser(firstName, lastName, email, _clock.Now().AddYears(years*-1), 1);

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void AddUser_CreditLimitLessThen500_ShouldReturnFalseValue()
        {
            //Arrange
            var client = _fixture.Build<Client>().With(x => x.Type, "AnyType").Create();
            var dateOfBirth = _clock.Now().AddYears(-30);
            var firstName = "Name";
            var lastName = "Surname";
            var email = "email@mail.com";
            var clientId = 1;
            _repository.GetById(1).Returns(client);
            _userCreditService.GetCreditLimit(lastName, dateOfBirth).Returns(499);

            //Act
            var result = _userService.AddUser(firstName, lastName, email, dateOfBirth, clientId);

            //Assert
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData("VeryImportantClient", 0)]
        [InlineData("ImportantClient", 0)]
        public void AddUser_CreateImportantUsersWithZeroCreditLimit_NewUserShouldBeCreated(string type, int limit)
        {
            //Arrange
            var client = _fixture.Build<Client>().With(x => x.Type, type).Create();
            var dateOfBirth = _clock.Now().AddYears(-30);
            var firstName = "Name";
            var lastName = "Surname";
            var email = "email@mail.com";
            var clientId = 1;
            _repository.GetById(1).Returns(client);
            _userCreditService.GetCreditLimit(lastName, dateOfBirth).Returns(limit);

            //Act
            var result = _userService.AddUser(firstName, lastName, email, dateOfBirth, clientId);

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void AddUser_InputCorrectValues_NewUserShouldBeCreated()
        {
            //Arrange
            var client = _fixture.Create<Client>();
            var dateOfBirth = _clock.Now().AddYears(-30);
            var firstName = "Name";
            var lastName = "Surname";
            var email = "email@mail.com";
            var clientId = 1;

            _repository.GetById(1).Returns(client);
            _userCreditService.GetCreditLimit(lastName, dateOfBirth).Returns(500);

            //Act
            var result = _userService.AddUser(firstName, lastName, email, dateOfBirth, clientId);

            //Assert
            result.ShouldBeTrue();
        }
    }
}
