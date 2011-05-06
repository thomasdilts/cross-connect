using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System.IO.IsolatedStorage;
using System.IO;
using SwordBackend;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CrossConnect
{
    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
        }
        private const string LucenePath = "LuceneIndex";
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                MessageBoxResult result = MessageBox.Show("This first time you search, a search index must be created.  This can take a long time depending on how powerful your device is.  Do you want to continue?", "Search", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    App.windowSettings.skipWindowSettings = true;
                    NavigationService.GoBack();
                }

                BibleZtextReader source = (BibleZtextReader)App.openWindows[App.windowSettings.openWindowIndex].state.source;

                //SwordBook swBook = null;
                //foreach (var book in App.installedBibles.installedBibles)
                //{
                //    if (state.bibleToLoad.Equals(book.Value.sbmd.internalName))
                //    {
                //        swBook = book.Value;
                //        break;
                //    }
                //}

                //IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();

                //if (!root.DirectoryExists(source.path + "/" + LucenePath))
                //{
                //    root.CreateDirectory(source.path + "/" + LucenePath);
                //}

                //DirectoryInfo fi = new DirectoryInfo(source.path + "/" + LucenePath);
                //FSDirectory directory = FSDirectory.Open(fi);
                //Lucene.Net.Util.Version ver = new Lucene.Net.Util.Version("crossconnect",1);
                //Analyzer analyzer = new StandardAnalyzer(ver);
                //IndexWriter writer = new IndexWriter(directory, analyzer);

                //this could also work
                //IndexWriter writer = new IndexWriter("LuceneIndex", analyzer);

                Document doc = new Document();

                Regex regex=new Regex("Maria",RegexOptions.IgnoreCase);

                for (int i = 0; i < BibleZtextReader.CHAPTERS_IN_BIBLE; i++)
                {
                    string chapter = source.GetChapterRaw(i);
                    var match = regex.Matches(chapter);
                    if (match != null && match.Count > 0)
                    {
                    }
                }

                //writer.Optimize();
                ////Close the writer
                //writer.Commit();
                //writer.Close();

                //QueryParser parser = new QueryParser("postBody", analyzer);
                //Query query = parser.Parse("text");

                ////this could also work
                ////Query query = new TermQuery(new Term("postBody", "text"));

                ////Setup searcher
                //IndexSearcher searcher = new IndexSearcher(directory);
                ////Do the search
                //Hits hits = searcher.Search(query);

                ////iterate over the results.
                //int results = hits.Length();
                //Console.WriteLine("Found {0} results", results);
                //for (int i = 0; i < results; i++)
                //{
                //    Document doc2 = hits.Doc(i);
                //    float score = hits.Score(i);
                //    Debug.WriteLine("Result num {0}, score {1}", i + 1, score);
                //    Debug.WriteLine("ID: {0}", doc2.Get("id"));
                //    Debug.WriteLine("Text found: {0}" + Environment.NewLine, doc2.Get("postBody"));
                //}

                //searcher.Close();
                //directory.Close();
            }

        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void butSearch_Click(object sender, RoutedEventArgs e)
        {






            App.windowSettings.skipWindowSettings = true;
            NavigationService.GoBack();
        }
    }
}