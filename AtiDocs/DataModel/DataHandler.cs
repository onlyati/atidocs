using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtiDocs.DataModel
{
    public class DataHandler
    {
        private DbFunction db;

        public DataHandler()
        {
            db = new();
            db.Database.Migrate();
        }

        /// <summary>
        /// Get the conext of EF interface
        /// </summary>
        /// <returns>Context as DbFunction</returns>
        public DbFunction GetContext() => db;

        /// <summary>
        /// Location of sqlite database
        /// </summary>
        /// <returns>Location as string</returns>
        public string GetDbPath() => db.DbPath;

        #region Folder functions
        /// <summary>
        /// Create new folder
        /// </summary>
        /// <param name="FolderName">Name of the folder, must be unique</param>
        /// <returns>Status and message as (bool Status, string Message)</returns>
        public async Task<(bool Status, string Message)> AddFolder(string FolderName)
        {
            (bool Status, string Message) retValue = new();

            Folder item = new();
            item.Name = FolderName;
            item.Slug = FolderName.ToLower().Replace(" ", "-");

            var record = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(item.Slug));

            if (record == null)
            {
                db.Folders.Add(item);
                await db.SaveChangesAsync();
                retValue.Status = true;
                retValue.Message = $"Folder '{FolderName}' is created";
            }
            else
            {
                retValue.Status = false;
                retValue.Message = $"Folder '{FolderName}' already exist";
            }

            return retValue;
        }

        /// <summary>
        /// Rename existing folder
        /// </summary>
        /// <param name="OldFolderName">Current name of the folder</param>
        /// <param name="NewFolderName">New name of the folder</param>
        /// <returns>Status and message as (bool Status, string Message)</returns>
        public async Task<(bool Status, string Message)> RenameFolder(string OldFolderSlug, string NewFolderName)
        {
            (bool Status, string Message) retValue = new();

            Folder newItem = new Folder();
            newItem.Name = NewFolderName;
            newItem.Slug = NewFolderName.ToLower().Replace(" ", "-");

            var record = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(OldFolderSlug));

            if(record != null)
            {
                var chk = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(newItem.Slug));

                if (chk == null)
                {
                    record.Name = NewFolderName;
                    await db.SaveChangesAsync();
                    retValue.Status = true;
                    retValue.Message = $"Folder is renamed from '{record.Name}' to '{NewFolderName}'";
                }
                else
                {
                    retValue.Status = false;
                    retValue.Message = $"Folder '{NewFolderName}' already exist";
                }
            }
            else
            {
                retValue.Status = false;
                retValue.Message = $"Folder '{OldFolderSlug}' does not exist";
            }

            return retValue;
        }

        /// <summary>
        /// Remove folder
        /// </summary>
        /// <param name="FolderName">Folder name which smut be removed</param>
        /// <returns>Status and message as (bool Status, string Message)</returns>
        public async Task<(bool Status, string Message)> RemoveFolder(string FolderSlug)
        {
            (bool Status, string Message) retValue = new();

            var record = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(FolderSlug));

            if (record != null)
            {
                var articles = await db.Articles.Select(s => s).Where(w => w.FolderId.Equals(record.Id)).ToListAsync();
                if (articles.Count == 0)
                {
                    db.Remove(record);
                    await db.SaveChangesAsync();
                    retValue.Status = true;
                    retValue.Message = $"Folder '{FolderSlug}' is removed";
                }
                else
                {
                    retValue.Status = false;
                    retValue.Message = $"Folder '{FolderSlug}' contains articles, first delete them";
                }
            }
            else
            {
                retValue.Status = false;
                retValue.Message = $"Folder '{FolderSlug}' does not exist";
            }

            return retValue;
        }

        /// <summary>
        /// List all folders
        /// </summary>
        /// <returns>Folders as List</returns>
        public async Task<List<Folder>> ListFolder() => await db.Folders.ToListAsync();

        /// <summary>
        /// Get details about specific folder
        /// </summary>
        /// <param name="Slug">Folder slug</param>
        /// <returns>Info as Folder</returns>
        public async Task<Folder> GetFolder(string Slug) => await db.Folders.FirstOrDefaultAsync(e => e.Slug.Equals(Slug));

        #endregion

        #region Article functions
        /// <summary>
        /// Create new article
        /// </summary>
        /// <param name="title">Title of article</param>
        /// <param name="content">Content of the article</param>
        /// <param name="folderNameSlug">Folder where the article belongs</param>
        /// <returns>Status and message as (bool Status, string Message)</returns>
        public async Task<(bool Status, string Message)> AddArticle(string title, string content, string folderNameSlug)
        {
            (bool Status, string Message) retValue = new();

            Article post = new();

            post.Content = content;
            post.Title = title;
            post.CreatedAt = DateTime.Now;
            post.ChangedAt = DateTime.Now;
            post.Slug = title.ToLower().Replace(" ", "-");

            if (folderNameSlug != null)
            {
                var folder = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(folderNameSlug));
                if(folder != null)
                {
                    post.FolderId = folder.Id;
                    var article = await db.Articles.SingleOrDefaultAsync(e => e.Slug.Equals(post.Slug));

                    if (article == null || article.FolderId != folder.Id)
                    {
                        db.Articles.Add(post);
                        await db.SaveChangesAsync();
                        retValue.Status = true;
                        retValue.Message = $"Article is created";
                    }
                    else
                    {
                        retValue.Status = false;
                        retValue.Message = $"Article title already exist";
                    }
                }
                else
                {
                    retValue.Status = false;
                    retValue.Message = $"Specified folder '{folderNameSlug}' does not exist";
                }
            }
            else
            {
                retValue.Status = false;
                retValue.Message = $"Folder is not specified";
            }

            return retValue;
        }

        /// <summary>
        /// Rename an existing article
        /// </summary>
        /// <param name="OldSlug">Old article slug</param>
        /// <param name="NewTitle">new article title</param>
        /// <returns>Status and message as (bool Status, string Message)</returns>
        public async Task<(bool Status, string Message)> RenameArticle(string folderSlug, string OldSlug, string NewTitle)
        {
            (bool Status, string Message) retValue = new();

            var folder = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(folderSlug));
            if(folder == null)
            {
                retValue.Status = false;
                retValue.Message = $"Folder '{folderSlug}' does not exist";
                return retValue;
            }

            var record = await db.Articles.SingleOrDefaultAsync(e => e.Slug.Equals(OldSlug) && e.FolderId == folder.Id);

            if(record != null)
            {
                string newSlug = NewTitle.ToLower().Replace(" ", "-");
                var newRecord = await db.Articles.SingleOrDefaultAsync(e => e.Slug.Equals(newSlug) && e.FolderId == folder.Id);

                if(newRecord == null)
                {
                    record.Title = NewTitle;
                    record.Slug = newSlug;
                    record.ChangedAt = DateTime.Now;
                    await db.SaveChangesAsync();
                    retValue.Status = true;
                    retValue.Message = $"Article is renamed to '{NewTitle}'";
                }
                else
                {
                    retValue.Status = false;
                    retValue.Message = $"Article '{NewTitle}' already exist";
                }
            }
            else
            {
                retValue.Status = false;
                retValue.Message = $"Article '{OldSlug}' does not exist";
            }

            return retValue;
        }

        /// <summary>
        /// Change content of the article
        /// </summary>
        /// <param name="articleSlug">ID for article</param>
        /// <param name="NewContent">New content of article</param>
        /// <returns>Status and message as (bool Status, string Message)</returns>
        public async Task<(bool Status, string Message)> ChangeArticle(string folderSlug, string articleSlug, string NewContent)
        {
            (bool Status, string Message) retValue = new();

            var folder = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(folderSlug));
            if (folder == null)
            {
                retValue.Status = false;
                retValue.Message = $"Folder '{folderSlug}' does not exist";
                return retValue;
            }

            var record = await db.Articles.SingleOrDefaultAsync(e => e.Slug.Equals(articleSlug) && e.FolderId == folder.Id);

            if(record != null)
            {
                record.Content = NewContent;
                record.ChangedAt = DateTime.Now;
                await db.SaveChangesAsync();
                retValue.Status = true;
                retValue.Message = $"Content is changed";
            }
            else
            {
                retValue.Status = false;
                retValue.Message = $"Article '{articleSlug}' already exist";
            }

            return retValue;
        }

        /// <summary>
        /// Move article to a differetn folder
        /// </summary>
        /// <param name="ArticleSlug">Slug of article which should be moved</param>
        /// <param name="NewFolderSlug">Slug of folder where article will be moved</param>
        /// <returns>Status and message as (bool Status, string Message)</returns>
        public async Task<(bool Status, string Message)> MoveArticle(string FolderSlug, string ArticleSlug, string NewFolderSlug)
        {
            (bool Status, string Message) retValue = new();

            var currentFolder = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(FolderSlug));
            if(currentFolder == null)
            {
                retValue.Status = false;
                retValue.Message = $"Current directory not found: '{FolderSlug}'";
                return retValue;
            }

            var record = await db.Articles.SingleOrDefaultAsync(e => e.Slug.Equals(ArticleSlug) && e.FolderId == currentFolder.Id);

            if(record != null)
            {
                var folder = await db.Folders.SingleOrDefaultAsync(e => e.Slug.Equals(NewFolderSlug));
                if(folder != null)
                {
                    var chkNewFolder = await db.Articles.SingleOrDefaultAsync(e => e.Slug.Equals(ArticleSlug) && e.FolderId == folder.Id);

                    if (chkNewFolder == null)
                    {
                        record.FolderId = folder.Id;
                        await db.SaveChangesAsync();
                        retValue.Status = true;
                        retValue.Message = $"Article '{FolderSlug}/{ArticleSlug}' is moved to '{NewFolderSlug}' folder";
                    }
                    else
                    {
                        retValue.Status = false;
                        retValue.Message = $"Folder '{NewFolderSlug}' already contains '{ArticleSlug}' article";
                    }
                }
                else
                {
                    retValue.Status = false;
                    retValue.Message = $"Folder '{NewFolderSlug}' does not exist";
                }
            }
            else
            {
                retValue.Status = false;
                retValue.Message = $"Article '{ArticleSlug}' does not exist";
            }

            return retValue;
        }

        /// <summary>
        /// Remove article
        /// </summary>
        /// <param name="articleSlug">Article slug</param>
        /// <returns>Status and message as (bool Status, string Message)</returns>
        public async Task<(bool Status, string Message)> RemoveArticle(string folderSlug, string articleSlug)
        {
            (bool Status, string Message) retValue = new();

            var folder = await db.Folders.SingleOrDefaultAsync(s => s.Slug.Equals(folderSlug));
            if(folder == null)
            {
                retValue.Status = false;
                retValue.Message = $"Article '{articleSlug}' does not exist";
                return retValue;
            }

            var record = await db.Articles.SingleOrDefaultAsync(s => s.Slug.Equals(articleSlug) && s.FolderId == folder.Id);

            if (record != null)
            {
                db.Articles.Remove(record);
                await db.SaveChangesAsync();
                retValue.Status = true;
                retValue.Message = $"Article '{articleSlug}' is removed";
            }
            else
            {
                retValue.Status = false;
                retValue.Message = $"Article '{articleSlug}' does not exist";
            }

            return retValue;
        }

        /// <summary>
        /// Get selected article
        /// </summary>
        /// <param name="articleSlug">ID of article</param>
        /// <returns>Record as Article</returns>
        public async Task<Article> GetArticle(string folderSlug, string articleSlug)
        {
            var result = await db.Folders.SingleOrDefaultAsync(s => s.Slug.Equals(folderSlug));
            return await db.Articles.SingleOrDefaultAsync(s => s.Slug == articleSlug && s.FolderId == result.Id);
        }

        /// <summary>
        /// List all articles without content
        /// </summary>
        /// <returns>Articles as List</returns>
        public async Task<List<Article>> ListArticles() =>
            await db.Articles
                .Select(s => new Article() { ChangedAt = s.ChangedAt, Content = null, CreatedAt = s.CreatedAt, FolderId = s.FolderId, Id = s.Id, Slug = s.Slug, Title = s.Title})
                .ToListAsync();

        /// <summary>
        /// List all articles within a folder without content
        /// </summary>
        /// <param name="FolderId">Folder ID</param>
        /// <returns>Articles as List</returns>
        public async Task<List<Article>> ListArticles(int FolderId) =>
            await db.Articles
                .Select(s => new Article() { ChangedAt = s.ChangedAt, Content = null, CreatedAt = s.CreatedAt, FolderId = s.FolderId, Id = s.Id, Slug = s.Slug, Title = s.Title })
                .Where(w => w.FolderId.Equals(FolderId))
                .ToListAsync();

        #endregion
    }
}
