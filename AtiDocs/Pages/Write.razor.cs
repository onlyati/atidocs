using AtiDocs.DataModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Timers;
using AtiDocs.Model;
using Microsoft.JSInterop;
using Markdig;
using Microsoft.AspNetCore.Components.Web;

namespace AtiDocs.Pages
{
    public partial class Write : ComponentBase
    {
        [Parameter]
        public string folderSlug { get; set; } = null;
        [Parameter]
        public string articleSlug { get; set; } = null;

        List<ContentPartItem> ContentParts = new();
        string DocTitle;
        
        
        protected override async Task OnInitializedAsync()
        {
            ContentParts.Add(new ContentPartItem() { Content = "", ElementReference = Guid.NewGuid().ToString() });
        }

        private async Task SaveTitle()
        {
            var title = await js.InvokeAsync<string>("GetText", "write-doc-title");
            DocTitle = title;
            StateHasChanged();
        }

        private async Task SaveContent(string reference)
        {
            var text = await js.InvokeAsync<string>("GetText", reference);

            ContentPartItem ActItem = ContentParts.SingleOrDefault(e => e.ElementReference == reference);

            string content = "";
            foreach (var item in text.Split("\n"))
            {
                if (string.IsNullOrEmpty(item) == false && string.IsNullOrWhiteSpace(item) == false)
                    content += $"{item}\n";
            }

            ActItem.Content = content;

            var pipe = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            ActItem.ContentHTML = Markdown.ToHtml(text, pipe);

            ActItem.Edit = false;

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
        }

        private async Task TurnOnEdit(string reference, ContentPartItem item)
        {
            await js.InvokeVoidAsync("EditResume", reference, item.Content);
            item.Edit = true;

            SetFocus(item);
        }

        private async Task KeyDownEvents(KeyboardEventArgs e, ContentPartItem item)
        {
            var text = await js.InvokeAsync<string>("GetText", item.ElementReference);

            if(e.ShiftKey && e.Key == "Enter")
            {
                ContentPartItem addItem = new ContentPartItem() { Content = "", ElementReference = Guid.NewGuid().ToString() };
                ContentParts.Insert(ContentParts.IndexOf(item) + 1, addItem);

                SetFocus(item);
            }

            if(e.Key == "Backspace" && (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)))
            {
                var actIndex = ContentParts.IndexOf(item);
                if(actIndex > 0)
                {
                    ContentParts.Remove(item);
                    SetFocus(ContentParts[actIndex - 1]);
                }
            }
        }
    }
}
