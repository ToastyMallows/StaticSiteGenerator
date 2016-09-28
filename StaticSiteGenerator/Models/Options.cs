using CommandLine;
using System.Text;

namespace StaticSiteGenerator.Models
{
    internal sealed class Options : IOptions
    {
        [Option( 'g', "generate", Required = true, HelpText = "Generate the website" )]
        public bool Generate { get; set; }

        [Option( 't', "template", DefaultValue = "template.html", HelpText = "The template file that all blog pages will be based on" )]
        public string TemplateFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append( "Static Site Generate for C#" );
            return stringBuilder.ToString();
        }
    }
}
