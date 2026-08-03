using AutoMapper;
using BusinessObjectsLayer.DTOs;
using BusinessObjectsLayer.Entities;

namespace InvoiceProjectAPI.Mapping
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<InvoiceDto, InvoiceModel>();
            CreateMap<InvoiceProductDto, InvoiceProductModel>();
            CreateMap<InvoiceAttachmentDto, InvoiceAttachmentModel>();

            CreateMap<InvoiceModel, InvoiceListDto>();
            CreateMap<InvoiceProductModel, InvoiceProductDto>();
            CreateMap<InvoiceAttachmentModel, InvoiceAttachmentDto>();
        }
    }
}
