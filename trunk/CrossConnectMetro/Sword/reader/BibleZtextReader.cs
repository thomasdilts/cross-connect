#region Header

// <copyright file="BibleZtextReader.cs" company="Thomas Dilts">
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace Sword.reader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    using ComponentAce.Compression.Libs.zlib;

    using Windows.Storage;

    [DataContract]
    public class BiblePlaceMarker
    {
        #region Fields

        [DataMember(Name = "chapterNum")]
        public int ChapterNum = 1;

        [DataMember(Name = "note")]
        public string Note = string.Empty;

        [DataMember(Name = "verseNum")]
        public int VerseNum = 1;

        [DataMember(Name = "when")]
        public DateTime When;

        #endregion

        #region Constructors and Destructors

        public BiblePlaceMarker(int chapterNum, int verseNum, DateTime when)
        {
            this.ChapterNum = chapterNum;
            this.VerseNum = verseNum;
            this.When = when;
        }

        #endregion

        #region Public Methods and Operators

        public string ToString()
        {
            return this.ChapterNum + ";" + this.VerseNum;
        }

        public static BiblePlaceMarker Clone(BiblePlaceMarker toClone)
        {
            var newMarker = new BiblePlaceMarker(toClone.ChapterNum, toClone.VerseNum, toClone.When)
                                {
                                    Note =
                                        toClone
                                        .Note
                                };
            return newMarker;
        }

        #endregion
    }

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract]
    public class BibleZtextReader : IBrowserTextSource
    {
        #region Constants

        /// <summary>
        ///     Constant for the number of books in the Bible
        /// </summary>
        public const int BooksInBible = 66;

        public const int BooksInNt = 27;

        public const int BooksInOt = 39;

        /// <summary>
        ///     Constant for the number of chapters in the Bible
        /// </summary>
        public const int ChaptersInBible = 1189;

        /// <summary>
        ///     Constant for the number of chapters in the NT
        /// </summary>
        public const int ChaptersInNt = 260;

        /// <summary>
        ///     Constant for the number of chapters in the OT
        /// </summary>
        public const int ChaptersInOt = 929;

        /// <summary>
        ///     * The configuration directory
        /// </summary>
        public const string DirConf = "mods.d";

        /// <summary>
        ///     * The data directory
        /// </summary>
        public const string DirData = "modules";

        /// <summary>
        ///     * Extension for config files
        /// </summary>
        public const string ExtensionConf = ".conf";

        /// <summary>
        ///     * Extension for data files
        /// </summary>
        public const string ExtensionData = ".dat";

        /// <summary>
        ///     * Extension for index files
        /// </summary>
        public const string ExtensionIndex = ".idx";

        /// <summary>
        ///     * Index file extensions
        /// </summary>
        public const string ExtensionVss = ".vss";

        /// <summary>
        ///     * New testament data files
        /// </summary>
        public const string FileNt = "nt";

        /// <summary>
        ///     * Old testament data files
        /// </summary>
        public const string FileOt = "ot";

        /// <summary>
        ///     Constant for the number of verses in the Bible
        /// </summary>
        internal const short VersesInBible = 31102;

        protected const long SkipBookFlag = 68;

        #endregion

        #region Static Fields

        /// <summary>
        ///     Constant for the number of chapters in each book
        /// </summary>
        public static readonly short[] ChaptersInBook =
            {
                50, 40, 27, 36, 34, 24, 21, 4, 31, 24, 22, 25, 29, 36, 10, 13,
                10, 42, 150, 31, 12, 8, 66, 52, 5, 48, 12, 14, 3, 9, 1, 4, 7,
                3, 3, 3, 2, 14, 4, 28, 16, 24, 21, 28, 16, 16, 13, 6, 6, 4, 4,
                5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22
            };

        public static readonly short[] FirstChapternumInBook =
            {
                0, 50, 90, 117, 153, 187, 211, 232, 236, 267, 291, 313,
                338, 367, 403, 413, 426, 436, 478, 628, 659, 671, 679,
                745, 797, 802, 850, 862, 876, 879, 888, 889, 893, 900,
                903, 906, 909, 911, 925, 929, 957, 973, 997, 1018, 1046
                , 1062, 1078, 1091, 1097, 1103, 1107, 1111, 1116, 1119,
                1125, 1129, 1132, 1133, 1146, 1151, 1156, 1159, 1164,
                1165, 1166, 1167
            };

        public static readonly Dictionary<string, int> OsisBibeNamesToAbsoluteChapterNum = new Dictionary<string, int>
                                                                                               {
                                                                                                   {
                                                                                                       "gen"
                                                                                                       ,
                                                                                                       0
                                                                                                   },
                                                                                                   {
                                                                                                       "exod"
                                                                                                       ,
                                                                                                       50
                                                                                                   },
                                                                                                   {
                                                                                                       "lev"
                                                                                                       ,
                                                                                                       90
                                                                                                   },
                                                                                                   {
                                                                                                       "num"
                                                                                                       ,
                                                                                                       117
                                                                                                   },
                                                                                                   {
                                                                                                       "deut"
                                                                                                       ,
                                                                                                       153
                                                                                                   },
                                                                                                   {
                                                                                                       "josh"
                                                                                                       ,
                                                                                                       187
                                                                                                   },
                                                                                                   {
                                                                                                       "judg"
                                                                                                       ,
                                                                                                       211
                                                                                                   },
                                                                                                   {
                                                                                                       "ruth"
                                                                                                       ,
                                                                                                       232
                                                                                                   },
                                                                                                   {
                                                                                                       "1sam"
                                                                                                       ,
                                                                                                       236
                                                                                                   },
                                                                                                   {
                                                                                                       "2sam"
                                                                                                       ,
                                                                                                       267
                                                                                                   },
                                                                                                   {
                                                                                                       "1kgs"
                                                                                                       ,
                                                                                                       291
                                                                                                   },
                                                                                                   {
                                                                                                       "2kgs"
                                                                                                       ,
                                                                                                       313
                                                                                                   },
                                                                                                   {
                                                                                                       "1chr"
                                                                                                       ,
                                                                                                       338
                                                                                                   },
                                                                                                   {
                                                                                                       "2chr"
                                                                                                       ,
                                                                                                       367
                                                                                                   },
                                                                                                   {
                                                                                                       "ezra"
                                                                                                       ,
                                                                                                       403
                                                                                                   },
                                                                                                   {
                                                                                                       "neh"
                                                                                                       ,
                                                                                                       413
                                                                                                   },
                                                                                                   {
                                                                                                       "esth"
                                                                                                       ,
                                                                                                       426
                                                                                                   },
                                                                                                   {
                                                                                                       "job"
                                                                                                       ,
                                                                                                       436
                                                                                                   },
                                                                                                   {
                                                                                                       "ps"
                                                                                                       ,
                                                                                                       478
                                                                                                   },
                                                                                                   {
                                                                                                       "prov"
                                                                                                       ,
                                                                                                       628
                                                                                                   },
                                                                                                   {
                                                                                                       "eccl"
                                                                                                       ,
                                                                                                       659
                                                                                                   },
                                                                                                   {
                                                                                                       "song"
                                                                                                       ,
                                                                                                       671
                                                                                                   },
                                                                                                   {
                                                                                                       "isa"
                                                                                                       ,
                                                                                                       679
                                                                                                   },
                                                                                                   {
                                                                                                       "jer"
                                                                                                       ,
                                                                                                       745
                                                                                                   },
                                                                                                   {
                                                                                                       "lam"
                                                                                                       ,
                                                                                                       797
                                                                                                   },
                                                                                                   {
                                                                                                       "ezek"
                                                                                                       ,
                                                                                                       802
                                                                                                   },
                                                                                                   {
                                                                                                       "dan"
                                                                                                       ,
                                                                                                       850
                                                                                                   },
                                                                                                   {
                                                                                                       "hos"
                                                                                                       ,
                                                                                                       862
                                                                                                   },
                                                                                                   {
                                                                                                       "joel"
                                                                                                       ,
                                                                                                       876
                                                                                                   },
                                                                                                   {
                                                                                                       "amos"
                                                                                                       ,
                                                                                                       879
                                                                                                   },
                                                                                                   {
                                                                                                       "obad"
                                                                                                       ,
                                                                                                       888
                                                                                                   },
                                                                                                   {
                                                                                                       "jonah"
                                                                                                       ,
                                                                                                       889
                                                                                                   },
                                                                                                   {
                                                                                                       "mic"
                                                                                                       ,
                                                                                                       893
                                                                                                   },
                                                                                                   {
                                                                                                       "nah"
                                                                                                       ,
                                                                                                       900
                                                                                                   },
                                                                                                   {
                                                                                                       "hab"
                                                                                                       ,
                                                                                                       903
                                                                                                   },
                                                                                                   {
                                                                                                       "zeph"
                                                                                                       ,
                                                                                                       906
                                                                                                   },
                                                                                                   {
                                                                                                       "hag"
                                                                                                       ,
                                                                                                       909
                                                                                                   },
                                                                                                   {
                                                                                                       "zech"
                                                                                                       ,
                                                                                                       911
                                                                                                   },
                                                                                                   {
                                                                                                       "mal"
                                                                                                       ,
                                                                                                       925
                                                                                                   },
                                                                                                   {
                                                                                                       "matt"
                                                                                                       ,
                                                                                                       929
                                                                                                   },
                                                                                                   {
                                                                                                       "mark"
                                                                                                       ,
                                                                                                       957
                                                                                                   },
                                                                                                   {
                                                                                                       "luke"
                                                                                                       ,
                                                                                                       973
                                                                                                   },
                                                                                                   {
                                                                                                       "john"
                                                                                                       ,
                                                                                                       997
                                                                                                   },
                                                                                                   {
                                                                                                       "acts"
                                                                                                       ,
                                                                                                       1018
                                                                                                   },
                                                                                                   {
                                                                                                       "rom"
                                                                                                       ,
                                                                                                       1046
                                                                                                   },
                                                                                                   {
                                                                                                       "1cor"
                                                                                                       ,
                                                                                                       1062
                                                                                                   },
                                                                                                   {
                                                                                                       "2cor"
                                                                                                       ,
                                                                                                       1078
                                                                                                   },
                                                                                                   {
                                                                                                       "gal"
                                                                                                       ,
                                                                                                       1091
                                                                                                   },
                                                                                                   {
                                                                                                       "eph"
                                                                                                       ,
                                                                                                       1097
                                                                                                   },
                                                                                                   {
                                                                                                       "phil"
                                                                                                       ,
                                                                                                       1103
                                                                                                   },
                                                                                                   {
                                                                                                       "col"
                                                                                                       ,
                                                                                                       1107
                                                                                                   },
                                                                                                   {
                                                                                                       "1thess"
                                                                                                       ,
                                                                                                       1111
                                                                                                   },
                                                                                                   {
                                                                                                       "2thess"
                                                                                                       ,
                                                                                                       1116
                                                                                                   },
                                                                                                   {
                                                                                                       "1tim"
                                                                                                       ,
                                                                                                       1119
                                                                                                   },
                                                                                                   {
                                                                                                       "2tim"
                                                                                                       ,
                                                                                                       1125
                                                                                                   },
                                                                                                   {
                                                                                                       "titus"
                                                                                                       ,
                                                                                                       1129
                                                                                                   },
                                                                                                   {
                                                                                                       "phlm"
                                                                                                       ,
                                                                                                       1132
                                                                                                   },
                                                                                                   {
                                                                                                       "heb"
                                                                                                       ,
                                                                                                       1133
                                                                                                   },
                                                                                                   {
                                                                                                       "jas"
                                                                                                       ,
                                                                                                       1146
                                                                                                   },
                                                                                                   {
                                                                                                       "1pet"
                                                                                                       ,
                                                                                                       1151
                                                                                                   },
                                                                                                   {
                                                                                                       "2pet"
                                                                                                       ,
                                                                                                       1156
                                                                                                   },
                                                                                                   {
                                                                                                       "1john"
                                                                                                       ,
                                                                                                       1159
                                                                                                   },
                                                                                                   {
                                                                                                       "2john"
                                                                                                       ,
                                                                                                       1164
                                                                                                   },
                                                                                                   {
                                                                                                       "3john"
                                                                                                       ,
                                                                                                       1165
                                                                                                   },
                                                                                                   {
                                                                                                       "jude"
                                                                                                       ,
                                                                                                       1166
                                                                                                   },
                                                                                                   {
                                                                                                       "rev"
                                                                                                       ,
                                                                                                       1167
                                                                                                   },
                                                                                               };

        // [DataMember] Tried it but it took 10 times longer then re-reading the mod-file
        /// <summary>
        ///     Constant for the number of verses in each book
        /// </summary>
        internal static readonly short[] VersesInBook =
            {
                1533, 1213, 859, 1288, 959, 658, 618, 85, 810, 695, 816, 719,
                942, 822, 280, 406, 167, 1070, 2461, 915, 222, 117, 1292, 1364
                , 154, 1273, 357, 197, 73, 146, 21, 48, 105, 47, 56, 53, 38,
                211, 55, 1071, 678, 1151, 879, 1007, 433, 437, 257, 149, 155,
                104, 95, 89, 47, 113, 83, 46, 25, 303, 108, 105, 61, 105, 13,
                14, 25, 404
            };

        /// <summary>
        ///     Constant for the number of verses in each chapter
        /// </summary>
        internal static readonly short[][] VersesInChapter =
            {
                new short[]
                    {
                        31, 25, 24, 26, 32, 22, 24, 22, 29, 32, 32, 20,
                        18, 24, 21, 16, 27, 33, 38, 18, 34, 24, 20, 67,
                        34, 35, 46, 22, 35, 43, 55, 32, 20, 31, 29, 43,
                        36, 30, 23, 23, 57, 38, 34, 34, 28, 34, 31, 22,
                        33, 26
                    },
                new short[]
                    {
                        22, 25, 22, 31, 23, 30, 25, 32, 35, 29, 10, 51,
                        22, 31, 27, 36, 16, 27, 25, 26, 36, 31, 33, 18,
                        40, 37, 21, 43, 46, 38, 18, 35, 23, 35, 35, 38,
                        29, 31, 43, 38
                    },
                new short[]
                    {
                        17, 16, 17, 35, 19, 30, 38, 36, 24, 20, 47, 8, 59
                        , 57, 33, 34, 16, 30, 37, 27, 24, 33, 44, 23, 55,
                        46, 34
                    },
                new short[]
                    {
                        54, 34, 51, 49, 31, 27, 89, 26, 23, 36, 35, 16,
                        33, 45, 41, 50, 13, 32, 22, 29, 35, 41, 30, 25,
                        18, 65, 23, 31, 40, 16, 54, 42, 56, 29, 34, 13
                    },
                new short[]
                    {
                        46, 37, 29, 49, 33, 25, 26, 20, 29, 22, 32, 32,
                        18, 29, 23, 22, 20, 22, 21, 20, 23, 30, 25, 22,
                        19, 19, 26, 68, 29, 20, 30, 52, 29, 12
                    },
                new short[]
                    {
                        18, 24, 17, 24, 15, 27, 26, 35, 27, 43, 23, 24,
                        33, 15, 63, 10, 18, 28, 51, 9, 45, 34, 16, 33
                    },
                new short[]
                    {
                        36, 23, 31, 24, 31, 40, 25, 35, 57, 18, 40, 15,
                        25, 20, 20, 31, 13, 31, 30, 48, 25
                    },
                new short[] { 22, 23, 18, 22 },
                new short[]
                    {
                        28, 36, 21, 22, 12, 21, 17, 22, 27, 27, 15, 25,
                        23, 52, 35, 23, 58, 30, 24, 42, 15, 23, 29, 22,
                        44, 25, 12, 25, 11, 31, 13
                    },
                new short[]
                    {
                        27, 32, 39, 12, 25, 23, 29, 18, 13, 19, 27, 31,
                        39, 33, 37, 23, 29, 33, 43, 26, 22, 51, 39, 25
                    },
                new short[]
                    {
                        53, 46, 28, 34, 18, 38, 51, 66, 28, 29, 43, 33,
                        34, 31, 34, 34, 24, 46, 21, 43, 29, 53
                    },
                new short[]
                    {
                        18, 25, 27, 44, 27, 33, 20, 29, 37, 36, 21, 21,
                        25, 29, 38, 20, 41, 37, 37, 21, 26, 20, 37, 20,
                        30
                    },
                new short[]
                    {
                        54, 55, 24, 43, 26, 81, 40, 40, 44, 14, 47, 40,
                        14, 17, 29, 43, 27, 17, 19, 8, 30, 19, 32, 31, 31
                        , 32, 34, 21, 30
                    },
                new short[]
                    {
                        17, 18, 17, 22, 14, 42, 22, 18, 31, 19, 23, 16,
                        22, 15, 19, 14, 19, 34, 11, 37, 20, 12, 21, 27,
                        28, 23, 9, 27, 36, 27, 21, 33, 25, 33, 27, 23
                    },
                new short[] { 11, 70, 13, 24, 17, 22, 28, 36, 15, 44 },
                new short[]
                    {
                        11, 20, 32, 23, 19, 19, 73, 18, 38, 39, 36, 47,
                        31
                    },
                new short[] { 22, 23, 15, 17, 14, 14, 10, 17, 32, 3 },
                new short[]
                    {
                        22, 13, 26, 21, 27, 30, 21, 22, 35, 22, 20, 25,
                        28, 22, 35, 22, 16, 21, 29, 29, 34, 30, 17, 25, 6
                        , 14, 23, 28, 25, 31, 40, 22, 33, 37, 16, 33, 24,
                        41, 30, 24, 34, 17
                    },
                new short[]
                    {
                        6, 12, 8, 8, 12, 10, 17, 9, 20, 18, 7, 8, 6, 7, 5
                        , 11, 15, 50, 14, 9, 13, 31, 6, 10, 22, 12, 14, 9
                        , 11, 12, 24, 11, 22, 22, 28, 12, 40, 22, 13, 17,
                        13, 11, 5, 26, 17, 11, 9, 14, 20, 23, 19, 9, 6, 7
                        , 23, 13, 11, 11, 17, 12, 8, 12, 11, 10, 13, 20,
                        7, 35, 36, 5, 24, 20, 28, 23, 10, 12, 20, 72, 13,
                        19, 16, 8, 18, 12, 13, 17, 7, 18, 52, 17, 16, 15,
                        5, 23, 11, 13, 12, 9, 9, 5, 8, 28, 22, 35, 45, 48
                        , 43, 13, 31, 7, 10, 10, 9, 8, 18, 19, 2, 29, 176
                        , 7, 8, 9, 4, 8, 5, 6, 5, 6, 8, 8, 3, 18, 3, 3,
                        21, 26, 9, 8, 24, 13, 10, 7, 12, 15, 21, 10, 20,
                        14, 9, 6
                    },
                new short[]
                    {
                        33, 22, 35, 27, 23, 35, 27, 36, 18, 32, 31, 28,
                        25, 35, 33, 33, 28, 24, 29, 30, 31, 29, 35, 34,
                        28, 28, 27, 28, 27, 33, 31
                    },
                new short[]
                    { 18, 26, 22, 16, 20, 12, 29, 17, 18, 20, 10, 14 },
                new short[] { 17, 17, 11, 16, 16, 13, 13, 14 },
                new short[]
                    {
                        31, 22, 26, 6, 30, 13, 25, 22, 21, 34, 16, 6, 22,
                        32, 9, 14, 14, 7, 25, 6, 17, 25, 18, 23, 12, 21,
                        13, 29, 24, 33, 9, 20, 24, 17, 10, 22, 38, 22, 8,
                        31, 29, 25, 28, 28, 25, 13, 15, 22, 26, 11, 23,
                        15, 12, 17, 13, 12, 21, 14, 21, 22, 11, 12, 19,
                        12, 25, 24
                    },
                new short[]
                    {
                        19, 37, 25, 31, 31, 30, 34, 22, 26, 25, 23, 17,
                        27, 22, 21, 21, 27, 23, 15, 18, 14, 30, 40, 10,
                        38, 24, 22, 17, 32, 24, 40, 44, 26, 22, 19, 32,
                        21, 28, 18, 16, 18, 22, 13, 30, 5, 28, 7, 47, 39,
                        46, 64, 34
                    },
                new short[] { 22, 22, 66, 22, 22 },
                new short[]
                    {
                        28, 10, 27, 17, 17, 14, 27, 18, 11, 22, 25, 28,
                        23, 23, 8, 63, 24, 32, 14, 49, 32, 31, 49, 27, 17
                        , 21, 36, 26, 21, 26, 18, 32, 33, 31, 15, 38, 28,
                        23, 29, 49, 26, 20, 27, 31, 25, 24, 23, 35
                    },
                new short[]
                    { 21, 49, 30, 37, 31, 28, 28, 27, 27, 21, 45, 13 },
                new short[]
                    {
                        11, 23, 5, 19, 15, 11, 16, 14, 17, 15, 12, 14, 16
                        , 9
                    },
                new short[] { 20, 32, 21 },
                new short[] { 15, 16, 15, 13, 27, 14, 17, 14, 15 },
                new short[] { 21 }, new short[] { 17, 10, 10, 11 },
                new short[] { 16, 13, 12, 13, 15, 16, 20 },
                new short[] { 15, 13, 19 }, new short[] { 17, 20, 19 },
                new short[] { 18, 15, 20 }, new short[] { 15, 23 },
                new short[]
                    {
                        21, 13, 10, 14, 11, 15, 14, 23, 17, 12, 17, 14, 9
                        , 21
                    },
                new short[] { 14, 17, 18, 6 },
                new short[]
                    {
                        25, 23, 17, 25, 48, 34, 29, 34, 38, 42, 30, 50,
                        58, 36, 39, 28, 27, 35, 30, 34, 46, 46, 39, 51,
                        46, 75, 66, 20
                    },
                new short[]
                    {
                        45, 28, 35, 41, 43, 56, 37, 38, 50, 52, 33, 44,
                        37, 72, 47, 20
                    },
                new short[]
                    {
                        80, 52, 38, 44, 39, 49, 50, 56, 62, 42, 54, 59,
                        35, 35, 32, 31, 37, 43, 48, 47, 38, 71, 56, 53
                    },
                new short[]
                    {
                        51, 25, 36, 54, 47, 71, 53, 59, 41, 42, 57, 50,
                        38, 31, 27, 33, 26, 40, 42, 31, 25
                    },
                new short[]
                    {
                        26, 47, 26, 37, 42, 15, 60, 40, 43, 48, 30, 25,
                        52, 28, 41, 40, 34, 28, 41, 38, 40, 30, 35, 27,
                        27, 32, 44, 31
                    },
                new short[]
                    {
                        32, 29, 31, 25, 21, 23, 25, 39, 33, 21, 36, 21,
                        14, 23, 33, 27
                    },
                new short[]
                    {
                        31, 16, 23, 21, 13, 20, 40, 13, 27, 33, 34, 31,
                        13, 40, 58, 24
                    },
                new short[]
                    {
                        24, 17, 18, 18, 21, 18, 16, 24, 15, 18, 33, 21,
                        14
                    },
                new short[] { 24, 21, 29, 31, 26, 18 },
                new short[] { 23, 22, 21, 32, 33, 24 },
                new short[] { 30, 30, 21, 23 },
                new short[] { 29, 23, 25, 18 },
                new short[] { 10, 20, 13, 18, 28 },
                new short[] { 12, 17, 18 },
                new short[] { 20, 15, 16, 16, 25, 21 },
                new short[] { 18, 26, 17, 22 },
                new short[] { 16, 15, 15 }, new short[] { 25 },
                new short[]
                    {
                        14, 18, 19, 16, 14, 20, 28, 13, 28, 39, 40, 29,
                        25
                    },
                new short[] { 27, 26, 18, 17, 20 },
                new short[] { 25, 25, 22, 19, 14 },
                new short[] { 21, 22, 18 },
                new short[] { 10, 29, 24, 21, 21 }, new short[] { 13 },
                new short[] { 14 }, new short[] { 25 },
                new short[]
                    {
                        20, 29, 22, 11, 14, 17, 17, 13, 21, 11, 19, 17,
                        18, 20, 8, 21, 18, 24, 21, 15, 27, 21
                    }
            };

        /// <summary>
        ///     Chapters divided into categories
        /// </summary>
        protected static readonly int[] ChapterCategories =
            {
                1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3
                , 3, 3, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                6, 6, 6, 6, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 9, 9
                , 9, 9, 9, 9, 9, 9, 10
            };

        protected static byte[] Prefix = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<versee>");

        protected static byte[] PrefixIso =
            Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n<versee>");

        protected static byte[] Suffix = Encoding.UTF8.GetBytes("\n</versee>");

        public static readonly HtmlColorRgba ColorWordsOfChrist = HtmlColorRgba.CreateWithHtmlRgb("ff1439");
                                              // deep pink

        #endregion

        #region Fields

        public List<ChapterPos> Chapters = new List<ChapterPos>();

        [DataMember(Name = "serial")]
        public BibleZtextReaderSerialData Serial = new BibleZtextReaderSerialData(
            false, string.Empty, string.Empty, 0, 0, string.Empty, string.Empty);

        protected IndexingBlockType BlockType = IndexingBlockType.Book;

        public BibleNames BookNames;

        private int _lastShownChapterNumber = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Load from a file all the book and verse pointers to the bzz file so that
        ///     we can later read the bzz file quickly and efficiently.
        /// </summary>
        /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
        /// <param name="iso2DigitLangCode"></param>
        /// <param name="isIsoEncoding"></param>
        public BibleZtextReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath)
        {
            this.Serial.Iso2DigitLangCode = iso2DigitLangCode;
            this.Serial.Path = path;
            this.Serial.IsIsoEncoding = isIsoEncoding;
            this.Serial.CipherKey = cipherKey;
            this.Serial.ConfigPath = configPath;
        }

        public async Task Initialize()
        {
            await this.ReloadSettingsFile();
            this.SetToFirstChapter();
        }

        #endregion

        #region Public Events

        public event WindowSourceChanged SourceChanged;

        #endregion

        #region Enums

        protected enum IndexingBlockType
        {
            Book = 'b',

            Chapter = 'c'
        }

        #endregion

        #region Public Properties

        public bool ExistsShortNames
        {
            get
            {
                if (this.BookNames == null)
                {
                    this.BookNames = new BibleNames(this.Serial.Iso2DigitLangCode);
                }

                return this.BookNames.ExistsShortNames;
            }
        }

        public virtual bool IsExternalLink
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsHearable
        {
            get
            {
                return true;
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

        public virtual bool IsTranslateable
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsLocked
        {
            get
            {
                return this.Serial.CipherKey != null && this.Serial.CipherKey.Length==0;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static bool ConvertOsisRefToAbsoluteChaptVerse(string osisRef, out int chaptNumLoc, out int verseNumLoc)
        {
            chaptNumLoc = 0;
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
                if (OsisBibeNamesToAbsoluteChapterNum.ContainsKey(osis[0].ToLower()))
                {
                    chaptNumLoc = OsisBibeNamesToAbsoluteChapterNum[osis[0].ToLower()];
                    if (osis.Length > 1)
                    {
                        int chapterRelative;
                        int.TryParse(osis[1], out chapterRelative);
                        chapterRelative--;
                        if (chapterRelative < 0)
                        {
                            chapterRelative = 0;
                        }

                        chaptNumLoc += chapterRelative;
                    }

                    if (osis.Length > 2)
                    {
                        int.TryParse(osis[2], out verseNumLoc);
                        verseNumLoc--;
                        if (verseNumLoc < 0)
                        {
                            verseNumLoc = 0;
                        }
                    }

                    return true;
                }

                return false;
            }

            return false;
        }

        public static async Task<bool> FileExists(string filePath)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath.Replace("/", "\\"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string HtmlHeader(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily)
        {
            var head = new StringBuilder();
            head.Append(
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1\">");

            head.Append("<style>");

            head.Append(
                string.Format(
                    "body {{background:{0};color:{1};font-size:{2}pt;margin:0;padding:0;{3} }}",
                    htmlBackgroundColor.GetHtmlRgba(),
                    htmlForegroundColor.GetHtmlRgba(),
                    (int)(htmlFontSize + 0.5),
                    fontFamily)); // old fashioned way to round an integer

            head.Append(
                string.Format(
                    "sup,sub {{color:{0};font-size: .83em;}} "
                    + "a.strongsmorph,a.strongsmorph:link,span.strongsmorph{{color:{1};text-decoration:none;}} "
                    + "a.normalcolor,a.normalcolor:link {{color:{2};text-decoration:none;}}",
                    displaySettings.HighlightMarkings
                        ? htmlPhoneAccentColor.GetHtmlRgba()
                        : htmlForegroundColor.GetHtmlRgba(),
                    displaySettings.HighlightMarkings
                        ? htmlPhoneAccentColor.GetHtmlRgba()
                        : htmlForegroundColor.GetHtmlRgba(),
                    htmlForegroundColor.GetHtmlRgba()));

            head.Append(
                string.Format(
                    " a.normalcolor:link span.christ {{ color: {1}; }}  a.normalcolor span.christ:visited {{ color: {3}; }}  a.normalcolor span.christ:hover {{ color: {2}; }} a.normalcolor:hover {{ color: {0}; }} ",
                    htmlPhoneAccentColor.GetHtmlRgba(),
                    ColorWordsOfChrist.GetHtmlRgba(),
                    htmlPhoneAccentColor.GetHtmlRgba(),
                    htmlPhoneAccentColor.GetHtmlRgba()));

            head.Append("</style>");
            head.Append(@"<script type=""text/javascript"">

function getVerticalScrollPosition() {
    return document.body.scrollTop.toString();
}
function setVerticalScrollPosition(position) {
    document.body.scrollTop = position;
}
function ScrollToAnchor(anchor, colorRgba) {
    window.location.hash=anchor;
    SetFontColorForElement(anchor, colorRgba);
}
function SetFontColorForElement(elemntId, colorRgba){
    var element = document.getElementById(elemntId);
    if(element!=null){
        element.style.color = colorRgba;
    }
}

</script>");
            head.Append("</head><body>");
            return head.ToString();
        }

        public virtual ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton)
        {
            switch (stage)
            {
                case 0:
                    {
                        // books
                        var colors = new List<int>();
                        var values = new List<int>();
                        var buttonNames = new List<string>();
                        if (this.BookNames == null)
                        {
                            this.BookNames = new BibleNames(this.Serial.Iso2DigitLangCode);
                        }

                        string[] buttonNamesStart = this.BookNames.GetAllShortNames();
                        if (!this.BookNames.ExistsShortNames)
                        {
                            buttonNamesStart = this.BookNames.GetAllFullNames();
                        }

                        // assumption. if the first chapter in the book does not exist then the book does not exist
                        for (int i = 0; i < BooksInBible; i++)
                        {
                            if (this.Chapters[FirstChapternumInBook[i]].Length != 0)
                            {
                                colors.Add(ChapterCategories[i]);
                                values.Add(FirstChapternumInBook[i]);
                                buttonNames.Add(buttonNamesStart[i]);
                            }
                        }

                        return new ButtonWindowSpecs(
                            stage,
                            "Select a book to view",
                            colors.Count,
                            colors.ToArray(),
                            buttonNames.ToArray(),
                            values.ToArray(),
                            !this.BookNames.ExistsShortNames ? ButtonSize.Large : ButtonSize.Medium);
                    }
                case 1:
                    {
                        //Chapters 
                        int booknum = 0;
                        for (int i = 0; i < BooksInBible; i++)
                        {
                            if (lastSelectedButton == FirstChapternumInBook[i])
                            {
                                booknum = i;
                                break;
                            }
                        }

                        // set up the array for the chapter selection
                        int numOfChapters = ChaptersInBook[booknum];

                        if (numOfChapters <= 1)
                        {
                            return null;
                        }

                        // Color butColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
                        var butColors = new int[numOfChapters];
                        var values = new int[numOfChapters];
                        var butText = new string[numOfChapters];
                        for (int i = 0; i < numOfChapters; i++)
                        {
                            butColors[i] = 0;
                            butText[i] = (i + 1).ToString();
                            values[i] = FirstChapternumInBook[booknum] + i;
                        }

                        // do a nice transition
                        return new ButtonWindowSpecs(
                            stage,
                            "Select a chapter to view",
                            numOfChapters,
                            butColors,
                            butText,
                            values,
                            ButtonSize.Small);
                    }
                case 2:
                    {
                        // verses
                        int booknum = 0;
                        int i;
                        for (i = 0; i < BooksInBible; i++)
                        {
                            if (lastSelectedButton < FirstChapternumInBook[i])
                            {
                                booknum = i - 1;
                                break;
                            }
                        }
                        if (i == BooksInBible)
                        {
                            booknum = BooksInBible - 1;
                        }

                        // set up the array for the verse selection

                        int numOfVerses = VersesInChapter[booknum][lastSelectedButton - FirstChapternumInBook[booknum]];

                        if (numOfVerses <= 1)
                        {
                            return null;
                        }

                        // Color butColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
                        var butColors = new int[numOfVerses];
                        var values = new int[numOfVerses];
                        var butText = new string[numOfVerses];
                        for (i = 0; i < numOfVerses; i++)
                        {
                            butColors[i] = 0;
                            butText[i] = (i + 1).ToString();
                            values[i] = i;
                        }

                        // do a nice transition
                        return new ButtonWindowSpecs(
                            stage, "Select a verse to view", numOfVerses, butColors, butText, values, ButtonSize.Small);
                    }
            }
            return null;
        }

        public virtual async Task<string> GetChapterHtml(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            return
                await
                this.GetChapterHtml(
                    displaySettings,
                    this.Serial.PosChaptNum,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlFontSize,
                    fontFamily,
                    isNotesOnly,
                    addStartFinishHtml,
                    forceReload);
        }

        /// <summary>
        ///     Return the entire chapter
        /// </summary>
        /// <param name="chapterNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <returns>Entire Chapter</returns>
        public async Task<string> GetChapterRaw(int chapterNumber)
        {
            byte[] chapterBuffer = await this.GetChapterBytes(chapterNumber);
            string retValue = Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            return retValue;
        }

        public virtual string GetExternalLink(DisplaySettings displaySettings)
        {
            return string.Empty;
        }

        public string GetFullName(int bookNum)
        {
            if (this.BookNames == null)
            {
                this.BookNames = new BibleNames(this.Serial.Iso2DigitLangCode);
            }

            return this.BookNames.GetFullName(bookNum);
        }

        public virtual void GetInfo(
            out int bookNum,
            out int absoluteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = this.Serial.PosVerseNum;
            absoluteChaptNum = this.Serial.PosChaptNum;
            this.GetInfo(
                this.Serial.PosChaptNum, this.Serial.PosVerseNum, out bookNum, out relChaptNum, out fullName, out title);
        }

        public void GetInfo(
            int chapterNum, int verseNum, out int bookNum, out int relChaptNum, out string fullName, out string title)
        {
            bookNum = 0;
            relChaptNum = 0;
            fullName = string.Empty;
            title = string.Empty;
            if (this.Chapters.Count == 0)
            {
                return;
            }
            try
            {
                ChapterPos chaptPos = this.Chapters[chapterNum];
                bookNum = chaptPos.Booknum;
                relChaptNum = chaptPos.BookRelativeChapterNum;
                bookNum = chaptPos.Booknum;
                relChaptNum = chaptPos.BookRelativeChapterNum;
                fullName = this.GetFullName(bookNum);
                title = fullName + " " + (relChaptNum + 1) + ":" + (verseNum + 1);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("BibleZtextReader.GetInfo; " + ee.Message);
            }
        }

        public virtual string GetLanguage()
        {
            return this.Serial.Iso2DigitLangCode;
        }

        public string GetShortName(int bookNum)
        {
            if (this.BookNames == null)
            {
                this.BookNames = new BibleNames(this.Serial.Iso2DigitLangCode);
            }

            return this.BookNames.GetShortName(bookNum);
        }

        public virtual async Task<object[]> GetTranslateableTexts(DisplaySettings displaySettings, string bibleToLoad)
        {
            var toTranslate = new string[2];
            var isTranslateable = new bool[2];

            int bookNum;
            int relChaptNum;
            string fullName;
            string titleText;
            int verseNum;
            int absoluteChaptNum;

            this.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
            string verseText = await this.GetVerseTextOnly(displaySettings, absoluteChaptNum, verseNum);

            toTranslate[0] = "<p>" + fullName + " " + (relChaptNum + 1) + ":" + (verseNum + 1) + " - " + bibleToLoad
                             + "</p>";
            toTranslate[1] =
                verseText.Replace("<p>", string.Empty)
                         .Replace("</p>", string.Empty)
                         .Replace("<br />", string.Empty)
                         .Replace("\n", " ");
            isTranslateable[0] = false;
            isTranslateable[1] = true;
            return new object[] { toTranslate, isTranslateable };
        }

        /// <summary>
        ///     Return a raw verse.  This is a very ineffective function so use it with caution.
        /// </summary>
        /// <param name="displaySettings"></param>
        /// <param name="chapterNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <param name="verseNumber">Verse number beginning with zero for the first verse</param>
        /// <returns>Verse raw</returns>
        public virtual async Task<string> GetVerseTextOnly(
            DisplaySettings displaySettings, int chapterNumber, int verseNumber)
        {
            byte[] chapterBuffer = await this.GetChapterBytes(chapterNumber);

            // debug only
            // string all = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            VersePos verse = this.Chapters[chapterNumber].Verses[verseNumber];
            int noteMarker = 'a';
            bool isInPoetry = false;
            return this.ParseOsisText(
                displaySettings,
                string.Empty,
                string.Empty,
                chapterBuffer,
                (int)verse.StartPos,
                verse.Length,
                this.Serial.IsIsoEncoding,
                false,
                true,
                ref noteMarker,
                ref isInPoetry,
                true);
        }

        public async Task<List<string>> MakeListDisplayText(
            DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay)
        {
            var returnList = new List<string>();
            bool isInPoetry = false;
            for (int j = listToDisplay.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = listToDisplay[j];
                ChapterPos chaptPos = this.Chapters[place.ChapterNum];
                byte[] chapterBuffer = await this.GetChapterBytes(place.ChapterNum);

                ChapterPos versesForChapterPositions = this.Chapters[place.ChapterNum];

                VersePos verse = versesForChapterPositions.Verses[place.VerseNum];
                int noteMarker = 'a';
                string verseTxt = this.ParseOsisText(
                    displaySettings,
                    this.GetFullName(chaptPos.Booknum) + " " + (chaptPos.BookRelativeChapterNum + 1) + ":"
                    + (place.VerseNum + 1) + "  " + place.When.ToString("yyyy-MM-dd") + " "
                    + place.When.ToString("hh.mm.ss") + "---",
                    string.Empty,
                    chapterBuffer,
                    (int)verse.StartPos,
                    verse.Length,
                    this.Serial.IsIsoEncoding,
                    false,
                    true,
                    ref noteMarker,
                    ref isInPoetry);
                returnList.Add(verseTxt);
            }

            return returnList;
        }

        public virtual void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source)
        {
            if (!(source is BibleZtextReader))
            {
                return;
            }

            try
            {
                // see if the chapter exists, if not, then don't do anything.
                if (this.Chapters != null && chapter < this.Chapters.Count && this.Chapters[chapter].Length > 0)
                {
                    this.Serial.PosChaptNum = chapter;
                    this.Serial.PosVerseNum = verse;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("moveChapterVerse " + e.Message + " ; " + e.StackTrace);
            }
        }

        public virtual void MoveNext()
        {
            this.Serial.PosChaptNum++;
            this.Serial.PosVerseNum = 0;
            if (this.Serial.PosChaptNum >= this.Chapters.Count)
            {
                this.Serial.PosChaptNum = 0;
            }

            for (;
                this.Serial.PosChaptNum < this.Chapters.Count && this.Chapters[this.Serial.PosChaptNum].Length == 0;
                this.Serial.PosChaptNum++)
            {
            }

            if (this.Serial.PosChaptNum >= this.Chapters.Count)
            {
                this.Serial.PosChaptNum = 0;
            }
            else
            {
                return;
            }

            for (;
                this.Serial.PosChaptNum < this.Chapters.Count && this.Chapters[this.Serial.PosChaptNum].Length == 0;
                this.Serial.PosChaptNum++)
            {
            }
        }

        public virtual void MovePrevious()
        {
            this.Serial.PosChaptNum--;
            this.Serial.PosVerseNum = 0;
            if (this.Serial.PosChaptNum < 0)
            {
                this.Serial.PosChaptNum = this.Chapters.Count - 1;
            }

            for (;
                this.Serial.PosChaptNum >= 0 && this.Chapters[this.Serial.PosChaptNum].Length == 0;
                this.Serial.PosChaptNum--)
            {
            }

            if (this.Serial.PosChaptNum < 0)
            {
                this.Serial.PosChaptNum = this.Chapters.Count - 1;
            }
            else
            {
                return;
            }

            for (;
                this.Serial.PosChaptNum >= 0 && this.Chapters[this.Serial.PosChaptNum].Length == 0;
                this.Serial.PosChaptNum--)
            {
            }
        }

        public async Task<string> PutHtmlTofile(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            string fileErase,
            string filePath,
            bool forceReload)
        {
            Debug.WriteLine("putHtmlTofile start");

            ApplicationData appData = ApplicationData.Current;
            StorageFolder folder = await appData.LocalFolder.GetFolderAsync(filePath.Replace("/", "\\"));

            // Find a new file name.
            // Must change the file name, otherwise the browser may or may not update.
            string fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";
            IReadOnlyList<StorageFile> files = null;
            try
            {
                files = await folder.CreateFileQuery().GetFilesAsync();

                // the name must be unique of course
                while (files.Any(p => p.Name.Equals(fileCreate)))
                {
                    fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";
                }
            }
            catch (Exception)
            {
            }

            // delete the old file
            string fileToErase = Path.GetFileName(fileErase);
            if (files != null && files.Any(p => p.Name.Equals(fileToErase)))
            {
                if (this.Serial.PosChaptNum == this._lastShownChapterNumber && !forceReload)
                {
                    // we dont need to rewrite everything. Just rename the file.
                    try
                    {
                        StorageFile fileRenaming = await folder.GetFileAsync(fileToErase);
                        await fileRenaming.RenameAsync(fileCreate);
                        return fileCreate;
                    }
                    catch (Exception ee)
                    {
                        // should never crash here but I have noticed any file rename is a risky business when you have more then one thread.
                        Debug.WriteLine("BibleZtextReader.putHtmlTofile; " + ee.Message);

                        // problems. lets just remake the file.
                    }
                }
                else
                {
                    try
                    {
                        StorageFile fileErasing = await folder.GetFileAsync(fileToErase);
                        await fileErasing.DeleteAsync();
                    }
                    catch (Exception ee)
                    {
                        // should never crash here but I have noticed any file delete is a risky business when you have more then one thread.
                        Debug.WriteLine("BibleZtextReader.putHtmlTofile; " + ee.Message);
                    }
                }
            }

            this._lastShownChapterNumber = this.Serial.PosChaptNum;

            StorageFile file = await folder.CreateFileAsync(fileCreate);
            Stream fs = await file.OpenStreamForWriteAsync();
            var tw = new StreamWriter(fs);
            string fileContent =
                await
                this.GetChapterHtml(
                    displaySettings,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlFontSize,
                    fontFamily,
                    false,
                    true,
                    forceReload);
            tw.Write(fileContent);
            tw.Flush();
            tw.Dispose();
            fs.Dispose();

            Debug.WriteLine("putHtmlTofile end");
            return fileCreate;
        }

        public void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true)
        {
            if (isRegister)
            {
                this.SourceChanged += sourceChangedMethod;
            }
            else
            {
                this.SourceChanged -= sourceChangedMethod;
            }
        }

        public virtual async Task Resume()
        {
            await this.ReloadSettingsFile();
        }

        public virtual void SerialSave()
        {
        }

        #endregion

        #region Methods

        protected static void AppendText(string text, StringBuilder plainText, StringBuilder noteText, bool isInElement)
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

        protected string[] GetAllShortNames()
        {
            if (this.BookNames == null)
            {
                this.BookNames = new BibleNames(this.Serial.Iso2DigitLangCode);
            }

            return this.BookNames.GetAllShortNames();
        }

        protected int GetByteFromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[1];
            isEnd = fs.Read(buf, 0, 1) != 1;
            return buf[0];
        }

        protected async Task<byte[]> GetChapterBytes(int chapterNumber)
        {
            //Debug.WriteLine("getChapterBytes start");
            int numberOfChapters = this.Chapters.Count;
            if (numberOfChapters == 0)
            {
                return Encoding.UTF8.GetBytes("Does not exist");
            }
            if (chapterNumber >= numberOfChapters)
            {
                chapterNumber = numberOfChapters - 1;
            }

            if (chapterNumber < 0)
            {
                chapterNumber = 0;
            }

            ChapterPos versesForChapterPositions = this.Chapters[chapterNumber];
            long bookStartPos = versesForChapterPositions.BookStartPos;
            long blockStartPos = versesForChapterPositions.StartPos;
            long blockLen = versesForChapterPositions.Length;
            Stream fs;

            string fileName = (chapterNumber < ChaptersInOt) ? "ot." : "nt.";
            try
            {
                //Windows.Storage.ApplicationData appData = Windows.Storage.ApplicationData.Current;
                //var folder = await appData.LocalFolder.GetFolderAsync(Serial.Path.Replace("/", "\\"));
                string filenameComplete = this.Serial.Path + fileName + ((char)this.BlockType) + "zz";
                fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filenameComplete.Replace("/", "\\"));
                //fs = await file.OpenStreamForReadAsync(); 
            }
            catch (Exception ee)
            {
                // does not exist
                return Encoding.UTF8.GetBytes("Does not exist");
            }

            // adjust the start postion of the stream to where this book begins.
            // we must read the entire book up to the chapter we want even though we just want one chapter.
            fs.Position = bookStartPos;
            ZInputStream zipStream;
            zipStream = string.IsNullOrEmpty(this.Serial.CipherKey) ? new ZInputStream(fs) : new ZInputStream(new SapphireStream(fs,this.Serial.CipherKey));

            var chapterBuffer = new byte[blockLen];
            int totalBytesRead = 0;
            int totalBytesCopied = 0;
            int len = 0;
            try
            {
                var buffer = new byte[10000];
                while (true)
                {
                    try
                    {
                        len = zipStream.read(buffer, 0, 10000);
                    }
                    catch (Exception ee)
                    {
                        Debug.WriteLine("caught a unzip crash 4.2" + ee);
                    }

                    if (len <= 0)
                    {
                        // we should never come to this point.  Just here as a safety procaution
                        break;
                    }

                    totalBytesRead += len;
                    if (totalBytesRead >= blockStartPos)
                    {
                        // we are now inside of where the chapter we want is so we need to start saving it.
                        int startOffset = 0;
                        if (totalBytesCopied == 0)
                        {
                            // but our actual chapter might begin in the middle of the buffer.  Find the offset from the
                            // beginning of the buffer.
                            startOffset = len - (totalBytesRead - (int)blockStartPos);
                        }

                        int i;
                        for (i = totalBytesCopied; i < blockLen && (i - totalBytesCopied) < (len - startOffset); i++)
                        {
                            chapterBuffer[i] = buffer[i - totalBytesCopied + startOffset];
                        }

                        totalBytesCopied += len - startOffset;
                        if (totalBytesCopied >= blockLen)
                        {
                            // we are done. no more reason to read anymore of this book stream, just get out.
                            break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("BibleZtextReader.getChapterBytes crash; " + ee.Message);
            }

            fs.Dispose();
            zipStream.Dispose();
            return chapterBuffer;
        }

        public async Task<bool> IsCipherKeyGood(string testKey)
        {
            var oldCipher = this.Serial.CipherKey;
            this.Serial.CipherKey = testKey;
            byte[] chapterBuffer = await this.GetChapterBytes(this.Serial.PosChaptNum);
            this.Serial.CipherKey = oldCipher;
            return chapterBuffer.Any(p => p != 0);
        }

        /// <summary>
        ///     Return the entire chapter without notes and with lots of html markup
        /// </summary>
        /// <param name="displaySettings"></param>
        /// <param name="chapterNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <param name="htmlBackgroundColor"></param>
        /// <param name="htmlForegroundColor"></param>
        /// <param name="htmlFontSize"></param>
        /// <param name="htmlPhoneAccentColor"></param>
        /// <param name="fontFamily"></param>
        /// <param name="isNotesOnly"></param>
        /// <param name="addStartFinishHtml"></param>
        /// <param name="forceReload"></param>
        /// <returns>Entire Chapter without notes and with lots of html markup for each verse</returns>
        protected async Task<string> GetChapterHtml(
            DisplaySettings displaySettings,
            int chapterNumber,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            if (this.Chapters.Count == 0)
            {
                return string.Empty;
            }

            Debug.WriteLine("GetChapterHtml start");
            var htmlChapter = new StringBuilder();
            ChapterPos versesForChapterPositions = this.Chapters[chapterNumber];
            string chapterStartHtml = string.Empty;
            string chapterEndHtml = string.Empty;
            if (addStartFinishHtml)
            {
                chapterStartHtml = HtmlHeader(
                    displaySettings,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlFontSize,
                    fontFamily);
                chapterEndHtml = "</body></html>";
            }

            string bookName = string.Empty;
            if (displaySettings.ShowBookName)
            {
                bookName = this.GetFullName(versesForChapterPositions.Booknum);
            }

            bool isVerseMarking = displaySettings.ShowBookName || displaySettings.ShowChapterNumber
                                  || displaySettings.ShowVerseNumber;
            string startVerseMarking = displaySettings.SmallVerseNumbers
                                           ? "<sup>"
                                           : (isVerseMarking ? "<span class=\"strongsmorph\">(" : string.Empty);
            string stopVerseMarking = displaySettings.SmallVerseNumbers
                                          ? "</sup>"
                                          : (isVerseMarking ? ")</span>" : string.Empty);
            int noteIdentifier = 'a';

            // in some commentaries, the verses repeat. Stop these repeats from comming in!
            var verseRepeatCheck = new Dictionary<long, int>();
            bool isInPoetry = false;

            // if the bible is locked and there is no key. Look for a key.
            if (this.Serial.CipherKey != null && this.Serial.CipherKey.Length == 0)
            {
                try
                {
                    string filenameComplete = this.Serial.Path + "CipherKey.txt";
                    var fs =
                        await
                        ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filenameComplete.Replace("/", "\\"));
                    // get the key from the file.
                    var buf = new byte[1000];
                    var len = await fs.ReadAsync(buf, 0, 1000);
                    this.Serial.CipherKey = Encoding.UTF8.GetString(buf,0,len);
                }
                catch (Exception ee)
                {
                }
                if (this.Serial.CipherKey.Length == 0)
                {
                    try
                    {
                        string filenameComplete = this.Serial.ConfigPath;
                        var fs =
                            await
                            ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                                this.Serial.ConfigPath.Replace("/", "\\"));
                        // show the about information instead
                        var config = new SwordBookMetaData(fs, "xx");
                        fs.Dispose();
                        return chapterStartHtml + "This bible is locked. Go to the menu to enter the key.<br /><br />"
                               + ((string)config.GetCetProperty(ConfigEntryType.About)).Replace("\\par", "<br />")
                                                                                       .Replace("\\qc", "")
                               + chapterEndHtml;
                    }
                    catch (Exception e)
                    {
                        // does not exist
                    }
                }
            }
            byte[] chapterBuffer = await this.GetChapterBytes(chapterNumber);

            // for debug
            //string xxxxxx = Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            //Debug.WriteLine("RawChapter: " + xxxxxx);

            for (int i = 0; i < versesForChapterPositions.Verses.Count; i++)
            {
                VersePos verse = versesForChapterPositions.Verses[i];
                string htmlChapterText = startVerseMarking
                                         + (displaySettings.ShowBookName ? bookName + " " : string.Empty)
                                         + (displaySettings.ShowChapterNumber
                                                ? ((versesForChapterPositions.BookRelativeChapterNum + 1) + ":")
                                                : string.Empty)
                                         + (displaySettings.ShowVerseNumber ? (i + 1).ToString() : string.Empty)
                                         + stopVerseMarking;
                string verseTxt;
                string id = "CHAP_" + chapterNumber + "_VERS_" + i;
                string restartText = "<a name=\"" + id + "\"></a><a class=\"normalcolor\" id=\"" + id
                                     + "\" href=\"#\" onclick=\"window.external.notify('" + id
                                     + "'); event.returnValue=false; return false;\" > ";
                string startText = htmlChapterText + restartText;
                if (!verseRepeatCheck.ContainsKey(verse.StartPos))
                {
                    verseRepeatCheck[verse.StartPos] = 0;

                    verseTxt = "*** ERROR ***";
                    try
                    {
                        verseTxt = this.ParseOsisText(
                            displaySettings,
                            startText,
                            restartText,
                            chapterBuffer,
                            (int)verse.StartPos,
                            verse.Length,
                            this.Serial.IsIsoEncoding,
                            isNotesOnly,
                            false,
                            ref noteIdentifier,
                            ref isInPoetry);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(verse.Length + ";" + verse.StartPos + ";" + e);
                    }
                }
                else
                {
                    verseTxt = "<p>" + startText + "</p>";
                }

                // create the verse
                htmlChapter.Append(
                    (displaySettings.EachVerseNewLine ? "<p>" : string.Empty) + chapterStartHtml + verseTxt
                    + (verseTxt.Length > 0 ? (displaySettings.EachVerseNewLine ? "</a></p>" : "</a>") : string.Empty));
                chapterStartHtml = string.Empty;
            }

            htmlChapter.Append(chapterEndHtml);
            Debug.WriteLine("GetChapterHtml end");
            return htmlChapter.ToString();
        }

        protected long GetInt48FromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[6];
            isEnd = fs.Read(buf, 0, 6) != 6;
            if (isEnd)
            {
                return 0;
            }

            return buf[1] * 0x100000000000 + buf[0] * 0x100000000 + buf[5] * 0x1000000 + buf[4] * 0x10000
                   + buf[3] * 0x100 + buf[2];
        }

        protected int GetShortIntFromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[2];
            isEnd = fs.Read(buf, 0, 2) != 2;
            if (isEnd)
            {
                return 0;
            }

            return buf[1] * 0x100 + buf[0];
        }

        protected long GetintFromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[4];
            isEnd = fs.Read(buf, 0, 4) != 4;
            if (isEnd)
            {
                return 0;
            }

            return buf[3] * 0x100000 + buf[2] * 0x10000 + buf[1] * 0x100 + buf[0];
        }

        protected async Task<string> MakeListDisplayText(
            DisplaySettings displaySettings,
            List<BiblePlaceMarker> listToDisplay,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool showBookTitles,
            string notesTitle)
        {
            if (htmlBackgroundColor == null)
            {
                // must wait a little until we get these values.
                return string.Empty;
            }

            string chapterStartHtml = HtmlHeader(
                displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlFontSize,
                fontFamily);
            const string chapterEndHtml = "</body></html>";
            var htmlListText = new StringBuilder(chapterStartHtml);
            int lastBookNum = -1;
            bool isVerseMarking = displaySettings.ShowBookName || displaySettings.ShowChapterNumber
                                  || displaySettings.ShowVerseNumber;
            string startVerseMarking = displaySettings.SmallVerseNumbers
                                           ? "<sup>"
                                           : (isVerseMarking ? "(" : string.Empty);
            string stopVerseMarking = displaySettings.SmallVerseNumbers
                                          ? "</sup>"
                                          : (isVerseMarking ? ")" : string.Empty);
            bool isInPoetry = false;
            for (int j = listToDisplay.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = listToDisplay[j];
                ChapterPos chaptPos = this.Chapters[place.ChapterNum];
                byte[] chapterBuffer = await this.GetChapterBytes(place.ChapterNum);

                // for debug
                // string all = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
                ChapterPos versesForChapterPositions = this.Chapters[place.ChapterNum];

                if (showBookTitles && lastBookNum != chaptPos.Booknum)
                {
                    htmlListText.Append("<h3>" + this.GetFullName(chaptPos.Booknum) + "</h3>");
                    lastBookNum = chaptPos.Booknum;
                }

                string htmlChapterText = startVerseMarking + this.GetFullName(chaptPos.Booknum) + " "
                                         + (chaptPos.BookRelativeChapterNum + 1) + ":" + (place.VerseNum + 1) + "  "
                                         + place.When.ToString("yyyy-MM-dd") + " " + place.When.ToString("hh.mm.ss")
                                         + stopVerseMarking;

                string textId = "CHAP_" + place.ChapterNum + "_VERS_" + place.VerseNum;
                VersePos verse = versesForChapterPositions.Verses[place.VerseNum];
                int noteMarker = 'a';
                string verseTxt = this.ParseOsisText(
                    displaySettings,
                    "<p><a name=\"" + textId + "\"></a><a class=\"normalcolor\" id=\"" + textId
                    + "\"  href=\"#\" onclick=\"window.external.notify('" + textId
                    + "'); event.returnValue=false; return false;\" >" + htmlChapterText,
                    string.Empty,
                    chapterBuffer,
                    (int)verse.StartPos,
                    verse.Length,
                    this.Serial.IsIsoEncoding,
                    false,
                    true,
                    ref noteMarker,
                    ref isInPoetry);

                // create the verse
                if (string.IsNullOrEmpty(place.Note))
                {
                    htmlListText.Append(verseTxt + "</a></p><hr />");
                }
                else
                {
                    htmlListText.Append(
                        verseTxt + "</p><p>" + (displaySettings.SmallVerseNumbers ? "<sup>" : "(") + notesTitle
                        + (displaySettings.SmallVerseNumbers ? "</sup>" : ") ") + place.Note + "</a></p><hr />");
                }
            }

            htmlListText.Append(chapterEndHtml);
            return htmlListText.ToString();
        }

        protected string ParseOsisText(
            DisplaySettings displaySettings,
            string chapterNumber,
            string restartText,
            byte[] xmlbytes,
            int startPos,
            int length,
            bool isIsoText,
            bool isNotesOnly,
            bool noTitles,
            ref int noteIdentifier,
            ref bool isInPoetry,
            bool isRaw = false)
        {
            var ms = new MemoryStream();
            if (isIsoText)
            {
                ms.Write(PrefixIso, 0, PrefixIso.Length);
            }
            else
            {
                ms.Write(Prefix, 0, Prefix.Length);
            }

            // Some indexes are bad. make sure the startpos and length are not bad
            if (length == 0)
            {
                return string.Empty;
            }

            if (startPos >= xmlbytes.Length)
            {
                Debug.WriteLine("Bad startpos;" + xmlbytes.Length + ";" + startPos + ";" + length);
                return "*** POSSIBLE ERROR IN BIBLE, TEXT MISSING HERE ***";
            }

            if (startPos + length > xmlbytes.Length)
            {
                // we can fix this
                Debug.WriteLine("Fixed bad length;" + xmlbytes.Length + ";" + startPos + ";" + length);
                length = xmlbytes.Length - startPos - 1;
                if (length == 0)
                {
                    // this might be a problem or it might not. Put some stars here anyway.
                    return "***";
                }
            }

            try
            {
                ms.Write(xmlbytes, startPos, length);
                ms.Write(Suffix, 0, Suffix.Length);
                ms.Position = 0;

                // debug
                //byte[] buf = new byte[ms.Length]; ms.Read(buf, 0, (int)ms.Length);
                //string xxxxxx = System.Text.UTF8Encoding.UTF8.GetString(buf, 0, buf.Length);
                //System.Diagnostics.Debug.WriteLine("osisbuf: " + xxxxxx);
                ms.Position = 0;
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }

            var plainText = new StringBuilder();
            var noteText = new StringBuilder();
            var settings = new XmlReaderSettings { IgnoreWhitespace = false };

            bool isInElement = false;
            bool isInQuote = false;
            bool isInInjectionElement = false;
            bool isInTitle = false;
            bool isChaptNumGiven = false;
            bool isChaptNumGivenNotes = false;
            bool isReferenceLinked = false;
            int isLastElementLineBreak = 0;
            string lemmaText = string.Empty;
            string morphText = string.Empty;
            using (XmlReader reader = XmlReader.Create(ms, settings))
            {
                try
                {
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        if (isLastElementLineBreak >= 1)
                        {
                            if (isLastElementLineBreak >= 2)
                            {
                                isLastElementLineBreak = 0;
                            }
                            else
                            {
                                isLastElementLineBreak = 2;
                            }
                        }

                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "CM":
                                        if (!isRaw && !displaySettings.EachVerseNewLine && isLastElementLineBreak == 0)
                                        {
                                            AppendText("<br />", plainText, noteText, isInElement);
                                            isLastElementLineBreak = 1;
                                        }

                                        break;
                                    case "lb":
                                        if (!isRaw && !displaySettings.EachVerseNewLine)
                                        {
                                            string paragraphXml = isLastElementLineBreak == 0 ? "<br />" : " ";
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();
                                                if (reader.Name.Equals("type"))
                                                {
                                                    {
                                                        paragraphXml = reader.Value.Equals("x-end-paragraph")
                                                                           ? "</p>"
                                                                           : (reader.Value.Equals("x-begin-paragraph")
                                                                                  ? "<p>"
                                                                                  : "<br />");
                                                    }
                                                }
                                            }

                                            AppendText(paragraphXml, plainText, noteText, isInElement);
                                            isLastElementLineBreak = 1;
                                        }

                                        break;
                                    case "title":
                                        isInTitle = true;
                                        if (!(noTitles || !displaySettings.ShowHeadings) && !isRaw)
                                        {
                                            AppendText("<h3>", plainText, noteText, isInElement);
                                        }

                                        break;
                                    case "reference":
                                        if (reader.HasAttributes)
                                        {
                                            reader.MoveToFirstAttribute();
                                            if (reader.Name.Equals("osisRef"))
                                            {
                                                int chaptNumLoc;
                                                int verseNumLoc;
                                                if (ConvertOsisRefToAbsoluteChaptVerse(
                                                    reader.Value, out chaptNumLoc, out verseNumLoc))
                                                {
                                                    isReferenceLinked = true;
                                                    string textId = "CHAP_" + chaptNumLoc + "_VERS_" + verseNumLoc;
                                                    noteText.Append(
                                                        "</a><a class=\"normalcolor\" id=\"" + textId
                                                        + "\"  href=\"#\" onclick=\"window.external.notify('" + textId
                                                        + "'); event.returnValue=false; return false;\" >");
                                                }
                                            }
                                        }

                                        noteText.Append("  [");
                                        break;
                                    case "lg":
                                        if (!isRaw && !displaySettings.EachVerseNewLine)
                                        {
                                            if (isInPoetry)
                                            {
                                                isInPoetry = false;
                                                AppendText("</blockquote>", plainText, noteText, isInElement);
                                            }
                                            else
                                            {
                                                isInPoetry = true;
                                                AppendText(
                                                    "<blockquote style=\"margin: 0 0 0 1.5em;padding 0 0 0 0;\">",
                                                    plainText,
                                                    noteText,
                                                    isInElement);
                                            }

                                            isLastElementLineBreak = 1;
                                        }

                                        break;
                                    case "l":
                                        if (!isRaw && !displaySettings.EachVerseNewLine && isLastElementLineBreak == 0)
                                        {
                                            AppendText(isInPoetry ? "<br />" : " ", plainText, noteText, isInElement);
                                            isLastElementLineBreak = 1;
                                        }

                                        break;
                                    case "FI":
                                        if (!isRaw && !isNotesOnly && displaySettings.ShowNotePositions)
                                        {
                                            plainText.Append(
                                                (displaySettings.SmallVerseNumbers ? "<sup>" : string.Empty)
                                                + this.convertNoteNumToId(noteIdentifier)
                                                + (displaySettings.SmallVerseNumbers ? "</sup>" : string.Empty));
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
                                        if (!isRaw && !isNotesOnly && displaySettings.ShowNotePositions)
                                        {
                                            plainText.Append(
                                                (displaySettings.SmallVerseNumbers ? "<sup>" : string.Empty)
                                                + this.convertNoteNumToId(noteIdentifier)
                                                + (displaySettings.SmallVerseNumbers ? "</sup>" : string.Empty));
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
                                            AppendText("<i>", plainText, noteText, isInElement);
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
                                        if (!isRaw && !isNotesOnly)
                                        {
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();
                                                do
                                                {
                                                    if (displaySettings.WordsOfChristRed && reader.Name.Equals("who"))
                                                    {
                                                        if (reader.Value.ToLower().Equals("jesus"))
                                                        {
                                                            AppendText(
                                                                "<span class=\"christ\">",
                                                                plainText,
                                                                noteText,
                                                                isInElement);
                                                            isInQuote = true;
                                                        }
                                                    }

                                                    if (reader.Name.Equals("marker"))
                                                    {
                                                        AppendText(reader.Value, plainText, noteText, isInElement);
                                                    }
                                                }
                                                while (reader.MoveToNextAttribute());
                                            }
                                        }

                                        break;
                                    case "w":

                                        // <w lemma="strong:G1078" morph="robinson:N-GSF">γενεσεως</w>
                                        if ((displaySettings.ShowStrongsNumbers || displaySettings.ShowMorphology)
                                            && !isRaw && !isNotesOnly)
                                        {
                                            lemmaText = string.Empty;
                                            morphText = string.Empty;
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();

                                                do
                                                {
                                                    if (displaySettings.ShowStrongsNumbers
                                                        && reader.Name.Equals("lemma"))
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

                                                                lemmaText +=
                                                                    "<a class=\"strongsmorph\" href=\"#\" onclick=\"window.external.notify('STRONG_"
                                                                    + lemma.Substring(7)
                                                                    + "'); event.returnValue=false; return false;\" >"
                                                                    + lemma.Substring(8) + "</a>";
                                                            }
                                                        }
                                                    }
                                                    else if (displaySettings.ShowMorphology
                                                             && reader.Name.Equals("morph"))
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

                                                                morphText +=
                                                                    "<a class=\"strongsmorph\" href=\"#\" onclick=\"window.external.notify('MORPH_"
                                                                    + subMorph
                                                                    + "'); event.returnValue=false; return false;\" >"
                                                                    + subMorph + "</a>";
                                                            }
                                                        }
                                                    }
                                                }
                                                while (reader.MoveToNextAttribute());
                                            }
                                        }

                                        break;

                                    case "versee":
                                        AppendText(" ", plainText, noteText, isInElement);
                                        break;
                                    default:
                                        AppendText(" ", plainText, noteText, isInElement);
                                        Debug.WriteLine("Element untreated: " + reader.Name);
                                        break;
                                }

                                break;
                            case XmlNodeType.Text:
                                if (!isInElement && !isInInjectionElement && chapterNumber.Length > 0 && !isInTitle
                                    && !isChaptNumGiven)
                                {
                                    if (isInQuote)
                                    {
                                        AppendText("</span>", plainText, noteText, isInElement);
                                    }

                                    plainText.Append(chapterNumber);
                                    if (isInQuote)
                                    {
                                        AppendText("<span class=\"christ\">", plainText, noteText, isInElement);
                                    }

                                    isChaptNumGiven = true;
                                }

                                string text;
                                try
                                {
                                    text = reader.Value;
                                }
                                catch (Exception e1)
                                {
                                    Debug.WriteLine("error in text: " + e1.Message);
                                    try
                                    {
                                        text = reader.Value;
                                    }
                                    catch (Exception e2)
                                    {
                                        Debug.WriteLine("second error in text: " + e2.Message);
                                        text = "*error*";
                                    }
                                }

                                if ((!(noTitles || !displaySettings.ShowHeadings) || !isInTitle) && text.Length > 0)
                                {
                                    char firstChar = text[0];
                                    AppendText(
                                        ((!firstChar.Equals(',') && !firstChar.Equals('.') && !firstChar.Equals(':')
                                          && !firstChar.Equals(';') && !firstChar.Equals('?'))
                                             ? " "
                                             : string.Empty) + text,
                                        plainText,
                                        noteText,
                                        isInElement || isInInjectionElement);
                                }

                                break;
                            case XmlNodeType.EndElement:
                                switch (reader.Name)
                                {
                                    case "title":
                                        if (!(noTitles || !displaySettings.ShowHeadings) && !isRaw)
                                        {
                                            AppendText("</h3>", plainText, noteText, isInElement);
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
                                            AppendText("</i>", plainText, noteText, isInElement);
                                        }

                                        break;
                                    case "q":
                                        if (isInQuote)
                                        {
                                            AppendText("</span>", plainText, noteText, isInElement);
                                            isInQuote = false;
                                        }

                                        break;
                                    case "w":

                                        // <w lemma="strong:G1078" morph="robinson:N-GSF">γενεσεως</w>
                                        if ((displaySettings.ShowStrongsNumbers || displaySettings.ShowMorphology)
                                            && !isRaw && !isNotesOnly
                                            && (!string.IsNullOrEmpty(lemmaText) || !string.IsNullOrEmpty(morphText)))
                                        {
                                            plainText.Append(
                                                "</a>"
                                                + (displaySettings.SmallVerseNumbers
                                                       ? "<sub>"
                                                       : "<span class=\"strongsmorph\">(</span>"));
                                            if (!string.IsNullOrEmpty(lemmaText))
                                            {
                                                plainText.Append(lemmaText);
                                            }

                                            if (!string.IsNullOrEmpty(morphText))
                                            {
                                                plainText.Append(
                                                    (string.IsNullOrEmpty(lemmaText) ? string.Empty : ",") + morphText);
                                            }

                                            plainText.Append(
                                                (displaySettings.SmallVerseNumbers
                                                     ? "</sub>"
                                                     : "<span class=\"strongsmorph\">)</span>") + restartText);
                                            lemmaText = string.Empty;
                                            morphText = string.Empty;
                                        }

                                        break;
                                    case "lg":
                                        if (!isRaw && !displaySettings.EachVerseNewLine)
                                        {
                                            isInPoetry = false;
                                            AppendText("</blockquote>", plainText, noteText, isInElement);
                                        }

                                        break;
                                    case "l":
                                        if (!isRaw && !displaySettings.EachVerseNewLine)
                                        {
                                            AppendText(" ", plainText, noteText, isInElement);
                                        }

                                        break;
                                    case "versee":
                                        AppendText(" ", plainText, noteText, isInElement);
                                        break;
                                    default:
                                        AppendText(" ", plainText, noteText, isInElement);
                                        Debug.WriteLine("EndElement untreated: " + reader.Name);
                                        break;
                                }

                                break;
                        }
                    }
                }
                catch (Exception e)
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

            // this replace fixes a character translation problem for slanted apostrophy
            return plainText.ToString().Replace('\x92', '\'');
        }

        protected void RaiseSourceChangedEvent()
        {
            if (this.SourceChanged != null)
            {
                this.SourceChanged();
            }
        }

        private async Task<bool> ReloadOneIndex(string filename, int startBook, int endBook)
        {
            if (string.IsNullOrEmpty(this.Serial.Path))
            {
                return false;
            }
            try
            {
                var bookPositions = new List<BookPos>();
                //Windows.Storage.ApplicationData appData = Windows.Storage.ApplicationData.Current;
                Stream fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + filename + ((char)this.BlockType) + "zs");
                //var folder = await appData.LocalFolder.GetFolderAsync(Serial.Path.Replace("/", "\\"));
                //var file = await folder.CreateFileAsync(filename + ((char)BlockType) + "zs");
                //var fs = await file.OpenStreamForReadAsync(); 
                bool isEnd;

                // read book position index
                for (int i = 0;; i++)
                {
                    long startPos = this.GetintFromStream(fs, out isEnd);
                    if (isEnd)
                    {
                        break;
                    }

                    long length = this.GetintFromStream(fs, out isEnd);
                    if (isEnd)
                    {
                        break;
                    }

                    long unused = this.GetintFromStream(fs, out isEnd);
                    if (isEnd)
                    {
                        break;
                    }

                    bookPositions.Add(new BookPos(startPos, length, unused));
                }

                fs.Dispose();

                // read the verse holder for versification bzv file
                //file = await folder.CreateFileAsync(filename + ((char)BlockType) + "zv");
                fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + filename + ((char)this.BlockType) + "zv");

                // dump the first 4 posts
                for (int i = 0; i < 4; i++)
                {
                    this.GetShortIntFromStream(fs, out isEnd);
                    this.GetInt48FromStream(fs, out isEnd);
                    this.GetShortIntFromStream(fs, out isEnd);
                }

                // now start getting each chapter in each book
                for (int i = startBook; i < endBook; i++)
                {
                    for (int j = 0; j < ChaptersInBook[i]; j++)
                    {
                        long chapterStartPos = 0;
                        ChapterPos chapt = null;
                        long lastNonZeroStartPos = 0;
                        long lastNonZeroLength = 0;
                        int length = 0;
                        for (int k = 0; k < VersesInChapter[i][j]; k++)
                        {
                            int booknum = this.GetShortIntFromStream(fs, out isEnd);
                            long startPos = this.GetInt48FromStream(fs, out isEnd);
                            if (startPos != 0)
                            {
                                lastNonZeroStartPos = startPos;
                            }

                            length = this.GetShortIntFromStream(fs, out isEnd);
                            if (length != 0)
                            {
                                lastNonZeroLength = length;
                            } 
                            
                            if (k == 0)
                            {
                                chapterStartPos = startPos;
                                long bookStartPos = 0;
                                if (booknum < bookPositions.Count)
                                {
                                    bookStartPos = bookPositions[booknum].StartPos;
                                }

                                if (this.BlockType == IndexingBlockType.Chapter)
                                {
                                    chapterStartPos = 0;
                                }

                                chapt = new ChapterPos(chapterStartPos, i, j, bookStartPos);
                            }

                            if (booknum == 0 && startPos == 0 && length == 0)
                            {
                                if (chapt != null)
                                {
                                    chapt.Verses.Add(new VersePos(0, 0, i));
                                }
                            }
                            else
                            {
                                if (chapt != null)
                                {
                                    chapt.Verses.Add(new VersePos(startPos - chapterStartPos, length, i));
                                }
                            }
                        }

                        // update the chapter length now that we know it
                        if (chapt != null)
                        {
                            chapt.Length = (int)(lastNonZeroStartPos - chapterStartPos) + lastNonZeroLength;
                            this.Chapters.Add(chapt);
                        }

                        // dump a post for the chapter break
                        this.GetShortIntFromStream(fs, out isEnd);
                        this.GetInt48FromStream(fs, out isEnd);
                        this.GetShortIntFromStream(fs, out isEnd);
                    }

                    // dump a post for the book break
                    this.GetShortIntFromStream(fs, out isEnd);
                    this.GetInt48FromStream(fs, out isEnd);
                    this.GetShortIntFromStream(fs, out isEnd);
                }

                fs.Dispose();
            }
            catch (Exception)
            {
                // failed to load old testement.  Maybe it does not exist.
                // fill with fake data
                for (int i = startBook; i < endBook; i++)
                {
                    for (int j = 0; j < ChaptersInBook[i]; j++)
                    {
                        var chapt = new ChapterPos(0, i, j, 0);
                        for (int k = 0; k < VersesInChapter[i][j]; k++)
                        {
                            chapt.Verses.Add(new VersePos(0, 0, i));
                        }

                        // update the chapter length now that we know it
                        chapt.Length = 0;
                        this.Chapters.Add(chapt);
                    }
                }
            }
            return true;
        }

        private async Task<bool> ReloadSettingsFile()
        {
            this.Chapters = new List<ChapterPos>();
            this.BlockType = IndexingBlockType.Book;
            // the name must be unique of course
            if (await FileExists(this.Serial.Path + "ot.czs") || await FileExists(this.Serial.Path + "nt.czs"))
            {
                this.BlockType = IndexingBlockType.Chapter;
            }

            await this.ReloadOneIndex("ot.", 0, BooksInOt);
            await this.ReloadOneIndex("nt.", BooksInOt, BooksInOt + BooksInNt);
            return true;
        }

        public virtual void SetToFirstChapter()
        {
            // find the first available chapter.
            this.Serial.PosChaptNum = 0;
            this.MoveNext();
            if (this.Serial.PosChaptNum == 1)
            {
                this.MovePrevious();
                if (this.Serial.PosChaptNum != 0)
                {
                    this.MoveNext();
                }
            }
        }

        private string convertNoteNumToId(int noteIdentifier)
        {
            string noteReturned = string.Empty;
            string startChar = ((char)((noteIdentifier - 'a') % 24 + 'a')).ToString();
            int numChars = (noteIdentifier - 'a') / 24;
            for (int i = 0; i <= numChars; i++)
            {
                noteReturned += startChar;
            }

            return "(" + noteReturned + ")";
        }

        #endregion

        [DataContract]
        public struct VersePos
        {
            #region Fields

            [DataMember(Name = "booknum")]
            public int Booknum;

            [DataMember(Name = "length")]
            public int Length;

            [DataMember(Name = "startPos")]
            public long StartPos;

            #endregion

            #region Constructors and Destructors

            public VersePos(long startPos, int length, int booknum)
            {
                this.StartPos = startPos;
                this.Length = length;
                this.Booknum = booknum;
            }

            #endregion

            #region Public Methods and Operators

            public bool Equals(VersePos equalsTo)
            {
                return equalsTo.Booknum == this.Booknum && equalsTo.Length == this.Length
                       && equalsTo.StartPos == this.StartPos;
            }

            #endregion
        }

        [DataContract(IsReference = true)]
        [KnownType(typeof(ChapterPos))]
        public class BookPos
        {
            #region Fields

            [DataMember(Name = "length")]
            public long Length;

            [DataMember(Name = "listChapters")]
            public List<ChapterPos> ListChapters = new List<ChapterPos>();

            [DataMember(Name = "startPos")]
            public long StartPos;

            [DataMember(Name = "unused")]
            public long Unused;

            #endregion

            #region Constructors and Destructors

            public BookPos(long startPos, long length, long unused)
            {
                this.StartPos = startPos;
                this.Length = length;
                this.Unused = unused;
            }

            #endregion
        }

        [DataContract(IsReference = true)]
        [KnownType(typeof(VersePos))]
        public class ChapterPos
        {
            #region Fields

            [DataMember(Name = "bookRelativeChapterNum")]
            public int BookRelativeChapterNum;

            [DataMember(Name = "bookStartPos")]
            public long BookStartPos;

            [DataMember(Name = "booknum")]
            public int Booknum;

            [DataMember(Name = "length")]
            public long Length;

            [DataMember(Name = "startPos")]
            public long StartPos;

            [DataMember(Name = "verses")]
            public List<VersePos> Verses = new List<VersePos>();

            #endregion

            #region Constructors and Destructors

            public ChapterPos(long startPos, int booknum, int bookRelativeChapterNum, long bookStartPos)
            {
                this.StartPos = startPos;
                this.Booknum = booknum;
                this.BookRelativeChapterNum = bookRelativeChapterNum;
                this.BookStartPos = bookStartPos;
            }

            #endregion
        }
    }

    [DataContract]
    public class BibleZtextReaderSerialData
    {
        #region Fields

        [DataMember(Name = "isIsoEncoding")]
        public bool IsIsoEncoding;

        [DataMember(Name = "iso2DigitLangCode")]
        public string Iso2DigitLangCode = string.Empty;

        [DataMember(Name = "path")]
        public string Path = string.Empty;

        [DataMember(Name = "posChaptNum")]
        public int PosChaptNum;

        [DataMember(Name = "posVerseNum")]
        public int PosVerseNum;

        [DataMember(Name = "CipherKey")]
        public string CipherKey;

        [DataMember(Name = "ConfigPath")]
        public string ConfigPath;

        #endregion

        #region Constructors and Destructors

        public BibleZtextReaderSerialData(BibleZtextReaderSerialData from)
        {
            this.CloneFrom(from);
        }

        public BibleZtextReaderSerialData(
            bool isIsoEncoding, string iso2DigitLangCode, string path, int posChaptNum, int posVerseNum, string cipherKey, string configPath)
        {
            this.IsIsoEncoding = isIsoEncoding;
            this.Iso2DigitLangCode = iso2DigitLangCode;
            this.Path = path;
            this.PosChaptNum = posChaptNum;
            this.PosVerseNum = posVerseNum;
            this.CipherKey = cipherKey;
            this.ConfigPath = configPath;
        }

        #endregion

        #region Public Methods and Operators

        public void CloneFrom(BibleZtextReaderSerialData from)
        {
            this.IsIsoEncoding = from.IsIsoEncoding;
            this.Iso2DigitLangCode = from.Iso2DigitLangCode;
            this.Path = from.Path;
            this.PosChaptNum = from.PosChaptNum;
            this.PosVerseNum = from.PosVerseNum;
            this.CipherKey = from.CipherKey;
            this.ConfigPath = from.ConfigPath;
        }

        #endregion
    }
}