using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static TaskForceApp.Pages.ExpenseTracker.IndexModel;

namespace TaskForceApp.Pages.ExpenseTracker
{
    public class AddExpenseModel : PageModel
    {
        [BindProperty]
        public string description { get; set; }

        [BindProperty]
        public string expense { get; set; }

        [BindProperty]
        public string account { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(expense) || string.IsNullOrEmpty(account))
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            int expenseAmount;
            if (!int.TryParse(expense, out expenseAmount))
            {
                ErrorMessage = "Invalid expense amount.";
                return Page();
            }

            try
            {
                string connectionString = "Data Source=SQL1002.site4now.net;Initial Catalog=db_ab1fb5_tellachris1;User Id=db_ab1fb5_tellachris1_admin;Password=database1";
                //string connectionString = "Data Source=SQL1002.site4now.net;Initial Catalog=db_ab1fc7_tellachris1;User Id=db_ab1fc7_tellachris1_admin;Password=database1";
                //string connectionString = "Data Source=DESKTOP-2042M6B\\SQLEXPRESS;Initial Catalog=TaskForceDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Check the remaining amount for the account
                    int inc = 0, exp = 0;
                    string checkBalanceQuery = "SELECT income, expense FROM ExpenseTracker WHERE account=@account";
                    using (SqlCommand cmd = new SqlCommand(checkBalanceQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@account", account);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inc += reader.GetInt32(0);
                                exp += reader.GetInt32(1);
                            }
                        }
                    }

                    int totalAccountBalance = inc - exp;

                    if (totalAccountBalance >= expenseAmount)
                    {
                        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                        string sqlQuery = "INSERT INTO ExpenseTracker (income,expense,remaining,account,description, Date) VALUES (0, @expense, 0, @account, @description, @Date)";
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@expense", expenseAmount);
                            cmd.Parameters.AddWithValue("@account", account);
                            cmd.Parameters.AddWithValue("@description", description);
                            cmd.Parameters.AddWithValue("@Date", currentDate);
                            cmd.ExecuteNonQuery();
                        }
                        SuccessMessage = "Expense recorded";

                        // return RedirectToPage("/ExpenseTracker/AllExpense");
                        return Page();
                    }
                    else
                    {
                        ErrorMessage = "The total account balance is insufficient to record the expense.";
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

       
    }
}
