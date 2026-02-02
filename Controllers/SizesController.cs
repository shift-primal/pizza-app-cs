using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class SizesController(SizesRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Size>>> GetAll()
    {
        return await repo.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Size>> GetById(int id)
    {
        var size = await repo.GetById(id);
        if (size == null)
            return NotFound();
        return size;
    }

    [HttpPost]
    public async Task<ActionResult> Create(Size size)
    {
        await repo.Create(size);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Size size)
    {
        size.Id = id;
        await repo.Update(size);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (!await repo.Delete(id))
            return NotFound();
        return Ok();
    }
}
