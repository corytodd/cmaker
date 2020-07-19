namespace CMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class StandardProject : IRootProject
    {
        public string ProjectName { get; set; } = "YourProject";

        /// <summary>
        /// No spaces
        /// </summary>
        private string NormalizedProjectName => ProjectName.Replace(" ", "");
        
        public override string ToString()
        {
            return ProjectName;
        }

        public async Task<CMakerResult> CreateAsync(string outPath)
        {
            var projectPath = Path.Combine(outPath, ProjectName);

            var result = await InitializeTemplate(outPath, projectPath);
            if (result != CMakerResult.Success)
            {
                return result;
            }

            result = ApplyRename(projectPath);
            if (result != CMakerResult.Success)
            {
                return result;
            }

            result = await ReplaceTemplateValues(projectPath);

            return result;
        }

        private async Task<CMakerResult> InitializeTemplate(string outPath, string projectPath)
        {
            if (Directory.Exists(projectPath))
            {
                return CMakerResult.ProjectAlreadyExists;
            }

            var root = await ZipHelper.ExtractTemplate(ZipHelper.StandardTemplate, outPath).ConfigureAwait(false);

            var di = new DirectoryInfo(root);

            di.MoveTo(projectPath);

            return CMakerResult.Success;
        }

        private CMakerResult ApplyRename(string projectRoot)
        {
            const string templatePattern = "*CMAKE_LIB_NAME*";
            const string templateName = "CMAKE_LIB_NAME";

            foreach (var directory in Directory.GetDirectories(projectRoot, templatePattern,
                SearchOption.AllDirectories))
            {
                var di = new DirectoryInfo(directory);

                var newName = directory.Replace(templateName, NormalizedProjectName);

                di.MoveTo(newName);
            }

            foreach (var file in Directory.GetFiles(projectRoot, templatePattern, SearchOption.AllDirectories))
            {
                // Test files use all small case
                var replacement = file.Contains("_test") ? NormalizedProjectName.ToLower() : NormalizedProjectName;

                var newName = file.Replace(templateName, replacement);

                File.Move(file, newName);
            }

            return CMakerResult.Success;
        }

        private async Task<CMakerResult> ReplaceTemplateValues(string projectRoot)
        {
            var excludeFiles = new HashSet<string>
                {".clang-format", ".clang-tidy", ".gitignore", ".gitattributes", "product.ico", "logo.jpg"};

            var dateString = DateTime.Now.ToShortDateString();

            // Get all files starting from the template root
            foreach (var file in Directory.GetFiles(projectRoot, "*", SearchOption.AllDirectories))
            {
                var justFileName = Path.GetFileName(file);
                if (excludeFiles.Contains(justFileName))
                {
                    continue;
                }

                var rules = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("%%CMAKER_PROJECT_NAME%%", ProjectName),
                    new Tuple<string, string>("%%CMAKER_NORMALIZED_NAME%%", NormalizedProjectName),
                    new Tuple<string, string>("%%CMAKER_NORMALIZED_NAME_LOWER%%", NormalizedProjectName.ToLower()),
                    new Tuple<string, string>("%%CMAKER_DATE%%", dateString),
                    new Tuple<string, string>("%%CMAKER_CLI_NAME%%",  $"{NormalizedProjectName}_cli"),
                };

                var editor = new StreamEditor(file);

                await editor.ReplaceAll(rules).ConfigureAwait(false);
            }

            return CMakerResult.Success;
        }
    }
}