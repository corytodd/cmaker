namespace CMaker.Core
{
    using System.Threading.Tasks;

    public interface IRootProject
    {
        string CppStdLevel { get; set; }

        string ProjectName { get; set; }
        
        Task<CMakerResult> CreateAsync(string outPath);
    }
}