using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BasarSoftTask3_API.Filters
{
    public class RoleAuthorizationFilter : IAsyncActionFilter
    {
        private readonly string _requiredRole;

        public RoleAuthorizationFilter(string requiredRole)
        {
            _requiredRole = requiredRole;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            // JWT token'dan kullanıcının rollerini alın
            var roles = context.HttpContext.User.Claims
                .Where(c => c.Type == "role")
                .Select(c => c.Value)
                .ToList();
            Console.WriteLine(roles);

            // Kullanıcı rollerinden gerekli rolü kontrol edin
            if (roles.Contains(_requiredRole))
            {
                // Gerekli rol kullanıcıya atanmışsa, devam edin
                await next();
            }
            else
            {
                // Gerekli rol kullanıcıya atanmamışsa, 401 Unauthorized hatası döndürün
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
