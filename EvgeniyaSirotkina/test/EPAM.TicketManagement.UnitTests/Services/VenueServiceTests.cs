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
    public class VenueServiceTests
    {
        private readonly IFixture _fixture;

        private readonly Mock<IRepository<Venue>> _mockVenue;

        private readonly Mock<IMapper> _mockMapper;

        private readonly IService<VenueDto> _service;

        public VenueServiceTests()
        {
            _fixture = new Fixture();
            _mockVenue = new Mock<IRepository<Venue>>();
            _mockMapper = new Mock<IMapper>();
            _service = new VenueService(_mockVenue.Object, _mockMapper.Object);
        }

        [Test]
        public void Create_WhenDataIsValid_ShouldCreateArea()
        {
            // Arrange
            var venueDto = _fixture.Create<VenueDto>();
            var venue = _fixture.Create<Venue>();
            var areas = _fixture.CreateMany<Venue>(3);

            _mockMapper.Setup(x => x.Map<VenueDto, Venue>(venueDto)).Returns(venue);
            _mockVenue.Setup(x => x.GetAll()).Returns(areas);
            _mockVenue.Setup(x => x.Create(venue)).Verifiable();

            // Act
            _service.Create(venueDto);

            // Assert
            _mockVenue.Verify(x => x.Create(venue), Times.Once);
        }

        [Test]
        public void Create_WhenAnAreaIsNull_ShouldThrowCustomException()
        {
            // Arrange
            var expectedErrorMessage = "Null venue object.";

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
            var venueDto = _fixture.Create<VenueDto>();
            var venues = new List<Venue>
            {
                _fixture.Build<Venue>()
                    .With(x => x.Id, venueDto.Id)
                    .With(x => x.Name, venueDto.Name)
                    .With(x => x.Description, venueDto.Description)
                    .With(x => x.Phone, venueDto.Phone)
                    .With(x => x.Address, venueDto.Address)
                    .Create(),
                _fixture.Create<Venue>(),
                _fixture.Create<Venue>(),
                _fixture.Create<Venue>(),
            };
            _mockVenue.Setup(x => x.GetAll()).Returns(venues);

            var expectedErrorMessage = "An venue with such a name already exists.";

            // Act
            Action act = () => _service.Create(venueDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Create_WhenAnAreaCannotBeSave_ShouldThrowCustomException()
        {
            // Arrange
            var venueDto = _fixture.Create<VenueDto>();
            var areas = _fixture.CreateMany<Venue>(3);

            _mockVenue.Setup(x => x.GetAll()).Returns(areas);
            _mockVenue.Setup(x => x.Create(It.IsAny<Venue>())).Throws<Exception>();

            _mockMapper.Setup(x => x.Map<VenueDto, Venue>(venueDto)).Returns(It.IsAny<Venue>());

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.Create(venueDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Delete_WhenDataIsValid_ShouldDeleteArea()
        {
            // Arrange
            var venueId = 1;
            var venueDto = _fixture.Build<VenueDto>().With(x => x.Id, venueId).Create();
            var venue = _fixture.Build<Venue>().With(x => x.Id, venueId).Create();

            _mockVenue.Setup(x => x.GetById(venueId)).Returns(venue);
            _mockMapper.Setup(x => x.Map<Venue, VenueDto>(venue)).Returns(venueDto);
            _mockVenue.Setup(x => x.Delete(venueId)).Verifiable();

            // Act
            _service.Delete(venueId);

            // Assert
            _mockVenue.Verify(x => x.Delete(venueId), Times.Once);
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
            var venueId = 2;
            Venue venue = null;
            var expectedErrorMessage = "The venue you want to delete does not exist.";
            _mockVenue.Setup(x => x.GetById(venueId)).Returns(venue);

            // Act
            Action act = () => _service.Delete(venueId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Delete_WhenAnAreaCannotBeDelete_ShouldThrowCustomException()
        {
            // Arrange
            var venueId = 3;
            var venuDto = _fixture.Build<VenueDto>().With(x => x.Id, venueId).Create();
            var venue = _fixture.Build<Venue>().With(x => x.Id, venueId).Create();

            _mockVenue.Setup(x => x.GetById(venueId)).Returns(venue);
            _mockMapper.Setup(x => x.Map<Venue, VenueDto>(venue)).Returns(venuDto);
            _mockVenue.Setup(x => x.Delete(It.IsAny<int>())).Throws<Exception>();

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.Delete(venueId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void GetAll_WhenDataIsValid_ShouldReturnListOfAreas()
        {
            // Arrange
            var length = 3;
            var venues = _fixture.CreateMany<Venue>(length);
            var venuesDto = _fixture.CreateMany<VenueDto>(length);

            _mockVenue.Setup(x => x.GetAll()).Returns(venues);
            _mockMapper.Setup(x => x.Map<IEnumerable<Venue>, IEnumerable<VenueDto>>(venues)).Returns(venuesDto);

            // Act
            var result = _service.GetAll();

            // Assert
            result.Should().BeEquivalentTo(venuesDto);
        }

        [Test]
        public void GetAll_WhenCannotGetAllAreas_ShouldThrowCustomException()
        {
            // Arrange
            _mockVenue.Setup(x => x.GetAll()).Throws<Exception>();

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
            var venue = _fixture.Create<Venue>();
            var venueDto = _fixture.Build<VenueDto>().With(x => x.Id, venue.Id).Create();

            _mockVenue.Setup(x => x.GetById(venueDto.Id)).Returns(venue);
            _mockMapper.Setup(x => x.Map<Venue, VenueDto>(venue)).Returns(venueDto);

            // Act
            var result = _service.GetById(venueDto.Id);

            // Assert
            result.Should().BeEquivalentTo(venueDto);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-5)]
        [Parallelizable(ParallelScope.All)]
        public void GetById_WhenIdIsNegative_ShouldThrowCustomException(int venueId)
        {
            // Arrange
            var expectedErrorMessage = "Id must be positive.";

            // Act
            Action act = () => _service.GetById(venueId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void GetById_WhenAnAreaDoesntExist_ShouldThrowCustomException()
        {
            // Arrange
            var venueId = 6;
            Venue venue = null;
            var expectedErrorMessage = "The venue does not exist.";
            _mockVenue.Setup(x => x.GetById(venueId)).Returns(venue);

            // Act
            Action act = () => _service.GetById(venueId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void GetById_WhenCannotGetArea_ShouldThrowCustomException()
        {
            // Arrange
            var venueId = 5;
            _mockVenue.Setup(x => x.GetById(venueId)).Throws<Exception>();

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.GetById(venueId);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_WhenDataIsValid_ShouldUpdateArea()
        {
            // Arrange
            var venueDto = _fixture.Create<VenueDto>();
            var venue = _fixture.Build<Venue>()
                .With(x => x.Id, venueDto.Id)
                .Create();

            _mockVenue.Setup(x => x.GetById(venue.Id)).Returns(venue);
            _mockMapper.Setup(x => x.Map<VenueDto, Venue>(venueDto)).Returns(venue);

            _mockVenue.Setup(x => x.Update(venue)).Verifiable();

            // Act
            _service.Update(venueDto);

            // Assert
            _mockVenue.Verify(x => x.Update(venue), Times.Once);
        }

        [Test]
        public void Update_WhenAnAreaIsNull_ShouldThrowCustomException()
        {
            // Arrange
            var expectedErrorMessage = "Null venue object.";

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
            var venueId = 6;
            var venueDto = _fixture.Build<VenueDto>()
                .With(x => x.Id, venueId)
                .Create();
            Venue venue = null;
            var expectedErrorMessage = "The venue you want to update does not exist.";
            _mockVenue.Setup(x => x.GetById(venueId)).Returns(venue);

            // Act
            Action act = () => _service.Update(venueDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_WhenCannotUpdateArea_ShouldThrowCustomException()
        {
            // Arrange
            var venueDto = _fixture.Create<VenueDto>();
            var venue = _fixture.Build<Venue>()
                .With(x => x.Id, venueDto.Id)
                .Create();

            _mockVenue.Setup(x => x.GetById(venue.Id)).Returns(venue);
            _mockMapper.Setup(x => x.Map<VenueDto, Venue>(venueDto)).Returns(venue);

            _mockVenue.Setup(x => x.Update(venue)).Throws<Exception>();

            var expectedErrorMessage = "Exception of type 'System.Exception' was thrown.";

            // Act
            Action act = () => _service.Update(venueDto);

            // Assert
            act.Should().Throw<CustomException>()
                .WithMessage(expectedErrorMessage);
        }
    }
}
