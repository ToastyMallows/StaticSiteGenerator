using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Models
{
    class FragmentMetadata : IFragmentMetadata
    {
        public string FragmentID { get; set; }
        public LayoutType LayoutType { get; set; }
        public string Markdown { get; set; }
    }
}
