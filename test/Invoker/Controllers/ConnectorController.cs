using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Invoker.Controllers
{
    [Route("connector")]
    public class ConnectorController : Controller
    {
        private readonly IMatchingEngineConnector _connector;

        public ConnectorController(IMatchingEngineConnector connector)
        {
            _connector = connector;
        }

        [HttpGet("transfer")]
        public async Task<IActionResult> Transfer()
        {
            await _connector.TransferAsync(Guid.NewGuid().ToString(),
                "4a543854-121e-48fa-8fc6-c1b65d5d82e9",
                "c2f42f71-cdaf-405d-be96-d79607ab591e", "BTC", 0.2);

            return Ok();
        }

        [HttpGet("cash-out")]
        public async Task<IActionResult> CashOut()
        {
            await _connector.CashInOutAsync(Guid.NewGuid().ToString(),
                "4a543854-121e-48fa-8fc6-c1b65d5d82e9", "BTC", -0.3);

            return Ok();
        }

        [HttpGet("cash-in")]
        public async Task<IActionResult> CashIn()
        {
            await _connector.CashInOutAsync(Guid.NewGuid().ToString(),
                "4a543854-121e-48fa-8fc6-c1b65d5d82e9", "BTC", 0.4);

            return Ok();
        }


        [HttpGet("swap")]
        public async Task<IActionResult> Swap()
        {
            await _connector.SwapAsync(Guid.NewGuid().ToString(),
                "4a543854-121e-48fa-8fc6-c1b65d5d82e9", "BTC", 0.1,
                "c2f42f71-cdaf-405d-be96-d79607ab591e", "BTC", 0.2);

            return Ok();
        }
    }
}
