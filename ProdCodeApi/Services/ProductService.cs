﻿using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using ProdCodeApi.Data;
using ProdCodeApi.Data.Models;
using ProdCodeApi.Models;
using ProdCodeApi.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdCodeApi.Services
{
    public class ProductService : IProductService
    {
        private IConfiguration configuration;
        private ProdCodeDbContext db;

        public ProductService(IConfiguration configuration, ProdCodeDbContext db)
        {
            this.configuration = configuration;
            this.db = db;
        }

        public int CreateProduct(string userEmail, CreateProductModel productModel)
        {
            User author = this.db.Users.FirstOrDefault(u => u.Email == userEmail);

            Account account = new Account(
            configuration["Cloudinary:CloudName"],
            configuration["Cloudinary:ApiKey"],
            configuration["Cloudinary:ApiSecret"]);

            Cloudinary cloudinary = new Cloudinary(account);
            string fileName = $"{Guid.NewGuid().ToString()}{System.IO.Path.GetExtension(productModel.ProductImage.FileName)}";
            using (Stream imageStream = productModel.ProductImage.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, imageStream),
                    Folder = "ProdCode"
                };
                var uploadResult = cloudinary.Upload(uploadParams);
                string imageUrl = uploadResult.JsonObj.Value<string>("url");
                Product newProduct = new Product()
                {
                    Name = productModel.ProductName,
                    Code = productModel.ProductCode,
                    ImageUrl = imageUrl,
                    AuthorId = author.Id,
                    CreatedDate = DateTime.UtcNow,
                    DisqusId = Guid.NewGuid()
                };

                this.db.Products.Add(newProduct);
                this.db.SaveChanges();

                return newProduct.Id;
            }
        }

        public int EditProduct(EditProductModel editProductModel)
        {
            Account account = new Account(
            configuration["Cloudinary:CloudName"],
            configuration["Cloudinary:ApiKey"],
            configuration["Cloudinary:ApiSecret"]);

            Cloudinary cloudinary = new Cloudinary(account);
            string fileName = $"{Guid.NewGuid().ToString()}{System.IO.Path.GetExtension(editProductModel.ProductImage.FileName)}";
            using (Stream imageStream = editProductModel.ProductImage.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, imageStream),
                    Folder = "ProdCode"
                };
                var uploadResult = cloudinary.Upload(uploadParams);
                string imageUrl = uploadResult.JsonObj.Value<string>("url");
                Product editedProduct = this.db.Products.Single(p => p.Id == editProductModel.Id);

                editedProduct.Name = editProductModel.ProductName;
                editedProduct.Code = editProductModel.ProductCode;
                editedProduct.ImageUrl = imageUrl;

                this.db.SaveChanges();

                return editedProduct.Id;
            }
        }

        public IEnumerable<ProductCard> GetAllMatches(string searchTerm)
        {
            return this.db.Products.Where(p => p.Name.Contains(searchTerm) && p.isArchived != true)
                .OrderByDescending(p => Guid.NewGuid().ToString())
                .Take(string.IsNullOrEmpty(searchTerm) ? 12 : int.MaxValue)
                .ProjectTo<ProductCard>().ToList();
        }

        public ProductDetailsModel GetDetailsById(int? productId)
        {
            if (productId.HasValue && this.db.Products.Any(p => p.Id == productId && p.isArchived != true))
            {
                return this.db.Products.Where(p => p.Id == productId).ProjectTo< ProductDetailsModel>().FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}