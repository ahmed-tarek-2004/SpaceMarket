using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.DataAccess.Services.DebrisAlert;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Jobs
{
    public class DebrisAlertJob : IDebrisAlertJob
    {
        private readonly IDebrisAlertService _service;
        public DebrisAlertJob(IDebrisAlertService service)
        {
            _service = service;
        }

        public async Task<Response<string>> ExecuteAsync()
        {
            return await _service.RunDebrisCheckAsync(DateTime.UtcNow);
        }
    }
}
