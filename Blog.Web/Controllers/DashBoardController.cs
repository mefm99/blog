using Blog.Core.Filters;
using Blog.Core.Helpers;
using Blog.Core.Interfaces;
using Blog.Core.Models;
using Blog.Core.ViewModels;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    [AuthorizeDashboard]
    public class DashBoardController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IUserRepository _userRepository;
        private readonly IArticleRepository _articleRepository;
        public DashBoardController(IWebHostEnvironment env,
            IUserRepository userRepository,
            IArticleRepository articleRepository)
        {
            _env = env;
            _userRepository = userRepository;
            _articleRepository = articleRepository;
        }
        [NonAction]
        private byte[] CompressImage(IFormFile fileImage)
        {
            var wwwroot = _env.WebRootPath;
            var extension = Path.GetExtension(fileImage.FileName);
            var fileName = "img_" + DateTime.Now.ToString("yymmssfff") + extension;
            var filePath = Path.Combine(wwwroot, "Images", fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileImage.CopyTo(fileStream);
            }
            var imageOptimizer = new FileInfo(filePath);
            var optimizer = new ImageOptimizer();
            optimizer.LosslessCompress(imageOptimizer);
            imageOptimizer.Refresh();
            var img = System.IO.File.OpenRead(imageOptimizer.FullName);
            var dataImage = new MemoryStream();
            img.CopyTo(dataImage);
            return dataImage.ToArray();
        }
        public IActionResult Index()
        {
            ViewBag.CounterArticles = _articleRepository.GetCounterArticle().Result.ToString();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Account()
        {
            long UserId = Convert.ToInt32(HttpContext.Session.GetString("IdUser"));
            User user = await _userRepository.GetUser(UserId);
            EditProfileVM model = new EditProfileVM
            {
                Id = user.Id,
                Username = user.Username,
                Fullname = user.Fullname,
                ConifrmPassword = "",
                ImageFile = null,
                Password = ""
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Account(EditProfileVM model)
        {
            byte[] image = null;
            long UserId = Convert.ToInt32(HttpContext.Session.GetString("IdUser"));
            if (ModelState.IsValid && UserId == model.Id)
            {
                User user = await _userRepository.GetUser(model.Id);
                if (user != null)
                {
                    if (model.ImageFile != null)
                    {
                        image = CompressImage(model.ImageFile);
                        await _userRepository.EditImage(UserId, image);
                        string imgDataURL = "data:image/png;base64," + Convert.ToBase64String(image);
                        HttpContext.Session.Remove("ImageUser");
                        HttpContext.Session.SetString("ImageUser", imgDataURL);
                    }
                    bool userCheck = await _userRepository.CheckUsername(model.Username, UserId);
                    if (userCheck)
                    {
                        ModelState.AddModelError("Username", "هذا المستخدم موجود بالفعل");
                        return View(model);
                    }
                    if (user.Username != model.Username)
                    {
                        await _userRepository.EditUsername(UserId, model.Username.Trim());
                    }
                    if (user.Fullname != model.Fullname)
                    {
                        await _userRepository.EditFullname(UserId, model.Fullname);
                        HttpContext.Session.SetString("FullnameUser", model.Fullname.ToString());
                    }
                    if (model.Password != null && model.Password == model.ConifrmPassword)
                    {
                        await _userRepository.EditPassword(UserId, model.Password);
                    }
                    ViewData["msgSuccess"] = "تم الحفظ بنجاح";
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ArticlesManagement(int? page = 1)
        {
            int pageSize = 10;
            long UserId = Convert.ToInt32(HttpContext.Session.GetString("IdUser"));
            List<ArticleVM> allArticles = await _articleRepository.AllArticles(UserId);
            return View(PaginatedList<ArticleVM>
                .Create(allArticles, page ?? 1, pageSize));
        }

        public IActionResult CreateArticle()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticle(ArticleVM model)
        {
            if (ModelState.IsValid)
            {
                model.Image = CompressImage(model.ImageFile);
                model.UserId = Convert.ToInt64(HttpContext.Session.GetString("IdUser"));
                bool check = await _articleRepository.AddArticle(model);
                if (check == true)
                {
                    return RedirectToAction("ArticlesManagement", "Dashboard");
                }
                else
                {
                    ViewBag.MessageError = "يوجد خطأ أثناء الإضافة";
                }
            }
            return View(model);
        }
        public async Task<IActionResult> EditArticle(long? articleId)
        {
            long UserId = Convert.ToInt32(HttpContext.Session.GetString("IdUser"));
            if (articleId == null)
                return RedirectToAction("ArticlesManagement", "DashBoard");
            ArticleVM article = await _articleRepository.GetArticle((long)articleId);
            EditArticleVM editArticleVM = new EditArticleVM();
            if (article != null || article.UserId == UserId)
            {
                editArticleVM.Id = article.Id;
                editArticleVM.Image = article.Image;
                editArticleVM.Value1 = article.Value1;
                editArticleVM.Value2 = article.Value2;
                editArticleVM.Property1 = article.Property1;
                editArticleVM.Property2 = article.Property2;
                editArticleVM.TitleInfo1 = article.TitleInfo1;
                editArticleVM.TitleInfo2 = article.TitleInfo2;
                editArticleVM.Title = article.Title;
                editArticleVM.Details = article.Details;
                editArticleVM.SubTitle = article.SubTitle;
                return View(editArticleVM);
            }
            else
                return RedirectToAction("ArticlesManagement", "DashBoard");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArticle(EditArticleVM model)
        {
            if (ModelState.IsValid)
            {
                ArticleVM article = await _articleRepository.GetArticle(model.Id);
                if (article != null)
                {
                    if (model.ImageFile == null)
                        model.Image = article.Image;
                    else
                        model.Image = CompressImage(model.ImageFile);
                    bool check = await _articleRepository.EditArticle(model.Id, model);
                    if (check == true)
                        return RedirectToAction("ArticlesManagement", "Dashboard");
                    else
                        ViewBag.MessageError = "يوجد خطأ أثناء التعديل";
                }

            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> HideArticle(long articleId)
        {
            bool result = await _articleRepository.HideArticel(articleId);
            return Json(new { url = Url.Action("ArticlesManagement", "DashBoard") });
        }
        [HttpPost]
        public async Task<IActionResult> ShowArticle(long articleId)
        {
            bool result = await _articleRepository.ShowArticel(articleId);
            return Json(new { url = Url.Action("ArticlesManagement", "DashBoard") });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteArticle(long articleId)
        {
            bool result = await _articleRepository.DeleteArticel(articleId);
            return Json(new { url = Url.Action("ArticlesManagement", "DashBoard") });
        }
    }
}
