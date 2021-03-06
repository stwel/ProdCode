﻿using ProdCodeApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdCodeApi.Services.Contracts
{
    public interface IProductService
    {
        int CreateProduct(string userEmail, CreateProductModel productModel);
        ProductDetailsModel GetDetailsById(int? productId);
        int EditProduct(EditProductModel editProductModel);
        IEnumerable<ProductCard> GetAllMatches(string searchTerm);
    }
}
