using System.Collections.Generic;

namespace StaticSiteGenerator.Generators
{
    internal sealed class SiteGenerator : IGenerator
    {
        private readonly IEnumerable<IGenerator> _siteGenerators;

        public SiteGenerator( IEnumerable<IGenerator> siteGenerators )
        {
            Guard.VerifyArgumentNotNull( siteGenerators, nameof( siteGenerators ) );

            _siteGenerators = siteGenerators;
        }

        public bool TryGenerate()
        {
            foreach ( IGenerator generator in _siteGenerators )
            {
                if ( !generator.TryGenerate() )
                {
                    return false;
                }

            }

            return true;
        }
    }
}
