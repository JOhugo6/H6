using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Extensions
{
  /// <summary>
  /// Extension for data type DateTime
  /// </summary>
  public static class DateTime_Extensions
  {
    /// <summary>
    /// For date returns date with last day in month
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime LastDayInMonth(this DateTime date) => new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

    /// <summary>
    /// For date returns date with first day in month
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime FirstDayInMonth(this DateTime date) => new DateTime(date.Year, date.Month, 1);

    /// <summary>
    /// For date returns date with first day in year
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime FirstDayInYear(this DateTime date) => new DateTime(date.Year, 1, 1);

    /// <summary>
    /// For date returns date with last day in year
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime LastDayInYear(this DateTime date) => new DateTime(date.Year, 12, 31);

    /// <summary>
    /// For date returns date with first day of week
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime FirstDayOfWeek(this DateTime date)
    {
      var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
      var diff = date.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
      if (diff < 0)
        diff += 7;
      return date.AddDays(-diff).Date;
    }

    /// <summary>
    /// For date returns date with last day of week
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime LastDayOfWeek(this DateTime date) => date.FirstDayOfWeek().AddDays(6);

    /// <summary>
    /// For date returns date with last day of next week
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime LastDayOfNextWeek(this DateTime date) => date.FirstDayOfWeek().AddDays(13);

    /// <summary>
    /// For date returns date with last day of next month
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime LastDayOfNextMonth(this DateTime date) => date.FirstDayInMonth().AddMonths(2).AddDays(-1);

    /// <summary>
    /// For date returns date with first day of next week
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime FirstDayOfNextMonth(this DateTime date) => date.FirstDayInMonth().AddMonths(1);
  }
}
