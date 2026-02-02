using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PizzasController(
    PizzasRepository repo,
    OrdersRepository ordersRepo,
    SizesRepository sizesRepo,
    ToppingsRepository toppingsRepo
) : ControllerBase
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
        if (await ordersRepo.GetById(dto.OrderId) == null)
            return BadRequest($"Order with ID: ({dto.OrderId}) not found");
        if (await sizesRepo.GetById(dto.SizeId) == null)
            return BadRequest($"Size with ID: ({dto.SizeId}) not found");

        if (dto.ToppingIds != null)
            foreach (var tid in dto.ToppingIds)
                if (await toppingsRepo.GetById(tid) == null)
                    return BadRequest($"Topping with ID: ({tid}) not found");

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
        if (await ordersRepo.GetById(dto.OrderId) == null)
            return BadRequest($"Order with ID: ({dto.OrderId}) not found");
        if (await sizesRepo.GetById(dto.SizeId) == null)
            return BadRequest($"Size with ID: ({dto.SizeId}) not found");

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
