using BusTicketingAI.Application.Interfaces;
using MediatR;

namespace BusTicketingAI.Application.Features.Cities.Queries.GetAllCities;

public record GetAllCitiesQuery : IRequest<List<CityResponseDto>>;

public class GetAllCitiesQueryHandler : IRequestHandler<GetAllCitiesQuery, List<CityResponseDto>>
{
    private readonly ICityRepository _cityRepository;

    public GetAllCitiesQueryHandler(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<List<CityResponseDto>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
    {
        var cities = await _cityRepository.GetAllCitiesOrderedAsync(cancellationToken);

        return cities.Select(c => new CityResponseDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
    }
}
