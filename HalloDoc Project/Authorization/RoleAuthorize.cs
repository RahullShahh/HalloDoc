using BAL.Repository;
using DAL.DataContext;
using DAL.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace HalloDoc_Project.Authorization
{
        public class RoleAuthorize : Attribute, IAuthorizationFilter
        {

            private readonly int _menuId;
            public RoleAuthorize(int menuId = 0)
            {
                _menuId = menuId;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {

                if (_menuId == 0)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "submit_request_page" }));
                    return;
                }

                IJwtToken? _jwtService = context.HttpContext.RequestServices.GetService<IJwtToken>();
                ApplicationDbContext? _dbcontext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();

                if (_jwtService == null || _dbcontext == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "submit_request_page" }));
                    context.HttpContext.Response.Cookies.Delete("jwt");
                    return;
                }

                var token = context.HttpContext.Request.Cookies["jwt"];

                if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "submit_request_page" }));
                    context.HttpContext.Response.Cookies.Delete("jwt");
                    return;
                }

                int roleId = Convert.ToInt32(jwtToken.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value);

                Role? role = _dbcontext.Roles.FirstOrDefault(role => role.Roleid == roleId);

                if (role == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "submit_request_page" }));
                    context.HttpContext.Response.Cookies.Delete("jwt");
                    return;
                }

                IEnumerable<Rolemenu> roleMenus = _dbcontext.Rolemenus.Where(rm => rm.Roleid == roleId);

                if (!roleMenus.Any(rm => rm.Menuid == _menuId))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "AccessDenied" }));
                    return;
                }

            }

        }
    
}
