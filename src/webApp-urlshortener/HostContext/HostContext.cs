using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webApp_urlshortener.HostContext
{
    public class HostContext : IHostContext
    {
        public string ContentRootPath { get; set; }
    }
}
