using AutoMapper;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductViewModel>();

        CreateMap<Supplier, SupplierViewModel>()
            .ForMember(
                destination => destination.Id,
                options => options.MapFrom(
                    source => source.SupplierId));
    }
}