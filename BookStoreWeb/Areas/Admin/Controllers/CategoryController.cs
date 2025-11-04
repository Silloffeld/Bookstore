using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BookStore.DataAccess;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Bookstore.Utility;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult Index()
        {
            return View(_unitOfWork.Category.GetAll());
        }

        public IActionResult Create() => View(new Category());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            _unitOfWork.Category.Add(category);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var cat = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id) return BadRequest();
            if (!ModelState.IsValid) return View(category);
            if(category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
                return View(category);
            }
            _unitOfWork.Category.update(category);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var cat = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var cat = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (cat == null) return NotFound();
          
            try
            {
                _unitOfWork.Category.Remove(cat);
            }
            catch (Exception e)
            {
                ViewBag.error = e.Message;
                throw;
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
