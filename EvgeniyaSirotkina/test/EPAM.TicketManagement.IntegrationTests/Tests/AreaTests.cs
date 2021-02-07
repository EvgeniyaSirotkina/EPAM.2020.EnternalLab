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
                mc.AddProfile(new AreaProfile());
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
            var newArea = _fixture.Build<AreaDto>()
                .With(x => x.LayoutId, 1)
                .Without(x => x.Id)
                .Create();

            var areas = _service.GetAll();

            // Act
            _service.Create(newArea);

            // Assert
            var areasAfterCreateNewOne = _service.GetAll();
            areasAfterCreateNewOne.Should().HaveCount(areas.Count() + 1);

            var areaFromDb = areasAfterCreateNewOne
                .FirstOrDefault(x => x.LayoutId == newArea.LayoutId && x.Description == newArea.Description);
            using (new AssertionScope())
            {
                areaFromDb.Should().NotBeNull();
                areaFromDb.Description.Should().Be(newArea.Description);
                areaFromDb.LayoutId.Should().Be(newArea.LayoutId);
                areaFromDb.CoordX.Should().Be(newArea.CoordX);
                areaFromDb.CoordY.Should().Be(newArea.CoordY);
            }

            // Remove data from db
            _service.Delete(areaFromDb.Id);
        }

        [Test]
        public void Create_WnenAreaAlreadyExistInLayout_ShouldThrowCustomException()
        {
            // Arrange
            var newArea = _fixture.Build<AreaDto>()
                .With(x => x.LayoutId, 1)
                .With(x => x.Description, "Test dscription")
                .Without(x => x.Id)
                .Create();

            var expectedErrorMessage = "An area with such a description already exists in this layout.";

            var areas = _service.GetAll();
            _service.Create(newArea);

            // Act
            Action act = () => _service.Create(newArea);

            // Assert
            var areasAfterCreateNewOne = _service.GetAll();
            areasAfterCreateNewOne.Should().HaveCount(areas.Count() + 1);

            var areaFromDb = areasAfterCreateNewOne
                .FirstOrDefault(x => x.LayoutId == newArea.LayoutId && x.Description == newArea.Description);
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);

            // Remove data from db
            _service.Delete(areaFromDb.Id);
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
    }
}
