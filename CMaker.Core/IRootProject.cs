namespace CMaker.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// Root CMake project configuration
    /// </summary>
    public interface IRootProject
    {
        /// <summary>
        /// The name of the CMake project.
        /// CMaker will normalize this for correct CMake usage
        /// </summary>
        string ProjectName { get; set; }
        
        /// <summary>
        /// Root namespace
        /// </summary>
        string Namespace { get; set; }
        
        /// <summary>
        /// Generate this project
        /// </summary>
        /// <param name="outPath">Directory to write project</param>
        /// <returns>Generation result</returns>
        Task<CMakerResult> CreateAsync(string outPath);
    }
}