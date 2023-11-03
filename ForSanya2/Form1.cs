using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ForSanya2
{
    public partial class Form1 : Form
    {
        private SQLiteConnection sqliteConnection;

        public Form1()
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

            using (SQLiteCommand cmd = new SQLiteCommand(sqliteConnection))
            {
                cmd.CommandText = "SELECT DepositName FROM Deposits;";

                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridView4.DataSource = dataTable;

                // Устанавливаем AutoSizeColumnsMode для DataGridView на Fill
                dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Находим столбец "DepositName"
                DataGridViewColumn column = dataGridView4.Columns["DepositName"];
            }

            using (SQLiteCommand cmd = new SQLiteCommand(sqliteConnection))
            {
                cmd.CommandText = "SELECT DISTINCT Type FROM UsefulMinerals;";

                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridView5.DataSource = dataTable;

                // Устанавливаем AutoSizeColumnsMode для DataGridView на Fill
                dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Находим столбец "Type"
                DataGridViewColumn column = dataGridView5.Columns["Type"];
            }

            using (SQLiteCommand cmd = new SQLiteCommand(sqliteConnection))
            {
                cmd.CommandText = "SELECT DISTINCT DevelopmentMethod FROM Deposits;";

                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridView2.DataSource = dataTable;

                // Устанавливаем AutoSizeColumnsMode для DataGridView на Fill
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Находим столбец "DevelopmentMethod"
                DataGridViewColumn column = dataGridView2.Columns["DevelopmentMethod"];
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqliteConnection != null)
            {
                sqliteConnection.Close();
            }
            Application.Exit();
        }

        #region sssssss
        private void button5_Click(object sender, EventArgs e)
        {
            tb1.Text = "UsefulMinerals";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tb1.Text = "Deposits";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            tb1.Text = "ExportPoints";
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
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
        #endregion

        #region Load

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
    #endregion

    #region Zaprosi
    private void button1_Click(object sender, EventArgs e)
        {
            // Создать SQL-запрос для получения месторождений, которые будут исчерпаны через 5 лет
            string query = "SELECT * FROM Deposits WHERE Reserves / AnnualProduction > 5;";

            // Вызвать метод ExecuteAndDisplayQuery с запросом
            ExecuteAndDisplayQuery(query);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Создать SQL-запрос для определения ископаемых, потребность в которых не удовлетворяется
            string query = @"
                SELECT UM.MineralName
                FROM UsefulMinerals AS UM
                LEFT JOIN (
                    SELECT DM.UsefulMineralsID, SUM(DM.AnnualProduction) AS TotalProduction
                    FROM Deposits AS DM
                    GROUP BY DM.UsefulMineralsID
                ) AS ProductionSum ON UM.ID = ProductionSum.UsefulMineralsID
                WHERE UM.AnnualDemand > IFNULL(ProductionSum.TotalProduction, 0);";

            // Вызвать метод ExecuteAndDisplayQuery с запросом
            ExecuteAndDisplayQuery(query);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Создать SQL-запрос для определения ископаемых, потребность в которых не удовлетворяется
            string query = $@"
                SELECT UM.MineralName
                FROM UsefulMinerals AS UM
                INNER JOIN Deposits AS DM ON UM.ID = DM.UsefulMineralsID
                WHERE DM.DepositName = '{textBox3.Text}';";

            // Вызвать метод ExecuteAndDisplayQuery с запросом
            ExecuteAndDisplayQuery(query);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string query = $@"
                SELECT DM.DepositName
                FROM Deposits AS DM
                INNER JOIN UsefulMinerals AS UM ON DM.UsefulMineralsID = UM.ID
                WHERE UM.Type = '{textBox4.Text}' AND DM.DevelopmentMethod = '{textBox1.Text}';";

            // Вызвать метод ExecuteAndDisplayQuery с запросом
            ExecuteAndDisplayQuery(query);
        }

        private void ExecuteAndDisplayQuery(string query)
        {
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, sqliteConnection))
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
        #endregion
        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Убедимся, что это не заголовок таблицы
            {
                DataGridViewCell selectedCell = dataGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex];
                textBox3.Text = selectedCell.Value.ToString();
            }
        }

        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Убедимся, что это не заголовок таблицы
            {
                DataGridViewCell selectedCell = dataGridView5.Rows[e.RowIndex].Cells[e.ColumnIndex];
                textBox4.Text = selectedCell.Value.ToString();
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Убедимся, что это не заголовок таблицы
            {
                DataGridViewCell selectedCell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                textBox1.Text = selectedCell.Value.ToString();
            }
        }
    }
}
