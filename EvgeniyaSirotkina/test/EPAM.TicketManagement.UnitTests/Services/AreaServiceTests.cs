using AutoFixture;
using AutoMapper;
using EPAM.TicketManagement.BLL.EntitiesDto;
using EPAM.TicketManagement.BLL.Interfaces;
using EPAM.TicketManagement.BLL.Services;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;
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
    }
}
