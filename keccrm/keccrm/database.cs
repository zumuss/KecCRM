using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kecnet
{
    public class DatabaseConnection
    {
        // Veritabanı bağlantı yolunu döndüren metod
        public static string GetConnectionString()
        {
            return @"Data Source=DESKTOP-9RDBB6I;Initial Catalog=kecnett;Integrated Security=True;Encrypt=True;;TrustServerCertificate=true";
        }
    }

}
