using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("pizzas/{pizzaId}/toppings")]
public class ToppingAssignmentController(
    PizzasRepository pizzasRepo,
    ToppingsRepository toppingsRepo
) : ControllerBase
{
    [HttpPost("{toppingId}")]
    public async Task<ActionResult> AddTopping(int pizzaId, int toppingId)
    {
        if (await pizzasRepo.GetById(pizzaId) == null)
            return BadRequest($"Pizza with ID: ({pizzaId}) not found");
        if (await toppingsRepo.GetById(toppingId) == null)
            return BadRequest($"Topping with ID: ({toppingId}) not found");

        await pizzasRepo.AddTopping(pizzaId, toppingId);
        return Created();
    }

    [HttpDelete("{toppingId}")]
    public async Task<ActionResult> RemoveTopping(int pizzaId, int toppingId)
    {
        await pizzasRepo.RemoveTopping(pizzaId, toppingId);
        return Ok();
    }
}
