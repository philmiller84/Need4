using Models;
using Need4Protocol;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Need4
{
    public interface IGenericCRUD
    {
        //public Task<ActionResponse> GenericCreate(object inputObject, Type inputType);
    }

    public static class GenericCRUDExtensions
    {
        public static Task<ActionResponse> GenericCreate(this IGenericCRUD i, object inputObject, Type inputType)
        {
            try
            {
                dynamic thisObject = Convert.ChangeType(inputObject, inputType);
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
    }
}
