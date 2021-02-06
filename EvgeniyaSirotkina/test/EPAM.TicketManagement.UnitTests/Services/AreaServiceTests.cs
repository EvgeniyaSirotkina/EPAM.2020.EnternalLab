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
            var areas = _fixture.CreateMany<Area>(3);

            _mockMapper.Setup(x => x.Map<AreaDto, Area>(areaDto)).Returns(area);
            _mockArea.Setup(x => x.GetAll()).Returns(areas);
            _mockArea.Setup(x => x.Create(area)).Verifiable();

            // Act
            _service.Create(areaDto);

            // Assert
            _mockArea.Verify(x => x.Create(area), Times.Once);
        }

        [Test]
        public void Create_WhenAnAreaIsNull_ShouldThrowCustomException()
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
        public void Create_WhenAnAreaIsAlreadyExist_ShouldThrowCustomException()
        {
            // Arrange
            var areaDto = _fixture.Create<AreaDto>();
            var areas = new List<Area>
            {
                _fixture.Build<Area>()
                    .With(x => x.Id, areaDto.Id)
                    .With(x => x.LayoutId, areaDto.LayoutId)
                    .With(x => x.Description, areaDto.Description)
                    .With(x => x.CoordX, areaDto.CoordX)
                    .With(x => x.CoordY, areaDto.CoordY)
                    .Create(),
                _fixture.Create<Area>(),
                _fixture.Create<Area>(),
                _fixture.Create<Area>(),
            };
            _mockArea.Setup(x => x.GetAll()).Returns(areas);

            var expectedErrorMessage = "An area with such a description already exists in this layout.";

            // Act
            Action act = () => _service.Create(areaDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Create_WhenAnAreaCannotBeSave_ShouldThrowCustomException()
        {
            // Arrange
            var areaDto = _fixture.Create<AreaDto>();
            var areas = _fixture.CreateMany<Area>(3);

            _mockArea.Setup(x => x.GetAll()).Returns(areas);
            _mockArea.Setup(x => x.Create(It.IsAny<Area>())).Throws<Exception>();

            _mockMapper.Setup(x => x.Map<AreaDto, Area>(areaDto)).Returns(It.IsAny<Area>());

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
        [TestCase(0)]
        [TestCase(-5)]
        [Parallelizable(ParallelScope.All)]
        public void Delete_WhenIdIsNegative_ShouldThrowCustomException(int areaId)
        {
            // Arrange
            var expectedErrorMessage = "Id must be positive.";

            // Act
            Action act = () => _service.Delete(areaId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Delete_WhenAnAreaDoesntExist_ShouldThrowCustomException()
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

        [Test]
        public void GetAll_WhenDataIsValid_ShouldReturnListOfAreas()
        {
            // Arrange
            var length = 3;
            var areas = _fixture.CreateMany<Area>(length);
            var areasDto = _fixture.CreateMany<AreaDto>(length);

            _mockArea.Setup(x => x.GetAll()).Returns(areas);
            _mockMapper.Setup(x => x.Map<IEnumerable<Area>, IEnumerable<AreaDto>>(areas)).Returns(areasDto);

            // Act
            var result = _service.GetAll();

            // Assert
            result.Should().BeEquivalentTo(areasDto);
        }

        [Test]
        public void GetAll_WhenCannotGetAllAreas_ShouldThrowCustomException()
        {
            // Arrange
            _mockArea.Setup(x => x.GetAll()).Throws<Exception>();

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.GetAll();

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void GetById_WhenDataIsValid_ShouldReturnArea()
        {
            // Arrange
            var area = _fixture.Create<Area>();
            var areaDto = _fixture.Build<AreaDto>().With(x => x.Id, area.Id).Create();

            _mockArea.Setup(x => x.GetById(areaDto.Id)).Returns(area);
            _mockMapper.Setup(x => x.Map<Area, AreaDto>(area)).Returns(areaDto);

            // Act
            var result = _service.GetById(areaDto.Id);

            // Assert
            result.Should().BeEquivalentTo(areaDto);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-5)]
        [Parallelizable(ParallelScope.All)]
        public void GetById_WhenIdIsNegative_ShouldThrowCustomException(int areaId)
        {
            // Arrange
            var expectedErrorMessage = "Id must be positive.";

            // Act
            Action act = () => _service.GetById(areaId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void GetById_WhenAnAreaDoesntExist_ShouldThrowCustomException()
        {
            // Arrange
            var areaId = 6;
            Area area = null;
            var expectedErrorMessage = "The area does not exist.";
            _mockArea.Setup(x => x.GetById(areaId)).Returns(area);

            // Act
            Action act = () => _service.GetById(areaId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void GetById_WhenCannotGetArea_ShouldThrowCustomException()
        {
            // Arrange
            var areaId = 5;
            _mockArea.Setup(x => x.GetById(areaId)).Throws<Exception>();

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.GetById(areaId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_WhenDataIsValid_ShouldUpdateArea()
        {
            // Arrange
            var areaDto = _fixture.Create<AreaDto>();
            var area = _fixture.Build<Area>()
                .With(x => x.Id, areaDto.Id)
                .Create();

            _mockArea.Setup(x => x.GetById(area.Id)).Returns(area);
            _mockMapper.Setup(x => x.Map<AreaDto, Area>(areaDto)).Returns(area);

            _mockArea.Setup(x => x.Update(area)).Verifiable();

            // Act
            _service.Update(areaDto);

            // Assert
            _mockArea.Verify(x => x.Update(area), Times.Once);
        }

        [Test]
        public void Update_WhenAnAreaIsNull_ShouldThrowCustomException()
        {
            // Arrange
            var expectedErrorMessage = "Null area object.";

            // Act
            Action act = () => _service.Update(null);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_WhenAnAreaDoesntExist_ShouldThrowCustomException()
        {
            // Arrange
            var areaId = 6;
            var areaDto = _fixture.Build<AreaDto>()
                .With(x => x.Id, areaId)
                .Create();
            Area area = null;
            var expectedErrorMessage = "The area you want to update does not exist.";
            _mockArea.Setup(x => x.GetById(areaId)).Returns(area);

            // Act
            Action act = () => _service.Update(areaDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_WhenCannotUpdateArea_ShouldThrowCustomException()
        {
            // Arrange
            var areaDto = _fixture.Create<AreaDto>();
            var area = _fixture.Build<Area>()
                .With(x => x.Id, areaDto.Id)
                .Create();

            _mockArea.Setup(x => x.GetById(area.Id)).Returns(area);
            _mockMapper.Setup(x => x.Map<AreaDto, Area>(areaDto)).Returns(area);

            _mockArea.Setup(x => x.Update(area)).Throws<Exception>();

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.Update(areaDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }
    }
}
