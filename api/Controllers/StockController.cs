using Microsoft.AspNetCore.Mvc;

using api.Data;
using api.Mappers;
using api.Dtos.Stock;
using api.Interfaces;

namespace api.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockReporitory _stockRepository;
        public StockController(ApplicationDBContext context, IStockReporitory stockRepository)
        {
            _context = context;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _stockRepository.GetAllAsync();

            var stockDtos = stocks.Select(stock => stock.ToStockDto());
            
            return Ok(stockDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _stockRepository.GetByIdAsync(id);

            if (stock == null)
                return NotFound();

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDTO();
            await _stockRepository.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            var existingStock = await _stockRepository.UpdateAsync(id, updateDto);

            if (existingStock == null)
                return NotFound();

            return Ok(existingStock.ToStockDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stock = await _stockRepository.DeleteAsync(id);
            if (stock == null)
                return NotFound();

            return NoContent();
        }
    }
}