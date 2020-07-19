namespace CMaker
{
    using System;
    using System.Threading.Tasks;
    using Core;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var project = Config.Parse(args);

            Console.WriteLine($"CMaker generating: {project}");

            try
            {
                var result = await project.CreateAsync(Environment.CurrentDirectory);

                switch (result)
                {
                    case CMakerResult.Success:
                        Console.WriteLine("Completed successfully");
                        break;
                    case CMakerResult.ProjectAlreadyExists:
                        Console.WriteLine("Aborted, project directory already exists");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CMaker failed: {ex.Message}");
            }
        }
    }
}