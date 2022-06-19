using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;

namespace API.Controllers
{
    //[Authorize(Roles = "Admin,Owner", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class SmilesController : ControllerBase
    {
        private readonly IRepository<Smile> _repository;
        private readonly IRepository<User> _userRepository;
        string collection = "smiles";

        public SmilesController(IRepository<Smile> repository)
        {
            _repository = repository;
        }

        //// POST: api/Smiles
        //// To protect from oversmileing attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Smile>> Insert(Smile smile)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claims = User.Claims.ToList();
            var userEmail = claimsIdentity.FindFirst("Email").ToString();
            var user = await _userRepository.FirstOrDefault(x => x.Email == userEmail, "Users");

            if (smile.UserId != user.Id)
                return Forbid("You can not give smiles from a different account other than yours.");

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.Add(smile, collection);
                }
                catch(FormatException) {
                    return BadRequest("Invalid Data.");
                }
                return CreatedAtAction("GetSmile", new { id = smile.Id }, smile);
            }

            return BadRequest();
        }

        //// DELETE: api/Smiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            var smile = await _repository.GetById(id, collection);
            if (smile == null)
            {
                return NotFound();
            }

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claims = User.Claims.ToList();
            var userEmail = claimsIdentity.FindFirst("Email").ToString();
            var user = await _userRepository.FirstOrDefault(x => x.Email == userEmail, "Users");

            if (smile.UserId != user.Id)
                return Forbid("You can not remove smiles from a different account other than yours.");

            await _repository.Remove(id, collection);

            return NoContent();
        }
    }
}
