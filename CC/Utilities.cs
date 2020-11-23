using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CC
{
    public static class Utilities
    {
        /// <summary>
        /// Dictionary used to hold sentences
        /// </summary>
        private static IDictionary<int, string> sentences { get; set; }

        //TODO: Adjust regex expressions in the future to handle IP Addresses on sentences (if needed)

        /// <summary>
        /// Regular expression that identifies words 
        /// </summary>
        private static Regex _wordMatcher = new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*");

        /// <summary>
        /// Regular expression that is used to split sentences from paragraphs
        /// </summary>
        private static Regex _sentenceMatcher = new Regex(@"(?<!Mr?s?|\b[A-Z])\.\s*", RegexOptions.IgnoreCase);


        public static void PrintWordCountsInFile(string articleFileName, string wordFileName)
        {
            if (File.Exists(articleFileName))
            {
                var article = File.ReadAllText(articleFileName);
                sentences = SplitIntoSentences(article + " ", _sentenceMatcher);

                //Get all unique words from article  if Words File does not exist, output this list
                var words = _wordMatcher.Split(article.ToLower() + " ");

                // Dictionary with word count from article
                var countTable = CountWordOccurrences(words);

                //Final dictinoary for printing
                Dictionary<string, int> forPrinting = new Dictionary<string, int>(countTable);

                //If Words file exists
                if (!string.IsNullOrEmpty(wordFileName))
                {
                    if (File.Exists(wordFileName))
                    { 
                        var wordFile = File.ReadAllText(wordFileName);

                        if (wordFile.Length > 0)
                        {
                            string wordFileText = wordFile.Replace("\n", "").Replace("\r", " ");
                            words = _wordMatcher.Split(wordFileText.ToLower());
                        }

                        forPrinting.Clear();

                        //AddRemove items for printing based on the words listed on the Words file
                        foreach (var word in words)
                        {
                            //Add item on countTable if it exists on word but not on article
                            if (!countTable.ContainsKey(word))
                            {
                                countTable.Add(word, 0);
                            }
                        }

                        //ReCopy countTable to forPriting table
                        forPrinting = new Dictionary<string, int>(countTable);

                        foreach (var item in countTable)
                        {
                            //Remove items on countTable if it does not exists on the word file
                            if (!words.Contains(item.Key))
                            {
                                forPrinting.Remove(item.Key);
                            }
                        }
                    }

                }

                GenerateCountOutput(forPrinting, System.Console.Out);
            }
        }

        /// <summary>
        /// Splits a given text into sentences using regular expression
        /// </summary>
        /// <param name="article">Text to compare</param>
        /// <param name="expression">REgular expression to match a sentence</param>
        /// <returns>A dictionary of sentences</returns>
        public static IDictionary<int, string> SplitIntoSentences(string article, Regex expression)
        {
            var dc = new Dictionary<int, string>();

            var sentences = expression.Split(article);
            int counter = 1;

            foreach (var item in sentences)
            {
                dc.Add(counter++, item.ToLower());
            }

            return dc;
        }


        /// <summary>
        /// Returns a comma delimited string that indicates which sentence line number did a word occured
        /// </summary>
        /// <param name="sentences">Dictionary of sentences</param>
        /// <param name="word">Word to search for</param>
        /// <returns>Comma delimted string that indicates location of the word occurence</returns>
        public static string FindWordInSentences(IDictionary<int, string> sentences, string word)
        {
            string retval = string.Empty;
            int currSentence = 1;

            foreach (var item in sentences)
            {
                var words = _wordMatcher.Split(item.Value);  
                foreach (var i in words)
                {
                    if (i.Equals(word.TrimEnd(new char[] { ',', '.', ' ' })))
                    {
                        if (retval.Trim() != "")
                        {
                            retval += "," + currSentence;
                        }
                        else
                        {
                            retval += "" + currSentence;
                        }
                    }
                }

                currSentence++;
            }

            return retval;
        }

        /// <summary>
        /// Counts the occurance of a word throughout a given text.
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static IDictionary<string, int> CountWordOccurrences(IEnumerable<string> words)
        {
            return CountOccurrences(words, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Counts the occurence of a word from a given set of Dictionary
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="items">Items to compare from</param>
        /// <param name="comparer">word to compare</param>
        /// <returns>A dictionary with words and total counts</returns>
        public static IDictionary<T, int> CountOccurrences<T>(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            var counts = new Dictionary<T, int>(comparer);

            foreach (T t in items)
            {
                int count;
                if (!counts.TryGetValue(t, out count))
                {
                    count = 0;
                }
                counts[t] = count + 1;
            }

            return counts;
        }


        /// <summary>
        /// Writes out the output of the counts to the console and the output file
        /// </summary>
        /// <param name="counts">Dictionary of words with respective count</param>
        /// <param name="writer">Text writer</param>
        public static void GenerateCountOutput(IDictionary<string, int> counts, TextWriter writer)
        {

            string currentLetter = "";
            string toPrint = string.Empty;
            var lines = new List<string>();

            foreach (KeyValuePair<string, int> kvp in counts)
            {
                currentLetter = Utilities.GetNextAlphaLine(currentLetter);
                string s = FindWordInSentences(sentences, kvp.Key.ToString().ToLower());

                toPrint = currentLetter + ". " + kvp.Key.ToLower() + " {" + kvp.Value + ":" + s + "}";
                writer.WriteLine(toPrint);
                lines.Add(toPrint);
            }

            string docPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin")) + @"\Files\Output\Output.txt";
            CreateOutputFile(lines, docPath);

        }

        /// <summary>
        /// Creates an output file
        /// </summary>
        /// <param name="lines">List of lines to print on the output</param>
        private static void CreateOutputFile(List<string> lines, string docPath)
        {
            using (StreamWriter outputFile = new StreamWriter(docPath))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }



        #region ALPHABETLINE_GENERATOR

        /// <summary>
        /// Returns the next set of characters that will be used as line numbering for output file
        /// </summary>
        /// <param name="currentChar">The current character on the list</param>
        /// <returns>Next set of characters for line numbering</returns>
        public static string GetNextAlphaLine(string currentChar)
        {

            string retVal = string.Empty;
            
            if (currentChar.Trim() != "")
            {
                if (currentChar.Length == 1)
                {
                    retVal = Base26Sequence().SkipWhile(x => x != currentChar).Skip(1).First();
                }
                else
                {
                    string lastChar = currentChar.Substring(currentChar.Length - 1);

                    string newVal = GetNextAlphaLine(lastChar);

                    if (lastChar != "z")
                    {

                        for (int i = 0; i < currentChar.Length; i++)
                        {
                            retVal += newVal;
                        }
                    }
                    else
                    {

                        for (int i = 0; i < currentChar.Length + 1; i++)
                        {
                            retVal += newVal.Substring(newVal.Length - 1);
                        }

                    }

                }
            }
            else
            {
                retVal = "a";
            }

            return retVal;
        }

        private static IEnumerable<string> Base26Sequence()
        {
            long i = 0L;
            while (true)
                yield return Base26Encode(i++);
        }

        private static char[] base26Chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        private static string Base26Encode(Int64 value)
        {
            string returnValue = null;
            do
            {
                returnValue = base26Chars[value % 26] + returnValue;
                value /= 26;
            } while (value-- != 0);
            return returnValue;
        }

        #endregion



    }


}
