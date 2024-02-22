using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

using Project_CRUDwithoutEF_.Models;

namespace Project_CRUDwithoutEF_.Controllers
{
    public class BooksController : Controller
    {
        private readonly IConfiguration _configuration;

        public BooksController(IConfiguration configuration )
        {
            this._configuration = configuration;
        }

       
        public IActionResult Index()
        {
            DataTable allbooks = new DataTable();  
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("ConStr")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlda = new SqlDataAdapter("BookViewAll",sqlConnection);
                sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlda.Fill(allbooks);
                
            }
            return View(allbooks);
        }

        

        
        public IActionResult AddOrEdit(int? id)
        {
            Book objbook = new Book();
            if(id > 0)
                objbook = GetBookById(id);
            return View(objbook);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(int id, [Bind("BookId,Title,Author,Price")] Book book)
        {
           

            if (ModelState.IsValid)
            {
                using(SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("ConStr")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlcmd = new SqlCommand("BookAddOrEdit", sqlConnection);
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("BookId", book.BookId);
                    sqlcmd.Parameters.AddWithValue("Title", book.Title);
                    sqlcmd.Parameters.AddWithValue("Author", book.Author);
                    sqlcmd.Parameters.AddWithValue("Price", book.Price);
                    sqlcmd.ExecuteNonQuery();
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        
        public IActionResult Delete(int? id)
        {
            Book delbook = GetBookById(id);

            return View(delbook);
        }

      
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("ConStr")))
            {
                sqlConnection.Open();
                SqlCommand sqlcmd = new SqlCommand("BookDeleteById", sqlConnection);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("BookId", id);
                sqlcmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public Book GetBookById(int? id)
        {

            Book book = new Book();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("ConStr")))
            {
                DataTable dataTable = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlda = new SqlDataAdapter("BookViewByID", sqlConnection);
                sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlda.SelectCommand.Parameters.AddWithValue("BookID", id);
                sqlda.Fill(dataTable);
                if(dataTable.Rows.Count == 1)
                {
                    book.BookId = Convert.ToInt32(dataTable.Rows[0]["BookID"].ToString());
                    book.Title = dataTable.Rows[0]["Title"].ToString();
                    book.Author = dataTable.Rows[0]["Author"].ToString();
                    book.Price = Convert.ToInt32(dataTable.Rows[0]["Price"].ToString());
                }

                return book;

            }
        }
       
    }
}
