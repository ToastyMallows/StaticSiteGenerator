using CommandLine;
using System.Text;

namespace StaticSiteGenerator.Models
{
    internal sealed class Options : IOptions
    {
        [Option( 'g', "generate", Required = true, HelpText = "Generate the website" )]
        public bool Generate { get; set; }

        [Option( 'b', "blogtemplate", DefaultValue = "blogtemplate.html", HelpText = "The template file that all blog pages will be based on, default is a file in this directory called template.html" )]
        public string BlogTemplateFile { get; set; }

        [Option( 'p', "pagetemplate", DefaultValue = "pagetemplate.html", HelpText = "The template file that all pages will be based on, default is a file in this directory called pagetemplate.html" )]
        public string PageTemplateFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append( "Static Site Generate for C#" );
            return stringBuilder.ToString();
        }

        [Option( 'o', "output", DefaultValue = "_website", HelpText = "The folder that the website files will be output to, default is '_website'." )]
        public string OutputDirectory { get; set; }

        [Option( 'f', "force", DefaultValue = false, HelpText = "Overwrites the output directory if it exists already, default is false." )]
        public bool Force { get; set; }
    }
}
