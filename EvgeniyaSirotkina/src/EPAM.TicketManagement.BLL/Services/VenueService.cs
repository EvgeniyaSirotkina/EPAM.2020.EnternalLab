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
    internal class VenueService : IService<VenueDto>
    {
        private readonly IRepository<Venue> _venueRepository;

        private readonly IMapper _mapper;

        public VenueService(IRepository<Venue> venueRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
        }

        public void Create(VenueDto item)
        {
            if (item == null)
            {
                throw new CustomException("Null venue object.");
            }

            try
            {
                var areas = _venueRepository.GetAll();

                if (areas.Any(a => a.Name == item.Name))
                {
                    throw new CustomException("An venue with such a name already exists.");
                }

                _venueRepository.Create(_mapper.Map<VenueDto, Venue>(item));
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
                var venue = _venueRepository.GetById(id);

                if (venue == null)
                {
                    throw new CustomException("The venue you want to delete does not exist.");
                }

                _venueRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }

        public IEnumerable<VenueDto> GetAll()
        {
            try
            {
                return _mapper.Map<IEnumerable<Venue>, IEnumerable<VenueDto>>(_venueRepository.GetAll());
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }

        public VenueDto GetById(int id)
        {
            if (id <= 0)
            {
                throw new CustomException("Id must be positive.");
            }

            try
            {
                var venue = _venueRepository.GetById(id);

                if (venue == null)
                {
                    throw new CustomException("The venue does not exist.");
                }

                return _mapper.Map<Venue, VenueDto>(venue);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }

        public void Update(VenueDto item)
        {
            if (item == null)
            {
                throw new CustomException("Null venue object.");
            }

            try
            {
                var venue = _venueRepository.GetById(item.Id);

                if (venue == null)
                {
                    throw new CustomException("The venue you want to update does not exist.");
                }

                _venueRepository.Update(_mapper.Map<VenueDto, Venue>(item));
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }
    }
}
