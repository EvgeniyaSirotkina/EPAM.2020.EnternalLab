using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AutoMapper;
using EPAM.TicketManagement.BLL.EntitiesDto;
using EPAM.TicketManagement.BLL.Exceptions;
using EPAM.TicketManagement.BLL.Interfaces;
using EPAM.TicketManagement.DAL.Entities;
using EPAM.TicketManagement.DAL.Interfaces;

[assembly: InternalsVisibleTo("EPAM.TicketManagement.UnitTests")]

namespace EPAM.TicketManagement.BLL.Services
{
    internal class AreaService : IService<AreaDto>
    {
        private readonly IRepository<Area> _areaRepository;

        private readonly IMapper _mapper;

        public AreaService(IRepository<Area> areaRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
        }

        public void Create(AreaDto item)
        {
            try
            {
                var area = _mapper.Map<AreaDto, Area>(item);
                _areaRepository.Create(area);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AreaDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public AreaDto GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(AreaDto item)
        {
            throw new NotImplementedException();
        }
    }
}
