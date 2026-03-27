using kecnet;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace keccrm
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection baglanti = new SqlConnection(DatabaseConnection.GetConnectionString()))
                {
                    baglanti.Open();

                    using (SqlCommand komut = new SqlCommand(@"
    SELECT 
        u.UserID,
        u.Email,
        sp.SalesPersonID,
        sp.CompanyID
    FROM Users u
    LEFT JOIN SalesPerson sp ON sp.UserID = u.UserID
    WHERE u.Email = @email AND u.PasswordHash = @Password;
", baglanti))
                    {
                        komut.Parameters.Add("@email", SqlDbType.VarChar).Value = textBox1.Text.Trim();
                        komut.Parameters.Add("@Password", SqlDbType.VarChar).Value = textBox2.Text;

                        using (SqlDataReader dr = komut.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                KullaniciBilgileri.KullaniciID = Convert.ToInt32(dr["UserID"]);
                                KullaniciBilgileri.Kullanicimail = dr["Email"].ToString();

                                // SalesPerson kaydı yoksa null gelebilir
                                KullaniciBilgileri.SalerpersonID = (dr["SalesPersonID"] == DBNull.Value) ? -1 : Convert.ToInt32(dr["SalesPersonID"]);
                                KullaniciBilgileri.CompanyID = (dr["CompanyID"] == DBNull.Value) ? -1 : Convert.ToInt32(dr["CompanyID"]);
                                home frm = new home();
                                frm.Show();
                                this.Hide();

                            }
                            else
                            {
                                MessageBox.Show("Yanlış kullanıcı adı veya parolası");
                                return;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message);
            }
        }
    }
}
