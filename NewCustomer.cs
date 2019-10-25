using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace TasksAleks
{
    public partial class NewCustomer : Form
    {
        private int parsedCustomerID;
        private int orderID;

        SqlConnection conn;

        public NewCustomer()
        {
            InitializeComponent();     
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        //-------------------------------------------------------------------

        private bool IsCustomerNameValid()
        {
            if (txtCustomerName.Text == "")
            {
                MessageBox.Show("Пожалуйста введите имя.");
                return false;
            }
            else
                return true;
        }

        private bool IsOrderDataValid()
        {
            if (txtCustomerID.Text == "")
            {
                MessageBox.Show("Пожалуйста, создайте учетную запись клиента перед оформлением заказа.");
                return false;
            }
            else if ((numOrderAmount.Value < 1))
            {
                MessageBox.Show("Пожалуйста, укажите сумму заказа.");
                return false;
            }
            else
                return true;
        }

        private void ClearForm()
        {
            txtCustomerName.Clear();
            txtCustomerID.Clear();

            dtpOrderDate.Value = DateTime.Now;
            numOrderAmount.Value = 0;

            this.parsedCustomerID = 0;
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (IsCustomerNameValid())
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["MadingConnString"].ConnectionString;

                using (SqlCommand sqlCommand = new SqlCommand("uspPlaceNewOrder", connection))
                    //using (SqlCommand sqlCommand = new SqlCommand("Sales.uspNewCustomer", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 40));
                        sqlCommand.Parameters["@CustomerName"].Value = txtCustomerName.Text;

                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int));
                        sqlCommand.Parameters["@CustomerID"].Direction = ParameterDirection.Output;

                        try
                        {
                            connection.Open();

                            sqlCommand.ExecuteNonQuery();

                            this.parsedCustomerID = (int)sqlCommand.Parameters["@CustomerID"].Value;
                            this.txtCustomerID.Text = Convert.ToString(parsedCustomerID);
                        }
                        catch
                        {
                            MessageBox.Show("Идентификатор клиента не был возвращен. Аккаунт не может быть создан.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                
            }
        }

        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderDataValid())
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["MadingConnString"].ConnectionString;

                using (SqlCommand sqlCommand = new SqlCommand("uspPlaceNewOrder", connection))
                {
                    //using (SqlCommand sqlCommand = new SqlCommand("SALES.uspPlaceNewOrder", connection))
                    // using (SqlCommand sqlCommand = new SqlCommand("Sales.uspPlaceNewOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int));
                        sqlCommand.Parameters["@CustomerID"].Value = this.parsedCustomerID;

                        sqlCommand.Parameters.Add(new SqlParameter("@OrderDate", SqlDbType.DateTime, 8));
                        sqlCommand.Parameters["@OrderDate"].Value = dtpOrderDate.Value;

                        sqlCommand.Parameters.Add(new SqlParameter("@Amount", SqlDbType.Int));
                        sqlCommand.Parameters["@Amount"].Value = numOrderAmount.Value;

                        sqlCommand.Parameters.Add(new SqlParameter("@Status", SqlDbType.Char, 1));
                        sqlCommand.Parameters["@Status"].Value = "O";

                        sqlCommand.Parameters.Add(new SqlParameter("@RC", SqlDbType.Int));
                        sqlCommand.Parameters["@RC"].Direction = ParameterDirection.ReturnValue;

                        try
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();

                            this.orderID = (int)sqlCommand.Parameters["@RC"].Value;
                            MessageBox.Show("Номер заказа " + this.orderID + " был представлен.");
                        }
                        catch
                        {
                            MessageBox.Show("Заказ не может быть размещен.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnAddAnotherAccount_Click(object sender, EventArgs e)
        {
            this.ClearForm();
        }

        private void btnAddFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
