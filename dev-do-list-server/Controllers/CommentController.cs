using DevDoListServer.Jwt;
using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DevDoListServer.Controllers
{
    [Route("api/v1/comment")]
    [ApiController]
    public class CommentController(CommentRepository commentRepository) : ControllerBase
    {
        [HttpGet("task/{taskId}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<CommentDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForTask([FromHeader(Name = "Authorization")] string authToken, [FromRoute] int taskId)
        { 
            var username = JwtUtils.GetClaim(authToken, "username");

            var comments = await commentRepository.FindAll(c => c.TaskId == taskId);
            var firstComment = comments.FirstOrDefault();
            if (firstComment != null && firstComment.Task!.User!.Username != username)
            {
                return Unauthorized();
            }

            if (firstComment == null)
            {
                return NotFound();
            }
            
            return comments.Select(comment => new CommentDto(comment)).ToList();
        }
        
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(CommentDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentDto>> GetComment([FromHeader(Name = "Authorization")] string authToken, [FromRoute] int id)
        {
            var username = JwtUtils.GetClaim(authToken, "username");
            var comment = await commentRepository.GetById(id);

            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            if (comment.Task!.User!.Username != username)
            {
                return Unauthorized();
            }

            return new CommentDto(comment);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutComment([FromHeader(Name = "Authorization")] string authToken,
            [FromRoute] int id, [FromBody] CommentDto commentDto)
        {
            if (id != commentDto.CommentId)
            {
                return BadRequest();
            }
            
            var username = JwtUtils.GetClaim(authToken, "username");
            var originalComment = await commentRepository.GetById(id);

            if (originalComment == null)
            {
                return NotFound("Comment not found");
            }
            
            if (originalComment.Task!.User!.Username != username)
            {
                return Unauthorized();
            }

            await commentRepository.Update(commentDto.ToComment());
            
            return NoContent();
        }
        
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(CommentDto))]
        public async Task<ActionResult<CommentDto>> PostComment([FromBody] CommentCreateDto commentCreateDto)
        {
            var createdComment = await commentRepository.Create(commentCreateDto.ToComment());
            var commentDto = new CommentDto(createdComment);
            return CreatedAtAction("GetComment", new { id = commentDto.CommentId }, commentDto);
        }
        
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment([FromHeader(Name = "Authorization")] string authToken, [FromRoute] int id)
        {
            var comment = await commentRepository.GetById(id);
            var username = JwtUtils.GetClaim(authToken, "username");
            
            if (comment == null)
            {
                return NotFound("Comment not found");
            }
            
            if (comment.Task!.User!.Username != username)
            {
                return Unauthorized();
            }

            await commentRepository.Delete(comment);

            return NoContent();
        }
    }
}
