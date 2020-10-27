using System;
using System.Collections.Generic;
using System.Text;

namespace H6.WeJobTasks.Configuration
{
  public class WeJobTasks
  {
    /// <summary>
    /// default assembly name where are tasks
    /// </summary>
    public string DefaultAssembly { get; set; }

    /// <summary>
    /// default namespace where are tasks
    /// </summary>
    public string DefaultNamespace { get; set; }

    /// <summary>
    /// Tasks
    /// </summary>
    public TaskDefinition[] Tasks { get; set; }
}
}
