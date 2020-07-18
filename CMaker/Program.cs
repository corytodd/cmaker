namespace CMaker
{
    using System;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var project = Config.Parse(args);

            Console.WriteLine($"CMaker generating: {project}");

            await project.CreateAsync(Environment.CurrentDirectory);
        }
    }
}