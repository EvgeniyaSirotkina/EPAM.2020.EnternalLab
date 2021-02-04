using System;
using System.Collections.Generic;
using System.Linq;
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
            if (item == null)
            {
                throw new CustomException("Null area object.");
            }

            if (GetAll().Any(a => a.LayoutId == item.LayoutId && a.Description == item.Description))
            {
                throw new CustomException("An area with such a description already exists in this layout.");
            }

            try
            {
                _areaRepository.Create(_mapper.Map<AreaDto, Area>(item));
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
            try
            {
                return _mapper.Map<IEnumerable<Area>, IEnumerable<AreaDto>>(_areaRepository.GetAll());
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
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
