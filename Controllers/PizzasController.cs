using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PizzasController(PizzasRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Pizza>>> GetAll()
    {
        return await repo.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pizza>> GetById(int id)
    {
        var pizza = await repo.GetById(id);
        if (pizza == null)
            return NotFound();
        return pizza;
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreatePizzaDto dto)
    {
        Pizza pizza = new() { OrderId = dto.OrderId, SizeId = dto.SizeId };

        var newId = await repo.Create(pizza);

        if (dto.ToppingIds != null)
            foreach (var tid in dto.ToppingIds)
                await repo.AddTopping(newId, tid);

        return Created($"/pizzas/{newId}", await repo.GetById(newId));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdatePizzaDto dto)
    {
        Pizza pizza = new()
        {
            Id = id,
            OrderId = dto.OrderId,
            SizeId = dto.SizeId,
        };

        await repo.Update(pizza);
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
