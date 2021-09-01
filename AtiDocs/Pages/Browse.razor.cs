using AtiDocs.DataModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                contentHtml = Markdig.Markdown.ToHtml(content.Content);
                StateHasChanged();
            }
        }


    }
}
