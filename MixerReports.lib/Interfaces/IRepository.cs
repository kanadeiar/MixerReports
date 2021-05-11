using System.Collections.Generic;
using MixerReports.lib.Models.Base;

namespace MixerReports.lib.Interfaces
{
    public interface IRepository<T> where T : Entity
    {
        int Add(T entity);
        int AddRange(IEnumerable<T> entities);
        T GetOne(int id);
        IEnumerable<T> GetAll();
        int Update(T entity);
        int Delete(T entity);
        int Delete(int Id, byte[] Timestamp);
    }
}
