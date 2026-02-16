using System;
using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if(context.HttpContext.User.Identity?.IsAuthenticated != true) return;

        var memberId= resultContext.HttpContext.User.GetMemberId();

        // θελω να παρω το dbContext απο το service container .
        // το services ειναι ενα property του HttpContext που μας επιτρεπει να παρουμε οτι service εχουμε κανει register στο startup
        // για να κανω update το lastActive του user που εκανε το request
        var dbContext= resultContext.HttpContext.RequestServices.GetRequiredService<AppDbcontext>();

        // εδω χρησιμοποιω το ExecuteUpdateAsync για να κανω update το lastActive χωρις να χρειαστει να φερω ολη την οντοτητα του user απο τη βαση
        await dbContext.Members.Where(x => x.Id==memberId).ExecuteUpdateAsync(setter => setter.SetProperty(x =>x.LastActive, DateTime.UtcNow));
    }
}
