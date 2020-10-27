using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace H6.WeJobTasks
{
  public abstract class WebJobTask
  {
    public string InstanceId { get; internal set; }

    public ILogger Logger { get; internal set; }

    public ILoggerFactory LoggerFactory { get; internal set; }

    /// <summary>
    /// Minutes after midnight for run task
    /// </summary>
    internal int RunAfterMidnight { get; set; }

    internal string TaskName { get; set; }

    /// <summary>
    /// Date and time in UTC for next run
    /// </summary>
    internal DateTime NextRun { get; set; }

    /// <summary>
    /// Last date and time in UCT when task starting
    /// </summary>
    internal DateTime? LastStart { get; private set; }

    internal long RunCounter { get; set; }

    internal bool Running => !(Runner?.IsCompleted ?? true);

    internal Task Runner { get; private set; }

    /// <summary>
    /// Maximum [s] task runtime
    /// </summary>
    internal int Timeout { get; set; }

    /// <summary>
    /// Period [s] - how often will task run
    /// </summary>
    internal int Period { get; set; }

    /// <summary>
    /// timeout for process on task expired
    /// </summary>
    public bool TimeoutExpired => this.Running && LastStart.Value.AddSeconds(this.Timeout) < DateTime.UtcNow;

    /// <summary>
    /// call when task is initialized in memory
    /// </summary>
    protected abstract void Initialize(IServiceProvider serviceProvider);

    private CancellationTokenSource _tokenSource;

    internal void InitTask(CancellationToken token, IServiceProvider serviceProvider)
    {
      LogInfo("Start - Init task");
      _tokenSource = new CancellationTokenSource();
      token.Register(() =>
      {
        if (_tokenSource != null)
          _tokenSource.Cancel();
      });

      Initialize(serviceProvider);

      LogInfo("End - Init task");
    }

    internal void Abort()
    {
      LogWarning("Abort task");
      Runner = null;
      _tokenSource?.Cancel();
    }


    protected abstract void MainProcess(CancellationToken cancellationToken);

    private void LogInfo(string message)
    {
      Logger.LogInformation($"{InstanceId}/{TaskName}: {message}");
    }

    private void LogWarning(string message)
    {
      Logger.LogWarning($"{InstanceId}/{TaskName}: {message}");
    }

    private void LogError(Exception ex, string message = "")
    {
      Logger.LogError(ex, $"{InstanceId}/{TaskName}: {message}; Ex: {ex}");
    }

    private void LogCritical(Exception ex, string message = "")
    {
      Logger.LogCritical(ex, $"{InstanceId}/{TaskName}: {message}; Ex: {ex}");
    }

    public void Start()
    {
      LastStart = DateTime.UtcNow;
      RunCounter++;

      try
      {
        if (_tokenSource.IsCancellationRequested)
        {
          _tokenSource.Dispose();
          _tokenSource = new CancellationTokenSource();
        }

        Runner = Task.Run(() =>
        {
          try
          {
            LogInfo($"Start - MainProcess - RunCounter:{RunCounter}");
            MainProcess(_tokenSource.Token);
          }
          catch (ThreadAbortException)
          {
            LogWarning("Abort of task");
          }
          catch (Exception ex)
          {
            LogError(ex, "Exception of task");
          }
          NextRun = DateTime.UtcNow.AddSeconds(this.Period);
          LogInfo($"End - MainProcess - NextRun:{NextRun}");
        });
      }
      catch (Exception ex)
      {
        LogCritical(ex);
      }
    }
  }
}
