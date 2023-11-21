using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace pubsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class pubsController : ControllerBase
    {
        private IConfiguration _configuration;
        public pubsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAuthors")]
        public JsonResult GetAuthors()
        {
            string query = "select * from authors";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("pubsCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        [Route("AddAuthors")]
        public JsonResult AddAuthors(string au_id, string au_lname, string au_fname)
        {
            string query = "insert into authors (au_id, au_lname, au_fname) values(@au_id, @au_lname, @au_fname)";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("pubsCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@au_id", au_id);
                    myCommand.Parameters.AddWithValue("@au_lname", au_lname);
                    myCommand.Parameters.AddWithValue("@au_fname", au_fname);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }

        [HttpDelete]
        [Route("DeleteAuthors")]
        public JsonResult DeleteAuthors(string au_id)
        {
            string query = "delete from dbo.authors where au_id = @au_id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("pubsCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@au_id", au_id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }

        [HttpGet]
        [Route("GetAuthorById/{id}")]
        public JsonResult GetAuthorById(string id)
        {
            string query = "select * from authors where au_id = @au_id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("pubsCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@au_id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPut]
        [Route("UpdateAuthors/{au_id}")]
        public JsonResult UpdateAuthors(string au_id, [FromBody] AuthorUpdateModel model)
        {
            string query = @"
        UPDATE authors
        SET
            au_lname = @au_lname,
            au_fname = @au_fname,
            phone = @phone,
            address = @address,
            city = @city,
            state = @state,
            zip = @zip,
            contract = @contract
        WHERE au_id = @au_id";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("pubsCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    // Bind the @au_id parameter
                    myCommand.Parameters.AddWithValue("@au_id", au_id);
                    myCommand.Parameters.AddWithValue("@au_lname", model.au_lname);
                    myCommand.Parameters.AddWithValue("@au_fname", model.au_fname);
                    myCommand.Parameters.AddWithValue("@phone", model.phone);
                    myCommand.Parameters.AddWithValue("@address", model.address);
                    myCommand.Parameters.AddWithValue("@city", model.city);
                    myCommand.Parameters.AddWithValue("@state", model.state);
                    myCommand.Parameters.AddWithValue("@zip", model.zip);
                    myCommand.Parameters.AddWithValue("@contract", model.contract);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }

        [HttpGet]
        [Route("GetTitlesWithAuthors")]
        public JsonResult GetTitlesWithAuthors()
        {
            string query = @"
        SELECT titles.*, titleauthor.au_id
        FROM titles
        INNER JOIN titleauthor ON titles.title_id = titleauthor.title_id";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("pubsCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }
        [HttpGet]
        [Route("GetBooksByTitleId/{title_id}")]
        public JsonResult GetBooksByTitleId(string title_id)
        {
            string query = @"
        SELECT titles.*, titleauthor.au_id, authors.au_fname, authors.au_lname
        FROM titles
        INNER JOIN titleauthor ON titles.title_id = titleauthor.title_id
        INNER JOIN authors ON titleauthor.au_id = authors.au_id
        WHERE titles.title_id = @title_id";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("pubsCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@title_id", title_id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                }

                myCon.Close();
            }

            return new JsonResult(table);
        }


        public class AuthorUpdateModel
        {
            public string au_lname { get; set; }
            public string au_fname { get; set; }
            public string phone { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            public bool contract { get; set; }
        }
    }
}
