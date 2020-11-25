//using System;
//using System.Text;
using System.Collections.Generic;
using QmsCore.UIModel;

namespace QMS.Utils
{
    public static class MenuUtil
    { 
        public static string FindControllerForUseCase(string useCase, List<ModuleMenuItem> moduleMenuItems)
        {
            //string logSnippet = new StringBuilder("[")
            //        .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
            //        .Append("][MenuUtil][FindControllerForUseCase] => ")
            //        .ToString();

            //Console.WriteLine(logSnippet + $"(useCase): '{useCase}'");

            foreach (ModuleMenuItem moduleMenuItem in moduleMenuItems)
            {
                foreach (MenuItem menuItem in moduleMenuItem.MenuItems)
                {
                    if (menuItem.UseCase != null
                            && menuItem.UseCase.Equals(useCase))
                    {
                        return menuItem.Controller;
                    }

                }
            }
            return null;
        }
    }
}