using AtiDocs.DataModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtiDocs.Pages
{
    public partial class Write : ComponentBase
    {
        [Parameter]
        public string folderSlug { get; set; } = null;
        [Parameter]
        public string articleSlug { get; set; } = null;

        private ArticleAddItem addArticle = new();

        private List<Folder> Folders;

        private Mode pageMode = Mode.Create;

        protected override async Task OnInitializedAsync()
        {
            Folders = await DbHandler.ListFolder();

            if(folderSlug != null && articleSlug != null)
            {
                var article = await DbHandler.GetArticle(folderSlug, articleSlug);
                addArticle.Title = article.Title;
                addArticle.Content = article.Content;
                addArticle.FolderId = article.FolderId;
                pageMode = Mode.Change;
            }
        }

        private async Task CreateDoc()
        {
            if(addArticle.Title == null || addArticle.Content == null)
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, "Title and/or content is missing");
                return;
            }

            if(string.IsNullOrEmpty(addArticle.Title) || string.IsNullOrWhiteSpace(addArticle.Title) ||
               string.IsNullOrEmpty(addArticle.Content) || string.IsNullOrWhiteSpace(addArticle.Content))
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, "Title and/or content is missing");
                return;
            }

            if (pageMode == Mode.Create)
            {
                var result = await DbHandler.AddArticle(addArticle.Title, addArticle.Content, addArticle.FolderSlug);
                if (result.Status)
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, result.Message);
                else
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, result.Message);
            }
            else
            {
                var result = await DbHandler.ChangeArticle(folderSlug, articleSlug, addArticle.Content);
                if (result.Status)
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, result.Message);
                else
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, result.Message);
            }

            NavManager.NavigateTo($"/e");
        }

        private enum Mode
        {
            Create,
            Change
        }

        private class ArticleAddItem : Article
        {
            public string FolderSlug { get; set; }
        }
    }
}
