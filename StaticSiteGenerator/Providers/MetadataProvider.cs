using Newtonsoft.Json;
using StaticSiteGenerator.Contexts;
using StaticSiteGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.FormattableString;

namespace StaticSiteGenerator.Providers
{
    internal sealed class MetadataProvider : IMetadataProvider
    {
        private LayoutType _layoutType;
        private List<IBasePageMetadata> _metadata = new List<IBasePageMetadata>();
        private Type _metadataType;

        public IReadOnlyCollection<IBasePageMetadata> Metadata
        {
            get
            {
                return new ReadOnlyCollection<IBasePageMetadata>(_metadata);
            }
        }

        private string FolderName
        {
            get
            {
                string folderName = string.Empty;

                switch (_layoutType)
                {
                    case LayoutType.Post:
                        folderName = "posts"; // TODO: not hardcoded
                        break;
                    case LayoutType.Page:
                        folderName = "pages"; // TODO: not hardcoded
                        break;
                    case LayoutType.Fragment:
                        folderName = "fragments"; // TODO: not hardcoded
                        break;
                    default:
                        ThrowInvalidLayoutTypeException();
                        break;
                }

                return folderName;
            }
        }

        public MetadataProvider( LayoutType layoutType )
        {
            Guard.VerifyArgumentNotNull(layoutType, nameof(layoutType));

            _layoutType = layoutType;

            ProcessLayoutType();
        }

        private void ProcessLayoutType()
        {
            switch (_layoutType)
            {
                case LayoutType.Post:
                    _metadataType = typeof(BlogPostMetadata);
                    break;
                case LayoutType.Page:
                    _metadataType = typeof(PageMetadata);
                    break;
                case LayoutType.Fragment:
                    _metadataType = typeof(FragmentMetadata);
                    break;
                default:
                    ThrowInvalidLayoutTypeException();
                    break;
            }

            ProcessMetadataFiles();
        }

        private void ProcessMetadataFiles()
        {
            IEnumerable<string> jsonMetadataFiles = GetJsonMetadataFiles(FolderName); 

            foreach (string jsonFile in jsonMetadataFiles)
            {
                string json = File.ReadAllText(jsonFile);

                dynamic metadata = JsonConvert.DeserializeObject(json, _metadataType);
                if (metadata.LayoutType != _layoutType)
                {
                    // This metadata file isn't the correct layout type, move on!
                    continue;
                }

                _metadata.Add(metadata);
            }
        }

        private IEnumerable<string> GetJsonMetadataFiles(string folder)
        {
            string directory = Path.Combine(IOContext.Current.GetCurrentDirectory(), folder);

            if (!IOContext.Current.DirectoryExists(directory))
            {
                throw new FileNotFoundException(Invariant($"No metadata files exist in the {_layoutType.ToString()} folder."));
            }

            return IOContext.Current.GetFilesFromDirectory(directory, "*.json", SearchOption.AllDirectories);
        }

        private void ThrowInvalidLayoutTypeException()
        {
            throw new ArgumentOutOfRangeException(Invariant($"The LayoutType of {_layoutType.ToString()} is unknown and not supported"));
        }
    }
}
