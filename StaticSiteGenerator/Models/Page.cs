using System;
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
                return getFolderName(Metadata.Title);
                //string path = System.IO.Path.Combine(urlSafeTitle, "index.html");
                //return path;
            }
        }

        private string getFolderName(string path)
        {
            return Regex.Replace(path, @"[!@#$%^&*()\[\]\\\/:;'"".,?=+{}|_~`<>]", string.Empty).Replace(" ", "-");
        }
    }
}
