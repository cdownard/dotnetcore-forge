using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Forge.Model
{
  public enum Status { SUCCESS, ERROR };

  public class Model
  {
    public string ClassName { get; set; }
    public Dictionary<string, string> Properties { get; set; }

    public static string ModelTemplate()
    {
      // TODO: Figure out how to get their app name from maybe project.json?
      return @"
      namespace YourAppName.Models
      {
        public class {{className}}
        {
          {{properties}}
        }
      }
      ";
    }
    public override string ToString()
    {
      var template = Model.ModelTemplate();
      template.Replace("{{className}}", this.ClassName);
      var sb = new StringBuilder();
      foreach(KeyValuePair<string, string> prop in this.Properties)
      {
        sb.AppendFormat("public {0} {1} { get; set; }", prop.Value, prop.Key);
      }
      template.Replace("{{properties}}", sb.ToString());

      return template;
    }

    public static Status CreateDirectory()
    {
      if(Model.DirectoryExists())
        return Status.SUCCESS;

      try
      {
        DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Models"));
        Console.WriteLine("Created {0}...", di.FullName);
      }
      catch(Exception e)
      {
        Console.WriteLine("Failed to create 'Models' directory: {0}", e.ToString());
        return Status.ERROR;
      }

      return Status.SUCCESS;
    }

    public static bool DirectoryExists()
    {
      return Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Models"));
    }

    public static string ModelsDirectory()
    {
      return Path.Combine(Directory.GetCurrentDirectory(), "Models");
    }

    public Status Save() {
      if(!Model.DirectoryExists())
      {
        Console.WriteLine("Failed to write '{0}': 'Models' directory does not exist", this.ClassName);
        return Status.ERROR;
      }

      try
      {
        File.WriteAllText(Path.Combine(Model.ModelsDirectory(), this.ClassName), this.ToString());
      }
      catch(Exception e)
      {
        Console.WriteLine("Error saving '{0}': {1}", this.ClassName, e.ToString());
        return Status.ERROR;
      }

      return Status.SUCCESS;
    }
  }
}
