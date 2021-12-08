using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace BR_WEB_API_NET_6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : Controller
    {
        private readonly IConfiguration _configuration;

        public ArtistsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        public JsonResult Get()
        {
            // artistsId and artistname
            string query = @"
            select artistsId as ""artistsId"",
            artistname as ""artistname""
            from Artists
            ";

           


            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("BR_PostGre4");
            NpgsqlDataReader myReader;
            
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                // Error 1
                // "BR_PostGre": "Server=DESKTOP-4LJRSHK\\localhost;Database=BR;port=5432;SSL mode=Prefer;User Id=postgres;Password=ICanandwill2021!",
                // System.Net.Internals.SocketExceptionFactory.ExtendedSocketException: 'Resource temporarily unavailable'

                // Error 2
                // "BR_PostGre2": "User ID=postgres;Password=ICanandwill2021!;Host=PostgreSQL14;Port=5432;Database=BR;",
                // System.Net.Internals.SocketExceptionFactory.ExtendedSocketException: 'Resource temporarily unavailable'

                // Error 3
                // "BR_PostGre3": "Server=localhost;Port=5432;Username=postgres;Password=ICanandwill2021!;Database=BR;"
                // Npgsql.NpgsqlException: 'Failed to connect to [::1]:5432'
                // Inner Exception ExtendedSocketException: Cannot assign requested address

                // Error 4
                // "BR_PostGre4": "User ID=postgres;Password=ICanandwill2021!;Host=localhost;Port=5432;Database=BR;"
                // Npgsql.NpgsqlException: 'Failed to connect to [::1]:5432'
                // ExtendedSocketException: Cannot assign requested address

                // Solution Finally :):)
                // Run with IIS as running in docker container localhost does not refer to local pc it refers to docker container localhost
                // https://stackoverflow.com/questions/63132779/how-to-connect-dockerized-asp-net-core-app-dockerized-postgresql

                myCon.Open();
                using(NpgsqlCommand myCommand = new NpgsqlCommand(query,myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

    }
}
