using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtiDocs.DataModel
{
    public class Article
    {
        public int Id { get; set; }

        public int FolderId { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ChangedAt { get; set; }

        public string Content { get; set; }

        public string Slug { get; set; }

        public virtual Folder Folder { get; set; }
    }
}
