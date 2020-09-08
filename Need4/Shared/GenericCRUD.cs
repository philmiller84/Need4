using Models;
using Need4Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Need4
{
    public interface IGenericCRUD
    {
    }

    public static class GenericCRUDExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension method")]
        public static Task<T> GenericCreate<T>(this IGenericCRUD i, Need4Context db, T inputObject)
        {
            bool created = db.Database.EnsureCreated();
            db.Add(inputObject);
            db.SaveChanges();
            return Task.FromResult(inputObject);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension method")]
        public static IQueryable<T> GenericWrappedInvoke<T>(this IGenericCRUD i, Need4Context db, T inputObject, Func<Need4Context, T, IQueryable<T>> filterFunction, Action<T> formatFunction)
        {
            return GenericWrappedInvoke<T, T>(i, db, inputObject, filterFunction, formatFunction);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension method")]
        public static IQueryable<To> GenericWrappedInvoke<Ti, To>(this IGenericCRUD i, Need4Context db, Ti inputObject, Func<Need4Context, Ti, IQueryable<To>> filterFunction, Action<To> formatFunction)
        {
            try
            {
                IQueryable<To> q = filterFunction.Invoke(db, inputObject);
                q.ToList().ForEach(formatFunction);
                return q;
            }
            catch
            {
                return null;
            }
        }
    }
}
