using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Music_Player
{

    public class Db
    {
        public String connectionString;
        public SQLiteConnection connection;

        public Db()
        {
            this.connectionString = "Data source=tracks.db;Version=3";
            createTableIfNotExists();
        }

        public void createTableIfNotExists()
        {
            connection = new SQLiteConnection(connectionString);
            connection.Open();
            String createTableSQL = "Create table if not exists Tracks(" +
    "Track_ID integer primary key autoincrement," +
    "Song Text," +
    "Artist Text," +
    "Year Text," +
    "Genre Text," +
    "Language Text," +
    "Frequency Integer)";

            SQLiteCommand command = new SQLiteCommand(createTableSQL, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public void incrementFrequency(String song)
        {
            connection.Open();
            String updateSQL = "UPDATE Tracks SET Frequency=Frequency+1 WHERE Song='" + song + "';";
            SQLiteCommand command = new SQLiteCommand(updateSQL, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        void addRow(String song)
        {
            connection.Open();
            String insertSQL = "INSERT INTO Tracks(" +
                "Song," +
                "Artist," +
                "Year," +
                "Genre," +
                "Language," +
                "Frequency) " +
                "values(" +
                "@song," +
                "@artist," +
                "@year," +
                "@genre," +
                "@language," +
                "@frequency" +
                ");";
            SQLiteCommand command = new SQLiteCommand(insertSQL, connection);
            command.Parameters.AddWithValue("song", song);
            command.Parameters.AddWithValue("artist", "");
            command.Parameters.AddWithValue("year", "");
            command.Parameters.AddWithValue("genre", "");
            command.Parameters.AddWithValue("language", "");
            command.Parameters.AddWithValue("frequency", 0);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void synchWithDbSongs()
        {
            String strArrToStr(String[] strArr)
            {
                string formattedValues = "";
                for (int i=0; i<strArr.Length; i++)
                {
                    strArr[i] = Path.GetFileNameWithoutExtension(strArr[i]);
                }
                 formattedValues = string.Join(", ", strArr.Select(value => $"'{value}'"));
                return formattedValues;
            }

            String toCheck = strArrToStr((Directory.GetFiles("../../../tracks/")));
            connection.Open();
            String selectSQL = "DELETE FROM Tracks WHERE Song NOT IN (" + toCheck + ");";
            SQLiteCommand command = new SQLiteCommand(selectSQL, connection);
            command.ExecuteNonQuery();
            connection.Close();

            List<String> songsInDb = new List<String>();
            connection.Open();
            String selectSQL2 = "SELECT Song FROM Tracks;";
            SQLiteCommand command2 = new SQLiteCommand(selectSQL2, connection);
            SQLiteDataReader sQLiteDataReaderreader2 = command2.ExecuteReader();
            while (sQLiteDataReaderreader2.Read())
            {
                songsInDb.Add(sQLiteDataReaderreader2["Song"].ToString());
            }
            connection.Close();

            List<String> diff = Form1.instance.listDiff(songsInDb);

            if (!(diff.Count == 0))
            {
                foreach (var i in diff)
                {
                    addRow(i);
                }
                Form1.instance.populateSongList();
            }
        }

        public void synchInfo(ListBox listBox2, TextBox textBox1, TextBox textBox2, TextBox textBox3, TextBox textBox4)
        {
            connection.Open();
            String updateSQL = "UPDATE Tracks SET " +
                "Artist=@artist, " +
                "Year=@year, " +
                "Genre=@genre, " +
            "Language=@language" +
                " WHERE Song='" + listBox2.Items[listBox2.SelectedIndex].ToString() + "';";
            SQLiteCommand command = new SQLiteCommand(updateSQL, connection);
            command.Parameters.AddWithValue("artist", textBox1.Text);
            command.Parameters.AddWithValue("year", textBox2.Text);
            command.Parameters.AddWithValue("genre", textBox3.Text);
            command.Parameters.AddWithValue("language", textBox4.Text);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void showInfo(ListBox listBox2, TextBox textBox1, TextBox textBox2, TextBox textBox3, TextBox textBox4)
        {
            connection.Open();
            String selectSQL = "Select * From Tracks WHERE Song= '" + listBox2.Items[listBox2.SelectedIndex].ToString() + "';";
            SQLiteCommand command = new SQLiteCommand(selectSQL, connection);
            SQLiteDataReader sQLiteDataReaderreader = command.ExecuteReader();
            while (sQLiteDataReaderreader.Read())
            {
                textBox1.Text = sQLiteDataReaderreader["Artist"].ToString();
                textBox2.Text = sQLiteDataReaderreader["Year"].ToString();
                textBox3.Text = sQLiteDataReaderreader["Genre"].ToString();
                textBox4.Text = sQLiteDataReaderreader["Language"].ToString();
            }
            connection.Close();
        }
    }
}

