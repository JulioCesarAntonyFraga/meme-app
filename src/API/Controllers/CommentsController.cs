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
    public class CommentsController : ControllerBase
    {
        private readonly IRepository<Comment> _repository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Post> _postRepository;
        string collection = "comments";

        public CommentsController(IRepository<Comment> repository)
        {
            _repository = repository;
        }

        //// POST: api/Comments
        //// To protect from overcommenting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> Insert(Comment comment)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claims = User.Claims.ToList();
            var userEmail = claimsIdentity.FindFirst("Email").ToString();
            var user = await _userRepository.FirstOrDefault(x => x.Email == userEmail, "Users");

            if (comment.UserId != user.Id)
                return Forbid("You can not give comments from a different account other than yours.");

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.Add(comment, collection);
                }
                catch(FormatException) {
                    return BadRequest("Invalid Data.");
                }
                return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
            }

            return BadRequest();
        }

        //// DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            var comment = await _repository.GetById(id, collection);
            if (comment == null)
            {
                return NotFound();
            }
            var post = await _postRepository.GetById(comment.PostId, collection);

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claims = User.Claims.ToList();
            var userEmail = claimsIdentity.FindFirst("Email").ToString();
            var user = await _userRepository.FirstOrDefault(x => x.Email == userEmail, "Users");

            if (comment.UserId != user.Id && user.Id != post.UserId)
                return Forbid("Only the post's owner and the comment's owner can remove the comment.");

            await _repository.Remove(id, collection);

            return NoContent();
        }
    }
}
