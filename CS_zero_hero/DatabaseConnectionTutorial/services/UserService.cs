
using DatabaseConnectionTutorial.Utility;  
using DatabaseConnectionTutorial.Services; 

namespace DatabaseConnectionTutorial.Services
{
    public class UserService : GenericCrudService<User>
    {
        public UserService(IConnectionFactory factory)
            : base(factory)
        {
        }

        // We could place any User-specific methods here, e.g.
        // public IEnumerable<User> FindByLastName(string lastName) { ... }
    }
}
