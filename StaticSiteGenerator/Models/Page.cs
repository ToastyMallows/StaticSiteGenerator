using System.Text.RegularExpressions;

namespace StaticSiteGenerator.Models
{
    internal class Page : IPage
    {
        public Page( IPageMetadata metadata, string html )
        {
            Guard.VerifyArgumentNotNull( metadata, nameof( metadata ) );
            Guard.VerifyArgumentNotNull( html, nameof( html ) );

            Metadata = metadata;
            HTML = html;
        }

        public IPageMetadata Metadata
        {
            get;
        }

        public string HTML
        {
            get;
        }

        public string Path
        {
            get
            {
                return getFolderName(Metadata.Title).ToLower();
            }
        }

        private string getFolderName(string path)
        {
            return Regex.Replace(path, @"[!@#$%^&*()\[\]\\\/:;'"".,?=+{}|_~`<>]", string.Empty).Replace(" ", "-");
        }
    }
}
