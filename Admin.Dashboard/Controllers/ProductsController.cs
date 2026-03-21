using Admin.Dashboard.Helpers;
using Admin.Dashboard.Models.Products;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.ProductModules;
using ECommerce.Service.Specifications;
using ECommerce.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace Admin.Dashboard.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public ProductsController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            var ProductsRepo = _unitOfWork.GetRepository<Product, int>();
            var QueryPrams = new ProductQueryParams();

            var spec = new ProductWithBrandsAndTypeSpecification(QueryPrams, true);
            var products = await ProductsRepo.GetAllAsync(spec);
            ViewBag.BaseUrl = _configuration["URLs:BaseURL"];
            var ProductsViewModel = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                PictureUrl = p.PictureUrl,
                BrandId = p.BrandId,
                TypeId = p.TypeId,
                Brand = p.ProductBrand,
                Type = p.ProductType

            });

            return View(ProductsViewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image is not null) model.PictureUrl = PictureSettings.UploadFile(model.Image, "products", _configuration["URLs:ApiWwwRoot"]);
                // else => Add default image
                var MappedProduct = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    PictureUrl = model.PictureUrl,
                    BrandId = model.BrandId,
                    TypeId = model.TypeId
                };
                var ProductsRepo = _unitOfWork.GetRepository<Product, int>();
                await ProductsRepo.AddAsync(MappedProduct);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(id);
            if (product == null) return NotFound();
            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                PictureUrl = product.PictureUrl,
                BrandId = product.BrandId,
                TypeId = product.TypeId
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id ,ProductViewModel model) 
        {
            if(id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(id);
                if (product == null) return NotFound();
                if (model.Image is not null) 
                {
                    var apiWwwRoot = _configuration["URLs:ApiWwwRoot"];
                    // Delete the old image from the server if it exists
                    if (!string.IsNullOrEmpty(product.PictureUrl))
                    {
                       
                        PictureSettings.DeleteFile("products", Path.GetFileName(product.PictureUrl), apiWwwRoot); 
                    }

                    // Upload the new image and update the PictureUrl
                    product.PictureUrl = PictureSettings.UploadFile(model.Image, "products", apiWwwRoot);

                } 
                // else => keep the old image
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
               
                product.BrandId = model.BrandId;
                product.TypeId = model.TypeId;
                var ProductsRepo = _unitOfWork.GetRepository<Product, int>();
                ProductsRepo.Update(product);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
             }
             return View(model);

        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(id);
            if (product is null) return NotFound();

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                PictureUrl = product.PictureUrl,
                BrandId = product.BrandId,
                TypeId = product.TypeId,
                Brand = product.ProductBrand,
                Type = product.ProductType
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, ProductViewModel model)
        {
            if (id != model.Id) return NotFound();

            try
            {
                var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(id);
                if (product is null) return NotFound();

                var apiWwwRoot = _configuration["URLs:ApiWwwRoot"];
                // Delete the product image from the server if it exists
                if (!string.IsNullOrEmpty(product.PictureUrl))

                    PictureSettings.DeleteFile("products", Path.GetFileName(product.PictureUrl), apiWwwRoot);

                // Delete the product from the database
                _unitOfWork.GetRepository<Product, int>().Remove(product);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(model);
            }
        }



    }
}
