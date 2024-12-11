using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StudyApp.Server.Models;
using System.Net;
using static StudyApp.Server.PixelaService;

namespace StudyApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowLocalhost")]
    public class PixelaController : ControllerBase
    {
        private readonly PixelaService _pixelaService;

        public PixelaController(PixelaService pixelaService)
        {
            _pixelaService = pixelaService;
        }

        // POST api/pixelaSignUpForm
        [HttpPost("pixelaSignUpForm")]
        public async Task<IActionResult> SignUp([FromBody] PixelaSignUpModel model)
        {
            if (ModelState.IsValid)
            {
                // Call the service method to create the user in Pixela
                var response = await _pixelaService.CreateUserAccount(model.Username, model.Token);
                // Process the form data
                return Ok(new { Message = "Sign-up successful", Data = model });
            }
            
            return BadRequest(ModelState);
        }


    }
}