using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using static TaskForceApp.Pages.ExpenseTracker.IndexModel;

namespace TaskForceApp.Pages.ExpenseTracker
{
    public class AddExpenseLimitModel : PageModel
    {
        public AllExpenseLimit AllInfo = new AllExpenseLimit();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        int totalIncome = 0;
        int totalExpense = 0;
        int remaining = 0;
        int lastExpenseLimit = 0;
        public List<AllInfo> ListAll = new List<AllInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=SQL1002.site4now.net;Initial Catalog=db_ab1fb5_tellachris1;User Id=db_ab1fb5_tellachris1_admin;Password=database1";
             //   string connectionString = "Data Source=SQL1002.site4now.net;Initial Catalog=db_ab1fc7_tellachris1;User Id=db_ab1fc7_tellachris1_admin;Password=database1";
                // string connectionString = "Data Source=DESKTOP-2042M6B\\SQLEXPRESS;Initial Catalog=TaskForceDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    // Select the last inserted record
                    string sqlQuery = "SELECT TOP 1 ID, ExpenseLimit FROM ExpenseLimit ORDER BY ID DESC";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                AllInfo.ID = reader.GetInt32(0);
                                AllInfo.ExpenseLimit = reader.GetInt32(1);
                            }
                            else
                            {
                                // If no record is found, set default values
                                AllInfo.ID = 1;
                                AllInfo.ExpenseLimit = 0;
                            }

                            lastExpenseLimit += AllInfo.ExpenseLimit;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }


            ViewData["lastExpenseLimit"] = lastExpenseLimit;
         
        }

        public IActionResult OnPost()
        {
            string ExpenseLimit = Request.Form["ExpenseLimit"];
            try
            {
                string connectionString = "Data Source=SQL1002.site4now.net;Initial Catalog=db_ab1fc7_tellachris1;User Id=db_ab1fc7_tellachris1_admin;Password=database1";
              //  string connectionString = "Data Source=DESKTOP-2042M6B\\SQLEXPRESS;Initial Catalog=TaskForceDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string sqlQuery;
                    if (AllInfo.ID > 0)
                    {
                        sqlQuery = "UPDATE ExpenseLimit SET ExpenseLimit=@ExpenseLimit WHERE ID=@ID";
                    }
                    else
                    {
                        sqlQuery = "INSERT INTO ExpenseLimit (ExpenseLimit) VALUES (@ExpenseLimit)";
                    }
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseLimit", ExpenseLimit);
                        if (AllInfo.ID > 0)
                        {
                            cmd.Parameters.AddWithValue("@ID", AllInfo.ID);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }


                SuccessMessage = "Expense Limit updated successfully.";
                return Page();
                

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
