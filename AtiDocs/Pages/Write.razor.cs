using AtiDocs.DataModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AtiDocs.Model;
using Microsoft.JSInterop;
using Markdig;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace AtiDocs.Pages
{
    public partial class Write : ComponentBase, IDisposable
    {
        [Parameter]
        public string folderSlug { get; set; } = null;
        [Parameter]
        public string articleSlug { get; set; } = null;

        List<ContentPartItem> ContentParts = new();
        string DocTitle;
        
        protected override async Task OnInitializedAsync()
        {
            if (articleSlug == null)
            {
                ContentParts.Add(new ContentPartItem() { Content = "", ElementReference = Guid.NewGuid().ToString() });
            }
            else
            {
                var article = await DbHandler.GetArticle(folderSlug, articleSlug);
                if(article == null)
                {
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, $"Article '{folderSlug}/{articleSlug}' did not found");
                }
                DocTitle = article.Title;
                ContentParts = JsonSerializer.Deserialize<List<ContentPartItem>>(article.Content);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var objRef = DotNetObjectReference.Create(this);
                await js.InvokeVoidAsync("RegisterObject", objRef);
                await js.InvokeVoidAsync("RegisterEventForPage");
                await js.InvokeVoidAsync("RegisterScroll");

                if (ContentParts.Count == 1)
                {
                    await js.InvokeVoidAsync("RegisterEvent", ContentParts[0].ElementReference);
                    await js.InvokeVoidAsync("PutText", ContentParts[0].ElementReference, ContentParts[0].Content);
                }
                else
                {
                    foreach (var part in ContentParts)
                    {
                        part.Edit = false;
                        await js.InvokeVoidAsync("RegisterEvent", part.ElementReference);
                        await js.InvokeVoidAsync("PutText", part.ElementReference, part.Content);
                    }
                    StateHasChanged();
                }
            }
        }

        public void Dispose()
        {
            js.InvokeVoidAsync("UnregisterEventForPage");
            js.InvokeVoidAsync("UnregisterScroll");
        }

        [JSInvokableAttribute("SaveArticle")]
        public async Task SaveArticle()
        {
            var resultJson = JsonSerializer.Serialize(ContentParts);

            if (DocTitle != null)
            {
                if (DocTitle.Contains('#'))
                {
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Warning, "Title cannot contains the following characters: #");
                    return;
                }

                if (DocTitle.Length > 2)
                {
                    DocTitle = DocTitle.Substring(0, 1).ToUpper() + DocTitle.Substring(1).ToLower();
                }
            }

            if (articleSlug == null)
            {
                var result = await DbHandler.AddArticle(DocTitle, resultJson, folderSlug);
                if (result.Status)
                {
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, result.Message);
                    NavManager.NavigateTo($"/w/{folderSlug}/{DocTitle.ToLower().Replace(" ", "-")}");
                }
                else
                {
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, result.Message);
                }
            }
            else
            {
                var resultContent = await DbHandler.ChangeArticle(folderSlug, articleSlug, resultJson);
                if(resultContent.Status)
                {
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Info, resultContent.Message);
                }
                else
                {
                    ToastController.PushNotification(ToastNotify.Model.ToastNotifyItemType.Error, resultContent.Message);
                }
            }
        }

        private async Task SaveTitle()
        {
            var title = await js.InvokeAsync<string>("GetText", "write-doc-title");
            DocTitle = title;
            StateHasChanged();
        }

        [JSInvokableAttribute("SaveContent")]
        public async Task SaveContent(string reference, string needSave = null)
        {
            System.Threading.Thread.Sleep(250);

            var text = await js.InvokeAsync<string>("GetText", reference);
            if (text == null)
            {
                if (needSave != null)
                    await SaveArticle();
                return;
            }

            ContentPartItem ActItem = ContentParts.SingleOrDefault(e => e.ElementReference == reference);
            ActItem.Content = text;

            var pipe = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            ActItem.ContentHTML = Markdown.ToHtml(text, pipe);

            ActItem.Edit = false;

            if (needSave != null)
                await SaveArticle();

            StateHasChanged();
        }

        private void SetFocus(ContentPartItem item)
        {
            item.FocusTimer = new(50);
            item.FocusTimer.Elapsed += (sender, e) => SetFocusCallback(sender, e, item);
            item.FocusTimer.Enabled = true;
        }

        private async void SetFocusCallback(object sender, ElapsedEventArgs e, ContentPartItem item)
        {
            item.FocusTimer.Enabled = false;
            await js.InvokeVoidAsync("SetFocus", item.ElementReference);
            await js.InvokeVoidAsync("EditResume", item.ElementReference, item.Content);
        }

        private void TurnOnEdit(ContentPartItem item)
        {
            item.Edit = true;

            SetFocus(item);
        }

        [JSInvokableAttribute("AddNewBlock")]
        public async Task AddNewBlock(string reference)
        {
            ContentPartItem ActItem = ContentParts.SingleOrDefault(e => e.ElementReference == reference);
            ContentPartItem addItem = new ContentPartItem() { Content = "", ElementReference = Guid.NewGuid().ToString() };
            ContentParts.Insert(ContentParts.IndexOf(ActItem) + 1, addItem);

            StateHasChanged();

            await js.InvokeVoidAsync("RegisterEvent", addItem.ElementReference);

            SetFocus(addItem);
        }

        [JSInvokableAttribute("DeleteBlock")]
        public async Task DeleteBlock(string reference)
        {
            if (ContentParts.Count > 1)
            {
                ContentPartItem ActItem = ContentParts.SingleOrDefault(e => e.ElementReference == reference);
                await js.InvokeVoidAsync("UnregisterEvent", ActItem.ElementReference);
                ContentParts.Remove(ActItem);
                StateHasChanged();
            }
        }
    }
}
