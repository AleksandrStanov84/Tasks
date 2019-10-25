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
using System.Text.RegularExpressions;
using System.Configuration;

namespace TasksAleks
{
    public partial class FillOrCancel : Form
    {
        SqlConnection conn;
        private int parsedOrderID;

        public FillOrCancel()
        {
            InitializeComponent();
        }

        private bool IsOrderIDValid()
        {
            if (txtOrderID.Text == "")
            {
                MessageBox.Show("Пожалуйста, укажите идентификатор заказа.");
                return false;
            }
            else if (Regex.IsMatch(txtOrderID.Text, @"^\D*$"))
            {
                MessageBox.Show("Идентификатор клиента должен содержать только цифры.");
                txtOrderID.Clear();
                return false;
            }
            else
            {
                parsedOrderID = Int32.Parse(txtOrderID.Text);
                return true;
            }
        }

        private void btnFindByOrderID_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["MadingConnString"].ConnectionString;
                using (SqlCommand sqlCommand = new SqlCommand("uspPlaceNewOrder", connection))
                {
                    const string sql = "SELECT * FROM  SALES.Orders WHERE orderID = @orderID";

                   // using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                   // {
                        sqlCommand.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;

                        try
                        {
                            connection.Open();
                            using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(dataReader);

                                this.dgvCustomerOrders.DataSource = dataTable;
                                dataReader.Close();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Запрошенный заказ не может быть загружен в форму.");
                        }
                        finally
                        { 
                            connection.Close();
                        }
                   // }
                }
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("SALES.uspCancelOrder", connection))
                    // using (SqlCommand sqlCommand = new SqlCommand("Sales.uspCancelOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;

                        try
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("Операция отмены не была завершена.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnFillOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("SALES.uspFillOrder", connection))
                    // using (SqlCommand sqlCommand = new SqlCommand("Sales.uspFillOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;

                        sqlCommand.Parameters.Add(new SqlParameter("@FilledDate", SqlDbType.DateTime, 8));
                        sqlCommand.Parameters["@FilledDate"].Value = dtpFillDate.Value;

                        try
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("Операция заполнения не была завершена.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnFinishUpdates_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
