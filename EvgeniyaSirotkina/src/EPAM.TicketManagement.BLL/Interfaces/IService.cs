using System.Collections.Generic;

namespace EPAM.TicketManagement.BLL.Interfaces
{
    public interface IService<T>
        where T : class
    {
        void Create(T item);

        void Delete(int id);

        void Update(T item);

        T GetById(int id);

        IEnumerable<T> GetAll();
    }
}
