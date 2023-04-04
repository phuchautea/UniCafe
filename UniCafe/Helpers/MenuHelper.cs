using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UniCafe.Models;

namespace UniCafe.Helpers
{
    public static class MenuHelper
    {
        public static MvcHtmlString Menu(this HtmlHelper helper)
        {
            string hostUrl = HttpContext.Current.Request.Url.Authority;
            string host = "https://"+hostUrl+"";

            StringBuilder result = new StringBuilder();
            using (var context = new ApplicationDbContext())
            {
                var categories = context.Categories.Where(c => c.ParentId == 0).ToList();
                
                foreach (var category in categories)
                {
                    result.Append("<li class='lv2_title'>");
                    result.AppendFormat("<a href='{0}/Collections/{1}'>{2}</a>", host, category.Slug, category.Name);
                    result.Append("<ul class='menu_child_lv3'>");
                    var subCategories = context.Categories.Where(c => c.ParentId == category.Id).ToList();

                    if (subCategories.Count > 0)
                    {

                        foreach (var subCategory in subCategories)
                        {
                            result.AppendFormat("<li class='lv3_title'><a href='{0}/Collections/{1}'>{2}</a></li>", host, subCategory.Slug, subCategory.Name);
                        }

                    }

                    result.Append("</ul></li>");
                }
            }

            return new MvcHtmlString(result.ToString());
        }
    }
}
