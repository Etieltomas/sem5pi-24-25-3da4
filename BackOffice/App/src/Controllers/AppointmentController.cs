using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.AppointmentEntity;


namespace Sempi5.Controllers 
{
    [Route("api/[controller]")]
    [ApiController] 
    public class AppointmentController : ControllerBase
    {

        private readonly AppointmentService _service;

        public AppointmentController(AppointmentService service)
        {
            _service = service;
        }
    }
}