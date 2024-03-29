﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        public HomeController()
        {
            _context = new ApplicationDbContext();
            _categoryRepository = new Repository<Category>(_context);
            _productRepository = new Repository<Product>(_context);
        }
        public ActionResult Index()
        {
            var showProducts = _context.Products.Where(c => c.Show == 1).ToList();
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            ViewBag.showProducts = showProducts;
            var products = _productRepository.GetAll().ToList().Take(10);
            return View(products);
        }
        //[Route("Collection/{id}")]
        public ActionResult Collection(string id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Slug == id);
            var products = _context.Products.Where(c => c.Category.Id == category.Id).ToList();
            ViewBag.Products = products;
            ViewBag.Category = category;
            return View();
        }

        public ActionResult Menu()
        {
            var category = _categoryRepository.GetAll();
            return View(category);
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult KhuyenMai() 
        {
            return View();
        }
        //public ActionResult GetProduct()
        //{
           
        //    return RedirectToAction("Index","Home");
        //}
    }
}