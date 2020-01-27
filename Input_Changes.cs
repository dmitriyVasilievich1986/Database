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
    public partial class Input_Changes : Form
    {
        int iden;

        public string[] module(string par)
        {
            List<string> new_list = new List<string>();
            List<string> output = new List<string>();
            using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=All_Moduls;Integrated Security=True"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT [{par}] FROM [All_Moduls].[dbo].[module]", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    try { new_list.Add(reader.GetString(0)); }
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

        public Input_Changes(int id, string name, int sn)
        {
            InitializeComponent();
            iden = id;
            label1.Text += $" {name}";
            label2.Text += $" {sn}";
            comboBox2.Items.AddRange(module("error"));
            comboBox2.Text = comboBox2.Items[0].ToString();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=All_Moduls;Integrated Security=True"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"UPDATE [All_Moduls].[dbo].[module] SET error = @error, malfunction = @malfunction WHERE id=@id", connection);
                command.Parameters.AddWithValue("@error", comboBox2.Text);
                command.Parameters.AddWithValue("@malfunction", textBox1.Text);
                command.Parameters.AddWithValue("@id", iden);
                command.ExecuteNonQuery();
            }
            this.Close();
        }
    }
}
