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

        public StreamEditor(string filePath)
        {
            _filePath = filePath;
        }

        public Task Replace(string pattern, string with)
        {
            var rules = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(pattern, with)
            };

            return ReplaceAll(rules);
        }
        
        public async Task ReplaceAll(IEnumerable<Tuple<string, string>> rules)
        {
            var tempFile = await EditAsStream(rules);

            File.Delete(_filePath);

            File.Move(tempFile.Path, _filePath);
        }

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