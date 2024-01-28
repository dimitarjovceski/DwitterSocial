using AutoMapper.Configuration.Annotations;
using DwitterSocial.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DwitterSocial.Helpers
{
    public class LogUserActive : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var username = resultContext.HttpContext.User.Identity.Name;
            var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await uow.UserRepository.GetByUsernameAsync(username);
            user.LastActive = DateTime.UtcNow;
            await uow.Complete();
        }
    }
}
