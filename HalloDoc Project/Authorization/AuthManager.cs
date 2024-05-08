
using BAL.Repository;
using DAL.DataContext;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

public class CustomAuthorize : Attribute, IAuthorizationFilter
{
    private readonly string _role;
    private readonly ApplicationDbContext _context;
    public CustomAuthorize(string role = "")
    {
        this._role = role;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var jwtService = context.HttpContext.RequestServices.GetService<IJwtToken>();
        var email = context.HttpContext.Session.GetString("Email");
        var role = context.HttpContext.Session.GetString("Role");

        if (jwtService == null)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Guest", Action = "submit_request_page" }));
            return;
        }

        var request = context.HttpContext.Request;
        //getting the value of cookie
        var token = request.Cookies["jwt"];
        // Redirect to login if not logged in 
        if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Guest", Action = "submit_request_page" }));
            return;
        }
        //role claim is in key value pair

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Role");
        // Access Denied if Role Not matched
        if (roleClaim == null)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Guest", Action = "submit_request_page" }));
            return;
        }

        if (string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Guest", Action = "AccessDenied" }));
        }
    }
}