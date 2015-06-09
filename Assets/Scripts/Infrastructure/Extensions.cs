namespace Assets.Scripts.Infrastructure
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static class Extensions
  {
    public static IList<T> FindMatches<T>(this IList<T> source, Func<T, T, bool> equal)
    {
      return source.SkipWhile((item, index) =>
      {
        if (index + 2 >= source.Count)
          return true;

        if (equal(item, source[index + 1]) && equal(item, source[index + 2]))
          return false;

        return true;
      }).ToList();
    }

    public static IList<T> TakeMatches<T>(this IList<T> source, Func<T, T, bool> equal)
    {
      return source.TakeWhile((item, index) =>
      {
        if (index < 1 || equal(item, source[index - 1]))
          return true;

        return false;
      }).ToList();
    }

  }
}
