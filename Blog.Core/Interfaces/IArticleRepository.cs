using Blog.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Interfaces
{
    public interface IArticleRepository
    {
        Task<List<AllArticlesVM>> AllArticles(string searchString);
        Task<List<ArticleVM>> AllArticles(long userId);
        Task<ArticleVM> Article(long articleId);
        Task<ArticleVM> GetArticle(long articleId);
        Task<bool> AddArticle(ArticleVM model);
        Task<int> GetCounterArticle();
        Task<bool> EditArticle(long articleId, EditArticleVM model);
        Task<bool> SetCounterArticle(long articleId);
        Task<bool> HideArticel(long articleId);
        Task<bool> ShowArticel(long articleId);
        Task<bool> DeleteArticel(long articleId);
    }

}
