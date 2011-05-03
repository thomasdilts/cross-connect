using System;
using System.Net;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Collections.Generic;

namespace book.install
{
    public class BibleNames
    {
        private string[] shortNames = new string[BibleZtextReader.BOOKS_IN_BIBLE];
        private string[] fullNames = new string[BibleZtextReader.BOOKS_IN_BIBLE];

        public BibleNames(string isoLang2digitCode)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            string isocode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
            string name = CultureInfo.CurrentCulture.Name.Replace('-', '_');
            Stream stream = null;
            
            if (isocode.Equals(isoLang2digitCode))
            {
                //here we have the same language as the phone standard langauge
                //
                //Because none of the bibles have a culture code, but bible names have culturecodes, we
                //will attempt to get the culture code of the phones standard language.
                //try to get the culture code if it exists
                stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + name + ".xml");
            }

            if(stream==null)
            {
                stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + isoLang2digitCode + ".xml");
                if(stream==null)
                {
                    stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_en.xml");
                }
            }
            using (XmlReader reader = XmlReader.Create(stream))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    if (reader.NodeType==XmlNodeType.Element)
                    {
                        if (reader.Name.ToLower().Equals("book") && reader.HasAttributes)
                        {
                            string shortName="";
                            string fullName="";
                            int keyNumber=0;
                            reader.MoveToFirstAttribute();
                            do
                            {
                                switch (reader.Name.ToLower())
                                {
                                    case "keynumber":
                                        keyNumber = int.Parse(reader.Value);
                                        break;
                                    case "short":
                                        shortName = reader.Value;
                                        break;
                                    case "full":
                                        fullName = reader.Value;
                                        break;
                                }
                            } while (reader.MoveToNextAttribute());

                            shortNames[keyNumber]=shortName;
                            fullNames[keyNumber] = fullName;
                        }
                    }
                }
            }
            stream.Close();

        }
        public string getFullName(int bookNum)
        {
            return fullNames[bookNum];
        }
        public string getShortName(int bookNum)
        {
            return shortNames[bookNum];
        }
    }
}
