using AtiDocs.DataModel;
using AtiDocs.Model;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ToastNotify.Model;

namespace AtiDocs.Pages
{
    public partial class Browse : ComponentBase, IDisposable
    {
        [Parameter]
        public string folderSlug { get; set; } = null;
        [Parameter]
        public string articleSlug { get; set; } = null;

        private Article content;
        private string contentHtml;

        protected override async Task OnInitializedAsync()
        {
            if (folderSlug != null && articleSlug != null)
            {
                content = await DbHandler.GetArticle(folderSlug, articleSlug);

                var list = JsonSerializer.Deserialize<List<ContentPartItem>>(content.Content);
                contentHtml = "";
                foreach (var item in list)
                {
                    contentHtml += item.ContentHTML;
                }

                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await js.InvokeVoidAsync("RegisterScroll");
            }
        }

        public void Dispose()
        {
            js.InvokeVoidAsync("UnregisterScroll");
        }
    }
}
