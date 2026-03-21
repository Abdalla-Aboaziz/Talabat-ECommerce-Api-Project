using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.ProductModules;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admin.Dashboard.Controllers
{
    public class ProductBrandsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductBrandsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();
            return View(brands);
        }
        public async Task<IActionResult> Create(ProductBrand brand)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.GetRepository<ProductBrand, int>().AddAsync(brand);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _unitOfWork.GetRepository<ProductBrand, int>().GetByIdAsync(id);
            if (brand is null) return NotFound();
            _unitOfWork.GetRepository<ProductBrand, int>().Remove(brand);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
