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

namespace Data_Base
{
    public partial class Form1 : Form
    {
        public string[] module(string par)
        {
            List<string> new_list = new List<string>();
            List<string> output = new List<string>();
            using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=All_Moduls;Integrated Security=True"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT [{par}] FROM [All_Moduls].[dbo].[module]", connection);
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    try { new_list.Add(reader.GetString(0));}
                    catch (Exception) { }                    
                }
            }
            output.Add("Все");
            for (int a = 0; a < new_list.Count; a++)
            {
                if (a == new_list.IndexOf(new_list[a])) output.Add(new_list[a]);
            }            
            return output.ToArray();
        }

        void Data_Set(bool error)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=All_Moduls;Integrated Security=True"))
            {
                connection.Open();
                string command = $"SELECT [id],[data],[module],[sn],[error],[malfunction] FROM [All_Moduls].[dbo].[module] where ";
                if (comboBox1.Text != "Все")
                {
                    command += $" module='{comboBox1.Text}'";
                    if (textBox2.Text != "") command += " and ";
                }
                if (textBox2.Text != "") command += $" sn={int.Parse(textBox2.Text)}";

                if (comboBox1.Text != "Все" || textBox2.Text != "")
                { command += " and data between @datastart and @dataend"; }
                else
                { command += " data between @datastart and @dataend"; }
                if (error && comboBox2.Text != "Все") command += $" and error = '{comboBox2.Text}'";
                else if (error) command += $" and not error = ''";

                SqlCommand c = new SqlCommand(command, connection);
                c.Parameters.AddWithValue($"@datastart", dateTimePicker1.Value);
                c.Parameters.AddWithValue($"@dataend", dateTimePicker2.Value);

                SqlDataAdapter adapter = new SqlDataAdapter(c);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                ds.Tables[0].Columns["data"].ColumnName = "Дата";
                ds.Tables[0].Columns["module"].ColumnName = "Модуль";
                ds.Tables[0].Columns["sn"].ColumnName = "Сер. номер";
                ds.Tables[0].Columns["error"].ColumnName = "Неисправность";
                ds.Tables[0].Columns["malfunction"].ColumnName = "Причина";
                dataGridView1.DataSource = ds.Tables[0];
            }
            dataGridView1.Columns["id"].Visible = false;

            Dictionary<string, int> modules = new Dictionary<string, int>();
            Dictionary<string, int> err = new Dictionary<string, int>();
            foreach (string a in comboBox1.Items) modules.Add(a, 0);
            foreach (string a in comboBox2.Items) err.Add(a, 0);

            for (int a = 0; a < dataGridView1.RowCount - 1; a++)
            {
                modules["Все"]++;
                foreach (string x in modules.Keys) { if (dataGridView1.Rows[a].Cells[2].Value.ToString() == x) { modules[x]++; break; } }
                foreach (string x in err.Keys) { if (dataGridView1.Rows[a].Cells[4].Value.ToString() == x) { err[x]++; err["Все"]++; break; } }
            }

            dataGridView1.Columns["Дата"].Width = 70;
            dataGridView1.Columns["Модуль"].Width = 70;
            dataGridView1.Columns["Сер. номер"].Width = 70;
            dataGridView1.Columns["Неисправность"].Width = 180;

            textBox1.Clear();
            textBox3.Clear();
            foreach (string a in modules.Keys)
                { if (a == "Все") textBox1.Text += "Всего: " + modules[a].ToString() + Environment.NewLine; else if (modules[a] > 0) textBox1.Text += a + ": " + modules[a].ToString() + Environment.NewLine; }
            foreach (string a in err.Keys)
                { if (a == "Все") textBox3.Text += "Всего: " + err[a].ToString() + Environment.NewLine; else if (err[a] > 0) textBox3.Text += a + ": " + err[a].ToString() + Environment.NewLine; }
        }

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(module("module"));
            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox2.Items.AddRange(module("error"));
            comboBox2.Text = comboBox2.Items[0].ToString();
            button1.Click += (s, e) => { Data_Set(false); };
            button3.Click += (s, e) => { Data_Set(true); };
            textBox2.MouseClick += (s, e) => { textBox2.Clear(); };
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Result res = new Result(int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
            res.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Input_Changes IC = new Input_Changes(int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()), dataGridView1.SelectedRows[0].Cells[2].Value.ToString(), int.Parse(dataGridView1.SelectedRows[0].Cells[3].Value.ToString()));
            IC.Show();
        }
    }
}
