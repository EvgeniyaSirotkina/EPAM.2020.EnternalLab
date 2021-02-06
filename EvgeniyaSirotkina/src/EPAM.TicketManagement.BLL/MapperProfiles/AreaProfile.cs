using System.Runtime.CompilerServices;
using AutoMapper;
using EPAM.TicketManagement.BLL.EntitiesDto;
using EPAM.TicketManagement.DAL.Entities;

[assembly: InternalsVisibleTo("EPAM.TicketManagement.IntegrationTests")]

namespace EPAM.TicketManagement.BLL.MapperProfiles
{
    internal class AreaProfile : Profile
    {
        public AreaProfile()
        {
            CreateMap<Area, AreaDto>().ReverseMap();
        }
    }
}
