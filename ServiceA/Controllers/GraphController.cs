using Microsoft.AspNetCore.Mvc;
using ServiceA.Interfaces;
using ServiceA.Models;

namespace ServiceA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphController : ControllerBase
    {
        private readonly IGraphService _graphService;
        private readonly ILogger<GraphController> _logger;

        public GraphController(IGraphService graphService, ILogger<GraphController> logger)
        {
            _graphService = graphService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Graph>> CreateGraph([FromBody] Graph graph)
        {
            try
            {
                var newGraph = await _graphService.CreateGraphAsync(graph);
                return CreatedAtAction(nameof(GetGraph), new { id = newGraph.Id }, newGraph);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating graph");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Graph>> GetGraph(Guid id)
        {
            try
            {
                var graph = await _graphService.GetGraphAsync(id);
                if (graph == null) return NotFound();
                return Ok(graph);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting graph");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGraph(Guid id, [FromBody] Graph graph)
        {
            try
            {
                await _graphService.UpdateGraphAsync(id, graph);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating graph");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGraph(Guid id)
        {
            try
            {
                await _graphService.DeleteGraphAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting graph");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
