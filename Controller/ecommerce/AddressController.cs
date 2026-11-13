using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WepApi.DTOs.ecommerce.Request;
using WepApi.Helpers;
using WepApi.Service.ecommerce.Interface;

namespace WepApi.Controller.ecommerce;

[ApiController]
[Route("api/address")]
[Authorize]
public class AddressController
{
    private readonly IAddressService _service;

    public AddressController(IAddressService service)
    {
        _service = service;
    }

    // [Authorize(Roles = "Admin,SuperAdmin")
    [HttpGet]
    public async Task<IActionResult> GetAddressByPageAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var data = await _service.GetAddressByPageAsync(page, pageSize, cancellationToken);
        return ApiResponse.Success(data);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAddressAsync(CancellationToken cancellationToken = default)
    {
        var data = await _service.GetAllAddressAsync(cancellationToken);
        return ApiResponse.Success(data);
    }

    [Authorize(Roles = "User,admin,superAdmin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAddressByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var data = await _service.GetAddressByIdAsync(id, cancellationToken);
        return ApiResponse.Success(data!);
    }

    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> AddAddressAsync([FromBody] AddressRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _service.AddAddressAsync(request, cancellationToken);
        return ApiResponse.Success(data);
    }
}