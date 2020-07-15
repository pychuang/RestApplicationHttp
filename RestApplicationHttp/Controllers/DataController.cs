using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestApplicationHttp.Controllers
{
    [ApiController]
    [Route("/")]
    public class DataController : ControllerBase
    {
        [HttpGet("random/{dataSize}")]
        public IActionResult Get([FromRoute] long dataSize)
        {
            Console.WriteLine($"{this.Request.Method} {this.Request.GetDisplayUrl()}");
            this.PrintHeaders();

            var stream = new RandomReadOnlyStream(dataSize);
            return this.Ok(stream);
        }

        [HttpPost("random/{dataSize}")]
        public async Task<IActionResult> PostAsync([FromRoute] long dataSize)
        {
            Console.WriteLine($"{this.Request.Method} {this.Request.GetDisplayUrl()}");
            this.PrintHeaders();

            long totalBytesRead = await this.ReadStreamAsync(this.Request.Body);
            Console.WriteLine($"totalBytesRead {totalBytesRead}");
            var stream = new RandomReadOnlyStream(dataSize);
            return this.Ok(stream);
        }

        [HttpPost("copy")]
        public IActionResult Post()
        {
            Console.WriteLine($"{this.Request.Method} {this.Request.GetDisplayUrl()}");
            this.PrintHeaders();

            return this.Ok(this.Request.Body);
        }

        private async Task<long> ReadStreamAsync(Stream stream)
        {
            long totalBytesRead = 0;
            byte[] buffer = new byte[1024];
            bool skipPrinting = false;
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                {
                    break;
                }

                totalBytesRead += bytesRead;
                Console.WriteLine($"totalBytesRead {totalBytesRead}");

                string s = Encoding.Default.GetString(buffer, 0, bytesRead);
                if (totalBytesRead <= 1024)
                {
                    Console.Write(s);
                }
                else if (!skipPrinting)
                {
                    Console.WriteLine("...");
                    skipPrinting = true;
                }
            }

            Console.WriteLine($"Total bytes: {totalBytesRead}");
            Console.WriteLine();
            return totalBytesRead;
        }

        private void PrintHeaders()
        {
            foreach (var header in this.Request.Headers)
            {
                Console.WriteLine($"{header.Key}={header.Value}");
            }
        }
    }
}
