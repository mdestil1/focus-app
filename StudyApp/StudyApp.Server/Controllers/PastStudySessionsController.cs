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
        private readonly StudySessionService _studySessionService;

        public PastStudySessionsController(StudySessionService studySessionService)
        {
            _studySessionService = studySessionService;
        }

        /** POST api/pastStudySessionsForm
        [HttpPost("sessionData")]
        public async Task<IActionResult> FindSession(string date, string userId, string token)
        {
            var studySession = _studySessionService.GetStudySession(date, userId, token);
            if (studySession == null)
            {
                return NotFound("No session found.");
            }

            return View(studySession);
        }**/

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
