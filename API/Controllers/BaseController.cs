using System;
using API.RequestHelper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    protected async Task<ActionResult> CreatePageResult<T>(IGenericRepository<T> repo, 
    ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseEntity
    {
        var count = await repo.CountAsync(spec);
        var data = await repo.ListAsync(spec);

        return Ok(new Pagination<T>(pageIndex, pageSize, count, data));
    }
}
