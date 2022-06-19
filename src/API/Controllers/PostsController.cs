using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson;
using System.Security.Claims;

namespace API.Controllers
{
    //[Authorize(Roles = "Admin,Owner", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IRepository<Post> _repository;
        private readonly IRepository<User> _userRepository;
        string collection = "posts";

        public PostsController(IRepository<Post> repository)
        {
            _repository = repository;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll()
        {
            return await _repository.GetAll(collection);
        }

        // GET: api/Posts
        [HttpGet("user/{userid}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetAllUser(string userId)
        {
            return await _repository.GetWhere(x => x.UserId == userId, collection);
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> Get(string id)
        {
            var post = await _repository.GetById(id, collection);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        //// POST: api/Posts
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> Insert(Post post)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claims = User.Claims.ToList();
            var userEmail = claimsIdentity.FindFirst("Email").ToString();
            var user = await _userRepository.FirstOrDefault(x => x.Email == userEmail, "Users");

            if (post.UserId != user.Id)
                return Forbid("You can not insert posts in a different account other than yours.");
            
            if(post.SmilesIds.Count > 0 || post.CommentsIds.Count > 0)
                return Forbid("You can not set comments and smiles.");

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.Add(post, collection);
                }
                catch(FormatException ex) {
                    return BadRequest("Invalid Data: " + ex.Message);
                }
                return CreatedAtAction("GetPost", new { id = post.Id }, post);
            }

            return BadRequest();
        }

        //// DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            var post = await _repository.GetById(id, collection);
            if (post == null)
            {
                return NotFound();
            }

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claims = User.Claims.ToList();
            var userEmail = claimsIdentity.FindFirst("Email").ToString();
            var user = await _userRepository.FirstOrDefault(x => x.Email == userEmail, "Users");

            if (post.UserId != user.Id)
                return Forbid("You can not remove posts in a different account other than yours.");

            await _repository.Remove(id, collection);

            return NoContent();
        }
    }
}
