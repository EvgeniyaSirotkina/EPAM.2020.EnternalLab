using System;
using System.Collections.Generic;
using AutoFixture;
using AutoMapper;
using EPAM.TicketManagement.BLL.EntitiesDto;
using EPAM.TicketManagement.BLL.Exceptions;
using EPAM.TicketManagement.BLL.Interfaces;
using EPAM.TicketManagement.BLL.Services;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EPAM.TicketManagement.UnitTests.Services
{
    [TestFixture]
    public class AreaServiceTests
    {
        private readonly IFixture _fixture;

        private readonly Mock<IRepository<Area>> _mockArea;

        private readonly Mock<IMapper> _mockMapper;

        private readonly IService<AreaDto> _service;

        public AreaServiceTests()
        {
            _fixture = new Fixture();
            _mockArea = new Mock<IRepository<Area>>();
            _mockMapper = new Mock<IMapper>();
            _service = new AreaService(_mockArea.Object, _mockMapper.Object);
        }

        [Test]
        public void Create_WhenDataIsValid_ShouldCreateArea()
        {
            // Arrange
            var areaDto = _fixture.Create<AreaDto>();
            var area = _fixture.Create<Area>();

            _mockMapper.Setup(x => x.Map<AreaDto, Area>(areaDto)).Returns(area);
            _mockArea.Setup(x => x.Create(area)).Verifiable();

            // Act
            _service.Create(areaDto);

            // Assert
            _mockArea.Verify(x => x.Create(area), Times.Once);
        }

        [Test]
        public void Create_WhenDataIsValidAreaIsNull_ShouldThrowCustomException()
        {
            // Arrange
            var expectedErrorMessage = "Null area object.";

            // Act
            Action act = () => _service.Create(null);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Create_WhenAreaIsAlreadyExist_ShouldThrowCustomException()
        {
            // Arrange
            var areaDto = _fixture.Create<AreaDto>();
            var areasDto = new List<AreaDto>
            {
                areaDto,
                _fixture.Create<AreaDto>(),
                _fixture.Create<AreaDto>(),
                _fixture.Create<AreaDto>(),
            };
            _mockArea.Setup(x => x.GetAll()).Returns(It.IsAny<IEnumerable<Area>>());

            _mockMapper.Setup(x => x.Map<IEnumerable<Area>, IEnumerable<AreaDto>>(It.IsAny<IEnumerable<Area>>())).Returns(areasDto);

            var expectedErrorMessage = "An area with such a description already exists in this layout.";

            // Act
            Action act = () => _service.Create(areaDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Create_WhenNewAreaCannotBeSave_ShouldThrowCustomException()
        {
            // Arrange
            var areaDto = _fixture.Create<AreaDto>();
            var areasDto = new List<AreaDto>
            {
                _fixture.Create<AreaDto>(),
                _fixture.Create<AreaDto>(),
                _fixture.Create<AreaDto>(),
            };
            _mockArea.Setup(x => x.GetAll()).Returns(It.IsAny<IEnumerable<Area>>());
            _mockArea.Setup(x => x.Create(It.IsAny<Area>())).Throws<Exception>();

            _mockMapper.Setup(x => x.Map<AreaDto, Area>(areaDto)).Returns(It.IsAny<Area>());
            _mockMapper.Setup(x => x.Map<IEnumerable<Area>, IEnumerable<AreaDto>>(It.IsAny<IEnumerable<Area>>())).Returns(areasDto);

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.Create(areaDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Delete_WhenDataIsValid_ShouldDeleteArea()
        {
            // Arrange
            var areaId = 1;
            var areaDto = _fixture.Build<AreaDto>().With(x => x.Id, areaId).Create();
            var area = _fixture.Build<Area>().With(x => x.Id, areaId).Create();

            _mockArea.Setup(x => x.GetById(areaId)).Returns(area);
            _mockMapper.Setup(x => x.Map<Area, AreaDto>(area)).Returns(areaDto);
            _mockArea.Setup(x => x.Delete(areaId)).Verifiable();

            // Act
            _service.Delete(areaId);

            // Assert
            _mockArea.Verify(x => x.Delete(areaId), Times.Once);
        }

        [Test]
        public void Delete_WhenIdIsNegative_ShouldThrowCustomException()
        {
            // Arrange
            var expectedErrorMessage = "Id must be positive.";

            // Act
            Action act = () => _service.Delete(0);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Delete_WhenAreaDoesntExist_ShouldThrowCustomException()
        {
            // Arrange
            var areaId = 2;
            Area area = null;
            var expectedErrorMessage = "The area you want to delete does not exist.";
            _mockArea.Setup(x => x.GetById(areaId)).Returns(area);

            // Act
            Action act = () => _service.Delete(areaId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Delete_WhenAnAreaCannotBeDelete_ShouldThrowCustomException()
        {
            // Arrange
            var areaId = 3;
            var areaDto = _fixture.Build<AreaDto>().With(x => x.Id, areaId).Create();
            var area = _fixture.Build<Area>().With(x => x.Id, areaId).Create();

            _mockArea.Setup(x => x.GetById(areaId)).Returns(area);
            _mockMapper.Setup(x => x.Map<Area, AreaDto>(area)).Returns(areaDto);
            _mockArea.Setup(x => x.Delete(It.IsAny<int>())).Throws<Exception>();

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.Delete(areaId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }
    }
}
