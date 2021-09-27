using System;
using System.Collections.Generic;

namespace Maersk.Sorting.Api.Services
{
    public interface ICacheService
    {
        bool Enqueue(SortJob sortJob);
        IEnumerable<SortJob> Get();
        public SortJob? Get(Guid jobId);
    }
}
