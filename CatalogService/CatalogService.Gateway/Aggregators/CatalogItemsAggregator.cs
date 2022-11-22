using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Gateway.Aggregators
{
    public class CatalogItemsAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var one = JObject.Parse((await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync()));
            var two = JObject.Parse(await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync());          
           

            one.Merge(two, new JsonMergeSettings
            {               
                MergeArrayHandling = MergeArrayHandling.Union
            });

            var stringContent = new StringContent(one.ToString())
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };

            return new DownstreamResponse(stringContent, HttpStatusCode.OK, new List<Header>(), "OK");

        }
    }
}
