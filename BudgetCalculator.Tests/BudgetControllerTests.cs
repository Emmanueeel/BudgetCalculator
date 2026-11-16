using System.Reflection;
using BudgetCalculator.Api.Controllers;
using BudgetCalculator.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BudgetCalculator.Tests;

public class BudgetControllerTests
{
    private decimal GetTotalAmount(IActionResult result)
    {
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = okResult.Value!;
        var prop = value.GetType().GetProperty("TotalAmount");
        return (decimal)prop!.GetValue(value)!;
    }

    [Fact]
    public void Calculate_NoInsurance_ReturnsOkWithCorrectTotal()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest { DailyCost = 100m, Days = 5, IncludeInsurance = false };
        var result = controller.Calculate(request);
        Assert.Equal(500m, GetTotalAmount(result));
    }

    [Fact]
    public void Calculate_WithInsurance_AddsFivePercent()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest { DailyCost = 100m, Days = 5, IncludeInsurance = true };
        var result = controller.Calculate(request);
        Assert.Equal(525m, GetTotalAmount(result));
    }

    [Fact]
    public void Calculate_NegativeDays_ReturnsBadRequest()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest { DailyCost = 100m, Days = -1 };
        var result = controller.Calculate(request);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Calculate_ZeroDailyCost_ReturnsZero()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest { DailyCost = 0m, Days = 5, IncludeInsurance = false };
        var result = controller.Calculate(request);
        Assert.Equal(0m, GetTotalAmount(result));
    }

    [Fact]
    public void Calculate_DecimalPrecision_CalculatesToKopecks()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest { DailyCost = 100.50m, Days = 3, IncludeInsurance = false };
        var result = controller.Calculate(request);
        Assert.Equal(301.50m, GetTotalAmount(result));
    }
}