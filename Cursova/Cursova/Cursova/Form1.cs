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
using System.Data.Sql;

namespace Cursova
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBiulder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;
        private int numoflist=1;
        private bool newRowAdding = false;
        private bool usecol = false;
        private bool usesl = false;
        private bool rivnizt = false;
        private bool systemadmin = true;

        public Form1()
        {
            InitializeComponent();
        }
        private void LoadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);

                sqlBiulder = new SqlCommandBuilder(sqlDataAdapter);

                sqlBiulder.GetInsertCommand();
                sqlBiulder.GetUpdateCommand();
                sqlBiulder.GetDeleteCommand();
                sqlBiulder.GetUpdateCommand();
                dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet, "dbo.Info");
                dataGridView1.DataSource = dataSet.Tables["dbo.Info"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++) {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, i] = linkCell;
                }

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ReloadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);

                sqlBiulder = new SqlCommandBuilder(sqlDataAdapter);



                dataSet.Tables["dbo.Info"].Clear();

                sqlDataAdapter.Fill(dataSet, "dbo.Info");
                dataGridView1.DataSource = dataSet.Tables["dbo.Info"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, i] = linkCell;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(@"Data Source=DESKTOP-ORGE9BG;Initial Catalog=Game;Integrated Security=True");
            sqlConnection.Open();
            LoadData();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try {
                if (e.ColumnIndex == 5) {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();

                    if (task == "Delete")
                    {
                        if (systemadmin){
                            if (MessageBox.Show("Видатили цю строку?", "Видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;

                            dataGridView1.Rows.RemoveAt(rowIndex);

                            dataSet.Tables["dbo.Info"].Rows[rowIndex].Delete();
                            sqlDataAdapter.Update(dataSet, "dbo.Info");

                        }
                    }
                        else { MessageBox.Show("Недостатньо доступу", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    else if (task == "Insert")
                    {
                        if (systemadmin)
                        {
                            int rowIndex = dataGridView1.Rows.Count - 2;



                            DataRow row = dataSet.Tables["dbo.Info"].NewRow();
                            row["TankNumber"] = dataGridView1.Rows[rowIndex].Cells["TankNumber"].Value;
                            row["TankName"] = dataGridView1.Rows[rowIndex].Cells["TankName"].Value;
                            row["TypeOfTank"] = dataGridView1.Rows[rowIndex].Cells["TypeOfTank"].Value;
                            row["CurrentLevel"] = dataGridView1.Rows[rowIndex].Cells["CurrentLevel"].Value;
                            row["Cost"] = dataGridView1.Rows[rowIndex].Cells["Cost"].Value;



                            dataSet.Tables["dbo.Info"].Rows.Add(row);

                            dataSet.Tables["dbo.Info"].Rows.RemoveAt(dataSet.Tables["dbo.Info"].Rows.Count - 1);

                            dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);

                            dataGridView1.Rows[e.RowIndex].Cells[5].Value = "Delete";


                            sqlDataAdapter.Update(dataSet, "dbo.Info");

                            newRowAdding = false;
                        }
                        else { MessageBox.Show("Недостатньо доступу", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    else if (task == "Update")
                    {
                         if (systemadmin) { 
                        int r = e.RowIndex;
                        string Num = dataGridView1.Rows[r].Cells["TankNumber"].Value.ToString();
                        string Name = dataGridView1.Rows[r].Cells["TankName"].Value.ToString();
                        string Type = dataGridView1.Rows[r].Cells["TypeOfTank"].Value.ToString();
                        string Lvl = dataGridView1.Rows[r].Cells["CurrentLevel"].Value.ToString();
                        string Cos = dataGridView1.Rows[r].Cells["Cost"].Value.ToString();

                        sqlDataAdapter.Update(dataSet, "dbo.Info");
                        //string a = "UPDATE Info SET .TankNumber = 16, .TankName = 'obj2', .TypeOfTank = 'Heavy', CurrentLevel = 5, .Cost = 1000 WHERE TankNumber = 15";

                        string rr = (r + 1).ToString();



                        // string expres = "UPDATE Info SET TankNumber = " + Num + " Where TankNumber = " + rr;
                        /// SqlCommand command = new SqlCommand(expres, sqlConnection);
                        // int num = command.ExecuteNonQuery();

                        string expres = "UPDATE Info SET Cost= " + Cos + " Where TankNumber = " + Num;
                        SqlCommand command = new SqlCommand(expres, sqlConnection);
                        int num = command.ExecuteNonQuery();



                        expres = "UPDATE Info SET TankName = '" + Name + "' Where TankNumber = " + Num;
                        command = new SqlCommand(expres, sqlConnection);
                        num = command.ExecuteNonQuery();

                        expres = "UPDATE Info SET TypeOfTank = '" + Type + "' Where TankNumber = " + Num;
                        command = new SqlCommand(expres, sqlConnection);
                        num = command.ExecuteNonQuery();

                        expres = "UPDATE Info SET CurrentLevel = " + Lvl + " Where TankNumber = " + Num;
                        command = new SqlCommand(expres, sqlConnection);
                        num = command.ExecuteNonQuery();



                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = "Delete";
                    }
                        else { MessageBox.Show("Недостатньо доступу", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    ReloadData();
                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try{
                if (!newRowAdding) {
                    newRowAdding = true;
                    int lastRow = dataGridView1.Rows.Count - 2;

                    DataGridViewRow row = dataGridView1.Rows[lastRow];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, lastRow] = linkCell;
                    row.Cells["COMAND"].Value = "Insert";
                
                }
            
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!newRowAdding)
            {
                try
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;

                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, rowIndex] = linkCell;
                    editingRow.Cells["COMAND"].Value = "Update";

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            usecol = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            usecol = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            usesl = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            usesl = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                numoflist = int.Parse(numericUpDown1.Value.ToString());
                string a = (numoflist * 11).ToString();
                string b;
                if (numoflist > 0)
                {
                    b = ((numoflist - 1) * 11).ToString();
                }
                else { b = "0"; }
                if (usesl && !usecol) {
                    sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);

                }
                if (!usesl && !usecol) {
                    sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                }
                if (!usesl && usecol)
                {
                    if (!rivnizt) {
                        string temp = comboBox1.Text;
                        if (temp == "TankNumber")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TankNumber=" + tn, sqlConnection);
                        }
                        else if (temp == "TankName")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TankName= '" + tn + "'", sqlConnection);
                        }
                        else if (temp == "TypeOfTank")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TypeOfTank= '" + tn + "'", sqlConnection);
                        }
                        else if (temp == "CurrentLevel")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.CurrentLevel=" + tn, sqlConnection);
                        }
                        else if (temp == "Cost")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.Cost=" + tn, sqlConnection);
                        }
                        else {
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                        }

                    }
                    else
                    {
                        string temp = comboBox1.Text;
                        if (temp == "TankNumber")
                        {
                            string tn = textBox2.Text;
                            string tn1 = textBox1.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TankNumber<=" + tn1 + "AND dbo.Info.TankNumber>=" + tn, sqlConnection);
                        }
                        else if (temp == "TankName")
                        {
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                        }
                        else if (temp == "TypeOfTank")
                        {
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                        }
                        else if (temp == "CurrentLevel")
                        {
                            string tn = textBox2.Text;
                            string tn1 = textBox1.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.CurrentLevel<=" + tn1 + "AND dbo.Info.CurrentLevel>=" + tn, sqlConnection);
                        }
                        else if (temp == "Cost")
                        {
                            string tn = textBox2.Text;
                            string tn1 = textBox1.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.Cost<=" + tn1 + "AND dbo.Info.Cost>=" + tn, sqlConnection);
                        }
                        else
                        {
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                        }

                    }

                }
                //+"AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b
                if (usecol && usesl) {
                    if (!rivnizt)
                    {
                        string temp = comboBox1.Text;
                        if (temp == "TankNumber")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TankNumber=" + tn + "AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);
                        }
                        else if (temp == "TankName")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TankName= '" + tn + "'" + "AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);
                        }
                        else if (temp == "TypeOfTank")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TypeOfTank= '" + tn + "'" + "AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);
                        }
                        else if (temp == "CurrentLevel")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.CurrentLevel=" + tn + "AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);
                        }
                        else if (temp == "Cost")
                        {
                            string tn = textBox3.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.Cost=" + tn + "AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);
                        }
                        else
                        {
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                        }

                    }
                    else
                    {
                        string temp = comboBox1.Text;
                        if (temp == "TankNumber")
                        {
                            string tn = textBox2.Text;
                            string tn1 = textBox1.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.TankNumber<=" + tn1 + "AND dbo.Info.TankNumber>=" + tn + "AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);
                        }
                        else if (temp == "TankName")
                        {
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                        }
                        else if (temp == "TypeOfTank")
                        {
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                        }
                        else if (temp == "CurrentLevel")
                        {
                            string tn = textBox2.Text;
                            string tn1 = textBox1.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.CurrentLevel<=" + tn1 + "AND dbo.Info.CurrentLevel>=" + tn + "AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);
                        }
                        else if (temp == "Cost")
                        {
                            string tn = textBox2.Text;
                            string tn1 = textBox1.Text;
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info WHERE dbo.Info.Cost<=" + tn1 + "AND dbo.Info.Cost>=" + tn + "AND dbo.Info.TankNumber<=" + a + "AND dbo.Info.TankNumber>" + b, sqlConnection);
                        }
                        else
                        {
                            sqlDataAdapter = new SqlDataAdapter("SELECT dbo.Info.TankNumber,dbo.Info.TankName,dbo.Info.TypeOfTank,dbo.Info.CurrentLevel,dbo.Info.Cost,'Delete' AS [COMAND] FROM Info  ", sqlConnection);
                        }

                    }

                }




                sqlBiulder = new SqlCommandBuilder(sqlDataAdapter);

                sqlBiulder.GetInsertCommand();
                sqlBiulder.GetUpdateCommand();
                sqlBiulder.GetDeleteCommand();
                sqlBiulder.GetUpdateCommand();
                dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet, "dbo.Info");
                dataGridView1.DataSource = dataSet.Tables["dbo.Info"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, i] = linkCell;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }




        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            rivnizt = false;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            rivnizt = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox5.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (systemadmin) {
                if (textBox4.Text == "Employee1") {
                    if (textBox5.Text == "qwerty123") {
                        MessageBox.Show("Увійшов", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox6.Text = "OnlyRead";
                        systemadmin = false;
                    }
                    else {
                        MessageBox.Show("Пароль Невірний", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else { MessageBox.Show("Логін Невірний", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else {
                if (textBox4.Text == "Employee2")
                {
                    if (textBox5.Text == "qwerty123")
                    {
                        MessageBox.Show("Увійшов", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox6.Text = "SystemAdmin";
                        systemadmin = true;
                    }
                    else
                    {
                        MessageBox.Show("Пароль Невірний", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else { MessageBox.Show("Логін Невірний", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            }
        }
    }
}
