// For this project, the home path for the songs that are stored is the ~\path\to\Music_Player\tracks

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;


namespace Music_Player
{

    public partial class Form1 : Form
    {
        public static Form1 instance;
        CustomPlayer customPlayer = new CustomPlayer();

        public Form1()
        {
            InitializeComponent();
            instance = this;

        }



        private void Form1_Load(object sender, EventArgs e)
        {
            customPlayer.connection = new SQLiteConnection(customPlayer.connectionString);
            customPlayer.synchWithDbSongs();
            populateSongList();
            populateFavouritesList();
            listBox2.SelectedIndex = 0;
        }

        #region Helper Methods

        void populateFavouritesList()
        {
            customPlayer.connection = new SQLiteConnection(customPlayer.connectionString);
            customPlayer.connection.Open();
            String selectSQL = "Select Song From Tracks WHERE Frequency > 0 ORDER BY Frequency DESC;";
            SQLiteCommand command = new SQLiteCommand(selectSQL, customPlayer.connection);
            SQLiteDataReader sQLiteDataReaderreader = command.ExecuteReader();
            listBox1.Items.Clear();
            while (sQLiteDataReaderreader.Read())
            {
                listBox1.Items.Add(sQLiteDataReaderreader["Song"].ToString());
            }
            customPlayer.connection.Close();
        }

        public void populateSongList()
        {
            listBox2.Items.Clear();
            foreach (var i in Directory.GetFiles("../../../tracks/"))
            {
                var song = Path.GetFileNameWithoutExtension(i);
                listBox2.Items.Add(song);
            }
        }

        public List<String> listDiff(List<String> dbSongReturn)
        {
            List<String> diff = new List<String>();

            foreach (var i in Directory.GetFiles("../../../tracks/"))
            {
                var song = Path.GetFileNameWithoutExtension(i);
                if (!dbSongReturn.Contains(song))
                {
                    diff.Add(song);
                }
            }

            return diff;
        }

        void clearTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        #endregion

        // Select song
        //private void button1_Click(object sender, EventArgs e)
        //{

        //}

        // Play
        private void button2_Click(object sender, EventArgs e)
        {
            // When a song is playing
            if (customPlayer.wplayer.controls.isAvailable["pause"])
            {
                customPlayer.wplayer.controls.pause();
                customPlayer.wplayer.URL = Path.GetFullPath(Directory.GetFiles("../../../tracks/")[listBox2.SelectedIndex]);
                customPlayer.playIncF();
            }
            // When a song finished playing
            else if (customPlayer.wplayer.controls.isAvailable["play"])
            {
                if (customPlayer.wplayer.URL != Path.GetFullPath(Directory.GetFiles("../../../tracks/")[listBox2.SelectedIndex]))
                {
                    customPlayer.wplayer.URL = Path.GetFullPath(Directory.GetFiles("../../../tracks/")[listBox2.SelectedIndex]);

                }
                customPlayer.playIncF();
            }
            // When a song is not selected
            else
            {
                customPlayer.wplayer.URL = Path.GetFullPath(Directory.GetFiles("../../../tracks/")[listBox2.SelectedIndex]);
                customPlayer.playIncF();
            }
        }

        // Stop
        private void button5_Click(object sender, EventArgs e)
        {
            customPlayer.wplayer.controls.stop();

        }

        // Previous
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == 0)
            {
                listBox2.SelectedIndex = listBox2.Items.Count - 1;
            }
            else
            {
                listBox2.SelectedIndex = listBox2.SelectedIndex - 1;
            }
            customPlayer.wplayer.URL = Path.GetFullPath(Directory.GetFiles("../../../tracks/")[listBox2.SelectedIndex]);
            customPlayer.playIncF();
        }

        // Next
        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == listBox2.Items.Count - 1)
            {
                listBox2.SelectedIndex = 0;
            }
            else
            {
                listBox2.SelectedIndex = listBox2.SelectedIndex + 1;
            }
            customPlayer.wplayer.URL = Path.GetFullPath(Directory.GetFiles("../../../tracks/")[listBox2.SelectedIndex]);
            customPlayer.playIncF();
        }

        // Play Random
        private void button6_Click(object sender, EventArgs e)
        {
            int size = listBox2.Items.Count;
            Random random = new Random();
            int index = random.Next(0, size);
            listBox2.SelectedIndex = index;
            customPlayer.wplayer.URL = Path.GetFullPath(Directory.GetFiles("../../../tracks/")[listBox2.SelectedIndex]);
            customPlayer.playIncF();
        }



        // Refresh Song list
        private void button1_Click_1(object sender, EventArgs e)
        {
            populateSongList();
            listBox2.SelectedIndex = 0;

        }

        // Refresh Favourites list  
        private void button7_Click(object sender, EventArgs e)
        {
            populateFavouritesList();
        }

        // Artist
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        // Year
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        // Genre
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        // Language
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        // Submit
        private void button8_Click(object sender, EventArgs e)
        {
            customPlayer.synchInfo(listBox2, textBox1, textBox2, textBox3, textBox4);
        }

        // Song double Click
        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            customPlayer.wplayer.URL = Path.GetFullPath(Directory.GetFiles("../../../tracks/")[listBox2.SelectedIndex]);
            customPlayer.playIncF();
        }

        // Songs
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearTextBoxes();
            customPlayer.showInfo(listBox2, textBox1, textBox2, textBox3, textBox4);
        }

        // Favourite double Click
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        // Favourites
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Add
        // Moves a file to ~\path\to\Music_Player\tracks
        private void button10_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select the song you want to add";
            ofd.Filter = "MP3 files (*.mp3)|*.mp3";
            var res = ofd.ShowDialog();
            if (res == DialogResult.OK && ofd.FileName.EndsWith(".mp3"))
            {
                File.Move(ofd.FileName, Path.GetFullPath("../../../tracks/") + Path.GetFileName(ofd.FileName));
                populateSongList();
                populateFavouritesList();
                customPlayer.synchWithDbSongs();
            }
        }

        // Delete
        // Deletes only from the ~\path\to\Music_Player\tracks
        private void button9_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select the song you want to delete";
            ofd.InitialDirectory = Path.GetFullPath("../../../tracks/");
            ofd.Filter = "MP3 files (*.mp3)|*.mp3";
            var res = ofd.ShowDialog();
            String[] f = Directory.GetFiles(Path.GetFullPath("../../../tracks/"));

            if (res == DialogResult.OK && ofd.FileName.EndsWith(".mp3") && f.Contains(ofd.FileName))
            {
                File.Delete(ofd.FileName);
                populateSongList();
                populateFavouritesList();
                listBox2.SelectedIndex = 0;
                customPlayer.synchWithDbSongs();
            }
        }
    }
}
