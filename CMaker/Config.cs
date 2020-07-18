namespace CMaker
{
    using System;
    using System.Collections.Generic;
    using Core;

    public static class Config
    {
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
                            throw new Exception($"Unknown argument: {arg}");
                    }
                }
                else
                {
                    next.Invoke(arg);
                }
            }

            return project;
        }
    }
}