using Blog.Core.Helpers;
using Blog.Core.Interfaces;
using Blog.Core.Models;
using Blog.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.EF.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ILogger<ArticleRepository> _logger;
        private readonly BlogDbContext _context;
        public ArticleRepository(ILogger<ArticleRepository> logger,
            BlogDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<int> GetCounterArticle()
        {
            try
            {
                int count = await _context.Articles.Where(a => a.IsDeleted == false
                && a.IsShow == true).CountAsync();
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddArticle(ArticleVM model)
        {
            try
            {
                Article addArticle = new Article
                {
                    Id = 0,
                    Details = model.Details,
                    SubTitle = model.SubTitle,
                    IsShow = model.IsPublish,
                    AddedDate = LocalTime.Now(),
                    IsDeleted = false,
                    Counter = 0,
                    Image = model.Image,
                    Title = model.Title,
                    UserId = model.UserId
                };
                await _context.Articles.AddAsync(addArticle);
                await _context.SaveChangesAsync();
                ArticleInfo addArticleInfo1 = new ArticleInfo
                {
                    Id = 0,
                    ArticleId = addArticle.Id,
                    Title = model.TitleInfo1,
                    Property = model.Property1,
                    Value = model.Value1,
                    IsDeleted = false,
                    IsShow = true,
                };
                ArticleInfo addArticleInfo2 = new ArticleInfo
                {
                    Id = 0,
                    ArticleId = addArticle.Id,
                    Title = model.TitleInfo2,
                    Property = model.Property2,
                    Value = model.Value2,
                    IsDeleted = false,
                    IsShow = true,
                };
                await _context.ArticleInfos.AddRangeAsync(addArticleInfo1, addArticleInfo2);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<bool> EditArticle(long articleId, EditArticleVM model)
        {
            try
            {
                ArticleInfo articleInfo1 = new ArticleInfo();
                Article article = await _context.Articles
                   .SingleOrDefaultAsync(a => a.Id == articleId
                   && a.IsDeleted == false);
                List<ArticleInfo> articlesInfo = await _context.ArticleInfos
                   .Where(a => a.ArticleId == article.Id
                   && a.IsDeleted == false).ToListAsync();
                if (article != null)
                    article.Title = model.Title;
                article.SubTitle = model.SubTitle;
                article.Details = model.Details;
                article.Image = model.Image;
                if (articlesInfo != null)
                    switch (articlesInfo.Count)
                    {
                        case 2:
                            articlesInfo[0].Title = model.TitleInfo1;
                            articlesInfo[0].Property = model.Property1;
                            articlesInfo[0].Value = model.Value1;
                            articlesInfo[1].Title = model.TitleInfo2;
                            articlesInfo[1].Property = model.Property2;
                            articlesInfo[1].Value = model.Value2;
                            break;
                        case 1:
                            articlesInfo[0].Title = model.TitleInfo1;
                            articlesInfo[0].Property = model.Property1;
                            articlesInfo[0].Value = model.Value1;
                            ArticleInfo addArticleInfo = new ArticleInfo
                            {
                                Id = 0,
                                ArticleId = articleId,
                                Title = model.TitleInfo1,
                                Property = model.Property1,
                                Value = model.Value1,
                                IsDeleted = false,
                                IsShow = true,
                            };
                            await _context.ArticleInfos.AddAsync(addArticleInfo);
                            break;
                        case 0:
                            ArticleInfo addArticleInfo1 = new ArticleInfo
                            {
                                Id = 0,
                                ArticleId = articleId,
                                Title = model.TitleInfo1,
                                Property = model.Property1,
                                Value = model.Value1,
                                IsDeleted = false,
                                IsShow = true,
                            };
                            ArticleInfo addArticleInfo2 = new ArticleInfo
                            {
                                Id = 0,
                                ArticleId = articleId,
                                Title = model.TitleInfo2,
                                Property = model.Property2,
                                Value = model.Value2,
                                IsDeleted = false,
                                IsShow = true,
                            };
                            await _context.ArticleInfos.AddRangeAsync(addArticleInfo1, addArticleInfo2);
                            break;
                        default:
                            break;
                    }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<List<AllArticlesVM>> AllArticles(string searchString)
        {
            try
            {
                List<Article> articles = new List<Article>();
                List<AllArticlesVM> allArticles = new List<AllArticlesVM>();

                if (string.IsNullOrEmpty(searchString))
                    articles = await _context.Articles.Where(a => a.IsDeleted == false
                 && a.IsShow == true).OrderByDescending(a => a.AddedDate).ToListAsync();
                else
                    articles = await _context.Articles.Where(a => a.Details.Contains(searchString)
                  || a.Title.Contains(searchString)
                  || a.SubTitle.Contains(searchString)
                  && a.IsDeleted == false
                  && a.IsShow == true).OrderByDescending(a => a.AddedDate).ToListAsync();

                if (articles != null || articles.Count != 0)
                    foreach (var item in articles)
                    {
                        allArticles.Add(new AllArticlesVM
                        {
                            Id = item.Id,
                            AddedDate = Convert.ToDateTime(item.AddedDate),
                            SubTitle = item.SubTitle ?? "Subtitle is null",
                            Image = item.Image ?? Encoding.ASCII.GetBytes("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BAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJDM/wcm8UW4x9ShEgAAAABJRU5ErkJggg=="),
                            Title = item.Title ?? "Title is null"
                        });
                    }
                return allArticles;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ArticleVM>> AllArticles(long userId)
        {
            try
            {
                List<ArticleVM> articlesTaregt = new List<ArticleVM>();
                User user = await _context.Users.SingleOrDefaultAsync(a => a.Id == userId && a.IsActive == true && a.IsDeleted == false);
                if (user == null)
                    return null;

                List<Article> articles = await _context.Articles
                    .Where(a => a.UserId == userId
                    && a.IsDeleted == false).ToListAsync();
                if (articles == null)
                    return null;
                foreach (var article in articles)
                {
                    ArticleVM articleTaregt = new ArticleVM();
                    List<ArticleInfo> articlesInfo = await _context.ArticleInfos
                    .Where(a => a.ArticleId == article.Id
                    && a.IsDeleted == false
                    && a.IsShow == true).ToListAsync();
                    articleTaregt.Id = article.Id;
                    articleTaregt.Details = article.Details ?? "Details is null";
                    articleTaregt.Image = article.Image ?? Encoding.ASCII.GetBytes("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BAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJDM/wcm8UW4x9ShEgAAAABJRU5ErkJggg==");
                    articleTaregt.SubTitle = article.SubTitle ?? "Subtitle is null";
                    articleTaregt.UserId = (long)article.UserId;
                    articleTaregt.Title = article.Title ?? "Title is null";
                    articleTaregt.Counter = article.Counter ?? 0;
                    articleTaregt.IsPublish = (bool)article.IsShow;
                    if (articlesInfo.Count == 0)
                    {
                        articleTaregt.TitleInfo1 = null;
                        articleTaregt.TitleInfo2 = null;
                        articlesTaregt.Add(articleTaregt);
                    }
                    if (articlesInfo.Count == 1)
                    {
                        articleTaregt.TitleInfo1 = articlesInfo[0].Title;
                        articleTaregt.Property1 = articlesInfo[0].Property;
                        articleTaregt.Value1 = articlesInfo[0].Value;
                        articleTaregt.TitleInfo2 = null;
                        articlesTaregt.Add(articleTaregt);
                    }
                    if (articlesInfo.Count >= 2)
                    {
                        articleTaregt.TitleInfo1 = articlesInfo[0].Title;
                        articleTaregt.Property1 = articlesInfo[0].Property;
                        articleTaregt.Value1 = articlesInfo[0].Value;
                        articleTaregt.TitleInfo2 = articlesInfo[1].Title;
                        articleTaregt.Property2 = articlesInfo[1].Property;
                        articleTaregt.Value2 = articlesInfo[1].Value;
                        articlesTaregt.Add(articleTaregt);
                    }
                }
                return articlesTaregt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<ArticleVM> Article(long articleId)
        {
            try
            {
                ArticleVM articleTaregt = new ArticleVM();
                Article article = await _context.Articles
                    .SingleOrDefaultAsync(a => a.Id == articleId
                    && a.IsDeleted == false && a.IsShow == true);
                List<ArticleInfo> articlesInfo = await _context.ArticleInfos
                    .Where(a => a.ArticleId == articleId
                    && a.IsDeleted == false
                    && a.IsShow == true).ToListAsync();
                if (article == null)
                    return null;
                if (article != null)
                    articleTaregt.Id = article.Id;
                articleTaregt.Details = article.Details ?? "Details is null";
                articleTaregt.Image = article.Image ?? Encoding.ASCII.GetBytes("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BAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJDM/wcm8UW4x9ShEgAAAABJRU5ErkJggg==");
                articleTaregt.SubTitle = article.SubTitle ?? "Subtitle is null";
                articleTaregt.UserId = (long)article.UserId;
                articleTaregt.Title = article.Title ?? "Title is null";
                articleTaregt.Counter = article.Counter ?? 0;
                if (articlesInfo.Count == 0)
                {
                    articleTaregt.TitleInfo1 = null;
                    articleTaregt.TitleInfo2 = null;
                    return articleTaregt;
                }
                if (articlesInfo.Count == 1)
                {
                    articleTaregt.TitleInfo1 = articlesInfo[0].Title;
                    articleTaregt.Property1 = articlesInfo[0].Property;
                    articleTaregt.Value1 = articlesInfo[0].Value;
                    articleTaregt.TitleInfo2 = null;
                    return articleTaregt;
                }
                if (articlesInfo.Count >= 2)
                {
                    articleTaregt.TitleInfo1 = articlesInfo[0].Title;
                    articleTaregt.Property1 = articlesInfo[0].Property;
                    articleTaregt.Value1 = articlesInfo[0].Value;
                    articleTaregt.TitleInfo2 = articlesInfo[1].Title;
                    articleTaregt.Property2 = articlesInfo[1].Property;
                    articleTaregt.Value2 = articlesInfo[1].Value;
                    return articleTaregt;
                }
                return articleTaregt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

        }


        public async Task<bool> SetCounterArticle(long articleId)
        {
            try
            {
                Article article = await _context.Articles.SingleOrDefaultAsync(a => a.Id == articleId
                                    && a.IsDeleted == false && a.IsShow == true);
                if (article != null)
                    article.Counter += 1;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<bool> HideArticel(long articleId)
        {
            try
            {
                Article article = await _context.Articles.SingleOrDefaultAsync(a => a.Id == articleId
                                    && a.IsShow == true);
                if (article != null)
                    article.IsShow = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<bool> ShowArticel(long articleId)
        {
            try
            {
                Article article = await _context.Articles.SingleOrDefaultAsync(a => a.Id == articleId
                                   && a.IsShow == false);
                if (article != null)
                    article.IsShow = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<bool> DeleteArticel(long articleId)
        {
            try
            {
                Article article = await _context.Articles.SingleOrDefaultAsync(a => a.Id == articleId);
                if (article != null)
                    article.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<ArticleVM> GetArticle(long articleId)
        {
            try
            {
                ArticleVM articleTaregt = new ArticleVM();
                Article article = await _context.Articles
                    .SingleOrDefaultAsync(a => a.Id == articleId
                    && a.IsDeleted == false);
                List<ArticleInfo> articlesInfo = await _context.ArticleInfos
                    .Where(a => a.ArticleId == articleId
                    && a.IsDeleted == false).ToListAsync();
                if (article == null)
                    return null;
                if (article != null)
                    articleTaregt.Id = article.Id;
                articleTaregt.Details = article.Details ?? "Details is null";
                articleTaregt.Image = article.Image ?? Encoding.ASCII.GetBytes("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BAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJCMAAEAAJIRIAAAQDICBAAASEaAAAAAyQgQAAAgGQECAAAkI0AAAIBkBAgAAJDM/wcm8UW4x9ShEgAAAABJRU5ErkJggg==");
                articleTaregt.SubTitle = article.SubTitle ?? "Subtitle is null";
                articleTaregt.UserId = (long)article.UserId;
                articleTaregt.Title = article.Title ?? "Title is null";
                articleTaregt.Counter = article.Counter ?? 0;
                if (articlesInfo.Count == 0)
                {
                    articleTaregt.TitleInfo1 = null;
                    articleTaregt.TitleInfo2 = null;
                    return articleTaregt;
                }
                if (articlesInfo.Count == 1)
                {
                    articleTaregt.TitleInfo1 = articlesInfo[0].Title;
                    articleTaregt.Property1 = articlesInfo[0].Property;
                    articleTaregt.Value1 = articlesInfo[0].Value;
                    articleTaregt.TitleInfo2 = null;
                    return articleTaregt;
                }
                if (articlesInfo.Count >= 2)
                {
                    articleTaregt.TitleInfo1 = articlesInfo[0].Title;
                    articleTaregt.Property1 = articlesInfo[0].Property;
                    articleTaregt.Value1 = articlesInfo[0].Value;
                    articleTaregt.TitleInfo2 = articlesInfo[1].Title;
                    articleTaregt.Property2 = articlesInfo[1].Property;
                    articleTaregt.Value2 = articlesInfo[1].Value;
                    return articleTaregt;
                }
                return articleTaregt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

        }
    }
}
