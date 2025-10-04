using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Jobs
{
    public interface IDebrisAlertJob
    {
        Task<Response<string>> ExecuteAsync();
    }
}
