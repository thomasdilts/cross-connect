using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Lucene.Net.Analysis
{
    /// <summary>
    /// Loads a text file and adds every line as an entry to a Hashtable. Every line
    /// should contain only one word. If the file is not found or on any error, an
    /// empty table is returned.
    /// </summary>
    public class WordlistLoaderForAnalyzer
    {
        /// <summary>
        /// Load words table from the file
        /// </summary>
        /// <param name="path">Path to the wordlist</param>
        /// <param name="wordfile">Name of the wordlist</param>
        /// <returns></returns>
        public static Dictionary<object,object> GetWordtable(String path, String wordfile)
        {
            if (path == null || wordfile == null)
            {
                return new Dictionary<object, object>();
            }
            return GetWordtable(new FileInfo(path + "\\" + wordfile));
        }

        /// <summary>
        /// Load words table from the file
        /// </summary>
        /// <param name="wordfile">Complete path to the wordlist</param>
        /// <returns></returns>
        public static Dictionary<object, object> GetWordtable(String wordfile)
        {
            if (wordfile == null)
            {
                return new Dictionary<object, object>();
            }
            return GetWordtable(new FileInfo(wordfile));
        }

        /// <summary>
        /// Load words table from the file 
        /// </summary>
        /// <param name="wordfile">File containing the wordlist</param>
        /// <returns></returns>
        public static Dictionary<object, object> GetWordtable(FileInfo wordfile)
        {
            if (wordfile == null)
            {
                return new Dictionary<object, object>();
            }
            StreamReader lnr = new StreamReader(wordfile.FullName);
            return GetWordtable(lnr);
        }

        /// <summary>
        /// Reads lines from a Reader and adds every line as an entry to a HashSet (omitting
        /// leading and trailing whitespace). Every line of the Reader should contain only
        /// one word. The words need to be in lowercase if you make use of an
        /// Analyzer which uses LowerCaseFilter (like StandardAnalyzer).
        /// </summary>
        /// <param name="reader">Reader containing the wordlist</param>
        /// <returns>A Hashtable with the reader's words</returns>
        public static Dictionary<object, object> GetWordtable(TextReader reader)
        {
            Dictionary<object,object> result = new Dictionary<object, object>();
            try
            {
                List<object> stopWords = new List<object>();
                String word = null;
                while ((word = reader.ReadLine()) != null)
                {
                    stopWords.Add(word.Trim());
                }
                result = MakeWordTable((String[])stopWords.ToArray(), stopWords.Count);
            }
            // On error, use an empty table
            catch (IOException)
            {
                result = new Dictionary<object, object>();
            }
            return result;
        }


        /// <summary>
        /// Builds the wordlist table.
        /// </summary>
        /// <param name="words">Word that where read</param>
        /// <param name="length">Amount of words that where read into <tt>words</tt></param>
        /// <returns></returns>
        private static Dictionary<object, object> MakeWordTable(String[] words, int length)
        {
            Dictionary<object, object> table = new Dictionary<object, object>(length);
            for (int i = 0; i < length; i++)
            {
                table.Add(words[i], words[i]);
            }
            return table;
        }
    }
}
