using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Configurations
{
    public interface IDatabaseConfig
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
        string CollectionName { get; set; }
        string User { get; set; }
        string Password { get; set; }
    }
}
