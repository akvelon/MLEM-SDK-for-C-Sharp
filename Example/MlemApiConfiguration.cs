using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MlemApi;

namespace Example
{
    internal class MlemApiConfiguration : IMlemApiConfiguration
    {
        public string Url { get; set; } = "https://example-mlem-get-started-app.herokuapp.com";
    }
}
