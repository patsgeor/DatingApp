using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("This was not a good request");//400
    }

     [HttpGet("auth")]
    public IActionResult GetAuth()
    {
        return Unauthorized(); //401
    }

    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    {
        return NotFound();//404
    }

     [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        throw new Exception("This is a server error");//500
    }

    
}
