using Printing.AppModels;
using Printing.Models;

namespace Printing.Interfaces
{
    public interface IAuthService
    {
        Users AddUser(Users users);
        string Login(LoginRequest loginRequest);
        Role AddRole(Role role);
        bool AssignRoleToUser(AddUserRole obj);

        Facilities AddFacilities(Facilities facilities);

        bool AssignFacilitiesToUser(AddUserFacilities obj);
    }
}
