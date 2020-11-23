using System;
using System.IO;

namespace CC
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: Make file repository settings configurable on appSettings.json file
            //      Make file names configurable on settings file

            string articleFileName = string.Empty;
            string wordsFileName = string.Empty;

            Console.WriteLine("\n Scanning for article file on project file folder...");
            articleFileName = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + @"\Files\Input\Article.txt";


            if (File.Exists(articleFileName))
            {
                Console.WriteLine("\n Article File found on: " + articleFileName);


                Console.WriteLine("\n Scanning for words file on project file folder...");
                wordsFileName = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + @"\Files\Input\Words.txt";


                if (File.Exists(wordsFileName))
                {
                    Console.WriteLine("\n Words File found on: " + wordsFileName);

                }
                else
                {
                    wordsFileName = string.Empty;
                    Console.WriteLine("\n Words File NOT FOUND. Program will produce unique words to scan for.");
                }
                
                
                Console.WriteLine("\n Press key to start processing article file: ");
                Console.ReadKey();
                Console.WriteLine("\n ");

                Utilities.PrintWordCountsInFile(articleFileName, wordsFileName);
                

            }
            else
            {
                Console.WriteLine("\n Article File NOT FOUND: " + articleFileName);
                Console.WriteLine("\n Please make sure that article file is present. Press any key to close. ");
            }


        }



    }
}
