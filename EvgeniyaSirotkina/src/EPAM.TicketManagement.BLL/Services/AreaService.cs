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
[assembly: InternalsVisibleTo("EPAM.TicketManagement.IntegrationTests")]

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

            try
            {
                var areas = _areaRepository.GetAll();

                if (areas.Any(a => a.LayoutId == item.LayoutId && a.Description == item.Description))
                {
                    throw new CustomException("An area with such a description already exists in this layout.");
                }

                _areaRepository.Create(_mapper.Map<AreaDto, Area>(item));
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }

        public void Delete(int id)
        {
            if (id <= 0)
            {
                throw new CustomException("Id must be positive.");
            }

            try
            {
                var area = _areaRepository.GetById(id);

                if (area == null)
                {
                    throw new CustomException("The area you want to delete does not exist.");
                }

                _areaRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
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
            if (id <= 0)
            {
                throw new CustomException("Id must be positive.");
            }

            try
            {
                var area = _areaRepository.GetById(id);

                if (area == null)
                {
                    throw new CustomException("The area does not exist.");
                }

                return _mapper.Map<Area, AreaDto>(area);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }

        public void Update(AreaDto item)
        {
            if (item == null)
            {
                throw new CustomException("Null area object.");
            }

            try
            {
                var area = _areaRepository.GetById(item.Id);

                if (area == null)
                {
                    throw new CustomException("The area you want to update does not exist.");
                }

                _areaRepository.Update(_mapper.Map<AreaDto, Area>(item));
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }
    }
}
