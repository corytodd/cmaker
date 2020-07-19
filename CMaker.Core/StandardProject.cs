namespace CMaker.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Standard template
    /// </summary>
    public class StandardProject : IRootProject
    {
        /// <inheritdoc />
        public string ProjectName { get; set; } = "YourProject";

        /// <summary>
        /// Project name with no space which is required for some CMake values
        /// </summary>
        private string NormalizedProjectName => ProjectName.Replace(" ", "");

        /// <inheritdoc />
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

        /// <summary>
        /// Extract and inflate the project template
        /// </summary>
        /// <param name="outPath">Write template to this path</param>
        /// <param name="projectPath">Give project directory this top-level name</param>
        /// <returns>Result of initialization</returns>
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

        /// <summary>
        /// Apply rename rules to all template files. All files that contain
        /// a placeholder name are renamed based on the project name.
        /// </summary>
        /// <param name="projectRoot">Project directory</param>
        /// <returns>Results of rename operation</returns>
        private CMakerResult ApplyRename(string projectRoot)
        {
            const string templatePattern = "*CMAKE_LIB_NAME*";
            const string templateName = "CMAKE_LIB_NAME";

            // First rename all template directories
            foreach (var directory in Directory.GetDirectories(projectRoot, templatePattern,
                SearchOption.AllDirectories))
            {
                var di = new DirectoryInfo(directory);

                var newName = directory.Replace(templateName, NormalizedProjectName);

                di.MoveTo(newName);
            }

            // Then rename all template files
            foreach (var file in Directory.GetFiles(projectRoot, templatePattern, SearchOption.AllDirectories))
            {
                // Test files use all small case
                var replacement = file.Contains("_test") ? NormalizedProjectName.ToLower() : NormalizedProjectName;

                var newName = file.Replace(templateName, replacement);

                File.Move(file, newName);
            }

            return CMakerResult.Success;
        }

        /// <summary>
        /// Replace all CMaker variables in all project files with their calculated values
        /// </summary>
        /// <param name="projectRoot">Project directory</param>
        /// <returns>Results of rename operation</returns>
        private async Task<CMakerResult> ReplaceTemplateValues(string projectRoot)
        {
            // TODO exclusion files might need to be a global setting
            var excludeFiles = new HashSet<string>
                {".clang-format", ".clang-tidy", ".gitignore", ".gitattributes", "product.ico", "logo.jpg"};

            // month/day/year
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
        
        /// <inheritdoc />
        public override string ToString()
        {
            return ProjectName;
        }

    }
}