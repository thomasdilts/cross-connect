/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
/// </summary>
/// <copyright file="BibleZtextReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace SwordBackend
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using ComponentAce.Compression.Libs.zlib;
    using System.Diagnostics;

    [DataContract]
    public class BiblePlaceMarker
    {
        #region Fields

        [DataMember]
        public int chapterNum = 1;
        [DataMember]
        public string note = "";
        [DataMember]
        public int verseNum = 1;
        [DataMember]
        public DateTime when;

        #endregion Fields

        #region Constructors

        public BiblePlaceMarker(int chapterNum, int verseNum, DateTime when)
        {
            this.chapterNum = chapterNum;
            this.verseNum = verseNum;
            this.when = when;
        }

        #endregion Constructors

        #region Methods

        public static BiblePlaceMarker clone(BiblePlaceMarker toClone)
        {
            BiblePlaceMarker newMarker = new BiblePlaceMarker(toClone.chapterNum,toClone.verseNum,toClone.when);
            newMarker.note = toClone.note;
            return newMarker;
        }

        #endregion Methods
    }

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract]
    public class BibleZtextReader : IBrowserTextSource
    {
        #region Fields

        /// <summary> Constant for the number of books in the Bible  </summary>
        public const int BOOKS_IN_BIBLE = 66;
        public const int BOOKS_IN_NT = 27;
        public const int BOOKS_IN_OT = 39;

        /// <summary> Constant for the number of chapters in the Bible  </summary>
        public const int CHAPTERS_IN_BIBLE = 1189;

        /// <summary> Constant for the number of chapters in the NT  </summary>
        public const int CHAPTERS_IN_NT = 260;

        /// <summary> Constant for the number of chapters in the OT  </summary>
        public const int CHAPTERS_IN_OT = 929;

        ///    
        /// <summary>* The configuration directory </summary>
        ///     
        public const string DIR_CONF = "mods.d";

        ///    
        /// <summary>* The data directory </summary>
        ///     
        public const string DIR_DATA = "modules";

        ///    
        /// <summary>* Extension for config files </summary>
        ///     
        public const string EXTENSION_CONF = ".conf";

        ///    
        /// <summary>* Extension for data files </summary>
        ///     
        public const string EXTENSION_DATA = ".dat";

        ///    
        /// <summary>* Extension for index files </summary>
        ///     
        public const string EXTENSION_INDEX = ".idx";

        ///    
        /// <summary>* Index file extensions </summary>
        ///     
        public const string EXTENSION_VSS = ".vss";

        ///    
        /// <summary>* New testament data files </summary>
        ///     
        public const string FILE_NT = "nt";

        ///    
        /// <summary>* Old testament data files </summary>
        ///     
        public const string FILE_OT = "ot";

        /// <summary> Constant for the number of chapters in each book  </summary>
        public static readonly short[] CHAPTERS_IN_BOOK = { 50, 40, 27, 36, 34, 24, 21, 4, 31, 24, 22, 25, 29, 36, 10, 13, 10, 42, 150, 31, 12, 8, 66, 52, 5, 48, 12, 14, 3, 9, 1, 4, 7, 3, 3, 3, 2, 14, 4, 28, 16, 24, 21, 28, 16, 16, 13, 6, 6, 4, 4, 5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22 };
        public static readonly Dictionary<string, int> osisBibeNamesToAbsoluteChapterNum = new Dictionary<string, int>
        {
            {"gen",0 },
            {"exod",50 },
            {"lev",90 },
            {"num",117 },
            {"deut",153 },
            {"josh",187 },
            {"judg",211 },
            {"ruth",232 },
            {"1sam",236 },
            {"2sam",267 },
            {"1kgs",291 },
            {"2kgs",313 },
            {"1chr",338 },
            {"2chr",367 },
            {"ezra",403 },
            {"neh",413 },
            {"esth",426 },
            {"job",436 },
            {"ps",478 },
            {"prov",628 },
            {"eccl",659 },
            {"song",671 },
            {"isa",679 },
            {"jer",745 },
            {"lam",797 },
            {"ezek",802 },
            {"dan",850 },
            {"hos",862 },
            {"joel",876 },
            {"amos",879 },
            {"obad",888 },
            {"jonah",889 },
            {"mic",893 },
            {"nah",900 },
            {"hab",903 },
            {"zeph",906 },
            {"hag",909 },
            {"zech",911 },
            {"mal",925 },
            {"matt",929 },
            {"mark",957 },
            {"luke",973 },
            {"john",997 },
            {"acts",1018 },
            {"rom",1046 },
            {"1cor",1062 },
            {"2cor",1078 },
            {"gal",1091 },
            {"eph",1097 },
            {"phil",1103 },
            {"col",1107 },
            {"1thess",1111 },
            {"2thess",1116 },
            {"1tim",1119 },
            {"2tim",1125 },
            {"titus",1129 },
            {"phlm",1132 },
            {"heb",1133 },
            {"jas",1146 },
            {"1pet",1151 },
            {"2pet",1156 },
            {"1john",1159 },
            {"2john",1164 },
            {"3john",1165 },
            {"jude",1166 },
            {"rev",1167 },
        };

        //[DataMember] Tried it but it took 10 times longer then re-reading the mod-file
        public List<BookPos> bookPositions = new List<BookPos>();

        //[DataMember] Tried it but it took 10 times longer then re-reading the mod-file
        public List<ChapterPos> chapters = new List<ChapterPos>();
        [DataMember]
        public BibleZtextReaderSerialData serial = new BibleZtextReaderSerialData(false,"","",0,0);

        /// <summary> Constant for the number of verses in the Bible  </summary>
        internal const short VERSES_IN_BIBLE = 31102;

        /// <summary> Constant for the number of verses in each book  </summary>
        internal static readonly short[] VERSES_IN_BOOK = { 1533, 1213, 859, 1288, 959, 658, 618, 85, 810, 695, 816, 719, 942, 822, 280, 406, 167, 1070, 2461, 915, 222, 117, 1292, 1364, 154, 1273, 357, 197, 73, 146, 21, 48, 105, 47, 56, 53, 38, 211, 55, 1071, 678, 1151, 879, 1007, 433, 437, 257, 149, 155, 104, 95, 89, 47, 113, 83, 46, 25, 303, 108, 105, 61, 105, 13, 14, 25, 404 };

        /// <summary> Constant for the number of verses in each chapter  </summary>
        internal static readonly short[][] VERSES_IN_CHAPTER = { new short[] { 31, 25, 24, 26, 32, 22, 24, 22, 29, 32, 32, 20, 18, 24, 21, 16, 27, 33, 38, 18, 34, 24, 20, 67, 34, 35, 46, 22, 35, 43, 55, 32, 20, 31, 29, 43, 36, 30, 23, 23, 57, 38, 34, 34, 28, 34, 31, 22, 33, 26 }, new short[] { 22, 25, 22, 31, 23, 30, 25, 32, 35, 29, 10, 51, 22, 31, 27, 36, 16, 27, 25, 26, 36, 31, 33, 18, 40, 37, 21, 43, 46, 38, 18, 35, 23, 35, 35, 38, 29, 31, 43, 38 }, new short[] { 17, 16, 17, 35, 19, 30, 38, 36, 24, 20, 47, 8, 59, 57, 33, 34, 16, 30, 37, 27, 24, 33, 44, 23, 55, 46, 34 }, new short[] { 54, 34, 51, 49, 31, 27, 89, 26, 23, 36, 35, 16, 33, 45, 41, 50, 13, 32, 22, 29, 35, 41, 30, 25, 18, 65, 23, 31, 40, 16, 54, 42, 56, 29, 34, 13 }, new short[] { 46, 37, 29, 49, 33, 25, 26, 20, 29, 22, 32, 32, 18, 29, 23, 22, 20, 22, 21, 20, 23, 30, 25, 22, 19, 19, 26, 68, 29, 20, 30, 52, 29, 12 }, new short[] { 18, 24, 17, 24, 15, 27, 26, 35, 27, 43, 23, 24, 33, 15, 63, 10, 18, 28, 51, 9, 45, 34, 16, 33 }, new short[] { 36, 23, 31, 24, 31, 40, 25, 35, 57, 18, 40, 15, 25, 20, 20, 31, 13, 31, 30, 48, 25 }, new short[] { 22, 23, 18, 22 }, new short[] { 28, 36, 21, 22, 12, 21, 17, 22, 27, 27, 15, 25, 23, 52, 35, 23, 58, 30, 24, 42, 15, 23, 29, 22, 44, 25, 12, 25, 11, 31, 13 }, new short[] { 27, 32, 39, 12, 25, 23, 29, 18, 13, 19, 27, 31, 39, 33, 37, 23, 29, 33, 43, 26, 22, 51, 39, 25 }, new short[] { 53, 46, 28, 34, 18, 38, 51, 66, 28, 29, 43, 33, 34, 31, 34, 34, 24, 46, 21, 43, 29, 53 }, new short[] { 18, 25, 27, 44, 27, 33, 20, 29, 37, 36, 21, 21, 25, 29, 38, 20, 41, 37, 37, 21, 26, 20, 37, 20, 30 }, new short[] { 54, 55, 24, 43, 26, 81, 40, 40, 44, 14, 47, 40, 14, 17, 29, 43, 27, 17, 19, 8, 30, 19, 32, 31, 31, 32, 34, 21, 30 }, new short[] { 17, 18, 17, 22, 14, 42, 22, 18, 31, 19, 23, 16, 22, 15, 19, 14, 19, 34, 11, 37, 20, 12, 21, 27, 28, 23, 9, 27, 36, 27, 21, 33, 25, 33, 27, 23 }, new short[] { 11, 70, 13, 24, 17, 22, 28, 36, 15, 44 }, new short[] { 11, 20, 32, 23, 19, 19, 73, 18, 38, 39, 36, 47, 31 }, new short[] { 22, 23, 15, 17, 14, 14, 10, 17, 32, 3 }, new short[] { 22, 13, 26, 21, 27, 30, 21, 22, 35, 22, 20, 25, 28, 22, 35, 22, 16, 21, 29, 29, 34, 30, 17, 25, 6, 14, 23, 28, 25, 31, 40, 22, 33, 37, 16, 33, 24, 41, 30, 24, 34, 17 }, new short[] { 6, 12, 8, 8, 12, 10, 17, 9, 20, 18, 7, 8, 6, 7, 5, 11, 15, 50, 14, 9, 13, 31, 6, 10, 22, 12, 14, 9, 11, 12, 24, 11, 22, 22, 28, 12, 40, 22, 13, 17, 13, 11, 5, 26, 17, 11, 9, 14, 20, 23, 19, 9, 6, 7, 23, 13, 11, 11, 17, 12, 8, 12, 11, 10, 13, 20, 7, 35, 36, 5, 24, 20, 28, 23, 10, 12, 20, 72, 13, 19, 16, 8, 18, 12, 13, 17, 7, 18, 52, 17, 16, 15, 5, 23, 11, 13, 12, 9, 9, 5, 8, 28, 22, 35, 45, 48, 43, 13, 31, 7, 10, 10, 9, 8, 18, 19, 2, 29, 176, 7, 8, 9, 4, 8, 5, 6, 5, 6, 8, 8, 3, 18, 3, 3, 21, 26, 9, 8, 24, 13, 10, 7, 12, 15, 21, 10, 20, 14, 9, 6 }, new short[] { 33, 22, 35, 27, 23, 35, 27, 36, 18, 32, 31, 28, 25, 35, 33, 33, 28, 24, 29, 30, 31, 29, 35, 34, 28, 28, 27, 28, 27, 33, 31 }, new short[] { 18, 26, 22, 16, 20, 12, 29, 17, 18, 20, 10, 14 }, new short[] { 17, 17, 11, 16, 16, 13, 13, 14 }, new short[] { 31, 22, 26, 6, 30, 13, 25, 22, 21, 34, 16, 6, 22, 32, 9, 14, 14, 7, 25, 6, 17, 25, 18, 23, 12, 21, 13, 29, 24, 33, 9, 20, 24, 17, 10, 22, 38, 22, 8, 31, 29, 25, 28, 28, 25, 13, 15, 22, 26, 11, 23, 15, 12, 17, 13, 12, 21, 14, 21, 22, 11, 12, 19, 12, 25, 24 }, new short[] { 19, 37, 25, 31, 31, 30, 34, 22, 26, 25, 23, 17, 27, 22, 21, 21, 27, 23, 15, 18, 14, 30, 40, 10, 38, 24, 22, 17, 32, 24, 40, 44, 26, 22, 19, 32, 21, 28, 18, 16, 18, 22, 13, 30, 5, 28, 7, 47, 39, 46, 64, 34 }, new short[] { 22, 22, 66, 22, 22 }, new short[] { 28, 10, 27, 17, 17, 14, 27, 18, 11, 22, 25, 28, 23, 23, 8, 63, 24, 32, 14, 49, 32, 31, 49, 27, 17, 21, 36, 26, 21, 26, 18, 32, 33, 31, 15, 38, 28, 23, 29, 49, 26, 20, 27, 31, 25, 24, 23, 35 }, new short[] { 21, 49, 30, 37, 31, 28, 28, 27, 27, 21, 45, 13 }, new short[] { 11, 23, 5, 19, 15, 11, 16, 14, 17, 15, 12, 14, 16, 9 }, new short[] { 20, 32, 21 }, new short[] { 15, 16, 15, 13, 27, 14, 17, 14, 15 }, new short[] { 21 }, new short[] { 17, 10, 10, 11 }, new short[] { 16, 13, 12, 13, 15, 16, 20 }, new short[] { 15, 13, 19 }, new short[] { 17, 20, 19 }, new short[] { 18, 15, 20 }, new short[] { 15, 23 }, new short[] { 21, 13, 10, 14, 11, 15, 14, 23, 17, 12, 17, 14, 9, 21 }, new short[] { 14, 17, 18, 6 }, new short[] { 25, 23, 17, 25, 48, 34, 29, 34, 38, 42, 30, 50, 58, 36, 39, 28, 27, 35, 30, 34, 46, 46, 39, 51, 46, 75, 66, 20 }, new short[] { 45, 28, 35, 41, 43, 56, 37, 38, 50, 52, 33, 44, 37, 72, 47, 20 }, new short[] { 80, 52, 38, 44, 39, 49, 50, 56, 62, 42, 54, 59, 35, 35, 32, 31, 37, 43, 48, 47, 38, 71, 56, 53 }, new short[] { 51, 25, 36, 54, 47, 71, 53, 59, 41, 42, 57, 50, 38, 31, 27, 33, 26, 40, 42, 31, 25 }, new short[] { 26, 47, 26, 37, 42, 15, 60, 40, 43, 48, 30, 25, 52, 28, 41, 40, 34, 28, 41, 38, 40, 30, 35, 27, 27, 32, 44, 31 }, new short[] { 32, 29, 31, 25, 21, 23, 25, 39, 33, 21, 36, 21, 14, 23, 33, 27 }, new short[] { 31, 16, 23, 21, 13, 20, 40, 13, 27, 33, 34, 31, 13, 40, 58, 24 }, new short[] { 24, 17, 18, 18, 21, 18, 16, 24, 15, 18, 33, 21, 14 }, new short[] { 24, 21, 29, 31, 26, 18 }, new short[] { 23, 22, 21, 32, 33, 24 }, new short[] { 30, 30, 21, 23 }, new short[] { 29, 23, 25, 18 }, new short[] { 10, 20, 13, 18, 28 }, new short[] { 12, 17, 18 }, new short[] { 20, 15, 16, 16, 25, 21 }, new short[] { 18, 26, 17, 22 }, new short[] { 16, 15, 15 }, new short[] { 25 }, new short[] { 14, 18, 19, 16, 14, 20, 28, 13, 28, 39, 40, 29, 25 }, new short[] { 27, 26, 18, 17, 20 }, new short[] { 25, 25, 22, 19, 14 }, new short[] { 21, 22, 18 }, new short[] { 10, 29, 24, 21, 21 }, new short[] { 13 }, new short[] { 14 }, new short[] { 25 }, new short[] { 20, 29, 22, 11, 14, 17, 17, 13, 21, 11, 19, 17, 18, 20, 8, 21, 18, 24, 21, 15, 27, 21 } };

        /// <summary> Chapters divided into categories  </summary>
        protected static readonly int[] CHAPTER_CATEGORIES = 
        {
            1, 1, 1, 1, 1,
            2,2,2,2,2,2,2,2,2,2,2,2,
            3,3,3,3,3,
            4,4,4,4,4,
            5,5,5,5,5,5,5,5,5,5,5,5,
            6,6,6,6,
            7,
            8,8,8,8,8,8,8,8,8,8,8,8,8,
            9,9,9,9,9,9,9,9,
            10
        };
        protected static readonly short[] FIRST_CHAPTERNUM_IN_BOOK = { 0, 50, 90, 117, 153, 187, 211, 232, 236, 267, 291, 313, 338, 367, 403, 413, 426, 436, 478, 628, 659, 671, 679, 745, 797, 802, 850, 862, 876, 879, 888, 889, 893, 900, 903, 906, 909, 911, 925, 929, 957, 973, 997, 1018, 1046, 1062, 1078, 1091, 1097, 1103, 1107, 1111, 1116, 1119, 1125, 1129, 1132, 1133, 1146, 1151, 1156, 1159, 1164, 1165, 1166, 1167 };

        protected static byte[] prefix = System.Text.UTF8Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<versee>");
        protected static byte[] prefixIso = System.Text.UTF8Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n<versee>");
        protected static byte[] suffix = System.Text.UTF8Encoding.UTF8.GetBytes("\n</versee>");

        protected BibleNames bookNames = null;

        private const string colorWordsOfChrist = "#ff1439"; //deep pink
        private const long SKIP_BOOK_FLAG = 68;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Load from a file all the book and verse pointers to the bzz file so that
        /// we can later read the bzz file quickly and efficiently.
        /// </summary>
        /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
        public BibleZtextReader(string path, string iso2DigitLangCode,bool isIsoEncoding)
        {
            this.serial.iso2DigitLangCode = iso2DigitLangCode;
            this.serial.path = path;
            this.serial.isIsoEncoding = isIsoEncoding;
            ReloadSettingsFile();
            SetToFirstChapter();
        }

        #endregion Constructors

        #region Events

        public event WindowSourceChanged SourceChanged;

        #endregion Events

        #region Properties

        public bool existsShortNames
        {
            get
            {
                if (bookNames == null)
                {
                    bookNames = new BibleNames(serial.iso2DigitLangCode);
                }
                return bookNames.existsShortNames;
            }
        }

        public virtual bool IsExternalLink
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsLocalChangeDuringLink
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsPageable
        {
            get
            {
            return true;
             }
        }

        public virtual bool IsSearchable
        {
            get
            {
            return true;
             }
        }

        public virtual bool IsSynchronizeable
        {
            get
            {
            return true;
             }
        }

        #endregion Properties

        #region Methods

        public static bool convertOsisRefToAbsoluteChaptVerse(string osisRef, out int chaptNumLoc, out int verseNumLoc)
        {
            chaptNumLoc=0;
            verseNumLoc = 0;
            if (osisRef.Contains(":"))
            {
                // remove everythign before :
                osisRef = osisRef.Substring(osisRef.IndexOf(":") + 1);
            }
            if (osisRef.Contains("@"))
            {
                // remove everythign after @
                osisRef = osisRef.Substring(0, osisRef.IndexOf("@"));
            }
            if (osisRef.Contains("-"))
            {
                // remove everythign after -
                osisRef = osisRef.Substring(0, osisRef.IndexOf("-"));
            }
            string[] osis = osisRef.Split(".".ToCharArray());
            if (osis.Length > 0)
            {
                if(osisBibeNamesToAbsoluteChapterNum.ContainsKey(osis[0].ToLower()))
                {
                    chaptNumLoc = osisBibeNamesToAbsoluteChapterNum[osis[0].ToLower()];
                    if (osis.Length > 1)
                    {
                        int chapterRelative = 0;
                        int.TryParse(osis[1], out chapterRelative);
                        chapterRelative--;
                        if (chapterRelative < 0) chapterRelative = 0;
                        chaptNumLoc += chapterRelative;
                    }
                    if (osis.Length > 2)
                    {
                        int.TryParse(osis[2], out verseNumLoc);
                        verseNumLoc--;
                        if (verseNumLoc < 0) verseNumLoc = 0;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static string HtmlHeader(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor,string htmlPhoneAccentColor, double htmlFontSize)
        {
            var head = new StringBuilder();
            head.Append("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />");

            head.Append("<style>");

            head.Append(string.Format(
                "body {{background:{0};color:{1};font-size:{2}pt;margin:0;padding:0 }}",
                htmlBackgroundColor,
                htmlForegroundColor,
                (int)(htmlFontSize + 0.5))); //old fashioned way to round an integer

            head.Append(string.Format("sup,sub {{color:{0};font-size: .83em;}} " +
                "a.strongsmorph,a.strongsmorph:link,span.strongsmorph{{color:{1};text-decoration:none;}} " +
                "a.normalcolor,a.normalcolor:link {{color:{2};text-decoration:none;}}",
                displaySettings.highlightMarkings ? htmlPhoneAccentColor : htmlForegroundColor,
                displaySettings.highlightMarkings ? htmlPhoneAccentColor : htmlForegroundColor,
                htmlForegroundColor));

            head.Append(string.Format(
                " a.normalcolor:link span.christ {{ color: {1}; }}  a.normalcolor span.christ:visited {{ color: {3}; }}  a.normalcolor span.christ:hover {{ color: {2}; }} a.normalcolor:hover {{ color: {0}; }} ",
                htmlPhoneAccentColor,
                colorWordsOfChrist,
                htmlPhoneAccentColor,
                htmlPhoneAccentColor));

            head.Append("</style>");

            head.Append("</head><body>");
            return head.ToString();
        }

        public virtual ButtonWindowSpecs GetButtonWindowSpecs(int stage,int lastSelectedButton)
        {
            if (stage == 0)
            {
                List<int> colors=new List<int>();
                List<int> values = new List<int>();
                List<string> buttonNames = new List<string>();
                if (bookNames == null)
                {
                    bookNames = new BibleNames(serial.iso2DigitLangCode);
                }
                string[] buttonNamesStart = bookNames.getAllShortNames();
                if (!bookNames.existsShortNames)
                {
                    buttonNamesStart = bookNames.getAllFullNames();
                }
                //assumption. if the first chapter in the book does not exist then the book does not exist
                for (int i = 0; i < BOOKS_IN_BIBLE; i++)
                {
                    if(chapters[FIRST_CHAPTERNUM_IN_BOOK[i]].length!=0)
                    {
                        colors.Add(CHAPTER_CATEGORIES[i]);
                        values.Add(FIRST_CHAPTERNUM_IN_BOOK[i]);
                        buttonNames.Add(buttonNamesStart[i]);
                    }
                }
                return new ButtonWindowSpecs(
                    stage,
                    "Select book",
                    colors.Count,
                    colors.ToArray(),
                    buttonNames.ToArray(),
                    values.ToArray(),
                    !bookNames.existsShortNames?ButtonSize.LARGE:ButtonSize.MEDIUM);
            }
            else
            {
                int booknum = 0;
                for (int i = 0; i < BOOKS_IN_BIBLE; i++)
                {
                    if (lastSelectedButton==FIRST_CHAPTERNUM_IN_BOOK[i])
                    {
                        booknum = i;
                        break;
                    }
                }

                // set up the array for the chapter selection
                int numOfChapters = SwordBackend.BibleZtextReader.CHAPTERS_IN_BOOK[booknum];

                if (numOfChapters <= 1)
                {
                    return null;
                }
                //Color butColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
                int[] butColors = new int[numOfChapters];
                int[] values = new int[numOfChapters];
                string[] butText = new string[numOfChapters];
                for (int i = 0; i < numOfChapters; i++)
                {
                    butColors[i] = 0;
                    butText[i] = (i + 1).ToString();
                    values[i] = i;
                }

                // do a nice transition

                return new ButtonWindowSpecs(
                    stage,
                    "Select chapter",
                    numOfChapters,
                    butColors,
                    butText,
                    values,
                    ButtonSize.SMALL);
            }
        }

        /// <summary>
        /// Return the entire chapter
        /// </summary>
        /// <param name="bookNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <returns>Entire Chapter</returns>
        public string GetChapterRaw(int chapterNumber)
        {
            byte[] chapterBuffer=getChapterBytes(chapterNumber);
            string retValue = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            return retValue;
        }

        public virtual string getExternalLink(DisplaySettings displaySettings)
        {
            return "";
        }

        public string getFullName(int bookNum)
        {
            if (bookNames == null)
            {
                bookNames = new BibleNames(serial.iso2DigitLangCode);
            }
            return bookNames.getFullName(bookNum);
        }

        public virtual void GetInfo( out int bookNum,out int absoluteChaptNum, out int relChaptNum,out int verseNum, out string fullName, out string title)
        {
            verseNum = serial.posVerseNum;
            absoluteChaptNum=serial.posChaptNum;
            GetInfo(serial.posChaptNum, serial.posVerseNum, out bookNum, out relChaptNum, out fullName, out title);
        }

        public void GetInfo(int chapterNum,int verseNum, out int bookNum, out int relChaptNum, out string fullName, out string title)
        {
            ChapterPos chaptPos = null;
            bookNum = 0;
            relChaptNum = 0;
            bookNum =0;
            relChaptNum = 0;
            fullName = "";
            title = "";
            try
            {
                chaptPos = chapters[chapterNum];
                bookNum = chaptPos.booknum;
                relChaptNum = chaptPos.bookRelativeChapterNum;
                bookNum = chaptPos.booknum;
                relChaptNum = chaptPos.bookRelativeChapterNum;
                fullName = getFullName(bookNum);
                title = fullName + " " + (relChaptNum + 1) + ":" + (verseNum + 1);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("BibleZtextReader.GetInfo; " + ee.Message);
            }
        }

        public string getShortName(int bookNum)
        {
            if (bookNames == null)
            {
                bookNames = new BibleNames(serial.iso2DigitLangCode);
            }
            return bookNames.getShortName(bookNum);
        }

        /// <summary>
        /// Return a raw verse.  This is a very ineffective function so use it with caution.
        /// </summary>
        /// <param name="chapterNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <param name="verseNumber">Verse number beginning with zero for the first verse</param>
        /// <returns>Verse raw</returns>   
        public virtual string GetVerseTextOnly(DisplaySettings displaySettings, int chapterNumber, int verseNumber)
        {
            byte[] chapterBuffer = getChapterBytes(chapterNumber);
            string chapter = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            //debug only
            //string all = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            BibleZtextReader.VersePos verse = chapters[chapterNumber].verses[verseNumber];
            int noteMarker = 'a';
            return parseOsisText(displaySettings,
                "",
                "",
                chapterBuffer,
                (int)verse.startPos,
                verse.length,
                serial.isIsoEncoding,
                false,
                true,
                ref noteMarker,
                true);
        }

        public List<string> MakeListDisplayText(DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay)
        {
            List<string> returnList = new List<string>();

            for (int j = listToDisplay.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = listToDisplay[j];
                ChapterPos chaptPos = chapters[place.chapterNum];
                byte[] chapterBuffer = getChapterBytes(place.chapterNum);

                ChapterPos versesForChapterPositions = chapters[place.chapterNum];

                VersePos verse = versesForChapterPositions.verses[place.verseNum];
                int noteMarker = 'a';
                string verseTxt = parseOsisText(displaySettings,
                    this.getFullName(chaptPos.booknum) + " " +
                    (chaptPos.bookRelativeChapterNum + 1) + ":" +
                    (place.verseNum + 1) + "  " +
                    place.when.ToShortDateString() + " " + place.when.ToShortTimeString() + "---",
                    "",
                    chapterBuffer,
                    (int)verse.startPos,
                    verse.length,
                    this.serial.isIsoEncoding,
                    false,
                    true,
                    ref noteMarker);
                returnList.Add(verseTxt);
            }
            return returnList;
        }

        public virtual void moveChapterVerse(int chapter, int verse, bool isLocalLinkChange)
        {
            //see if the chapter exists, if not, then don't do anything.
            if (chapters != null && chapter < chapters.Count && chapters[chapter].length > 0)
            {
                serial.posChaptNum = chapter;
                serial.posVerseNum = verse;
            }
        }

        public virtual void moveNext()
        {
            serial.posChaptNum++;
            if (serial.posChaptNum >= chapters.Count)
            {
                serial.posChaptNum = 0;
            }
            for (; serial.posChaptNum < chapters.Count && chapters[serial.posChaptNum].length == 0; serial.posChaptNum++)
            {
            }
            if (serial.posChaptNum >= chapters.Count)
            {
                serial.posChaptNum = 0;
            }
            else
            {
                return;
            }
            for (; serial.posChaptNum < chapters.Count && chapters[serial.posChaptNum].length == 0; serial.posChaptNum++)
            {
            }
        }

        public virtual void movePrevious()
        {
            serial.posChaptNum--;
            if (serial.posChaptNum < 0)
            {
                serial.posChaptNum = chapters.Count - 1;
            }
            for (; serial.posChaptNum >= 0 && chapters[serial.posChaptNum].length == 0; serial.posChaptNum--)
            {
            }
            if (serial.posChaptNum < 0)
            {
                serial.posChaptNum = chapters.Count - 1;
            }
            else
            {
                return;
            }
            for (; serial.posChaptNum >= 0 && chapters[serial.posChaptNum].length == 0; serial.posChaptNum--)
            {
            }
        }

        public virtual string putHtmlTofile(DisplaySettings displaySettings, string htmlBackgroundColor, 
            string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize,
            string fileErase, string filePath)
        {
            Debug.WriteLine("putHtmlTofile start");

            var root = IsolatedStorageFile.GetUserStoreForApplication();

            if (root.FileExists( fileErase))
            {
                try
                {
                    root.DeleteFile(fileErase);
                }
                catch (Exception ee)
                {
                    //should never crash here but I have noticed any file delete is a risky business when you have more then one thread.
                    Debug.WriteLine("BibleZtextReader.putHtmlTofile; " + ee.Message);
                }
            }

            // Must change the file name, otherwise the browser may or may not update.
            string fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";

            //the name must be unique of course
            while (root.FileExists(filePath + "/" + fileCreate))
            {
                fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";
            }

            IsolatedStorageFileStream fs = root.CreateFile(filePath + "/" + fileCreate);
            System.IO.StreamWriter tw = new System.IO.StreamWriter(fs);
            tw.Write(GetChapterHtml(displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlFontSize,
                false));
            tw.Close();
            fs.Close();
            
            Debug.WriteLine("putHtmlTofile end");
            return fileCreate;
        }

        public void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod,bool isRegister=true)
        {
            if (isRegister)
            {
                SourceChanged += sourceChangedMethod;
            }
            else
            {
                SourceChanged -= sourceChangedMethod;
            }
        }

        public virtual void Resume()
        {
            ReloadSettingsFile();
        }

        public virtual void SerialSave()
        {
        }

        public virtual void SetToFirstChapter()
        {
            // find the first available chapter.
            this.serial.posChaptNum = 0;
            moveNext();
            if (this.serial.posChaptNum == 1)
            {
                movePrevious();
                if (this.serial.posChaptNum != 0)
                {
                    moveNext();
                }
            }
        }

        protected static void appendText(string text,StringBuilder plainText,StringBuilder noteText, bool isInElement)
        {
            if (!isInElement)
            {
                plainText.Append(text);
            }
            else
            {
                noteText.Append(text);
            }
        }

        protected string[] getAllShortNames()
        {
            if (bookNames == null)
            {
                bookNames = new BibleNames(serial.iso2DigitLangCode);
            }
            return bookNames.getAllShortNames();
        }

        protected int getByteFromStream(FileStream fs)
        {
            byte[] buf = new byte[1];
            fs.Read(buf, 0, 1);
            return  buf[0];
        }

        protected byte[] getChapterBytes(int chapterNumber)
        {
            Debug.WriteLine("getChapterBytes start");
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            ChapterPos versesForChapterPositions = chapters[chapterNumber];
            BookPos bookPos = bookPositions[versesForChapterPositions.booknum];

            FileStream fs = null;
            if (chapterNumber < CHAPTERS_IN_OT)
            {
                try
                {
                    fs = fileStorage.OpenFile(serial.path + "ot.bzz", FileMode.Open, FileAccess.Read);
                }
                catch (Exception)
                {
                    // does not exist
                    return System.Text.UTF8Encoding.UTF8.GetBytes("Does not exist");
                }
            }
            else
            {
                try
                {
                    fs = fileStorage.OpenFile(serial.path + "nt.bzz", FileMode.Open, FileAccess.Read);
                }
                catch (Exception)
                {
                    // does not exist
                    return System.Text.UTF8Encoding.UTF8.GetBytes("Does not exist");
                }
            }

            // adjust the start postion of the stream to where this book begins.
            // we must read the entire book up to the chapter we want even though we just want one chapter.
            fs.Position = bookPos.startPos;
            ZInputStream zipStream = new ZInputStream(fs);
            byte[] chapterBuffer = new byte[versesForChapterPositions.length];
            int totalBytesRead = 0;
            int totalBytesCopied = 0;
            int len = 0;

            try
            {
                byte[] buffer = new byte[10000];

                while (true)
                {
                    try
                    {
                        len = zipStream.read(buffer, 0, 10000);
                    }
                    catch (Exception ee)
                    {
                        Debug.WriteLine("caught a unzip crash 4.2" + ee.ToString());
                    }
                    if (len <= 0)
                    {
                        // we should never come to this point.  Just here as a safety procaution
                        break;
                    }
                    else
                    {
                        totalBytesRead += len;
                        if (totalBytesRead >= versesForChapterPositions.startPos)
                        {
                            // we are now inside of where the chapter we want is so we need to start saving it.
                            int startOffset = 0;
                            if (totalBytesCopied == 0)
                            {
                                // but our actual chapter might begin in the middle of the buffer.  Find the offset from the
                                // beginning of the buffer.
                                startOffset = len - (totalBytesRead - (int)versesForChapterPositions.startPos);
                            }
                            int i;
                            for (i = totalBytesCopied; i < versesForChapterPositions.length && (i - totalBytesCopied) < (len - startOffset); i++)
                            {
                                chapterBuffer[i] = buffer[i - totalBytesCopied + startOffset];
                            }
                            totalBytesCopied += (len - startOffset);
                            if (totalBytesCopied >= versesForChapterPositions.length)
                            {
                                // we are done. no more reason to read anymore of this book stream, just get out.
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("BibleZtextReader.getChapterBytes crash; " + ee.Message);
            }
            fs.Close();
            zipStream.Close();
            return chapterBuffer;
        }

        /// <summary>
        /// Return the entire chapter without notes and with lots of html markup
        /// </summary>
        /// <param name="bookNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <param name="htmlBackgroundColor"></param>
        /// <param name="htmlForegroundColor"></param>
        /// <param name="htmlFontSize"></param>
        /// <param name="htmlPhoneAccentColor"></param>
        /// <returns>Entire Chapter without notes and with lots of html markup for each verse</returns>        
        protected string GetChapterHtml(DisplaySettings displaySettings, int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            Debug.WriteLine("GetChapterHtml start");
            byte[] chapterBuffer = getChapterBytes(chapterNumber);
            // for debug
            // System.Diagnostics.Debug.WriteLine("RawChapter: " + System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length));
            StringBuilder htmlChapter = new StringBuilder();
            ChapterPos versesForChapterPositions = chapters[chapterNumber];
            string chapterStartHtml = "";
            string chapterEndHtml = "";
            if (addStartFinishHtml)
            {
                chapterStartHtml = HtmlHeader(displaySettings, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
                chapterEndHtml = "</body></html>";
            }
            string bookName="";
            if(displaySettings.showBookName)
            {
                bookName=this.getFullName(versesForChapterPositions.booknum);
            }
            bool isVerseMarking = displaySettings.showBookName || displaySettings.showChapterNumber || displaySettings.showVerseNumber;
            string startVerseMarking=(displaySettings.smallVerseNumbers?"<sup>":(isVerseMarking?"<span class=\"strongsmorph\">(":""));
            string stopVerseMarking = (displaySettings.smallVerseNumbers ? "</sup>" : (isVerseMarking ? ")</span>" : ""));
            int noteIdentifier = (int)'a';
            for (int i = 0; i < versesForChapterPositions.verses.Count; i++)
            {
                string htmlChapterText =
                        startVerseMarking +
                        (displaySettings.showBookName ? bookName + " " : "") +
                        (displaySettings.showChapterNumber ? ((versesForChapterPositions.bookRelativeChapterNum + 1) + ":") : "") +
                        (displaySettings.showVerseNumber ? (i + 1).ToString() : "") +
                        stopVerseMarking;

                string id = "CHAP_" + chapterNumber + "_VERS_" + i;
                VersePos verse = versesForChapterPositions.verses[i];
                string restartText = "<a name=\"" + id
                    + "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.Notify('" + id
                    + "'); event.returnValue=false; return false;\" > ";
                string startText = htmlChapterText + restartText;
                string verseTxt = "*** ERROR ***";

                try
                {
                    verseTxt = parseOsisText(displaySettings,
                    startText,
                    restartText,
                    chapterBuffer,
                    (int)verse.startPos,
                    verse.length,
                    this.serial.isIsoEncoding,
                    isNotesOnly,
                    false,
                    ref noteIdentifier);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(verse.length + ";" + verse.startPos + ";" + e.ToString());
                }
                // create the verse
                htmlChapter.Append(
                    (displaySettings.eachVerseNewLine ? "<p>" : "") +
                    chapterStartHtml
                    + verseTxt
                    + (verseTxt.Length > 0 ? (displaySettings.eachVerseNewLine ? "</a></p>" : "</a>") : ""));
                chapterStartHtml = string.Empty;
            }
            htmlChapter.Append(chapterEndHtml);
            Debug.WriteLine("GetChapterHtml end");
            return htmlChapter.ToString();
        }

        protected virtual string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize,bool isNotesOnly, bool addStartFinishHtml=true)
        {
            return GetChapterHtml(displaySettings, serial.posChaptNum, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, isNotesOnly, addStartFinishHtml );
        }

        protected long getInt48FromStream(FileStream fs)
        {
            byte[] buf = new byte[7];
            fs.Read(buf, 0, 7);
            return buf[2] * 0x100000000000000 + buf[1] * 0x100000000000 + buf[0] * 0x100000000 + buf[6] * 0x1000000 + buf[5] * 0x10000 + buf[4] * 0x100 + buf[3];
        }

        protected long getintFromStream(FileStream fs)
        {
            byte[] buf = new byte[4];
            fs.Read(buf, 0, 4);
            return buf[3] * 0x100000 + buf[2] * 0x10000 + buf[1] * 0x100 + buf[0];
        }

        protected int getShortIntFromStream(FileStream fs)
        {
            byte[] buf = new byte[2];
            fs.Read(buf, 0, 2);
            return buf[1] * 0x100 + buf[0];
        }

        protected string MakeListDisplayText(DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, bool showBookTitles, string notesTitle)
        {
            if (htmlBackgroundColor.Length == 0)
            {
                // must wait a little untill we get these values.
                return "";
            }

            string chapterStartHtml = HtmlHeader(displaySettings, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
            string chapterEndHtml = "</body></html>";
            StringBuilder htmlListText = new StringBuilder(chapterStartHtml);
            int lastBookNum = -1;
            bool isVerseMarking = displaySettings.showBookName || displaySettings.showChapterNumber || displaySettings.showVerseNumber;
            string startVerseMarking = (displaySettings.smallVerseNumbers ? "<sup>" : (isVerseMarking ? "(" : ""));
            string stopVerseMarking = (displaySettings.smallVerseNumbers ? "</sup>" : (isVerseMarking ? ")" : ""));

            for (int j = listToDisplay.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = listToDisplay[j];
                ChapterPos chaptPos= chapters[place.chapterNum];
                byte[] chapterBuffer = getChapterBytes(place.chapterNum);
                // for debug
                //string all = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);

                ChapterPos versesForChapterPositions = chapters[place.chapterNum];

                if (showBookTitles && lastBookNum != chaptPos.booknum)
                {
                    htmlListText.Append("<h3>" + this.getFullName(chaptPos.booknum) + "</h3>");
                    lastBookNum = chaptPos.booknum;
                }

                string htmlChapterText = startVerseMarking + this.getFullName(chaptPos.booknum) + " " +
                    (chaptPos.bookRelativeChapterNum + 1) + ":" +
                    (place.verseNum + 1) + "  " +
                    place.when.ToShortDateString() + " " + place.when.ToShortTimeString() + stopVerseMarking;

                string textId = "CHAP_" + place.chapterNum + "_VERS_" + place.verseNum;
                VersePos verse = versesForChapterPositions.verses[place.verseNum];
                int noteMarker = 'a';
                string verseTxt = parseOsisText(displaySettings, "<p><a name=\"" + textId +
                    "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.Notify('" +
                    textId + "'); event.returnValue=false; return false;\" >" + htmlChapterText,
                    "",
                    chapterBuffer,
                    (int)verse.startPos,
                    verse.length,
                    this.serial.isIsoEncoding,
                    false,
                    true,
                    ref noteMarker);

                // create the verse
                if (string.IsNullOrEmpty(place.note))
                {
                    htmlListText.Append(verseTxt + "</a></p><hr />");
                }
                else
                {
                    htmlListText.Append(verseTxt + "</p><p>" + (displaySettings.smallVerseNumbers ? "<sup>" : "(") + notesTitle + (displaySettings.smallVerseNumbers ? "</sup>" : ") ") + place.note + "</a></p><hr />");
                }
            }
            htmlListText.Append(chapterEndHtml);
            return htmlListText.ToString();
        }

        protected string parseOsisText(DisplaySettings displaySettings, string chapterNumber,string restartText, byte[] xmlbytes, int startPos, int length, bool isIsoText,bool isNotesOnly, bool noTitles,ref int noteIdentifier, bool isRaw=false)
        {
            MemoryStream ms = new MemoryStream();
            if(isIsoText)
            {
                ms.Write(prefixIso, 0, prefixIso.Length);
            }
            else
            {
                ms.Write(prefix, 0, prefix.Length);
            }
            //Some indexes are bad. make sure the startpos and length are not bad
            if (length == 0)
            {
                return "";
            }
            if (startPos >= xmlbytes.Length)
            {
                Debug.WriteLine("Bad startpos;" + xmlbytes.Length + ";" + startPos + ";" + length);
                return "*** POSSIBLE ERROR IN BIBLE, TEXT MISSING HERE ***";
            }
            if (startPos + length > xmlbytes.Length)
            {
                //we can fix this
                Debug.WriteLine("Fixed bad length;" + xmlbytes.Length + ";" + startPos + ";" + length);
                length = xmlbytes.Length - startPos - 1;
                if (length == 0)
                {
                    //this might be a problem or it might not. Put some stars here anyway.
                    return "***";
                }
            }
            ms.Write(xmlbytes, startPos, length);
            ms.Write(suffix, 0, suffix.Length);
            ms.Position = 0;
            StringBuilder plainText = new StringBuilder();
            StringBuilder noteText = new StringBuilder();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace=false;

            bool isInElement = false;
            bool isInQuote = false;
            bool isInInjectionElement = false;
            bool isInTitle = false;
            bool isChaptNumGiven = false;
            bool isChaptNumGivenNotes = false;
            bool isReferenceLinked = false;
            string lemmaText = "";
            string morphText = "";
            using (XmlReader reader = XmlReader.Create(ms, settings))
            {
                try
                {
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {

                                    case "CM":
                                        if (!isRaw && !displaySettings.eachVerseNewLine)
                                        {
                                            appendText("<br />", plainText, noteText, isInElement);
                                        }
                                        break;
                                    case "lb":
                                        if (reader.HasAttributes && !isRaw && !displaySettings.eachVerseNewLine)
                                        {
                                            reader.MoveToFirstAttribute();
                                            if (reader.Name.Equals("type"))
                                            {
                                                if (reader.Value.Equals("x-end-paragraph"))
                                                {
                                                    appendText("</p>", plainText, noteText, isInElement);
                                                }
                                                else
                                                {
                                                    appendText("<p>", plainText, noteText, isInElement);
                                                }
                                            }
                                        }
                                        break;
                                    case "title":
                                        isInTitle = true;
                                        if (!(noTitles || !displaySettings.showHeadings) && !isRaw)
                                        {
                                            appendText("<h3>", plainText, noteText, isInElement);
                                        }
                                        break;
                                    case "reference":
                                        if (reader.HasAttributes)
                                        {
                                            reader.MoveToFirstAttribute();
                                            if (reader.Name.Equals("osisRef"))
                                            {
                                                int chaptNumLoc = 0;
                                                int verseNumLoc = 0;
                                                if (convertOsisRefToAbsoluteChaptVerse(reader.Value, out chaptNumLoc, out verseNumLoc))
                                                {
                                                    isReferenceLinked = true;
                                                    string textId = "CHAP_" + chaptNumLoc + "_VERS_" + verseNumLoc;
                                                    noteText.Append("</a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.Notify('" +
                                                        textId + "'); event.returnValue=false; return false;\" >");
                                                }
                                            }
                                        }
                                        noteText.Append("  [");
                                        break;
                                    case "lg":
                                        break;
                                    case "FI":
                                        if (!isRaw && !isNotesOnly && displaySettings.showNotePositions)
                                        {
                                            plainText.Append((displaySettings.smallVerseNumbers ? "<sup>" : "") + convertNoteNumToId(noteIdentifier) + (displaySettings.smallVerseNumbers ? "</sup>" : ""));
                                            noteIdentifier++;
                                        }
                                        if (!isChaptNumGivenNotes && !isRaw)
                                        {
                                            noteText.Append("<p>" + chapterNumber);
                                            isChaptNumGivenNotes = true;
                                        }
                                        noteText.Append("(");
                                        isInInjectionElement = true;
                                        break;
                                    case "RF":
                                    case "note":
                                        if (!isRaw && !isNotesOnly && displaySettings.showNotePositions)
                                        {
                                            plainText.Append((displaySettings.smallVerseNumbers ? "<sup>" : "") + convertNoteNumToId(noteIdentifier) + (displaySettings.smallVerseNumbers ? "</sup>" : ""));
                                            noteIdentifier++;
                                        }
                                        if (!isChaptNumGivenNotes && !isRaw)
                                        {
                                            noteText.Append("<p>" + chapterNumber);
                                            isChaptNumGivenNotes = true;
                                        }
                                        isInElement = true;
                                        break;
                                    case "hi":
                                        if (!isRaw)
                                        {
                                            appendText("<i>", plainText, noteText, isInElement);
                                        }
                                        break;
                                    case "Rf":
                                        isInElement = false;
                                        break;
                                    case "Fi":
                                        noteText.Append(") ");
                                        isInInjectionElement = false;
                                        break;
                                    case "q":
                                        if (displaySettings.wordsOfChristRed && !isRaw && !isNotesOnly)
                                        {
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();
                                                do
                                                {
                                                    if (reader.Name.Equals("who"))
                                                    {
                                                        if (reader.Value.ToLower().Equals("jesus"))
                                                        {
                                                            appendText("<span class=\"christ\">", plainText, noteText, isInElement);
                                                            isInQuote = true;
                                                        }
                                                    }
                                                } while (reader.MoveToNextAttribute());

                                            }
                                        }
                                        break;
                                    case "w":
                                        //<w lemma="strong:G1078" morph="robinson:N-GSF">γενεσεως</w>
                                        if ((displaySettings.showStrongsNumbers || displaySettings.showMorphology) && !isRaw && !isNotesOnly)
                                        {
                                            lemmaText = "";
                                            morphText = "";
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();

                                                do
                                                {
                                                    if (displaySettings.showStrongsNumbers && reader.Name.Equals("lemma"))
                                                    {
                                                        string[] lemmas = reader.Value.Split(' ');
                                                        foreach (string lemma in lemmas)
                                                        {
                                                            if (lemma.StartsWith("strong:"))
                                                            {
                                                                if (!string.IsNullOrEmpty(lemmaText))
                                                                {
                                                                    lemmaText += ",";
                                                                }
                                                                lemmaText += "<a class=\"strongsmorph\" href=\"#\" onclick=\"window.external.Notify('STRONG_" + lemma.Substring(7) + "'); event.returnValue=false; return false;\" >" + lemma.Substring(8) + "</a>";
                                                            }
                                                        }
                                                    }
                                                    else if (displaySettings.showMorphology && reader.Name.Equals("morph"))
                                                    {
                                                        string[] morphs = reader.Value.Split(' ');
                                                        foreach (string morph in morphs)
                                                        {
                                                            if (morph.StartsWith("robinson:"))
                                                            {
                                                                string subMorph = morph.Substring(9);
                                                                if (!string.IsNullOrEmpty(morphText))
                                                                {
                                                                    morphText += ",";
                                                                }
                                                                morphText += "<a class=\"strongsmorph\" href=\"#\" onclick=\"window.external.Notify('MORPH_" + subMorph + "'); event.returnValue=false; return false;\" >" + subMorph + "</a>";
                                                            }
                                                        }
                                                    }
                                                } while (reader.MoveToNextAttribute());
                                            }
                                        }
                                        break;

                                    case "versee":
                                        break;
                                    default:
                                        System.Diagnostics.Debug.WriteLine("Element untreated: " + reader.Name);
                                        break;
                                }
                                break;
                            case XmlNodeType.Text:
                                if (!isInElement && !isInInjectionElement && chapterNumber.Length > 0 && !isInTitle && !isChaptNumGiven)
                                {
                                    if (isInQuote)
                                    {
                                        appendText("</span>", plainText, noteText, isInElement);
                                    }
                                    plainText.Append(chapterNumber);
                                    if (isInQuote)
                                    {
                                        appendText("<span class=\"christ\">", plainText, noteText, isInElement);
                                    }
                                    isChaptNumGiven = true;
                                }
                                string text = string.Empty;
                                try
                                {
                                    text = reader.Value;
                                }
                                catch (Exception)
                                {
                                    text = "*error*";
                                }
                                if ((!(noTitles || !displaySettings.showHeadings) || !isInTitle) && text.Length>0)
                                {
                                    char firstChar = text[0];
                                    appendText(((!firstChar.Equals(',') && !firstChar.Equals('.') && !firstChar.Equals(':') && !firstChar.Equals(';') && !firstChar.Equals('?')) ? " " : "") + text, plainText, noteText, isInElement || isInInjectionElement);
                                }
                                break;
                            case XmlNodeType.EndElement:
                                switch (reader.Name)
                                {
                                    case "title":
                                        if (!(noTitles || !displaySettings.showHeadings) && !isRaw)
                                        {
                                            appendText("</h3>", plainText, noteText, isInElement);
                                        }
                                        isInTitle = false;
                                        break;
                                    case "reference":
                                        noteText.Append("] ");
                                        if (isReferenceLinked)
                                        {
                                            noteText.Append("</a>" + restartText);
                                        }
                                        isReferenceLinked = false;
                                        break;
                                    case "note":
                                        isInElement = false;
                                        break;
                                    case "hi":
                                        if (!isRaw)
                                        {
                                            appendText("</i>", plainText, noteText, isInElement);
                                        }
                                        break;
                                    case "q":
                                        if (isInQuote)
                                        {
                                            appendText("</span>", plainText, noteText, isInElement);
                                            isInQuote = false;
                                        }
                                        break;
                                    case "w":
                                        //<w lemma="strong:G1078" morph="robinson:N-GSF">γενεσεως</w>
                                        if ((displaySettings.showStrongsNumbers || displaySettings.showMorphology) && !isRaw && !isNotesOnly && (!string.IsNullOrEmpty(lemmaText) || !string.IsNullOrEmpty(morphText)))
                                        {
                                            plainText.Append("</a>" + (displaySettings.smallVerseNumbers ? "<sub>" : "<span class=\"strongsmorph\">(</span>"));
                                            if (!string.IsNullOrEmpty(lemmaText))
                                            {
                                                plainText.Append(lemmaText);
                                            }
                                            if (!string.IsNullOrEmpty(morphText))
                                            {
                                                plainText.Append((string.IsNullOrEmpty(lemmaText) ? "" : ",") + morphText);
                                            }
                                            plainText.Append((displaySettings.smallVerseNumbers ? "</sub>" : "<span class=\"strongsmorph\">)</span>") + restartText);
                                            lemmaText = "";
                                            morphText = "";
                                        }
                                        break;
                                    case "versee":
                                        break;
                                    default:
                                        System.Diagnostics.Debug.WriteLine("EndElement untreated: " + reader.Name);
                                        break;
                                }
                                break;
                        }
                    }
                }catch(Exception e)
                {
                    Debug.WriteLine("BibleZtextReader.parseOsisText " + e.Message);
                }
            }
            if (isNotesOnly && !isRaw)
            {
                if (noteText.Length > 0)
                {
                    noteText.Append("</p>");
                }
                return noteText.ToString();
            }
            //this replace fixes a character translation problem for slanted apostrophy
            return plainText.ToString().Replace('\x92', '\'');
        }

        protected void raiseSourceChangedEvent()
        {
            if (SourceChanged != null)
            {
                SourceChanged();
            }
        }

        private string convertNoteNumToId(int noteIdentifier)
        {
            string noteReturned = "";
            string startChar = ((char)((noteIdentifier-(int)'a')%24 + 'a')).ToString();
            int numChars=(noteIdentifier-(int)'a')/24;
            for (int i = 0; i <= numChars; i++)
            {
                noteReturned += startChar;
            }
            return "(" + noteReturned + ")";
        }

        private void ReloadSettingsFile()
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            bookPositions = new List<BookPos>();
            chapters = new List<ChapterPos>();
            FileStream fs = null;
            // read the Sword index to the ot bzz file which is the bzs file
            try
            {
                fs = fileStorage.OpenFile(serial.path + "ot.bzs", FileMode.Open, FileAccess.Read);
                for (int i = 0; i < BOOKS_IN_OT; i++)
                {
                    bookPositions.Add(new BookPos(getintFromStream(fs), getintFromStream(fs), getintFromStream(fs)));
                }
                fs.Close();

                // read the verse holder for versification bzv file old testememt
                fs = fileStorage.OpenFile(serial.path + "ot.bzv", FileMode.Open, FileAccess.Read);
                // dump the first 4 posts
                for (int i = 0; i < 4; i++)
                {
                    int booknum = getByteFromStream(fs);
                    long startPos = getInt48FromStream(fs);
                    int length = getShortIntFromStream(fs);
                }

                // now start getting old testament
                for (int i = 0; i < BOOKS_IN_OT; i++)
                {
                    for (int j = 0; j < CHAPTERS_IN_BOOK[i]; j++)
                    {
                        long chapterStartPos = 0;
                        ChapterPos chapt = null;
                        int booknum = 0;
                        long startPos = 0;
                        long lastNonZeroStartPos = 0;
                        int length = 0;
                        for (int k = 0; k < VERSES_IN_CHAPTER[i][j]; k++)
                        {
                            booknum = getByteFromStream(fs);
                            startPos = getInt48FromStream(fs);
                            if (startPos != 0)
                            {
                                lastNonZeroStartPos = startPos;
                            }
                            length = getShortIntFromStream(fs);
                            if (k == 0)
                            {
                                chapterStartPos = startPos;
                                chapt = new ChapterPos(chapterStartPos, i, j);
                                bookPositions[i].chapters.Add(chapt);
                            }
                            if (booknum == 0 && startPos == 0 && length == 0)
                            {
                                chapt.verses.Add(new VersePos(0, 0, i));
                            }
                            else
                            {
                                chapt.verses.Add(new VersePos(startPos - chapterStartPos, length, i));
                            }
                        }
                        // update the chapter length now that we know it
                        chapt.length = (int)(lastNonZeroStartPos - chapterStartPos) + length;

                        chapters.Add(chapt);

                        // dump a post for the chapter break
                        getByteFromStream(fs);
                        long x = getInt48FromStream(fs);
                        getShortIntFromStream(fs);
                    }
                    // dump a post for the book break
                    getByteFromStream(fs);
                    getInt48FromStream(fs);
                    getShortIntFromStream(fs);
                }
                fs.Close();
            }
            catch (Exception)
            {
                // failed to load old testement.  Maybe it does not exist.
                // fill with fake data
                for (int i = 0; i < BOOKS_IN_OT; i++)
                {
                    bookPositions.Add(new BookPos(0, 0, 0));
                }
                for (int i = 0; i < BOOKS_IN_OT; i++)
                {
                    for (int j = 0; j < CHAPTERS_IN_BOOK[i]; j++)
                    {
                        ChapterPos chapt = new ChapterPos(0, i, j);
                        for (int k = 0; k < VERSES_IN_CHAPTER[i][j]; k++)
                        {
                            chapt.verses.Add(new VersePos(0, 0, i));
                        }
                        // update the chapter length now that we know it
                        chapt.length =0;
                        chapters.Add(chapt);
                        bookPositions[i].chapters.Add(chapt);
                    }
                }
            }
            try
            {
                // do it for the new testement
                fs = fileStorage.OpenFile(serial.path + "nt.bzs", FileMode.Open, FileAccess.Read);
                for (int i = 0; i < BOOKS_IN_NT; i++)
                {
                    long startPos=getintFromStream(fs);
                    long length=getintFromStream(fs);
                    long unused=getintFromStream(fs);
                    if(unused!=SKIP_BOOK_FLAG || i!=0)
                    {
                        bookPositions.Add(new BookPos(startPos, length, unused));
                    }
                }
                fs.Close();

                // read the verse holder for versification bzv file new testememt
                fs = fileStorage.OpenFile(serial.path + "nt.bzv", FileMode.Open, FileAccess.Read);
                // dump the first 4 posts
                for (int i = 0; i < 4; i++)
                {
                    int booknum = getByteFromStream(fs);
                    long startPos = getInt48FromStream(fs);
                    int length = getShortIntFromStream(fs);
                }

                // now start getting new testament
                for (int i = BOOKS_IN_OT; i < (BOOKS_IN_OT + BOOKS_IN_NT); i++)
                {
                    for (int j = 0; j < CHAPTERS_IN_BOOK[i]; j++)
                    {
                        long chapterStartPos = 0;
                        ChapterPos chapt = null;
                        int booknum = 0;
                        long startPos = 0;
                        long lastNonZeroStartPos = 0;
                        int length = 0;
                        for (int k = 0; k < VERSES_IN_CHAPTER[i][j]; k++)
                        {
                            booknum = getByteFromStream(fs);
                            startPos = getInt48FromStream(fs);
                            if (startPos != 0)
                            {
                                lastNonZeroStartPos = startPos;
                            }
                            length = getShortIntFromStream(fs);
                            if (k == 0)
                            {
                                chapterStartPos = startPos;
                                chapt = new ChapterPos(chapterStartPos, i, j);
                                bookPositions[i].chapters.Add(chapt);
                            }
                            if (booknum == 0 && startPos == 0 && length == 0)
                            {
                                chapt.verses.Add(new VersePos(0, 0, i));
                            }
                            else
                            {
                                chapt.verses.Add(new VersePos(startPos - chapterStartPos, length, i));
                            }
                        }
                        // update the chapter length now that we know it
                        chapt.length = (int)(lastNonZeroStartPos - chapterStartPos) + length;

                        chapters.Add(chapt);

                        // dump a post for the chapter break
                        getByteFromStream(fs);
                        long x = getInt48FromStream(fs);
                        getShortIntFromStream(fs);
                    }
                    // dump a post for the book break
                    getByteFromStream(fs);
                    getInt48FromStream(fs);
                    getShortIntFromStream(fs);
                }
                fs.Close();
            }
            catch (Exception)
            {
                // failed to load new testement.  Maybe it does not exist.
                // fill with fake data
                for (int i = 0; i < BOOKS_IN_NT; i++)
                {
                    bookPositions.Add(new BookPos(0, 0, 0));
                }
                for (int i = BOOKS_IN_OT; i < (BOOKS_IN_OT + BOOKS_IN_NT); i++)
                {
                    for (int j = 0; j < CHAPTERS_IN_BOOK[i]; j++)
                    {
                        ChapterPos chapt = new ChapterPos(0, i, j);
                        for (int k = 0; k < VERSES_IN_CHAPTER[i][j]; k++)
                        {
                            chapt.verses.Add(new VersePos(0, 0, i));
                        }
                        // update the chapter length now that we know it
                        chapt.length = 0;
                        chapters.Add(chapt);
                        bookPositions[i].chapters.Add(chapt);
                    }
                }
            }
        }

        #endregion Methods

        #region Nested Types

        [DataContract]
        public struct VersePos
        {
            #region Fields

            [DataMember]
            public int booknum;
            [DataMember]
            public int length;
            [DataMember]
            public long startPos;

            #endregion Fields

            #region Constructors

            public VersePos(long startPos, int length, int booknum)
            {
                this.startPos = startPos;
                this.length = length;
                this.booknum = booknum;
            }

            #endregion Constructors
        }

        [DataContract(IsReference = true)]
        [KnownType(typeof(ChapterPos))]
        public class BookPos
        {
            #region Fields

            [DataMember]
            public List<ChapterPos> chapters = new List<ChapterPos>();
            [DataMember]
            public long length;
            [DataMember]
            public long startPos;
            [DataMember]
            public long unused;

            #endregion Fields

            #region Constructors

            public BookPos(long startPos, long length, long unused)
            {
                this.startPos = startPos;
                this.length = length;
                this.unused = unused;
            }

            #endregion Constructors
        }

        [DataContract(IsReference = true)]
        [KnownType(typeof(VersePos))]
        public class ChapterPos
        {
            #region Fields

            [DataMember]
            public int booknum;
            [DataMember]
            public int bookRelativeChapterNum;
            [DataMember]
            public int length = 0;
            [DataMember]
            public long startPos;
            [DataMember]
            public List<VersePos> verses = new List<VersePos>();

            #endregion Fields

            #region Constructors

            public ChapterPos(long startPos, int booknum, int bookRelativeChapterNum)
            {
                this.startPos = startPos;
                this.booknum = booknum;
                this.bookRelativeChapterNum = bookRelativeChapterNum;
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }

    [DataContract]
    public class BibleZtextReaderSerialData
    {
        #region Fields

        [DataMember]
        public bool isIsoEncoding = false;
        [DataMember]
        public string iso2DigitLangCode = string.Empty;
        [DataMember]
        public string path = string.Empty;
        [DataMember]
        public int posChaptNum = 0;
        [DataMember]
        public int posVerseNum = 0;

        #endregion Fields

        #region Constructors

        public BibleZtextReaderSerialData(BibleZtextReaderSerialData from)
        {
            cloneFrom(from);
        }

        public BibleZtextReaderSerialData(bool isIsoEncoding,string iso2DigitLangCode,string path,int posChaptNum,int posVerseNum)
        {
            this.isIsoEncoding = isIsoEncoding;
            this.iso2DigitLangCode=iso2DigitLangCode;
            this.path=path;
            this.posChaptNum=posChaptNum;
            this.posVerseNum=posVerseNum;
        }

        #endregion Constructors

        #region Methods

        public void cloneFrom(BibleZtextReaderSerialData from)
        {
            this.isIsoEncoding=from.isIsoEncoding;
            this.iso2DigitLangCode=from.iso2DigitLangCode;
            this.path=from.path;
            this.posChaptNum=from.posChaptNum;
            this.posVerseNum=from.posVerseNum;
        }

        #endregion Methods
    }
}