using AtiDocs.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtiDocs.Model
{
    public class ArticleTableItem
    {
        public Article Article { get; set; } = new();

        public bool Display { get; set; } = false;

        public string ElementReference { get; set; } = Guid.NewGuid().ToString();
    }
}
