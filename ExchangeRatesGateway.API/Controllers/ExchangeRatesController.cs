using System;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRatesGateway.API.Validators;
using ExchangeRatesGateway.Domain;
using ExchangeRatesGateway.Domain.Model;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable 1591

namespace ExchangeRatesGateway.API.Controllers
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
        
        /// <summary>
        /// Calculate minimum, maximum and average rate based on provided dates
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult>GetHistoryRatesForGivenPeriodsAsync([FromBody] HistoryRatesRequest model)
        {
            try
            {
                var validator = new HistoryRatesRequestValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                    return BadRequest(string.Join('\n', validationResult.Errors.Select(x => x.ErrorMessage)));
                    
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