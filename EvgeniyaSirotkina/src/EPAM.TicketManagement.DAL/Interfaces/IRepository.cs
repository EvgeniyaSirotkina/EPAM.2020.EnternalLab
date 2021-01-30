using System.Collections.Generic;

namespace EPAM.TicketManagement.DAL.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        void Create(T item);

        void Delete(int id);

        void Update(T item);

        T GetById(int id);

        IEnumerable<T> GetAll();
    }
}
