using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StudyApp.Server.Models;

namespace StudyApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowLocalhost")]
    public class PastStudySessionsController : Controller
    {


        // POST api/pastStudySessionsForm
        [HttpPost("pastStudySessionsForm")]
        public async Task<IActionResult> PastStudySessionDateRetrieval([FromBody] PastStudySessionsModel model)
        {
            if (ModelState.IsValid)
            {
                // Call the service method to create the user in Pixela
                var date = model.PastStudySessionsDate;
                // Process the form data
                return Ok();
            }

            return BadRequest(ModelState);
        }
    }

    public class PastStudySessionsModel
    {
        public string PastStudySessionsDate { get; set; }
    }

}
