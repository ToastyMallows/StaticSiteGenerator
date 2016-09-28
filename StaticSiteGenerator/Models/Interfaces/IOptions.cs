
namespace StaticSiteGenerator.Models
{
    public interface IOptions
    {
        bool Generate { get; set; }

        string TemplateFile { get; set; }

        string GetUsage();
    }
}
