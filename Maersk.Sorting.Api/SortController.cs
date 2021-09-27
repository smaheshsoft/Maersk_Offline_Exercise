using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using Maersk.Sorting.Api.ExceptionHandler;
using System.Net;
using Maersk.Sorting.Api.Services;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private readonly ICacheService _cacheService;
        public SortController(ISortJobProcessor sortJobProcessor, ICacheService cacheService)
        {
            _sortJobProcessor = sortJobProcessor;
            _cacheService = cacheService;
        }

        [HttpPost("run")]
        [Obsolete("This executes the sort job asynchronously. Use the asynchronous 'EnqueueJob' instead.")]
        public async Task<ActionResult<SortJob>> EnqueueAndRunJob(int[] values)
        {
            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);

            var completedJob = await _sortJobProcessor.Process(pendingJob);

            return Ok(completedJob);
        }

        [HttpPost]
        public async Task<ActionResult<SortJob>> EnqueueJob(int[] values)
        {
            var pendingJob = new SortJob(
             id: Guid.NewGuid(),
             status: SortJobStatus.Pending,
             duration: null,
             input: values,
             output: null);
            _cacheService.Enqueue(pendingJob);
            return await Task.FromResult(pendingJob);
        }

        [HttpGet]
        public async Task<ActionResult<SortJob[]>> GetJobs()
        {
            // TODO: Should return all jobs that have been enqueued (both pending and completed).
            var result = _cacheService.Get();
            if (result != null && result.Any())
            {
                return await Task.FromResult(result.ToArray());
            }
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "No Jobs found"));
        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<SortJob>> GetJob(Guid jobId)
        {
            // TODO: Should return a specific job by ID.
            var result = _cacheService.Get(jobId);
            if (result != null)
            {
                return await Task.FromResult(result);
            }
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Job with id doesnot exist"));
        }
    }
}
