// <copyright file="StaticBibleVerses.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sword.reader;
using Sword.versification;

namespace Sword
{
    public class StaticBibleVerses
    {
        private static List<BiblePlaceMarker> markers = null;

        public static List<BiblePlaceMarker> Markers
        {
            get
            {
                if (markers == null)
                {
                    ReadTranslationsFromFile();
                }
                return markers;
            }
        }
        private static void ReadTranslationsFromFile()
        {
            markers = new List<BiblePlaceMarker>();
            var now = DateTime.Now;
            Assembly assem = Assembly.Load(new AssemblyName("Sword"));
            Stream stream = assem.GetManifestResourceStream(
                "Sword.Properties.verses.xml");
            var canon = CanonManager.GetCanon("KJV");
            if (stream != null)
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name.ToLower().Equals("verse") && reader.HasAttributes)
                                {
                                    int chapter = 0;
                                    int verse = 0; 
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        switch (reader.Name.ToLower())
                                        {
                                            case "chapter":
                                                chapter = int.Parse(reader.Value);
                                                break;
                                            case "verse":
                                                verse = int.Parse(reader.Value);
                                                break;
                                        }
                                    }
                                    while (reader.MoveToNextAttribute());
                                    var book = canon.GetBookFromAbsoluteChapter(chapter);
                                    markers.Add(new BiblePlaceMarker(book.ShortName1, chapter-book.VersesInChapterStartIndex, verse, now));
                                }

                                break;
                        }
                    }
                }
                stream.Dispose();
            }
        }
    }
}
