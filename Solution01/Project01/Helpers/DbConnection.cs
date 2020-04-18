using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project01
{
    public class DbConnection
    {
        public static string connectionString { get; } = @"Data Source=(LocalDb)\LocalDatabase;Initial Catalog=semester4_dotnet;Integrated Security=True";
    }
}
