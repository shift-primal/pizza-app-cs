using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class DoughsController(DoughsRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Dough>>> GetAll()
    {
        return await repo.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Dough>> GetById(int id)
    {
        var dough = await repo.GetById(id);
        if (dough == null)
            return NotFound();
        return dough;
    }

    [HttpPost]
    public async Task<ActionResult> Create(Dough dough)
    {
        await repo.Create(dough);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Dough dough)
    {
        dough.Id = id;
        await repo.Update(dough);
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
