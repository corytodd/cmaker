namespace CMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Embedded zip file loader
    /// </summary>
    public static class ZipHelper
    {
        private static readonly Lazy<HashSet<string>> AvailableResources =
            new Lazy<HashSet<string>>(() =>
                new HashSet<string>(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames()));

        public const string StandardTemplate = "CMaker.Core.Templates.standard.template";

        /// <summary>
        /// Extracts the requested template to the specified path
        /// </summary>
        /// <returns>Full path to extracted template</returns>
        public static async Task<string> ExtractTemplate(string template, string outPath)
        {
            if (string.IsNullOrEmpty(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (string.IsNullOrEmpty(outPath))
            {
                throw new ArgumentNullException(nameof(outPath));
            }

            if (!AvailableResources.Value.Contains(template))
            {
                throw new Exception($"Unknown template requested: {template}");
            }

            var tempZip = new TempFile();

            await UnpackTemplate(template, tempZip).ConfigureAwait(false);

            // Cleanup in case a prior extraction attempt was interrupted
            var rootName = GetZipRootName(tempZip.Path);
            var unpackedPath = Path.Combine(outPath, rootName);
            if (Directory.Exists(unpackedPath))
            {
                Directory.Delete(unpackedPath);
            }

            ZipFile.ExtractToDirectory(tempZip.Path, outPath);

            return Path.Combine(outPath, rootName);
        }

        /// <summary>
        /// Unpacks this project template to a temporary file
        /// </summary>
        /// <param name="template">Template to unpack</param>
        /// <param name="temp">Template written to this file as Zip</param>
        /// <exception cref="ArgumentException">Template is not found</exception>
        private static async Task UnpackTemplate(string template, TempFile temp)
        {
            //write the resource zip file to the temp directory
            await using var source = System.Reflection.Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(template);

            if (source is null)
            {
                throw new ArgumentException($"Could not find template {template}");
            }

            await using var dest = new FileStream(temp.Path, FileMode.Create);

            await source.CopyToAsync(dest).ConfigureAwait(false);
        }

        /// <summary>
        /// Inspect zip file to find the original root folder name
        /// </summary>
        /// <param name="zipFile">Zip file to inspect</param>
        /// <returns>Relative name of root or null on error</returns>
        private static string GetZipRootName(string zipFile)
        {
            using var archive = ZipFile.OpenRead(zipFile);
            var root = archive.Entries.FirstOrDefault();
            var name = root?.FullName;

            // Strip any leading or trailing path separators
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim('/').Trim('\\');
            }

            return name;
        }
    }
}