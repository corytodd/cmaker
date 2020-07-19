namespace CMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple Unix sed implementation
    /// </summary>
    public class StreamEditor
    {
        private readonly string _filePath;

        /// <summary>
        /// Create a new editor for this file
        /// </summary>
        /// <param name="filePath">File to edit</param>
        public StreamEditor(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Replace all occurrence of 'pattern' with 'replacement'
        /// </summary>
        /// <param name="pattern">string pattern</param>
        /// <param name="replacement">string replacement</param>
        public Task Replace(string pattern, string replacement)
        {
            var rules = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(pattern, replacement)
            };

            return ReplaceAll(rules);
        }
        
        /// <summary>
        /// Replace all patterns with their corresponding replacement
        /// </summary>
        /// <param name="rules">List of rules in the format 'pattern':'replace'</param>
        public async Task ReplaceAll(IEnumerable<Tuple<string, string>> rules)
        {
            var tempFile = await EditAsStream(rules);

            File.Delete(_filePath);

            File.Move(tempFile.Path, _filePath);
        }

        /// <summary>
        /// Perform stream edit on this file and return cached result. We use a temp file
        /// to avoid reading the entire file into memory.
        /// </summary>
        /// <param name="rules">List of rules in the format 'pattern':'replace'</param>
        /// <returns>Path to temporary file containing all edits</returns>
        private async Task<TempFile> EditAsStream(IEnumerable<Tuple<string, string>> rules)
        {
            var rulesList = rules.ToList();
            var tempFile = new TempFile();

            using var source = new StreamReader(_filePath, Encoding.UTF8);
            await using var destStream = new FileStream(tempFile.Path, FileMode.Create);
            await using var destFile = new StreamWriter(destStream);

            while (!source.EndOfStream)
            {
                var line = await source.ReadLineAsync();

                if (line is null)
                {
                    break;
                }

                foreach (var (pattern, replacement) in rulesList)
                {
                    line = line.Replace(pattern, replacement);
                }

                await destFile.WriteLineAsync(line);
            }

            return tempFile;
        }
    }
}