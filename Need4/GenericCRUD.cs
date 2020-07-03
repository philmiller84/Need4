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
        public static Task<ActionResponse> GenericCreate<T>(this IGenericCRUD i, T inputObject)
        {
            try
            {
                using (Need4Context db = new Need4Context())
                {
                    bool created = db.Database.EnsureCreated();

                    db.Add(inputObject);
                    db.SaveChanges();

                    return Task.FromResult(new ActionResponse { Result = (int)HttpStatusCode.OK });
                }
            }
            catch
            {
                return Task.FromResult(new ActionResponse { Result = (int)HttpStatusCode.Forbidden });
            }
        }

        public static IQueryable<T> GenericWrappedInvoke<T>(this IGenericCRUD i, T inputObject, Func<Need4Context, IQueryable<T>> filterFunction, Action<T> formatFunction)
        {
            try
            {
                using (Need4Context db = new Need4Context())
                {
                    IQueryable<T> q =  filterFunction.Invoke(db);
                    q.ToList().ForEach(formatFunction);
                    return q;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
