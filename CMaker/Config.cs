namespace CMaker
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Core;

    /// <summary>
    /// Command line argument parser
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Parses command lines args into a project configuration
        /// </summary>
        /// <param name="args">Program arguments</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown on unknown argument</exception>
        public static IRootProject Parse(IEnumerable<string> args)
        {
            var project = new StandardProject();

            Action<string> next = null;
            foreach (var arg in args)
            {
                if (next is null)
                {
                    switch (arg)
                    {
                        case "-p":
                        case "--project-name":
                            next = s => project.ProjectName = s;
                            break;

                        default:
                            throw new ArgumentException($"Unknown argument: {arg}");
                    }
                }
                else
                {
                    next.Invoke(arg);
                }
            }

            return project;
        }

        /// <summary>
        /// Return usage details
        /// </summary>
        /// <returns>Formatted usage string</returns>
        public static string Usage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage: CMaker -p ProjectName");

            return sb.ToString();
        }
    }
}