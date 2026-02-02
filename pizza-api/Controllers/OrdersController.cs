using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class OrdersController(
    OrdersRepository repo,
    PizzasRepository pizzasRepo,
    CustomersRepository customersRepo,
    SizesRepository sizesRepo,
    ToppingsRepository toppingsRepo
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetAll()
    {
        return await repo.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(int id)
    {
        var order = await repo.GetById(id);
        if (order == null)
            return NotFound();
        return order;
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateOrderDto dto)
    {
        if (await customersRepo.GetById(dto.CustomerId) == null)
            return BadRequest($"Customer with ID: ({dto.CustomerId}) not found");

        if (dto.Pizzas != null)
        {
            foreach (var pizzaDto in dto.Pizzas)
            {
                if (await sizesRepo.GetById(pizzaDto.SizeId) == null)
                    return BadRequest($"Size with ID: ({pizzaDto.SizeId}) not found");

                if (pizzaDto.ToppingIds != null)
                    foreach (var tid in pizzaDto.ToppingIds)
                        if (await toppingsRepo.GetById(tid) == null)
                            return BadRequest($"Topping with ID: ({tid}) not found");
            }
        }

        Order order = new()
        {
            CustomerId = dto.CustomerId,
            Date = dto.Date,
            Total = dto.Total,
        };

        var newId = await repo.Create(order);

        if (dto.Pizzas != null)
            foreach (var pizzaDto in dto.Pizzas)
            {
                Pizza pizza = new() { OrderId = newId, SizeId = pizzaDto.SizeId };
                var pizzaId = await pizzasRepo.Create(pizza);

                if (pizzaDto.ToppingIds != null)
                    foreach (var tid in pizzaDto.ToppingIds)
                        await pizzasRepo.AddTopping(pizzaId, tid);
            }

        return Created($"/orders/{newId}", await repo.GetById(newId));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateOrderDto dto)
    {
        if (await customersRepo.GetById(dto.CustomerId) == null)
            return BadRequest($"Customer with ID: ({dto.CustomerId}) not found");

        Order order = new()
        {
            Id = id,
            CustomerId = dto.CustomerId,
            Date = dto.Date,
            Total = dto.Total,
        };

        await repo.Update(order);
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
