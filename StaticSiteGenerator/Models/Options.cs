using CommandLine;
using StaticSiteGenerator.Models.Interfaces;
using System.Text;

namespace StaticSiteGenerator.Models
{
    internal sealed class Options : IOptions
    {
        [Option( 'g', "generate", HelpText = "Generate the website" )]
        public bool Generate { get; set; }

        [Option( 't', "template", DefaultValue = "template.html", HelpText = "The template file that all blog pages will be based on" )]
        public string TemplateFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Static Site Generate for C#" );
            return stringBuilder.ToString();
        }
    }
}
