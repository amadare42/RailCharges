using System;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;

public static class ProjectExtensions
{
    public static string GetRequiredProperty(this Project project, string name)
    {
        var valueStr = project.GetProperty(name);

        if (string.IsNullOrEmpty(valueStr))
        {
            throw new Exception($"Property {name} is missing in project '{project.Name}'");
        }

        return valueStr;
    }
}