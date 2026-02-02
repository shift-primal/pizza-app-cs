using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ToppingsController(ToppingsRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Topping>>> GetAll()
    {
        return await repo.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Topping>> GetById(int id)
    {
        var topping = await repo.GetById(id);
        if (topping == null)
            return NotFound();
        return topping;
    }

    [HttpPost]
    public async Task<ActionResult> Create(Topping topping)
    {
        await repo.Create(topping);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Topping topping)
    {
        topping.Id = id;
        await repo.Update(topping);
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
