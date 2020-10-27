using System;
using System.Collections.Generic;
using System.Text;

namespace H6.WeJobTasks.Configuration
{
  public class TaskDefinition
  {
    /// <summary>
    /// name of assembly where is task definition
    /// </summary>
    public string Assembly { get; set; }

    /// <summary>
    /// Namespace where is task definition
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Name of class
    /// </summary>
    public string Class { get; set; } //required

    /// <summary>
    /// Minutes after midnight for run task
    /// </summary>
    public int RunAfterMidnight { get; set; } //default 0

    /// <summary>
    /// Period [s] - how often will task run
    /// </summary>
    public int Period { get; set; } = 300;

    /// <summary>
    /// Maximum [s] task runtime
    /// </summary>
    public int Timeout { get; set; } = 10 * 60;
  }
}
