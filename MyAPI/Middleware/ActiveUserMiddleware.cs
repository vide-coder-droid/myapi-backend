using MyAPI.Repositories;

namespace MyAPI.Middleware
{
    public class ActiveUserMiddleware
    {
        private readonly RequestDelegate _next;

        public ActiveUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository repo)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var username = context.User.Identity?.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var user = await repo.GetUserAsync(username);

                    if (user == null || !user.IsActive)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        await context.Response.WriteAsJsonAsync(new
                        {
                            success = false,
                            message = "Account disabled"
                        });

                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}