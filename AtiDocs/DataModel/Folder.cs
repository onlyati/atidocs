using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtiDocs.DataModel
{
    public class Folder
    {
        public Folder()
        {
            Articles = new HashSet<Article>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
