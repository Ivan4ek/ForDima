using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForSanya2
{
    public partial class Form3 : Form
    {
        private SQLiteConnection sqliteConnection;
        private string currentTableName;
        public Form3()
        {
            InitializeComponent();

            sqliteConnection = new SQLiteConnection("Data Source=Database.db;Version=3;");
            sqliteConnection.Open();

            // Проверяем, существуют ли таблицы, и создаем их при необходимости
            CreateTablesIfNotExists();

            // Проверяем существование данных в таблицах "Ships," "Ports," и "Combo"
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM UsefulMinerals;", sqliteConnection))
            {
                int shipCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (shipCount == 0)
                {
                    // Таблица "Ships" пуста, вставляем примерные данные
                    InsertSampleData();
                }
            }

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM Deposits;", sqliteConnection))
            {
                int portCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (portCount == 0)
                {
                    // Таблица "Ports" пуста, вставляем примерные данные
                    InsertSampleData();
                }
            }

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM ExportPoints;", sqliteConnection))
            {
                int portCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (portCount == 0)
                {
                    // Таблица "Ports" пуста, вставляем примерные данные
                    InsertSampleData();
                }
            }
        }

        private void CreateTablesIfNotExists()
        {
            using (SQLiteCommand cmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", sqliteConnection))
            {
                cmd.ExecuteNonQuery();
            }

            // Проверяем существование таблицы "UsefulMinerals"
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='UsefulMinerals';", sqliteConnection))
            {
                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    // Таблица "UsefulMinerals" не существует, создаем её
                    using (SQLiteCommand createCmd = new SQLiteCommand("CREATE TABLE " +
                        "UsefulMinerals (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "MineralName TEXT, " +
                        "UnitOfMeasurement TEXT, " +
                        "AnnualDemand REAL, " +
                        "PricePerUnit REAL, " +
                        "Type TEXT);", sqliteConnection))
                    {
                        createCmd.ExecuteNonQuery();
                    }
                }
            }

            // Проверяем существование таблицы "Deposits"
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='Deposits';", sqliteConnection))
            {
                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    // Таблица "Deposits" не существует, создаем её
                    using (SQLiteCommand createCmd = new SQLiteCommand("CREATE TABLE " +
                        "Deposits (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "DepositName TEXT, " +
                        "Reserves REAL, " +
                        "DevelopmentMethod TEXT, " +
                        "AnnualProduction REAL, " +
                        "UnitCost REAL, " +
                        "UsefulMineralsID INTEGER);", sqliteConnection))
                    {
                        createCmd.ExecuteNonQuery();
                    }
                }
            }

            // Проверяем существование таблицы "ExportPoints"
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='ExportPoints';", sqliteConnection))
            {
                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    // Таблица "ExportPoints" не существует, создаем её
                    using (SQLiteCommand createCmd = new SQLiteCommand("CREATE TABLE " +
                        "ExportPoints (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "PointName TEXT, " +
                        "ThroughputCapacity REAL);", sqliteConnection))
                    {
                        createCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void InsertSampleData()
        {
            using (SQLiteTransaction transaction = sqliteConnection.BeginTransaction())
            {
                // Добавляем данные в таблицу "UsefulMinerals"
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO UsefulMinerals " +
                    "(MineralName, UnitOfMeasurement, AnnualDemand, PricePerUnit, Type) " +
                    "VALUES " +
                    "('Уголь', 'тонн', 1000000, 50.0, 'Энергетический'), " +
                    "('Нефть', 'баррель', 500000, 70.0, 'Нефтепродукты'), " +
                    "('Золото', 'унция', 10000, 1500.0, 'Драгоценные металлы'), " +
                    "('Серебро', 'унция', 20000, 200.0, 'Драгоценные металлы'), " +
                    "('Железо', 'тонн', 800000, 70.0, 'Металлы'), " +
                    "('Медь', 'тонн', 40000, 1500.0, 'Металлы');", sqliteConnection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Добавляем данные в таблицу "Deposits"
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Deposits " +
                    "(DepositName, Reserves, DevelopmentMethod, AnnualProduction, UnitCost, UsefulMineralsID) " +
                    "VALUES " +
                    "('Угольный карьер 1', 50000000, 'Открытая разработка', 10000000, 25.0, 1), " +
                    "('Угольный карьер 2', 1000000, 'Открытая разработка', 50000, 24.0, 1), " +
                    "('Нефтяная вышка 1', 300000, 'Буровая разработка', 1000000, 60.0, 2), " +
                    "('Нефтяная вышка 2', 2000000, 'Буровая разработка', 400000, 63.0, 2), " +
                    "('Золотая руда 1', 20000, 'Подземная разработка', 4000, 100.0, 3), " +
                    "('Золотая руда 2', 320000, 'Шахтовый метод', 5500, 120.0, 3), " +
                    "('Серебряная руда 1', 50000, 'Подземная разработка', 12000, 120.0, 4), " +
                    "('Железная руда 1', 350000, 'Открытая разработка', 90000, 50.0, 5), " +
                    "('Медная руда 1', 15000, 'Буровая разработка', 5000, 80.0, 6);", sqliteConnection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Добавляем данные в таблицу "ExportPoints"
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO ExportPoints " +
                    "(PointName, ThroughputCapacity) " +
                    "VALUES " +
                    "('Пункт вывоза 1', 5000.0), " +
                    "('Пункт вывоза 2', 3000.0), " +
                    "('Пункт вывоза 3', 2000.0), " +
                    "('Пункт вывоза 4', 7000.0), " +
                    "('Пункт вывоза 5', 6000.0), " +
                    "('Пункт вывоза 6', 4000.0);", sqliteConnection))
                {
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tb1.Text = "UsefulMinerals";
            currentTableName = tb1.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tb1.Text = "Deposits";
            currentTableName = tb1.Text;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            tb1.Text = "ExportPoints";
            currentTableName = tb1.Text;
        }

        private void tb1_TextChanged(object sender, EventArgs e)
        {
            string tableName = tb1.Text;

            // Проверяем, существует ли указанная таблица
            if (TableExistsMenu(tableName))
            {
                LoadDataToDataGridView(tableName);
            }
            else
            {
                // Если таблица не существует, выводим сообщение об ошибке
                MessageBox.Show("Указанная таблица не существует.");
            }
        }

        private bool TableExistsMenu(string tableName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(sqliteConnection))
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["name"].ToString() == tableName)
                            {
                                return true; // Таблица существует
                            }
                        }
                    }
                }
            }
            return false; // Таблица не существует
        }

        private void LoadDataToDataGridView(string tableName)
        {
            using (SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {tableName}", sqliteConnection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;

                    // Растянуть столбцы равномерно
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqliteConnection != null)
            {
                sqliteConnection.Close();
            }
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string tableName = tb1.Text;

            // Проверяем, существует ли указанная таблица
            if (TableExistsMenu(tableName))
            {
                // Если таблица существует, загружаем ее данные в dgv1
                LoadDataIntoDataGridView(dataGridView1, tableName);

                // Растянуть столбцы равномерно
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                // Растянуть столбцы равномерно
                foreach (DataGridViewColumn column in dgv2.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            else
            {
                // Если таблица не существует, выводим сообщение об ошибке
                MessageBox.Show("Указанная таблица не существует.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgv2.Rows.Count > 0)
            {
                bool isRowValid = true;

                foreach (DataGridViewCell cell in dgv2.Rows[0].Cells)
                {
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        isRowValid = false;
                        break;
                    }
                }

                if (isRowValid)
                {
                    using (SQLiteCommand insertCommand = new SQLiteCommand(sqliteConnection))
                    {
                        StringBuilder columns = new StringBuilder();
                        StringBuilder values = new StringBuilder();

                        foreach (DataGridViewColumn column in dgv2.Columns)
                        {
                            if (columns.Length > 0)
                            {
                                columns.Append(", ");
                                values.Append(", ");
                            }
                            columns.Append(column.Name);
                            values.Append($"@{column.Name}");
                            insertCommand.Parameters.AddWithValue($"@{column.Name}", dgv2.Rows[0].Cells[column.Index].Value);
                        }

                        insertCommand.CommandText = $"INSERT INTO {currentTableName} ({columns}) VALUES ({values})";
                        insertCommand.ExecuteNonQuery();
                    }

                    LoadDataIntoDataGridView(dataGridView1, currentTableName);

                    dgv2.Rows.RemoveAt(0);

                    // Растянуть столбцы равномерно
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    // Растянуть столбцы равномерно
                    foreach (DataGridViewColumn column in dgv2.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }
                else
                {
                    MessageBox.Show("Не все поля в строке таблицы заполнены. Заполните все поля перед добавлением.");
                }
            }
            else
            {
                MessageBox.Show("Таблица пуста. Нечего добавлять.");
            }
        }

        private void LoadDataIntoDataGridView(DataGridView dataGridView, string tableName)
        {
                using (SQLiteCommand command = new SQLiteCommand(sqliteConnection))
                {
                    command.CommandText = $"SELECT * FROM {tableName}";
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Очистим существующие столбцы в DataGridView
                    dataGridView.Columns.Clear();

                    // Устанавливаем источник данных для DataGridView
                    dataGridView.DataSource = dataTable;

                    // Автоматически подгоним размер столбцов
                    dataGridView.AutoResizeColumns();

                    currentTableName = tableName; // Сохраняем имя текущей таблицы
                }
            // Очистите dgv2 от существующих столбцов
            dgv2.Columns.Clear();

            // Добавьте все столбцы из dgv1, кроме первого столбца (с индексом 0), в dgv2
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Index != 0) // Исключаем первый столбец
                {
                    dgv2.Columns.Add((DataGridViewColumn)col.Clone());
                }
            }

            // Добавьте пустую строку в dgv2
            dgv2.Rows.Add();
        }

        private int selectedRowIndex = -1;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedRowIndex = e.RowIndex;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (sqliteConnection != null)
            {
                sqliteConnection.Close();
            }

            if (selectedRowIndex >= 0)
            {
                int recordID = Convert.ToInt32(dataGridView1.Rows[selectedRowIndex].Cells["ID"].Value);

                if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Удаление записи", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(sqliteConnection))
                    {
                        connection.Open();

                        using (SQLiteCommand deleteCommand = new SQLiteCommand($"DELETE FROM {currentTableName} WHERE ID = @id", connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@id", recordID);
                            deleteCommand.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    // Удалите строку из DataGridView
                    dataGridView1.Rows.RemoveAt(selectedRowIndex);
                    selectedRowIndex = -1;
                }
            }

            // Восстановите соединение
            sqliteConnection.Open();
        }

    }
}
