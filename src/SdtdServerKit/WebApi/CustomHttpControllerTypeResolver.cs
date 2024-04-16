using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace SdtdServerKit.WebApi
{
    public class CustomHttpControllerTypeResolver : DefaultHttpControllerTypeResolver
    {
        public CustomHttpControllerTypeResolver() : base(IsControllerType)
        {
        }

        internal static bool IsControllerType(Type t)
        {
            try
            {
                return
                t != null &&
                t.IsClass &&
                t.IsVisible &&
                !t.IsAbstract &&
                typeof(IHttpController).IsAssignableFrom(t);
            }
            catch
            {
                return false;
            }
        }
    }
}
