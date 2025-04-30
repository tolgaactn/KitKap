using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.ProductImagesDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Concretes
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _uow;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ProductImageService(IUnitOfWork uow, IFileService fileService, IMapper mapper)
        {
            _uow = uow;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task AddImagesAsync(long productId, List<IFormFile> images)
        {
            var productImages = new List<ProductImage>();

            foreach (var image in images)
            {
                var imageUrl = await _fileService.UploadFileAsync(image, "uploads/products");

                productImages.Add(new ProductImage
                {
                    ProductId = productId,
                    ImageUrl = imageUrl,
                    AltText = Path.GetFileNameWithoutExtension(image.FileName),
                    IsMain = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _uow.GetRepository<ProductImage>().AddRangeAsync(productImages);
            await _uow.CommitAsync();
        }

        public async Task SetMainImageAsync(long selectedImageId, long productId)
        {
            var images = await _uow.GetRepository<ProductImage>()
                .GetAll(x => x.ProductId == productId && !x.IsDeleted);

            foreach (var image in images)
            {
                image.IsMain = (image.Id == selectedImageId);
                image.UpdatedAt = DateTime.UtcNow;
            }

            await _uow.CommitAsync();
        }

        public async Task MarkAsDeletedAsync(List<long> imageIds)
        {
            var images = await _uow.GetRepository<ProductImage>()
                .GetAll(x => imageIds.Contains(x.Id));

            foreach (var image in images)
            {
                image.IsDeleted = true;
                image.UpdatedAt = DateTime.UtcNow;
            }

            await _uow.CommitAsync();
        }

        public async Task<List<RequestProductImageDto>> GetByIdProductImagesAsync(long id)
        {
            var images = await _uow.GetRepository<ProductImage>().GetAll(b => b.ProductId == id);
            return _mapper.Map<List<RequestProductImageDto>>(images);
        }
    }

}
