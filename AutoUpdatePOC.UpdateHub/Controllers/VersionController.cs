using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AutoUpdatePOC.UpdateHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private const string ResourcesPath = "Resources";
        private readonly IHubContext<UpdateHub> hubContext;

        public VersionController(IHubContext<UpdateHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //await hubContext.Clients.All.SendAsync("CheckUpdate", "Unknown Version");

            if (Directory.Exists(ResourcesPath))
            {
                var files = Directory.GetFiles(ResourcesPath);
                var dict = new Dictionary<string, byte[]>();
                foreach (var file in files)
                {
                    dict.Add(Path.GetFileName(file), System.IO.File.ReadAllBytes(file));
                }
                await hubContext.Clients.All.SendAsync("PushFiles", dict);
                return new OkObjectResult(string.Join(", ", files));
            }

            return new OkResult();
        }

        [HttpPost]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var dir = Directory.CreateDirectory(ResourcesPath);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    var filePath = Path.Combine(dir.FullName, formFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }
    }
}
