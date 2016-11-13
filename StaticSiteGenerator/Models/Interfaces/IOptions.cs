
namespace StaticSiteGenerator.Models
{
    public interface IOptions
    {
        bool Generate { get; set; }

        string BlogTemplateFile { get; set; }

        string PageTemplateFile { get; set; }

        string GetUsage();

        string OutputDirectory { get; set; }

        bool Force { get; set; }
    }
}
