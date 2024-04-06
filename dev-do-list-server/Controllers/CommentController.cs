using DevDoListServer.Jwt;
using DevDoListServer.Models;
using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DevDoListServer.Controllers
{
    [Route("api/v1/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentRepository _commentRepository;

        public CommentController(CommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        
        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForTask([FromHeader(Name = "Authorization")] string authToken, [FromRoute] int taskId)
        { 
            var username = JwtUtils.GetClaim(authToken, "username");
            
            var comments = await _commentRepository.FindAll(c => c.TaskId == taskId);
            var comment = comments.FirstOrDefault();
            if (comment != null && comment.Task!.User!.Username != username)
            {
                return Unauthorized();
            }

            if (comment == null)
            {
                return NotFound();
            }
            
            return comments.Select(comment => new CommentDto(comment)).ToList();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment([FromHeader(Name = "Authorization")] string authToken, [FromRoute] int id)
        {
            var username = JwtUtils.GetClaim(authToken, "username");
            var comment = await _commentRepository.GetById(id);

            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            if (comment.Task.User.Username != username)
            {
                return Unauthorized();
            }

            return new CommentDto(comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment([FromHeader(Name = "Authorization")] string authToken, [FromRoute] int id, [FromBody] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return BadRequest();
            }
            
            var username = JwtUtils.GetClaim(authToken, "username");
            var originalComment = await _commentRepository.GetById(id);

            if (originalComment == null)
            {
                return NotFound("Comment not found");
            }
            
            if (originalComment.Task!.User!.Username != username)
            {
                return Unauthorized();
            }
          
            await _commentRepository.Update(comment);
            
            return NoContent();
        }
        
        [HttpPost]
        public async Task<ActionResult<CommentDto>> PostComment([FromBody] Comment comment)
        {
            var createdComment = await _commentRepository.Create(comment);
            var commentDto = new CommentDto(createdComment);
            return CreatedAtAction("GetComment", new { id = commentDto.CommentId }, commentDto);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromHeader(Name = "Authorization")] string authToken, [FromRoute] int id)
        {
            var comment = await _commentRepository.GetById(id);
            var username = JwtUtils.GetClaim(authToken, "username");
            
            if (comment == null)
            {
                return NotFound("Comment not found");
            }
            
            if (comment.Task!.User!.Username != username)
            {
                return Unauthorized();
            }
            
            await _commentRepository.Delete(comment);

            return NoContent();
        }
    }
}
