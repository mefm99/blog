using Blog.Core.Helpers;
using Blog.Core.Interfaces;
using Blog.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IArticleRepository _articleRepository;
        public HomeController(ILogger<HomeController> logger,
                        IUserRepository userRepository,
            IArticleRepository articleRepository)
        {
            _userRepository = userRepository;
            _articleRepository = articleRepository;
            _logger = logger;
        }
        public async Task<IActionResult> Index(string searchString,
                                       string currentFilter,
                                       int? page = 1)
        {
            int pageSize = 5;
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            ViewData["CurrentFilter"] = searchString;
            List<AllArticlesVM> allArticles = await _articleRepository.AllArticles(searchString);
            return View(PaginatedList<AllArticlesVM>
                .Create(allArticles, page ?? 1, pageSize));
        }
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("IdUser")))
                return RedirectToAction("Index", "DashBoard");


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.Login(model.Username, model.Password);
                if (result != null)
                {
                    if (result.IsDeleted == true || result.IsActive == false)
                    {
                        ViewBag.LoginMessageInfo = "تم تعطيل أو حذف الحساب";
                    }
                    else
                    {
                        string imgDataURL = "data:image/png;base64," + Convert.ToBase64String(result.Image);
                        HttpContext.Session.SetString("FullnameUser", result.Fullname.ToString());
                        HttpContext.Session.SetString("IdUser", result.Id.ToString());
                        HttpContext.Session.SetString("ImageUser", imgDataURL);
                        return RedirectToAction("Index", "DashBoard");
                    }
                }
                ViewBag.LoginMessageError = "كلمة المرور أو اسم المستخدم غير صحيح";
            }
            return View();
        }
        public async Task<IActionResult> Article(long idArticle)
        {
            string urlFriendly = string.Empty;
            ArticleVM article = await _articleRepository.Article(idArticle);
            if (article != null)
                urlFriendly = FriendlyUrlHelper.GetFriendlyTitle(article.Title, false, 50);
            return RedirectToAction("Articles", "Home", new { idArticle = idArticle, title = urlFriendly });
        }
        [Route("[controller]/Articles/{idArticle}/{title}")]
        public async Task<IActionResult> Articles(long idArticle,
                                                  string title)
        {
            string urlFriendly = string.Empty;
            await _articleRepository.SetCounterArticle(idArticle);
            ArticleVM article = await _articleRepository.Article(idArticle);
            if (article != null)
                urlFriendly = FriendlyUrlHelper.GetFriendlyTitle(article.Title, false, 50);
            if (title != urlFriendly)
                article = null;
            return View(article);
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Json(new { url = Url.Action("Index", "Home") });
        }
        [Route("Error/{statuscode}")]
        public IActionResult HttpStatusCodeHandler(int statuscode)
        {
            switch (statuscode)
            {
                case 404:
                    return View("NotFound");
                default:
                    ViewBag.Statuscode = statuscode.ToString();
                    return View("Error");
            }
        }
    }
}