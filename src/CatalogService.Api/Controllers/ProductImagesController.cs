using CatalogService.Application.ProductImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/v1/products/{productId:guid}/images")]
public sealed class ProductImagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductImagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductImageDto>>> Get(Guid productId, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetProductImagesQuery(productId), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductImageDto>> Add(Guid productId, ProductImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddProductImageCommand(productId, request.Url, request.AltText, request.SortOrder, request.IsPrimary),
            cancellationToken);

        return CreatedAtAction(nameof(Get), new { productId }, result);
    }

    [HttpPut("{imageId:guid}/primary")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetPrimary(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SetPrimaryProductImageCommand(productId, imageId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{imageId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductImageCommand(productId, imageId), cancellationToken);
        return NoContent();
    }
}
