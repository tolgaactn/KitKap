using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.ProductImagesDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Interfaces
{
    public interface IProductImageService
    {
        Task AddImagesAsync(long productId, List<IFormFile> images);
        Task SetMainImageAsync(long selectedImageId, long productId);
        Task MarkAsDeletedAsync(List<long> imageIds);
        Task<List<RequestProductImageDto>> GetByIdProductImagesAsync(long id);
    }

}

