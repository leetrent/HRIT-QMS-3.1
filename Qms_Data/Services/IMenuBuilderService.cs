using System.Collections.Generic;
using QmsCore.UIModel;

namespace QmsCore.Services
{
    public interface IMenuBuilderService
    {
        List<ModuleMenuItem> RetrieveMenuForUser(int userId);
    }
}
