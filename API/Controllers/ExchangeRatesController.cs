using System;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRateGateway.API.Validators;
using ExchangeRateGateway.Domain;
using ExchangeRateGateway.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRateGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IExchangeRatesManagement _exchangeRatesManagement;

        public ExchangeRatesController(IExchangeRatesManagement exchangeRatesManagement)
        {
            _exchangeRatesManagement = exchangeRatesManagement ?? throw new ArgumentNullException(nameof(IExchangeRatesManagement),$"Cannot resolve {nameof(IExchangeRatesManagement)}");
        }
        
        [HttpPost]
        public async Task<IActionResult>GetHistoryRatesForGivenPeriodsAsync([FromBody] HistoryRatesRequest model)
        {
            try
            {
                var validator = new HistoryRatesRequestValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    return BadRequest(string.Join('\n', validationResult.Errors.Select(x => x.ErrorMessage)));
                }
                    
                var result = await _exchangeRatesManagement.GetRatesForGivenPeriodsAsync(model);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}