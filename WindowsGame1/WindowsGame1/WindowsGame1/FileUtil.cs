using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class FileUtil
    {
        public List<int> highscores;

        public FileUtil()
        {
        }

        public void readFile()
        {
            highscores = new List<int>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader("Content/highscores.txt");
            while ((line = file.ReadLine()) != null)
            {
                highscores.Add(Int32.Parse(line));
            }
            highscores.Sort();
            highscores.Reverse();
            file.Close();
        }

        public void saveFile()
        {
            highscores.Sort();
            highscores.Reverse();
            string line;
            System.IO.StreamWriter file = new System.IO.StreamWriter("Content/highscores.txt");
            foreach (int highscore in highscores)
            {
                line = "" + highscore;
                file.WriteLine(line);
            }
            file.Close();
        }
    }





}
