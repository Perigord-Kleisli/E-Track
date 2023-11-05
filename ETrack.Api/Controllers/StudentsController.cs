using ETrack.Api.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;


namespace ETrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   public class StudentsController : ControllerBase
   {
        private readonly IAuthRepository authRepository;
        private readonly IConfiguration configuration;

        public StudentsController( IAuthRepository authRepository, IConfiguration configuration)
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
        }
   } 
}