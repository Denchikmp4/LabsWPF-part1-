using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Lab2_SQLiteStudents_Variant2
{
    public partial class MainWindow : Window
    {
        private const string DbFile = "students.sqlite";
        private readonly string connectionString = $"Data Source={DbFile};Version=3;";
        private ObservableCollection<StudentRow> students = new();

        public MainWindow()
        {
            InitializeComponent();
            EnsureDatabase();
            LoadStudents();
        }

        private void EnsureDatabase()
        {
            if (!File.Exists(DbFile)) SQLiteConnection.CreateFile(DbFile);
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            Execute(connection, "CREATE TABLE IF NOT EXISTS Students (Id INTEGER PRIMARY KEY, FullName TEXT NOT NULL, Phone TEXT)");
            Execute(connection, "CREATE TABLE IF NOT EXISTS Marks (StudentId INTEGER PRIMARY KEY, Physics INTEGER, Math INTEGER, FOREIGN KEY(StudentId) REFERENCES Students(Id) ON DELETE CASCADE)");
        }

        private static void Execute(SQLiteConnection connection, string sql)
        {
            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private bool ReadForm(out StudentRow row)
        {
            row = new StudentRow();
            if (!int.TryParse(IdBox.Text, out int id) || !int.TryParse(PhysicsBox.Text, out int physics) || !int.TryParse(MathBox.Text, out int math))
            {
                MessageBox.Show("Проверьте номер и оценки: они должны быть целыми числами.");
                return false;
            }
            row.Id = id;
            row.FullName = NameBox.Text.Trim();
            row.Phone = PhoneBox.Text.Trim();
            row.Physics = physics;
            row.Math = math;
            if (row.FullName.Length == 0)
            {
                MessageBox.Show("Введите ФИО студента.");
                return false;
            }
            return true;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!ReadForm(out var row)) return;
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            using var tr = connection.BeginTransaction();
            Execute(connection, $"INSERT INTO Students (Id, FullName, Phone) VALUES ({row.Id}, '{Escape(row.FullName)}', '{Escape(row.Phone)}')");
            Execute(connection, $"INSERT INTO Marks (StudentId, Physics, Math) VALUES ({row.Id}, {row.Physics}, {row.Math})");
            tr.Commit();
            LoadStudents();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (!ReadForm(out var row)) return;
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            Execute(connection, $"UPDATE Students SET FullName='{Escape(row.FullName)}', Phone='{Escape(row.Phone)}' WHERE Id={row.Id}");
            Execute(connection, $"UPDATE Marks SET Physics={row.Physics}, Math={row.Math} WHERE StudentId={row.Id}");
            LoadStudents();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(IdBox.Text, out int id)) return;
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            Execute(connection, $"DELETE FROM Marks WHERE StudentId={id}");
            Execute(connection, $"DELETE FROM Students WHERE Id={id}");
            LoadStudents();
        }

        private void Load_Click(object sender, RoutedEventArgs e) => LoadStudents();
        private void Clear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void LoadStudents()
        {
            students = new ObservableCollection<StudentRow>();
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            string sql = "SELECT Students.Id, FullName, Phone, Physics, Math FROM Students INNER JOIN Marks ON Students.Id = Marks.StudentId ORDER BY Students.Id";
            using var command = new SQLiteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new StudentRow
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    FullName = reader["FullName"].ToString() ?? "",
                    Phone = reader["Phone"].ToString() ?? "",
                    Physics = Convert.ToInt32(reader["Physics"]),
                    Math = Convert.ToInt32(reader["Math"])
                });
            }
            StudentsGrid.ItemsSource = students;
        }

        private void StudentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is StudentRow row)
            {
                IdBox.Text = row.Id.ToString();
                NameBox.Text = row.FullName;
                PhoneBox.Text = row.Phone;
                PhysicsBox.Text = row.Physics.ToString();
                MathBox.Text = row.Math.ToString();
            }
        }

        private void ClearForm()
        {
            IdBox.Clear(); NameBox.Clear(); PhoneBox.Clear(); PhysicsBox.Clear(); MathBox.Clear();
        }

        private static string Escape(string value) => value.Replace("'", "''");
    }
}
