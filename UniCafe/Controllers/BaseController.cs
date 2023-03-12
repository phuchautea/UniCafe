using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class BaseController<T> : Controller where T : class
    {
        public readonly ApplicationDbContext Context;

        public readonly IUnitOfWork UnitOfWork;
        public readonly IRepository<T> Repository;
        public UserManager<ApplicationUser> UserManager;
        public RoleManager<IdentityRole> RoleManager;
        public BaseController()
        {
            Context = new ApplicationDbContext();
            UnitOfWork = new UnitOfWork(Context);
            Repository = new Repository<T>(Context);
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
        }
        public void Add(T entity)
        {
            Repository.Add(entity);
            UnitOfWork.BeginTransaction();
            try
            {
                UnitOfWork.Commit();
            }
            catch
            {
                UnitOfWork.Rollback();
                throw;
            }
        }
        public IEnumerable<T> GetAll()
        {
            return Repository.GetAll();
        }

        public T GetById(int Id)
        {
            return Repository.GetById(Id);
        }

        public void Update(T entity)
        {
            Repository.Update(entity);
            UnitOfWork.BeginTransaction();
            try
            {
                UnitOfWork.Commit();
            }
            catch
            {
                UnitOfWork.Rollback();
                throw;
            }
        }

        public void Remove(T entity)
        {
            Repository.Remove(entity);
            UnitOfWork.BeginTransaction();
            try
            {
                UnitOfWork.Commit();
            }
            catch
            {
                UnitOfWork.Rollback();
                throw;
            }
        }
    }
}