using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;

namespace UniCafe.Data
{
    public class ApplicationUser : IdentityUser
    {

    }
    //public class ApplicationUser : IdentityUser
    //{
    //    [MaxLength(250)]
    //    [Required]
    //    public string FirstName { get; set; }
    //    public string MiddleName { get; set; }
    //    [MaxLength(250)]
    //    [Required]
    //    public string LastName { get; set; }
    //}
}
