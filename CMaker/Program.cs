namespace CMaker
{
    using System;
    using System.Threading.Tasks;
    using Core;

    class Program
    {
        /// <summary>
        ///     Generate a CMake template project
        /// </summary>
        /// <param name="projectName">Top-level name of project</param>
        /// <param name="namespace">Outermost namespace in your project</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        static async Task<int> Main(string projectName, string @namespace)
        {
            try
            {
                var project = new StandardProject
                {
                    ProjectName = projectName,
                    Namespace = @namespace
                };
                
                Console.WriteLine($"CMaker generating: {project}");

                var result = await project.CreateAsync(Environment.CurrentDirectory);

                switch (result)
                {
                    case CMakerResult.Success:
                        Console.WriteLine("Completed successfully");
                        return 0;
                    
                    case CMakerResult.ProjectAlreadyExists:
                        Console.WriteLine("Aborted, project directory already exists");
                        return 1;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CMaker failed: {ex.Message}");
                return 0;
            }
        }
    }
}