using AtiDocs.DataModel;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToastNotify.Model;

namespace AtiDocs.Pages
{
    public partial class Browse : ComponentBase
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
                var pipe = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                contentHtml = Markdown.ToHtml(content.Content, pipe);
                StateHasChanged();
            }
        }
    }
}
