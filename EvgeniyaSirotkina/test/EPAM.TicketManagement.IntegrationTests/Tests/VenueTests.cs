using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using AutoFixture;
using AutoMapper;
using EPAM.TicketManagement.BLL.EntitiesDto;
using EPAM.TicketManagement.BLL.Exceptions;
using EPAM.TicketManagement.BLL.Interfaces;
using EPAM.TicketManagement.BLL.MapperProfiles;
using EPAM.TicketManagement.BLL.Services;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;
using EPAM.TicketManagement.DAL.Repositories;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.SqlServer.Dac;
using NUnit.Framework;

namespace EPAM.TicketManagement.IntegrationTests.Tests
{
    [TestFixture]
    public class VenueTests
    {
        private readonly string _connectionString;

        private readonly IFixture _fixture;

        private readonly IRepository<Venue> _repository;

        private readonly IMapper _mapper;

        private readonly IService<VenueDto> _service;

        public VenueTests()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            _fixture = new Fixture();

            _repository = new VenueRepository(_connectionString);

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ServiceProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _service = new VenueService(_repository, _mapper);
        }

        [OneTimeSetUp]
        public void DeployDb()
        {
            var dacOptions = new DacDeployOptions { CreateNewDatabase = true };
            var dacServiceInstance = new DacServices(_connectionString);
            var currentWorkDirectory = TestContext.CurrentContext.WorkDirectory;
            var path = currentWorkDirectory + ConfigurationManager.AppSettings["dacpacFilePath"];
            using (var dacpac = DacPackage.Load(path))
            {
                dacServiceInstance.Deploy(dacpac, "TicketManagementTestDatabase", true, dacOptions);
            }
        }

        [Test]
        public void Create_WnenDataIsValid_ShouldCreateNewVenue()
        {
            // Arrange
            var testVenue = TestVenue();

            var venues = _service.GetAll();

            // Act
            _service.Create(testVenue);

            // Assert
            var venuesAfterCreateNewOne = _service.GetAll();
            venuesAfterCreateNewOne.Should().HaveCount(venues.Count() + 1);

            var venueFromDb = venuesAfterCreateNewOne.FirstOrDefault(x => x.Name == testVenue.Name);

            using (new AssertionScope())
            {
                venueFromDb.Should().NotBeNull();
                venueFromDb.Description.Should().Be(testVenue.Description);
                venueFromDb.Name.Should().Be(testVenue.Name);
                venueFromDb.Phone.Should().Be(testVenue.Phone);
                venueFromDb.Address.Should().Be(testVenue.Address);
            }

            // Remove data from db
            _service.Delete(venueFromDb.Id);
        }

        [Test]
        public void Create_WnenVenueAlreadyExist_ShouldThrowCustomException()
        {
            // Arrange
            var testVenue = TestVenue();

            var expectedErrorMessage = "An venue with such a name already exists.";

            var venues = _service.GetAll();
            _service.Create(testVenue);

            // Act
            Action act = () => _service.Create(testVenue);

            // Assert
            var venuesAfterCreateNewOne = _service.GetAll();
            venuesAfterCreateNewOne.Should().HaveCount(venues.Count() + 1);

            var venueFromDb = venuesAfterCreateNewOne.FirstOrDefault(x => x.Name == testVenue.Name);

            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);

            // Remove data from db
            _service.Delete(venueFromDb.Id);
        }

        [Test]
        public void Delete_WnenVenueExist_ShouldDeleteVenue()
        {
            // Arrange
            var testVenue = TestVenue();

            _service.Create(testVenue);
            var venues = _service.GetAll();
            var venueFromDb = venues.FirstOrDefault(x => x.Name == testVenue.Name);

            // Act
            _service.Delete(venueFromDb.Id);

            // Assert
            var venuesAfterCreateNewOne = _service.GetAll();
            venuesAfterCreateNewOne.Should().HaveCount(venues.Count() - 1);
        }

        [Test]
        public void Delete_WnenVenueDoesntExistInLayout_ShouldThrowCustomException()
        {
            // Arrange
            var testVenue = TestVenue();
            testVenue.Id = 10;

            var expectedErrorMessage = "The venue you want to delete does not exist.";

            // Act
            Action act = () => _service.Delete(testVenue.Id);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_WhenVenueExistAndDataIsValid_SouldUpdateVenue()
        {
            // Arrange
            var testVenue = TestVenue();

            _service.Create(testVenue);
            var venues = _service.GetAll();
            var venueFromDb = venues.FirstOrDefault(x => x.Name == testVenue.Name);

            venueFromDb.Description = "Test Description Updated";

            // Act
            _service.Update(venueFromDb);

            // Assert
            var venuesAfterCreateNewOne = _service.GetAll();
            var updatedVenueFromDb = venuesAfterCreateNewOne
                .FirstOrDefault(x => x.Name == venueFromDb.Name);

            using (new AssertionScope())
            {
                updatedVenueFromDb.Should().NotBeNull();
                updatedVenueFromDb.Description.Should().Be(venueFromDb.Description);
                updatedVenueFromDb.Name.Should().Be(venueFromDb.Name);
                updatedVenueFromDb.Phone.Should().Be(venueFromDb.Phone);
                updatedVenueFromDb.Address.Should().Be(venueFromDb.Address);
            }

            // Remove data from db
            _service.Delete(venueFromDb.Id);
        }

        [Test]
        public void Update_WhenVenueDoesntExist_SouldUpdateVenue()
        {
            // Arrange
            var testVenue = TestVenue();
            testVenue.Id = 10;

            var expectedErrorMessage = "The venue you want to update does not exist.";

            // Act
            Action act = () => _service.Update(testVenue);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [OneTimeTearDown]
        public void DropDb()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sqlCommandText = @"USE master; ALTER DATABASE [TicketManagementTestDatabase] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [TicketManagementTestDatabase]";
                using (var sqlCommand = new SqlCommand(sqlCommandText, conn))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private VenueDto TestVenue()
            => _fixture.Build<VenueDto>()
                .With(x => x.Name, "Test Name")
                .With(x => x.Description, "Test Description")
                .With(x => x.Phone, "Test Phone")
                .With(x => x.Address, "Test Address")
                .Without(x => x.Id)
                .Create();
    }
}
