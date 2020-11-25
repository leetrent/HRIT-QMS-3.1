using QmsCore.UIModel;
using QMS.ViewModels;

namespace QMS.Utils
{
    public interface IUserFormUtil
    {
        void PopulateCheckboxRolesForCreateUser(UserFormViewModel newUser);
        void PopulateCheckboxRolesForUser(UserFormViewModel userFormVM, string[] selectedRoleIdStringArrayForUser);
        User MapToUIModelOnCreate(UserFormViewModel userVM, string[] selectedRoleIdsForUser);
        UserFormViewModel MapToViewModel(User userDB);
    }
}