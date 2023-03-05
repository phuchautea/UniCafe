using UniCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using System.Data.Entity;


namespace UniCafe.Controllers
{
    public class CategoryController : MasterController<Category>
    {
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly IRepository<Category> _categoryRepository;
        //private readonly ApplicationDbContext _context;

        //public CategoryController()
        //{
        //    _context = new ApplicationDbContext();
        //    _unitOfWork = new UnitOfWork(_context);
        //    _categoryRepository = new Repository<Category>(_context);
        //}
        //public void AddCategory(Category category)
        //{
        //    Repository.Add(category);
        //    UnitOfWork.BeginTransaction();
        //    try
        //    {
        //        UnitOfWork.Commit();
        //    }
        //    catch
        //    {
        //        UnitOfWork.Rollback();
        //        throw;
        //    }
        //}
        //public IEnumerable<Category> GetAllCategories()
        //{
        //    return Repository.GetAll();
        //}

        //public Category GetCategoryById(int Id)
        //{
        //    return Repository.GetById(Id);
        //}

        //public void UpdateCategory(Category category)
        //{
        //    Repository.Update(category);
        //    UnitOfWork.BeginTransaction();
        //    try
        //    {
        //        UnitOfWork.Commit();
        //    }
        //    catch
        //    {
        //        UnitOfWork.Rollback();
        //        throw;
        //    }
        //}

        //public void DeleteCategory(Category category)
        //{
        //    //Category category = _categoryRepository.GetById(Id);
        //    Repository.Remove(category);
        //    UnitOfWork.BeginTransaction();
        //    try
        //    {
        //        UnitOfWork.Commit();
        //    }
        //    catch
        //    {
        //        UnitOfWork.Rollback();
        //        throw;
        //    }
        //}
        public ActionResult Index()
        {
            var listCategory = GetAll().ToList();
            return View(listCategory);
        }
        public ActionResult Details(int Id)
        {
            Category category = GetById(Id);
            return View(category);
        }
        [HttpPost]
        public ActionResult Create(Category category)
        {
            Add(category);
            return RedirectToAction("Index", "Category");
        }
        public ActionResult Edit(int Id)
        {
            Category category = GetById(Id);
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            Update(category);
            return RedirectToAction("Index", "Category");
        }
        public ActionResult Delete(int Id)
        {
            Category p = GetById(Id);
            return View(p);
        }
        [HttpPost]
        public ActionResult Delete(Category category)
        {
            Remove(category);
            return RedirectToAction("Index", "Category");
        }
    }
}