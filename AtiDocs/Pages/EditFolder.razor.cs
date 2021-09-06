using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtiDocs.DataModel;
using AtiDocs.Model;

namespace AtiDocs.Pages
{
    public partial class EditFolder : ComponentBase
    {
        private Folder addFolder = new();
        private bool ShowAdd = false;
        private PopuMode ShowMode;

        private List<FolderTableItem> FolderList = new();

        private MoveModel moveModel = new();
        private bool ShowMove = false;

        private RenameModel renameModel = new();
        private bool ShowRename = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadFolders();
        }

        private async Task LoadFolders()
        {
            FolderList = new();
            var tempList = await DbHandler.ListFolder();
            tempList = tempList.OrderBy(o => o.Name).ToList();

            foreach (var item in tempList)
            {
                FolderList.Add(new FolderTableItem() { Folder = item });
            }
        }

        private async Task AddNewFolder()
        {
            if(addFolder.Name != null)
            {
                if(string.IsNullOrEmpty(addFolder.Name) != true && string.IsNullOrWhiteSpace(addFolder.Name) != true)
                {
                    (bool Status, string Message) result;
                    string oldslug = addFolder.Slug;

                    if (ShowMode == PopuMode.Add)
                        result = await DbHandler.AddFolder(addFolder.Name);
                    else
                        result = await DbHandler.RenameFolder(oldslug, addFolder.Name);

                    if(result.Status)
                    {
                        ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, result.Message);
                    }
                    else
                    {
                        ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, result.Message);
                    }
                }
                else
                {
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, "Folder name is empty");
                }
            }
            else
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, "Folder name is empty");
            }

            await LoadFolders();
            addFolder = new();
            ShowAdd = false;
        }

        private void DisplayPopup(PopuMode mode, string title, string slug)
        {
            ShowMode = mode;

            if(ShowMode == PopuMode.Rename)
            {
                addFolder.Slug = slug;
                addFolder.Name = title;
            }

            ShowAdd = true;
        }

        private async Task DeleteFolder(string Slug)
        {
            var result = await DbHandler.RemoveFolder(Slug);
            if(result.Status)
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, result.Message);
            }
            else
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, result.Message);
            }

            await LoadFolders();
        }

        private async Task LoadArticles(FolderTableItem Folder)
        {
            if (!Folder.DisplayDocs)
            {
                var result = await DbHandler.ListArticles(Folder.Folder.Id);
                result = result.OrderBy(o => o.Title).ToList();
                Folder.ArticleList = new();
                foreach (var item in result)
                {
                    Folder.ArticleList.Add(new ArticleTableItem() { Article = item });
                }
                Folder.DisplayDocs = true;
            }
            else
                Folder.DisplayDocs = false;
        }

        private void OpenArticle(string folderSlug, string articleSlug) => NavManager.NavigateTo($"/b/{folderSlug}/{articleSlug}");

        private void EditArticle(string folderSlug, string articleSlug) => NavManager.NavigateTo($"/w/{folderSlug}/{articleSlug}");

        private void DisplayMove(string folderSlug, string articleSlug)
        {
            moveModel.ArticleSlug = articleSlug;
            moveModel.CurrentSlug = folderSlug;
            ShowMove = true;
        }

        private async Task MoveDoc()
        {
            var result = await DbHandler.MoveArticle(moveModel.CurrentSlug, moveModel.ArticleSlug, moveModel.NewSlug);
            if(result.Status)
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, result.Message);
            }
            else
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, result.Message);
            }

            await ReloadDocList(moveModel.CurrentSlug);
            await ReloadDocList(moveModel.NewSlug);

            moveModel = new();
            ShowMove = false;
            StateHasChanged();
        }

        private async Task DeleteDoc(string folderSlug, string articleSlug)
        {
            var result = await DbHandler.RemoveArticle(folderSlug, articleSlug);
            if(result.Status)
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, result.Message);
            }
            else
            {
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, result.Message);
            }

            await ReloadDocList(folderSlug);

            StateHasChanged();
        }

        private async Task ReloadDocList(string folderSlug)
        {
            FolderTableItem reloadItem = null;
            foreach (var item in FolderList)
            {
                if (item.Folder.Slug == folderSlug)
                    reloadItem = item;
            }

            var list = await DbHandler.ListArticles(reloadItem.Folder.Id);
            list = list.OrderBy(o => o.Title).ToList();
            reloadItem.ArticleList = new();
            foreach (var item in list)
            {
                reloadItem.ArticleList.Add(new ArticleTableItem() { Article = item });
            }
        }

        private async Task DisplayRename(string folderSlug, string articleSlug)
        {
            renameModel.CurrentSlug = folderSlug;
            renameModel.ArticleSlug = articleSlug;

            var article = await DbHandler.GetArticle(folderSlug, articleSlug);
            renameModel.NewTitle = article.Title;

            ShowRename = true;
        }

        private async Task RenameDoc()
        {
            var result = await DbHandler.RenameArticle(renameModel.CurrentSlug, renameModel.ArticleSlug, renameModel.NewTitle);
            if (result.Status)
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, result.Message);
            else
                ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, result.Message);

            await ReloadDocList(renameModel.CurrentSlug);
            renameModel = new();
            ShowRename = false;
            StateHasChanged();
        }

        private void AddArticle(string folderSlug) => NavManager.NavigateTo($"/w/{folderSlug}");

        private enum PopuMode
        {
            Add,
            Rename
        }

        private class MoveModel
        {
            public string CurrentSlug { get; set; }

            public string NewSlug { get; set; }

            public string ArticleSlug { get; set; }
        }

        private class RenameModel
        {
            public string CurrentSlug { get; set; }

            public string NewTitle { get; set; }

            public string ArticleSlug { get; set; }
        }
    }
}
