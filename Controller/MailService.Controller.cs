using Microsoft.AspNetCore.Mvc;
using ISLE.Interfaces;
using System.Threading.Tasks;

namespace ISLE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailServiceController : ControllerBase
    {
        private readonly IMailService _mailService;
        public MailServiceController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] MailRequest request)
        {
            var result = await _mailService.SendMailAsync(request.To, request.Subject, request.Body);
            if (result)
                return Ok(new { message = "郵件發送成功" });
            return StatusCode(500, new { message = "郵件發送失敗" });
        }

        public class MailRequest
        {
            public string To { get; set; }
            public string Subject { get; set; }       
            public string Body { get; set; }
        }
    }
}
