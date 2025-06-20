using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectionTutorial
{
    //  The generic CRUD interface
    public interface IService<T>
    {
        IEnumerable<T> Read();         // all records
        T Read(int id);     // one record by primary key
        void Create(T item);
        void Update(T item);
        void Delete(int id);
    }

}