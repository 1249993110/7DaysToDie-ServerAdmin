using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    internal class CustomHttpControllerTypeResolver : DefaultHttpControllerTypeResolver
    {
        public CustomHttpControllerTypeResolver() : base(IsControllerType)
        {
        }

        public static bool IsControllerType(Type t)
        {
            try
            {
                return
                t != null &&
                t.IsClass &&
                t.IsVisible &&
                !t.IsAbstract &&
                t.Assembly == typeof(CustomHttpControllerTypeResolver).Assembly &&
                typeof(IHttpController).IsAssignableFrom(t);
            }
            catch
            {
                return false;
            }
        }
    }
}
