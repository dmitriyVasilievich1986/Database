using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Data_Base
{
    public partial class Result : Form
    {
        public Summary Stat (int iden, string name, string parameters)
        {
            Summary output = new Summary(name);
            using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=All_Moduls;Integrated Security=True"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT * FROM [All_Moduls].[dbo].[{parameters}] where iden={iden}", connection);
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows) output.name = "";
                else
                {
                    while (reader.Read())
                    {
                        output.add_test();
                        for (int a = 2; a < reader.FieldCount; a += 2)
                        {
                            try { output.add_item(Convert.ToSingle(reader.GetDecimal(a)), reader.GetBoolean(a + 1)); }
                            catch (Exception) { }
                        }
                    }
                }
            }
            if (output.name != "") 
            {
                foreach(Result_Test res in output.result)
                {
                    for (int x = 0; x < res.result.Count; x++)
                    {
                        if (!res.result[x].Item2) output.all_result = false;
                    }
                }
            }
            return output;
        }

        public Result(int iden)
        {
            InitializeComponent();
            DataTable DT = new DataTable();            
            List<Summary> result = new List<Summary>();

            result.Add(Stat(iden, "Проверка питания", "current"));
            result.Add(Stat(iden, "Проверка Din", "din"));            
            result.Add(Stat(iden, "Проверка КФ", "kf"));
            result.Add(Stat(iden, "Проверка TC", "tc"));
            result.Add(Stat(iden, "Проверка 12B TC", "tc12v"));
            result.Add(Stat(iden, "Проверка ТУ", "tu"));
            result.Add(Stat(iden, "Проверка питания MTU5", "MTU5_Power"));
            result.Add(Stat(iden, "Проверка температуры", "temperature"));
            result.Add(Stat(iden, "Проверка ток 0", "current0"));
            result.Add(Stat(iden, "Проверка ТУ MTU5", "MTU5_TU"));
            result.Add(Stat(iden, "Проверка EnTU", "entu"));

            int column = 0;
            DataColumn dc = new DataColumn("Тест"); DT.Columns.Add(dc);

            for (int a = 0; a < result.Count; a++)
            {
                try { while (result[a].name == "") result.RemoveAt(a); }
                catch (Exception) { }                
            }

            foreach(Summary a in result) { if (a.result.Count > column) column = a.result.Count;  }
            for (int a = 0; a < column; a++) { dc = new DataColumn($"Тест {a+1}"); DT.Columns.Add(dc); }
            
            foreach (Summary sum in result)
            {
                DataRow DR = DT.NewRow();
                DR[0] = sum.name;
                DT.Rows.Add(DR);
                foreach (Result_Test a in sum.result)
                {
                    DR = DT.NewRow();
                    for (int x = 0; x < a.result.Count; x++) { DR[x + 1] = a.result[x].Item1; }
                    DT.Rows.Add(DR);
                }
            }
            dataGridView1.DataSource = DT; colorized(result);
        }

        public async void colorized(List<Summary> result)
        {
            await Task.Delay(250);
            int row = 0;
            foreach (Summary sum in result)
            {
                dataGridView1.Rows[row].Cells[0].Style.BackColor = sum.all_result ? Color.Green : Color.Red;
                row++;
                foreach (Result_Test a in sum.result)
                {
                    for (int x = 0; x < a.result.Count; x++) { dataGridView1.Rows[row].Cells[x + 1].Style.BackColor = a.result[x].Item2 ? Color.Green : Color.Red; }
                    row++;
                }
            }
        }
    }

    

    public class Result_Test
    {
        public List<(float,bool)> result;

        public Result_Test()
        {
            result = new List<(float, bool)>();
        }
    }
    public class Summary
    {
        public string name = "";
        public bool all_result = true;

        public List<Result_Test> result;
        private int count = -1;

        public Summary(string Name) { name = Name; result = new List<Result_Test>(); }

        public void add_test() { result.Add(new Result_Test()); count++; }

        public void add_item(float a, bool b) { result[count].result.Add((a, b)); }
    }
}
