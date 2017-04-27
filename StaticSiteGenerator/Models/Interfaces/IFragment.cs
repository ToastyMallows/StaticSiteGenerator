using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Models
{
    public interface IFragment : IBasePage
    {
        IFragmentMetadata Metadata
        {
            get;
        }
    }
}
