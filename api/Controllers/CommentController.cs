using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comments")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockReporitory _stockRepository;

        public CommentController(ICommentRepository commentRepository, IStockReporitory stockRepository)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepository.GetAllAsync();

            var commentDtos = comments.Select(comment => comment.ToCommentDto());

            return Ok(commentDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);

            if (comment == null)
                return NotFound();

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, [FromBody] CreateCommentRequestDto commentCreateDto)
        {
            if (!await _stockRepository.StockExists(stockId))
                return BadRequest("Stock does not exist");

            var comment = commentCreateDto.ToCommentFromCreateDTO(stockId);
            await _commentRepository.CreateAsync(comment);

            return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto commentUpdateDto)
        {
            var comment = await _commentRepository.UpdateAsync(id, commentUpdateDto.ToCommentFromUpdateDTO());

            if (comment == null)
                return NotFound();

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var comment = await _commentRepository.DeleteAsync(id);

            if (comment == null)
                return NotFound();

            return NoContent();
        }
    }
}