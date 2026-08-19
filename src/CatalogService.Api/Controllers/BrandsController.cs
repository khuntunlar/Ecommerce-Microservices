using CatalogService.Application.Brands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/v1/brands")]
public sealed class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<BrandDto>>> Get(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetBrandsQuery(), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandDto>> Create(BrandRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateBrandCommand(request.Name, request.Slug), cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, BrandRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateBrandCommand(id, request.Name, request.Slug, request.IsActive), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteBrandCommand(id), cancellationToken);
        return NoContent();
    }
}
