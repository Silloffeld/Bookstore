using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Bookstore.Utility;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult Index()
        {
            return View(_unitOfWork.CoverType.GetAll());
        }

        public IActionResult Create() => View(new CoverType());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType)
        {
            if (!ModelState.IsValid) return View(coverType);
            _unitOfWork.CoverType.Add(coverType);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverType == null) return NotFound();
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CoverType coverType)
        {
            if (id != coverType.Id) return BadRequest();
            if (!ModelState.IsValid) return View(coverType);
            _unitOfWork.CoverType.update(coverType);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverType == null) return NotFound();
            return View(coverType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverType == null) return NotFound();
          
            try
            {
                _unitOfWork.CoverType.Remove(coverType);
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
