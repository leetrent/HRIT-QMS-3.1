using QmsCore.UIModel;
using QMS.ViewModels;

namespace QMS.Utils
{
    public interface IUAUserUtil
    {
        UAUserViewModel MapToViewModel(User dbUser);
        void PopulateCheckboxRolesForCreateUser(UAUserViewModel newUser);
    }
}