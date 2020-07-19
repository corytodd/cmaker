namespace CMaker.Core
{
    using System.Threading.Tasks;

    public interface IRootProject
    {
        string ProjectName { get; set; }
        
        Task<CMakerResult> CreateAsync(string outPath);
    }
}