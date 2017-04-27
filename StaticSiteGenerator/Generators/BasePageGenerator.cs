using StaticSiteGenerator.Composers;

namespace StaticSiteGenerator.Generators
{
    internal class BasePageGenerator : IGenerator
    {
        private readonly ITemplateComposer _templateComposer;

        public BasePageGenerator( ITemplateComposer templateComposer )
        {
            Guard.VerifyArgumentNotNull( templateComposer, nameof( templateComposer ) );

            _templateComposer = templateComposer;
        }

        public void Generate()
        {

            _templateComposer.Compose();
        }
    }
}
