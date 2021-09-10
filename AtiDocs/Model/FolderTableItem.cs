using AtiDocs.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtiDocs.Model
{
    public class FolderTableItem
    {
        public Folder Folder { get; set; }

        public bool Display { get; set; } = false;

        public List<ArticleTableItem> ArticleList { get; set; } = new();

        public bool DisplayDocs { get; set; } = false;

        public string ElementReference { get; set; } = Guid.NewGuid().ToString();
    }
}
