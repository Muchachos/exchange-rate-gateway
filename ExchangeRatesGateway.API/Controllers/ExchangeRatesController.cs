using System;
using System.Threading.Tasks;
using ExchangeRatesGateway.Domain;
using ExchangeRatesGateway.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#pragma warning disable 1591

namespace ExchangeRatesGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ILogger<ExchangeRatesController> _logger;
        private readonly IExchangeRatesManagement _exchangeRatesManagement;

        public ExchangeRatesController(ILogger<ExchangeRatesController> logger, IExchangeRatesManagement exchangeRatesManagement)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<ExchangeRatesController>),$"Cannot resolve {nameof(ILogger<ExchangeRatesController>)}");
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
                if(model == null)
                    return BadRequest("Argument cannot be null");
                
                var result = await _exchangeRatesManagement.GetRatesForGivenPeriodsAsync(model);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    _logger.LogError(ex, ex.Message);
                }
                
                return BadRequest(ex.Message);
            }
        }
    }
}