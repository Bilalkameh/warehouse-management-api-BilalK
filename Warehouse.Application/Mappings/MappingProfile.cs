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
        
        CreateMap<ShipmentProduct, ShipmentProductViewModel>()
            .ForMember(
                destination => destination.ProductName,
                options => options.MapFrom(
                    source => source.Product.Name))
            .ForMember(
                destination => destination.SKU,
                options => options.MapFrom(
                    source => source.Product.SKU));

        CreateMap<Shipment, ShipmentViewModel>()
            .ForMember(
                destination => destination.SupplierName,
                options => options.MapFrom(
                    source => source.Supplier.Name));
    }
}