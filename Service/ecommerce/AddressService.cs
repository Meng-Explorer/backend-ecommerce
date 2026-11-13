using AutoMapper;
using WepApi.DTOs.ecommerce.Request;
using WepApi.DTOs.ecommerce.Response;
using WepApi.Exceptions;
using WepApi.Helpers;
using WepApi.Models;
using WepApi.Repository.ecommerce.Interface;
using WepApi.Service.ecommerce.Interface;

namespace WepApi.Service.ecommerce;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<AddressService> _logger;  

    public AddressService(IAddressRepository repository, IMapper mapper, ILogger<AddressService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PageResult<AddressResponse>> GetAddressByPageAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {Page}, PageSize: {PageSize}", page, pageSize);
            throw new ValidationException(new List<string> { "Page and PageSize must be greater than zero." });
        }

        _logger.LogInformation("Getting Stores. Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var address = await _repository.GetAddressByPageAsync(page, pageSize, cancellationToken);
        if (address == null || address.Data == null)
        {
            _logger.LogWarning("Address not found.");
            throw new NotFoundException("Address not found.");
        }
        return new PageResult<AddressResponse>
        {
            PageNumber = address.PageNumber,
            PageSize = address.PageSize,
            TotalRecords = address.TotalRecords,
            TotalPages = address.TotalPages,
            Data = _mapper.Map<List<AddressResponse>>(address.Data)
        };
    }

    public async Task<List<AddressResponse>> GetAllAddressAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all stores.");
        var store = await _repository.GetAllAddressAsync(cancellationToken);
        return _mapper.Map<List<AddressResponse>>(store);
    }

    public async Task<AddressResponse?> GetAddressByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Attempted to get Store with invalid ID: {Id}", id);
            throw new ValidationException(new List<string> { "Id must be greater than zero." });
        }

        var store = await _repository.GetAddressByIdAsync(id, cancellationToken);
        if (store == null)
        {
            _logger.LogWarning("Store with ID {Id} not found.", id);
            throw new NotFoundException("Store not found or no data available.");
        }

        return _mapper.Map<AddressResponse>(store);
    }

    public async Task<AddressResponse> AddAddressAsync(AddressRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            _logger.LogWarning("Attempted to create an Address with a null request.");
            throw new BadRequestException("Request cannot be null or empty.");
        }

        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.AddressLine)) validationErrors.Add("AddressLine is required.");
        if (string.IsNullOrWhiteSpace(request.City)) validationErrors.Add("City is required.");
        if (string.IsNullOrWhiteSpace(request.Country)) validationErrors.Add("Country is required.");
        if (string.IsNullOrWhiteSpace(request.Phone)) validationErrors.Add("Phone is required.");
        if (string.IsNullOrWhiteSpace(request.RecipientName)) validationErrors.Add("Recipient name is required.");
        if (string.IsNullOrWhiteSpace(request.UserId)) validationErrors.Add("UserId is required.");

        if (validationErrors.Any())
        {
            throw new ValidationException(validationErrors);
        }

        _logger.LogInformation("Creating new Address for User: {UserId}, Recipient: {RecipientName}", request.UserId, request.RecipientName);
        var addressEntity = _mapper.Map<Address>(request);
    
        // Default is True
        addressEntity.IsDefault = request.IsDefault; 
        var savedAddress = await _repository.AddAddressAsync(addressEntity, cancellationToken);
        return _mapper.Map<AddressResponse>(savedAddress);
    }

    
}