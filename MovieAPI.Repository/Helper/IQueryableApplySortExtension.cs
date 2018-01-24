using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;

namespace MovieAPI.Repository
{
    public static class IQueryableApplySortExtension
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string sortItem, int desc)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", "Data source is empty.");
            }

            if (sortItem == null)
            {
                return source;
            }

            var lstSort = sortItem.Split(',');

            string sortExpression = string.Empty;

            foreach (var sortOption in lstSort)
            {
                if (isValidSortOption(sortOption.Trim())) {
                    sortExpression = sortExpression + sortOption + " " + (desc == 1 ? "descending" : "") + "," ;
                }
            }

            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                source = source.OrderBy(sortExpression.Remove(sortExpression.Count() - 1));
            }

            return source;
        }

        public static IQueryable<Movie> ApplyFilter<Mobvie>(this IQueryable<Movie> source, string filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", "Data source is empty.");
            }

            // searching
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.ToLower();
                source = source.Where(x =>
                    x.Title.ToLower().Contains(filter) ||
                    x.Classification.ToLower().Contains(filter) ||
                    x.Genre.ToLower().Contains(filter) ||
                    x.Rating.ToString().Contains(filter) ||
                    x.ReleaseDate.ToString().Contains(filter)
                    );
            }
            return source;
        }

        private static bool isValidSortOption(string sortOption)
        {
            foreach (PropertyInfo prop in typeof(Movie).GetProperties())
            {
                if (sortOption.ToUpper() == prop.Name.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
    }
}