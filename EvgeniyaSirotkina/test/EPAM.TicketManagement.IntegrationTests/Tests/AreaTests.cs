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
    public class AreaTests
    {
        private readonly string _connectionString;

        private readonly IFixture _fixture;

        private readonly IRepository<Area> _repository;

        private readonly IMapper _mapper;

        private readonly IService<AreaDto> _service;

        public AreaTests()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            _fixture = new Fixture();

            _repository = new AreaRepository(_connectionString);

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ServiceProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _service = new AreaService(_repository, _mapper);
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
        public void Create_WnenDataIsValid_ShouldCreateNewArea()
        {
            // Arrange
            var testArea = TestArea();

            var areas = _service.GetAll();

            // Act
            _service.Create(testArea);

            // Assert
            var areasAfterCreateNewOne = _service.GetAll();
            areasAfterCreateNewOne.Should().HaveCount(areas.Count() + 1);

            var areaFromDb = areasAfterCreateNewOne
                .FirstOrDefault(x => x.LayoutId == testArea.LayoutId && x.Description == testArea.Description);
            using (new AssertionScope())
            {
                areaFromDb.Should().NotBeNull();
                areaFromDb.Description.Should().Be(testArea.Description);
                areaFromDb.LayoutId.Should().Be(testArea.LayoutId);
                areaFromDb.CoordX.Should().Be(testArea.CoordX);
                areaFromDb.CoordY.Should().Be(testArea.CoordY);
            }

            // Remove data from db
            _service.Delete(areaFromDb.Id);
        }

        [Test]
        public void Create_WnenAnAreaAlreadyExistInLayout_ShouldThrowCustomException()
        {
            // Arrange
            var testArea = TestArea();

            var expectedErrorMessage = "An area with such a description already exists in this layout.";

            var areas = _service.GetAll();
            _service.Create(testArea);

            // Act
            Action act = () => _service.Create(testArea);

            // Assert
            var areasAfterCreateNewOne = _service.GetAll();
            areasAfterCreateNewOne.Should().HaveCount(areas.Count() + 1);

            var areaFromDb = areasAfterCreateNewOne
                .FirstOrDefault(x => x.LayoutId == testArea.LayoutId && x.Description == testArea.Description);
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);

            // Remove data from db
            _service.Delete(areaFromDb.Id);
        }

        [Test]
        public void Delete_WnenAnAreaExist_ShouldDeleteAnArea()
        {
            // Arrange
            var testArea = TestArea();

            _service.Create(testArea);
            var areas = _service.GetAll();
            var areaFromDb = areas
                 .FirstOrDefault(x => x.LayoutId == testArea.LayoutId && x.Description == testArea.Description);

            // Act
            _service.Delete(areaFromDb.Id);

            // Assert
            var areasAfterCreateNewOne = _service.GetAll();
            areasAfterCreateNewOne.Should().HaveCount(areas.Count() - 1);
        }

        [Test]
        public void Delete_WnenAnAreaDoesntExistInLayout_ShouldThrowCustomException()
        {
            // Arrange
            var testArea = TestArea();
            testArea.Id = 10;

            var expectedErrorMessage = "The area you want to delete does not exist.";

            // Act
            Action act = () => _service.Delete(testArea.Id);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_WhenAnAreaExistAndDataIsValid_SouldUpdateAnArea()
        {
            // Arrange
            var testArea = TestArea();

            _service.Create(testArea);
            var areas = _service.GetAll();
            var areaFromDb = areas
                 .FirstOrDefault(x
                 => x.LayoutId == testArea.LayoutId && x.Description == testArea.Description);

            areaFromDb.Description = "Test Description Updated";

            // Act
            _service.Update(areaFromDb);

            // Assert
            var areasAfterCreateNewOne = _service.GetAll();
            var updatedAreaFromDb = areasAfterCreateNewOne
                .FirstOrDefault(x => x.LayoutId == areaFromDb.LayoutId && x.Description == areaFromDb.Description);

            using (new AssertionScope())
            {
                updatedAreaFromDb.Should().NotBeNull();
                updatedAreaFromDb.Description.Should().Be(areaFromDb.Description);
                updatedAreaFromDb.LayoutId.Should().Be(areaFromDb.LayoutId);
                updatedAreaFromDb.CoordX.Should().Be(areaFromDb.CoordX);
                updatedAreaFromDb.CoordY.Should().Be(areaFromDb.CoordY);
            }

            // Remove data from db
            _service.Delete(areaFromDb.Id);
        }

        [Test]
        public void Update_WhenAnAreaDoesntExist_SouldUpdateAnArea()
        {
            // Arrange
            var testArea = TestArea();
            testArea.Id = 10;

            var expectedErrorMessage = "The area you want to update does not exist.";

            // Act
            Action act = () => _service.Update(testArea);

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

        private AreaDto TestArea()
            => _fixture.Build<AreaDto>()
                .With(x => x.LayoutId, 1)
                .With(x => x.Description, "Test Description")
                .Without(x => x.Id)
                .Create();
    }
}
