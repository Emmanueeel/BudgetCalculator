using BudgetCalculator.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCalculator.Api.Controllers;

[ApiController]
[Route("api/budget")]
public class BudgetController : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] BudgetRequest request)
    {
        if (request.Days <= 0) 
            return BadRequest("Количество дней должно быть больше нуля.");

        var total = request.DailyCost * request.Days;

        // БАГ: умножаем на 5 (500%), а нужно прибавить 5%
        if (request.IncludeInsurance)
        {
            total = total + total * 0.05m; // Прибавляем 5% (страховка)
        }

        return Ok(new { TotalAmount = total });
    }
}
