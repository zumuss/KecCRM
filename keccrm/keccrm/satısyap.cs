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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace keccrm
{
    public partial class satısyap : Form
    {
        


        public satısyap()
        {
            InitializeComponent();
            SepetiHazirla();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            home frm = new home();
            frm.Show();
            this.Hide();
        }

        
        private void satısyap_Load(object sender, EventArgs e)
        {
            SepetiHazirla();
        }

        private DataTable sepetDT;

        private void SepetiHazirla()
        {
            sepetDT = new DataTable();
            sepetDT.Columns.Add("CustomerID", typeof(int));
            sepetDT.Columns.Add("ProductID", typeof(int));
            sepetDT.Columns.Add("ProductName", typeof(string));
            sepetDT.Columns.Add("Price", typeof(decimal));
            sepetDT.Columns.Add("Qty", typeof(int));

            dataGridView2.DataSource = sepetDT;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.ReadOnly = true;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.RowHeadersVisible = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void SiparisToplamGuncelle()
        {
            decimal total = 0;
            foreach (DataRow r in sepetDT.Rows)
            {
                total += r.Field<decimal>("Price") * r.Field<int>("Qty");
            }
            labelSiparisToplam.Text = total.ToString("N2") + " ₺";
        }

        private void UrunAra()
        {
            using (SqlConnection baglanti = new SqlConnection(DatabaseConnection.GetConnectionString()))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT ProductID, ProductName, Price, StockAmount " +
                    "FROM Products " +
                    "WHERE ProductName LIKE @text + '%' " +
                    "OR ProductID = TRY_CONVERT(int, @text) " +
                    "ORDER BY ProductName",
                    baglanti
                );

                da.SelectCommand.Parameters.AddWithValue("@text", textBox1.Text);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // ✅ sonuç yoksa temizle ve çık
                if (dt.Rows.Count == 0 || dataGridView1.Rows.Count == 0)
                {
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Text = "1";
                    label2.Text = "(Fiyat: -)";
                    label3.Text = "(Stok: -)";
                    return;
                }

                // ✅ ilk satırı seç + CurrentCell ayarla (CurrentRow NULL kalmasın)
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];

                // ✅ seçilen satırdan doldur (INDEX)
                var row = dataGridView1.Rows[0];

                textBox2.Text = row.Cells[0].Value?.ToString(); // ProductID
                textBox3.Text = row.Cells[1].Value?.ToString(); // ProductName
                textBox4.Text = row.Cells[2].Value?.ToString(); // Price
                textBox5.Text = "1";

                label2.Text = "(Fiyat: " + row.Cells[2].Value?.ToString() + ")";
                label3.Text = "(Stok: " + row.Cells[3].Value?.ToString() + ")";
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            UrunAra();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UrunAra();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
        }


        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            textBox2.Text = dataGridView1.CurrentRow.Cells["ProductID"].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells["ProductName"].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells["Price"].Value.ToString();
            textBox5.Text = "1";
            label2.Text = "(Fiyat: " + dataGridView1.CurrentRow.Cells["Price"].Value.ToString() + ")";
            label3.Text = "(Stok: " + dataGridView1.CurrentRow.Cells["StockAmount"].Value.ToString() + ")";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            textBox2.Text = dataGridView1.CurrentRow.Cells["ProductID"].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells["ProductName"].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells["Price"].Value.ToString();
            textBox5.Text = "1";
            label2.Text = "(Fiyat: " + dataGridView1.CurrentRow.Cells["Price"].Value.ToString() + ")";
            label3.Text = "(Stok: " + dataGridView1.CurrentRow.Cells["StockAmount"].Value.ToString() + ")";
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (sepetDT == null) SepetiHazirla();

            // CustomerID zorunlu
            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Müşteri ID gir.");
                return;
            }
            int customerId = Convert.ToInt32(textBox6.Text);

            // ürün seçili mi?
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Önce bir ürün seç.");
                return;
            }

            int productId = Convert.ToInt32(textBox2.Text);
            string name = textBox3.Text;

            decimal price = Convert.ToDecimal(textBox4.Text);
            int qty = Convert.ToInt32(textBox5.Text);

            int stock = Convert.ToInt32(dataGridView1.CurrentRow.Cells[3].Value);

            if (qty <= 0)
            {
                MessageBox.Show("Adet 1'den küçük olamaz.");
                return;
            }
            if (qty > stock)
            {
                MessageBox.Show("Stok yetersiz.");
                return;
            }

            // ✅ aynı ürün + aynı fiyat + aynı müşteri => birleştir
            DataRow existing = sepetDT.AsEnumerable().FirstOrDefault(r =>
                r.Field<int>("CustomerID") == customerId &&
                r.Field<int>("ProductID") == productId &&
                r.Field<decimal>("Price") == price
            );

            if (existing != null)
            {
                int oldQty = existing.Field<int>("Qty");
                int newQty = oldQty + qty;

                if (newQty > stock)
                {
                    MessageBox.Show("Toplam adet stoktan fazla olamaz.");
                    return;
                }

                existing["Qty"] = newQty;
            }
            else
            {
                // ✅ fiyat farklıysa ya da sepette yoksa ayrı satır
                DataRow row = sepetDT.NewRow();
                row["CustomerID"] = customerId;
                row["ProductID"] = productId;
                row["ProductName"] = name;
                row["Price"] = price;
                row["Qty"] = qty;
                sepetDT.Rows.Add(row);
            }

            // Genel toplamı label'da göstermek istiyorsan (istersen)
            // SiparisToplamGuncelle();
            SiparisToplamGuncelle();
        }

        private void satısyap_Load_1(object sender, EventArgs e)
        {
            SiparisToplamGuncelle();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 1️⃣ Emin misin?
            DialogResult dr = MessageBox.Show(
                "Siparişi kaydetmek istediğinize emin misiniz?",
                "Sipariş Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dr != DialogResult.Yes)
                return;

            // 2️⃣ Sepet kontrol
            if (sepetDT == null || sepetDT.Rows.Count == 0)
            {
                MessageBox.Show("Sepet boş.");
                return;
            }

            // 3️⃣ CustomerID kontrol
            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Müşteri ID giriniz.");
                return;
            }

            int customerId = Convert.ToInt32(textBox6.Text);

            // Şimdilik sabit (istersen login’den alırız)
            int companyId = KullaniciBilgileri.CompanyID;
            int salesPersonId = KullaniciBilgileri.SalerpersonID;


            // 4️⃣ Toplam sipariş tutarı
            decimal orderTotal = 0;
            foreach (DataRow r in sepetDT.Rows)
                orderTotal += r.Field<decimal>("Price") * r.Field<int>("Qty");

            using (SqlConnection con = new SqlConnection(DatabaseConnection.GetConnectionString()))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 5️⃣ SALES (HEADER)
                    int saleId;
                    using (SqlCommand cmdSale = new SqlCommand(
                        @"INSERT INTO Sales 
                  (CustomerID, SaleDate, Amount, Status, CompanyID, SalesPersonID)
                  VALUES 
                  (@CustomerID, GETDATE(), @Amount, @Status, @CompanyID, @SalesPersonID);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        con, tran))
                    {
                        cmdSale.Parameters.AddWithValue("@CustomerID", customerId);
                        cmdSale.Parameters.AddWithValue("@Amount", orderTotal);
                        cmdSale.Parameters.AddWithValue("@Status", "Pending");
                        cmdSale.Parameters.AddWithValue("@CompanyID", companyId);
                        cmdSale.Parameters.AddWithValue("@SalesPersonID", salesPersonId);

                        saleId = (int)cmdSale.ExecuteScalar();
                    }

                    // 6️⃣ SALESORDERS + STOK DÜŞ
                    foreach (DataRow r in sepetDT.Rows)
                    {
                        int productId = r.Field<int>("ProductID");
                        int qty = r.Field<int>("Qty");
                        decimal price = r.Field<decimal>("Price");
                        decimal lineTotal = price * qty;

                        // 6a) SalesOrders insert
                        using (SqlCommand cmdDet = new SqlCommand(
                            @"INSERT INTO SalesOrders
                      (SaleID, ProductID, Quantity, TotalAmount, OrderDate, CompanyID)
                      VALUES
                      (@SaleID, @ProductID, @Qty, @TotalAmount, GETDATE(), @CompanyID);",
                            con, tran))
                        {
                            cmdDet.Parameters.AddWithValue("@SaleID", saleId);
                            cmdDet.Parameters.AddWithValue("@ProductID", productId);
                            cmdDet.Parameters.AddWithValue("@Qty", qty);
                            cmdDet.Parameters.AddWithValue("@TotalAmount", lineTotal);
                            cmdDet.Parameters.AddWithValue("@CompanyID", companyId);

                            cmdDet.ExecuteNonQuery();
                        }

                        // 6b) Stock düş
                        using (SqlCommand cmdStock = new SqlCommand(
                            @"UPDATE Products
                      SET StockAmount = StockAmount - @Qty
                      WHERE ProductID = @ProductID AND StockAmount >= @Qty;",
                            con, tran))
                        {
                            cmdStock.Parameters.AddWithValue("@Qty", qty);
                            cmdStock.Parameters.AddWithValue("@ProductID", productId);

                            int affected = cmdStock.ExecuteNonQuery();
                            if (affected == 0)
                                throw new Exception("Stok yetersiz! ÜrünID: " + productId);
                        }
                    }

                    // 7️⃣ COMMIT
                    tran.Commit();

                    MessageBox.Show("Sipariş başarıyla kaydedildi.\nSipariş No: " + saleId);

                    // 8️⃣ Temizlik
                    sepetDT.Clear();
                    SiparisToplamGuncelle();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Kayıt başarısız:\n" + ex.Message);
                }
            }
        }



    }
}
