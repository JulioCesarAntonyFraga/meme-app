using API.Interfaces;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginSuccess>> Authenticate(Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authRepository.Login(login);

            if(success == null)
                return NotFound("Invalid email or password");

            return Ok(success);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginSuccess>> Register(Register register)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authRepository.RegisterUser(register);

            if (success == null)
                return Conflict("User already exists");

            return Ok(success);
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<ActionResult<RefreshTokenSuccess>> Refresh(RefreshTokenRequest refreshTokenRequest)
        {
            var token = await _authRepository.RefreshToken(refreshTokenRequest);

            return Ok(token);
        }
    }
}
