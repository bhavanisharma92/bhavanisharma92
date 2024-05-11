using Bhaktimarg.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Bhaktimarg.Controllers
{
    public class BlogController : Controller
    {
        BhaktimargEntities db = new BhaktimargEntities();
        [HttpGet]
        [Route("blog-List")]
        public ActionResult Index()
        {
            
            var res = db.Blogs.ToList();
            List<productdetailsdto> blogdtos = new List<productdetailsdto>();
            foreach (var data in res)
            {
                productdetailsdto blogdto = new productdetailsdto
                {
                    Image = data.ImageUrl,
                    Title = data.Title,
                    Description = data.Descriptions,
                    Url = GenerateItemNameAsParam(data.Url)
                };
                blogdtos.Add(blogdto);
            }
            return View(blogdtos);

        }
        [HttpGet]
        [Route("LoginPage")]
        public  ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("LoginPage")]
        public ActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                using (BhaktimargEntities db = new BhaktimargEntities())
                {
                    //var obj1 = db.Logins.Where(x => x.UserName == objUser.UserName && x.Password == objUser.Password).FirstOrDefault();
                    var obj = db.Logins.Where(a => a.UserName.Equals(login.UserName) && a.Password.Equals(login.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.UserId.ToString();
                        Session["UserName"] = obj.UserName.ToString();
                        return Redirect("CareerA");
                    }
                }
            }
            return View(login);
        }

        [HttpGet]
        [Route("CareerA")]
        public ActionResult CareerA()
        {

            if (Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                BhaktimargEntities db = new BhaktimargEntities();
                Login login = db.Logins.Where(x => x.UserId == userid).FirstOrDefault();
                byte roleid = (byte)login.RoleId;
                List<MapMenuToRole> mapMenuToRole = db.MapMenuToRoles.Where(x => x.RoleId == roleid).ToList();
                List<Menu> menus = new List<Menu>();
                if (mapMenuToRole != null)
                {
                    foreach (var data in mapMenuToRole)
                    {
                        Menu menuss = db.Menus.Where(x => x.Id == data.MenuId).FirstOrDefault();
                        menus.Add(menuss);
                    }
                }
                //TempData["key"] = FireBaseKey;

                return View(menus);
            }
            else
            {
                return RedirectToAction("Login");
            }





        }

        [HttpGet]
        [Route("add-blog"), Route("add-blog/{id}")]
        public ActionResult AddBlog(int? id)
        {
            if (Session["UserID"] != null)
            {
                blogdto blogdto = new blogdto();
                BhaktimargEntities db = new BhaktimargEntities();
                Blog blog = db.Blogs.Where(x => x.Id == id).FirstOrDefault() ?? new Blog();
                if (blog != null)
                {
                    blogdto.ID = blog.Id;
                    blogdto.Description = blog.Descriptions;
                    blogdto.Title = blog.Title;
                    blogdto.Url = blog.Url;
                    blogdto.MetaTag = blog.MetaTag;
                    blogdto.IsActive = blog.IsActive ?? false;
                    blogdto.ShortDescription = blog.ShortDescription;
                    //blogdto.Image = blog.Image;
                }
                return View(blogdto);
            }
            else
            {
                return Redirect("LoginPage");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("add-blog"), Route("add-blog/{id}")]
        public ActionResult AddIndex(blogdto model)
        {
            if (Session["UserID"] != null)
            {
                BhaktimargEntities db = new BhaktimargEntities();
                Blog obj = db.Blogs.Where(x => x.Id == model.ID).FirstOrDefault() ?? new Blog();
                if (ModelState.IsValid)
                {

                    obj.Url = model.Url == null ? model.Title.Replace(" ", "-") : model.Url.Replace(" ", "-");
                    obj.Title = model.Title;
                    obj.Descriptions = model.Description;
                    obj.ShortDescription = model.ShortDescription;
                    obj.MetaTag = model.MetaTag;
                    obj.IsActive = model.IsActive;
                    if (model.Image != null)
                    {
                        string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/images/");
                        string filename = Path.GetFileName(model.Image.FileName);
                        string fullPath = Path.Combine(path, filename);
                        model.Image.SaveAs(fullPath);
                        obj.ImageUrl = "/Content/images/" + filename;
                    }
                    if (obj.Id == 0)
                    {
                        obj.AddedDate = DateTime.Now;
                        db.Blogs.Add(obj);
                    }
                    db.SaveChanges();
                    ModelState.Clear();
                }
                return RedirectToAction("Productlist");
            }
            else
            {
                return Redirect("LoginPage");
            }
        }
        [HttpGet]
        [Route("Product-list")]
        public ActionResult Productlist()
        {
            if (Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                BhaktimargEntities db = new BhaktimargEntities();
                Login login = db.Logins.Where(x => x.UserId == userid).FirstOrDefault();
                byte roleid = (byte)login.RoleId;
                if (roleid == 1 || roleid == 4)
                {
                    var res = db.Blogs.ToList();
                    List<productdetailsdto> blogdtos = new List<productdetailsdto>();
                    foreach (var data in res)
                    {
                        productdetailsdto blogdto = new productdetailsdto
                        {
                            ID = data.Id,
                            Image = data.ImageUrl,
                            Title = data.Title,
                            Description = data.Descriptions,
                            IsActive = data.IsActive ?? false,
                            Url = GenerateItemNameAsParam(data.Url)
                        };
                        blogdtos.Add(blogdto);
                    }
                    return View(blogdtos);
                }
                else
                {
                    return Redirect("LoginPage");
               }

            }
            else
            {
                return Redirect("LoginPage");
            }
        }
        [HttpGet]
        [Route("blog/{id}")]
        public ActionResult Details(string id)
        {
            //if (id == null)
            //{
            //    return Redirect("~/blog");
            //}
            BhaktimargEntities db = new BhaktimargEntities();
            var res = db.Blogs.Where(x => x.Url == id && x.IsActive == true).FirstOrDefault();
            //if (res == null)
            //{
            //    return Redirect("~/blog");
            //}
            productdetailsdto blogdto = new productdetailsdto
            {
                Image = res.ImageUrl ?? "/content/image/blog-post-1.jpg",
                Title = res.Title,
                Description = res.Descriptions,
                Url = res.Url,
                Dates = res.AddedDate?.ToString("dd MMMM yyyy"),
                MetaTag = res.MetaTag ?? ""
            };
            ViewBag.Slug = id;
            var res1 = db.Blogs.Where(x => x.IsActive == true).Take(5).OrderByDescending(x => x.AddedDate).ToList();
            ViewBag.Data = res1;
            return View(blogdto);
        }

        [HttpGet]
        [Route("delete-blog/{id}")]
        public ActionResult Deleteblog(int? id)
        {
            if (id > 0)
            {
                BhaktimargEntities db = new BhaktimargEntities();
                Blog blog = db.Blogs.Where(x => x.Id == id).FirstOrDefault();
                if (blog == null)
                {
                    return Redirect("~/Product-list");
                }
                else
                {
                    db.Blogs.Remove(blog);
                    db.SaveChanges();
                }
            }
            return Redirect("~/Product-list");
        }


        private string RemoveAccent(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }



        [Route("blog"), Route("blog/{id}")]

        public ActionResult blog(int? id)
        {
            try

            {
                BhaktimargEntities db = new BhaktimargEntities();
                List<Blog> res = db.Blogs.Where(x => x.IsActive == true).OrderByDescending(x => x.AddedDate).ToList();
                decimal totalnextcount = 0;

                BlogListdto blogListdto = new BlogListdto();
                List<productdetailsdto> blogdtos = new List<productdetailsdto>();
                foreach (var data in res)
                {
                    productdetailsdto blogdto = new productdetailsdto
                    {
                        Image = data.ImageUrl ?? "/content/image/blog-post-1.jpg",
                        Title = data.Title,
                        Description = data.Descriptions,
                        Url = GenerateItemNameAsParam(data.Url),
                        Dates = data.AddedDate?.ToString("dd MMMM yyyy")
                    };
                    blogdtos.Add(blogdto);
                }
                var res1 = db.Blogs.Where(x => x.IsActive == true).Take(10).OrderByDescending(x => x.AddedDate).ToList();
                ViewBag.Data = res1;
                var res2 = db.Blogs.Where(x => x.IsActive == true).OrderByDescending(x => x.Id).FirstOrDefault();
                if (res2 != null)
                {
                    productdetailsdto productdetailsdto = new productdetailsdto
                    {
                        Description = res2.Descriptions,
                        ShortDescription = res2.ShortDescription,
                        Image = res2.ImageUrl ?? "/content/image/blog-post-1.jpg",
                        Title = res2.Title,
                        Url = res2.Url
                    };
                    TempData["Blogs"] = productdetailsdto;
                }
                blogListdto.Blogs = blogdtos;
                blogListdto.Moreid = totalnextcount;

                return View(blogListdto);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private string GenerateItemNameAsParam(string Name)
        {
            string phrase = string.Format("{0}", Name);
            string str = RemoveAccent(phrase).ToLower();
            // invalid chars str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); 
            // convert multiple spaces into one space str = Regex.Replace(str, @"\s+", " ").Trim(); 
            // cut and trim str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); 
            str = Regex.Replace(str, @"\s", "-");
            return str;

            //string phrase = string.Format("{0}-{1}");// Creates in the specific pattern  
            //string str = GetByteArray(phrase).ToLower();
            //str = Regex.Replace(str, @"[^a-z0-9\s-]", "");// Remove invalid characters for param  
            //str = Regex.Replace(str, @"\s+", "-").Trim(); // convert multiple spaces into one hyphens   
            //str = str.Substring(0, str.Length <= 30 ? str.Length : 30).Trim(); //Trim to max 30 char  
            //str = Regex.Replace(str, @"\s", "-"); // Replaces spaces with hyphens     
            //return str;
        }

        [HttpGet]
        [Route("Logout")]
        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session.Abandon();
            return Redirect("LoginPage");
            //Session.Remove("email");
            //return RedirectToAction("LOGIN");
        }
        [HttpGet]
        [Route("EnquiryList")]
        public ActionResult EnquiryList()
        {
            int idd = Convert.ToInt32 (Session["UserID"]);
            //Session.Abandon();
            //return Redirect("LoginPage");
            //Session.Remove("email");
            //return RedirectToAction("LOGIN");
            if (idd==1)
            {
                var list = db.Contacts.ToList();
                return View(list);
            }
            else
            {
                return Redirect("LoginPage");
            }
            
           
        }
        [HttpGet]
        [Route("contact-us")]
        public ActionResult EnquiryForm()
        {
            return View();
        }

        [HttpPost]
        [Route("contact-us")]
        public JsonResult EnquiryForm(Contactdto contact)
        {
            if (contact.Name != "" || contact.Email != "" || contact.MobileNumber != "" || contact.HostingType != "" || contact.Message != "")
            {
                BhaktimargEntities db = new BhaktimargEntities();
                DateTime dt = DateTime.Now;
                string da = DateTime.Now.ToString("yyyy-MM-dd");


                //var user = db.Contacts.AsEnumerable().Where(x => ((x.Contactdate ?? DateTime.Now).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd")) && (x.Email == cu.Email)).FirstOrDefault();
                var mob = db.Contacts.Where(x => x.MobileNumber == contact.MobileNumber && x.Email == contact.Email).FirstOrDefault();
                var isEmailAlreadyExists = db.Contacts.Where(x => x.MobileNumber == contact.MobileNumber || x.Email == contact.Email).ToList();
                if (mob == null && isEmailAlreadyExists.Count == 0)
                {

                    try
                    {
                        Contact contactu = new Contact();
                        contactu.Name = contact.Name;
                        contactu.Email = contact.Email;
                        contactu.MobileNumber = contact.MobileNumber;
                        contactu.HostingType = contact.HostingType;
                        contactu.Message = contact.Message;
                        DateTime Contactdate = DateTime.Now;
                        DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                        contactu.CreatedDateTime = Contactdate;
                        //contactu.Company = "Ezytm Technology";
                        //contactu.Ipaddress = "contact-usSS";


                        db.Contacts.Add(contactu);

                        db.SaveChanges();

                        return Json(1, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {

                        return Json(0, JsonRequestBehavior.AllowGet);


                    }

                }
                else if (isEmailAlreadyExists != null)
                {
                    return Json(4, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { err = 0, mess = "Form Already  Submitted  Please try after 24 hours!" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(2, JsonRequestBehavior.AllowGet);
            }

        }
    }
}