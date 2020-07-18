namespace CMaker.Core
{
    using System.IO;
    using System.Threading.Tasks;

    public class StandardProject : IRootProject
    {
        public string ProjectName { get; set; } = "YourProject";

        public string CppStdLevel { get; set; } = "20";

        public override string ToString()
        {
            return ProjectName;
        }

        public async Task<CMakerResult> CreateAsync(string outPath)
        {
            var projectPath = Path.Combine(outPath, ProjectName);

            if (Directory.Exists(projectPath))
            {
                return CMakerResult.ProjectAlreadyExists;
            }

            var root = await ZipHelper.ExtractTemplate(ZipHelper.StandardTemplate, outPath).ConfigureAwait(false);

            var di = new DirectoryInfo(root);

            di.MoveTo(projectPath);

            return CMakerResult.Success;
        }
    }
}