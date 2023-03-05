using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class PropertyProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PropertyProduct> _propertyProductRepository;
        private readonly ApplicationDbContext _context;

        public PropertyProductController()
        {
            _context = new ApplicationDbContext();
            _unitOfWork = new UnitOfWork(_context);
            _propertyProductRepository = new Repository<PropertyProduct>(_context);
        }

        public void AddPropertyProduct(PropertyProduct propertyProduct)
        {
            _propertyProductRepository.Add(propertyProduct);
            _unitOfWork.BeginTransaction();
            try
            {
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
        public IEnumerable<PropertyProduct> GetAllPropertyProducts()
        {
            return _propertyProductRepository.GetAll();
        }

        public PropertyProduct GetPropertyProductById(int Id)
        {
            return _propertyProductRepository.GetById(Id);
        }

        public void UpdatePropertyProduct(PropertyProduct propertyProduct)
        {
            _propertyProductRepository.Update(propertyProduct);
            _unitOfWork.BeginTransaction();
            try
            {
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void DeletePropertyProduct(PropertyProduct propertyProduct)
        {
            _propertyProductRepository.Remove(propertyProduct);
            _unitOfWork.BeginTransaction();
            try
            {
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        // List

        public ActionResult Index()
        {
            var listPropertyProduct = GetAllPropertyProducts().ToList();
            return View(listPropertyProduct);
        }
        // Details
        public ActionResult Details(int Id)
        {
            PropertyProduct propertyProduct = GetPropertyProductById(Id);
            return View(propertyProduct);
        }
        // Add
        [HttpPost]
        public ActionResult Create(PropertyProduct propertyProduct)
        {
            AddPropertyProduct(propertyProduct);
            return RedirectToAction("Index", "PropertyProduct");
        }
        // Update
        public ActionResult Edit(int Id)
        {
            PropertyProduct propertyProduct = GetPropertyProductById(Id);
            return View(propertyProduct);
        }
        [HttpPost]
        public ActionResult Edit(PropertyProduct propertyProduct)
        {
            UpdatePropertyProduct(propertyProduct);
            return RedirectToAction("Index", "PropertyProduct");
        }
        //Remove
        public ActionResult Delete(int Id)
        {
            PropertyProduct propertyProduct = GetPropertyProductById(Id);
            return View(propertyProduct);
        }
        [HttpPost]
        public ActionResult Delete(PropertyProduct propertyProduct)
        {
            DeletePropertyProduct(propertyProduct);
            return RedirectToAction("Index", "PropertyProduct");
        }
    }
}