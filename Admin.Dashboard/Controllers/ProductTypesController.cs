using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.ProductModules;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admin.Dashboard.Controllers
{
    public class ProductTypesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync();
            return View(types);
        }
        public async Task<IActionResult> Create(ProductType type)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.GetRepository<ProductType, int>().AddAsync(type);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(type);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var type = await _unitOfWork.GetRepository<ProductType, int>().GetByIdAsync(id);
            if (type is null) return NotFound();
            _unitOfWork.GetRepository<ProductType, int>().Remove(type);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
