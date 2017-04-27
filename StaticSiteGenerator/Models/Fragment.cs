using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Models
{
    internal sealed class Fragment : IFragment
    {
        public Fragment(IFragmentMetadata metadata, string html)
        {
            Guard.VerifyArgumentNotNull(metadata, nameof(metadata));
            Guard.VerifyArgumentNotNull(html, nameof(html));

            Metadata = metadata;
            HTML = html;
        }

        public IFragmentMetadata Metadata
        {
            get;
        }

        public string HTML
        {
            get;
        }
    }
}
