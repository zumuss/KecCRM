using kecnet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace keccrm
{
    public partial class home : Form
    {
        public home()
        {
            InitializeComponent();
        }
        void agoster()
        {
            dataGridView1.Visible = true;
            using (SqlConnection baglanti = new SqlConnection(DatabaseConnection.GetConnectionString()))
            {
                baglanti.Open();

                string sql = @"SELECT *
                       FROM Customers
                       WHERE SalesPersonID = @SalesPersonID
                       ORDER BY CustomerID";

                using (SqlCommand cmd = new SqlCommand(sql, baglanti))
                {
                    cmd.Parameters.Add("@SalesPersonID", SqlDbType.Int)
                                  .Value = KullaniciBilgileri.SalerpersonID;

                    using (SqlDataAdapter vericek = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        vericek.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }

        void bgoster()
        {
            dataGridView1.Visible = true;
            using (SqlConnection baglanti = new SqlConnection(DatabaseConnection.GetConnectionString()))
            {
                baglanti.Open();

                string sql = @"SELECT 
                            ProductID,
                            ProductName,
                            Description,
                            Price,
                            StockAmount,
                            CreatedAt
                       FROM Products
                       ORDER BY ProductName";

                using (SqlCommand cmd = new SqlCommand(sql, baglanti))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }

        void cgoster()
        {
            dataGridView1.Visible = true;
            using (SqlConnection baglanti = new SqlConnection(DatabaseConnection.GetConnectionString()))
            {
                baglanti.Open();

                string sql = @"SELECT *
                       FROM SalesOrders
                       ORDER BY OrderID";

                using (SqlCommand cmd = new SqlCommand(sql, baglanti))
                {
                    cmd.Parameters.Add("@SalesPersonID", SqlDbType.Int)
                                  .Value = KullaniciBilgileri.CompanyID;

                    using (SqlDataAdapter vericek = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        vericek.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }




        void GridReset()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
        }



        private void home_Load(object sender, EventArgs e)
        {
            GridReset();
            agoster();
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GridReset();
            agoster();
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GridReset();
            bgoster();
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            satısyap frm = new satısyap();
            frm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GridReset();
            cgoster();
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
