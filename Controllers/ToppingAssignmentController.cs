using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("pizzas/{pizzaId}/toppings")]
public class ToppingAssignmentController(PizzasRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Topping>>> GetToppings(int pizzaId)
    {
        return await repo.GetToppings(pizzaId);
    }

    [HttpPost("{toppingId}")]
    public async Task<ActionResult> AddTopping(int pizzaId, int toppingId)
    {
        await repo.AddTopping(pizzaId, toppingId);
        return Created();
    }

    [HttpDelete("{toppingId}")]
    public async Task<ActionResult> RemoveTopping(int pizzaId, int toppingId)
    {
        await repo.RemoveTopping(pizzaId, toppingId);
        return Ok();
    }
}
