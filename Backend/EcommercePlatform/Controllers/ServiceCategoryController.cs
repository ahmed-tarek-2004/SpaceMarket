using Ecommerce.DataAccess.Services.ServiceCategory;
using Ecommerce.Entities.DTO.ServiceCategory;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ServiceCategoryController(ResponseHandler responseHandler, ICategoryService categoryService) : ControllerBase
{
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly ICategoryService _categoryService = categoryService;

    [HttpPost("")]
    [Authorize(Roles = "ServiceProvider")]
    public async Task<IActionResult> AddServiceCategory([FromBody] CreateServiceCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
        }

        var result = await _categoryService.AddServiceCategoryAsync(request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServiceCategories()
    {
        var result = await _categoryService.GetAllServiceCategoriesAsync();
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetServiceCategoryById(Guid id)
    {
        var result = await _categoryService.GetServiceCategoryByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _categoryService.GetServiceCategoryByNameAsync(name);
        return StatusCode((int)result.StatusCode, result);
    }


    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ServiceProvider")]
    public async Task<IActionResult> UpdateServiceCategory(Guid id, [FromBody] UpdateServiceCategoryRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _categoryService.UpdateServiceCategoryAsync(id, dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ServiceProvider")]
    public async Task<IActionResult> DeleteServiceCategory(Guid id)
    {
        var result = await _categoryService.DeleteServiceCategoryAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

}