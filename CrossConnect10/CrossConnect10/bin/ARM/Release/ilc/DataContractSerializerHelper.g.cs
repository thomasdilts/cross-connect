using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Xml;

[assembly: global::System.Reflection.AssemblyVersion("4.0.0.0")]



namespace System.Runtime.Serialization.Generated
{
    [global::System.Runtime.CompilerServices.__BlockReflection]
    public static partial class DataContractSerializerHelper
    {
        public static void InitDataContracts()
        {
            global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Runtime.Serialization.DataContract> dataContracts = global::System.Runtime.Serialization.DataContract.GetDataContracts();
            PopulateContractDictionary(dataContracts);
            global::System.Collections.Generic.Dictionary<global::System.Runtime.Serialization.DataContract, global::System.Runtime.Serialization.Json.JsonReadWriteDelegates> jsonDelegates = global::System.Runtime.Serialization.Json.JsonReadWriteDelegates.GetJsonDelegates();
            PopulateJsonDelegateDictionary(
                                dataContracts, 
                                jsonDelegates
                            );
        }
        static int[] s_knownContractsLists = new int[] {
              -1,   40,   -1,   45,   48,   49,   51,   53,   55,   56, 
              57,   58,   59,   60,   46,   -1,   49,   51,   53,   -1, 
              51,   -1,   49,   51,   -1,   49,   51,   53,   -1,   49, 
              51,   53,   -1,   49,   51,   53,   -1,   49,   51,   53, 
              -1,   49,   51,   53,   -1,   49,   51,   53,   -1,   49, 
              51,   53,   -1,   49,   51,   53,   -1,   49,   51,   53, 
              -1,   88,   -1, }
        ;
        // Count = 886
        static int[] s_xmlDictionaryStrings = new int[] {
                0, // array length: 0
                7, // array length: 7
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                7, // array length: 7
                2707, // index: 2707, string: "IsWindowsPhone"
                2722, // index: 2722, string: "bibles"
                2729, // index: 2729, string: "bookmarks"
                2739, // index: 2739, string: "highlighting"
                2752, // index: 2752, string: "settings"
                2761, // index: 2761, string: "themes"
                2768, // index: 2768, string: "windowSetup"
                7, // array length: 7
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                2, // array length: 2
                2729, // index: 2729, string: "bookmarks"
                2780, // index: 2780, string: "history"
                2, // array length: 2
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                5, // array length: 5
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                5, // array length: 5
                2788, // index: 2788, string: "BookShortName"
                2802, // index: 2802, string: "chapterNum"
                2813, // index: 2813, string: "note"
                2818, // index: 2818, string: "verseNum"
                2827, // index: 2827, string: "when"
                5, // array length: 5
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                32, // array length: 32
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                32, // array length: 32
                2832, // index: 2832, string: "AddLineBetweenNotes"
                2852, // index: 2852, string: "HighlightName1"
                2867, // index: 2867, string: "HighlightName2"
                2882, // index: 2882, string: "HighlightName3"
                2897, // index: 2897, string: "HighlightName4"
                2912, // index: 2912, string: "HighlightName5"
                2927, // index: 2927, string: "HighlightName6"
                2942, // index: 2942, string: "MarginInsideTextWindow"
                2965, // index: 2965, string: "NumberOfScreens"
                2981, // index: 2981, string: "OneDriveFolder"
                2996, // index: 2996, string: "RemoveScreenTransitions"
                3020, // index: 3020, string: "Show2titleRows"
                3035, // index: 3035, string: "SyncMediaVerses"
                3051, // index: 3051, string: "UseHighlights"
                3065, // index: 3065, string: "customBibleDownloadLinks"
                3090, // index: 3090, string: "eachVerseNewLine"
                3107, // index: 3107, string: "greekDictionaryLink"
                3127, // index: 3127, string: "hebrewDictionaryLink"
                3148, // index: 3148, string: "highlightMarkings"
                3166, // index: 3166, string: "showAddedNotesByChapter"
                3190, // index: 3190, string: "showBookName"
                3203, // index: 3203, string: "showChapterNumber"
                3221, // index: 3221, string: "showHeadings"
                3234, // index: 3234, string: "showMorphology"
                3249, // index: 3249, string: "showNotePositions"
                3267, // index: 3267, string: "showStrongsNumbers"
                3286, // index: 3286, string: "showVerseNumber"
                3302, // index: 3302, string: "smallVerseNumbers"
                3320, // index: 3320, string: "soundLink"
                3330, // index: 3330, string: "useInternetGreekHebrewDict"
                3357, // index: 3357, string: "userUniqueGuuid"
                3373, // index: 3373, string: "wordsOfChristRed"
                32, // array length: 32
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                16, // array length: 16
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                16, // array length: 16
                3390, // index: 3390, string: "Font"
                3395, // index: 3395, string: "IsNtOnly"
                3404, // index: 3404, string: "Pattern"
                3412, // index: 3412, string: "Src"
                3416, // index: 3416, string: "VSchrollPosition"
                3433, // index: 3433, string: "VoiceName"
                3443, // index: 3443, string: "Window"
                3450, // index: 3450, string: "bibleDescription"
                3467, // index: 3467, string: "bibleToLoad"
                3479, // index: 3479, string: "code"
                3484, // index: 3484, string: "curIndex"
                3493, // index: 3493, string: "htmlFontSize"
                3506, // index: 3506, string: "isSynchronized"
                3521, // index: 3521, string: "numRowsIown"
                3533, // index: 3533, string: "source"
                3540, // index: 3540, string: "windowType"
                16, // array length: 16
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                15, // array length: 15
                504, // index: 504, string: "WindowBible"
                516, // index: 516, string: "WindowBibleNotes"
                533, // index: 533, string: "WindowBook"
                544, // index: 544, string: "WindowSearch"
                557, // index: 557, string: "WindowHistory"
                571, // index: 571, string: "WindowBookmarks"
                587, // index: 587, string: "WindowSelectedVerses"
                608, // index: 608, string: "WindowDailyPlan"
                624, // index: 624, string: "WindowAddedNotes"
                641, // index: 641, string: "WindowCommentary"
                658, // index: 658, string: "WindowTranslator"
                675, // index: 675, string: "WindowInternetLink"
                694, // index: 694, string: "WindowLexiconLink"
                712, // index: 712, string: "WindowMediaPlayer"
                730, // index: 730, string: "WindowDictionary"
                2, // array length: 2
                -1, // string: null
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                2, // array length: 2
                3551, // index: 3551, string: "serial"
                3558, // index: 3558, string: "serial2"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                1, // array length: 1
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                3551, // index: 3551, string: "serial"
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                9, // array length: 9
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                9, // array length: 9
                3566, // index: 3566, string: "CipherKey"
                3576, // index: 3576, string: "ConfigPath"
                3587, // index: 3587, string: "Versification"
                3601, // index: 3601, string: "isIsoEncoding"
                3615, // index: 3615, string: "iso2DigitLangCode"
                3633, // index: 3633, string: "path"
                3638, // index: 3638, string: "posBookShortName"
                3655, // index: 3655, string: "posChaptNum"
                3667, // index: 3667, string: "posVerseNum"
                9, // array length: 9
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                3551, // index: 3551, string: "serial"
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                6, // array length: 6
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                6, // array length: 6
                3679, // index: 3679, string: "bookRelativeChapterNum"
                3702, // index: 3702, string: "bookStartPos"
                3715, // index: 3715, string: "booknum"
                3723, // index: 3723, string: "length"
                3730, // index: 3730, string: "startPos"
                3739, // index: 3739, string: "verses"
                6, // array length: 6
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                3, // array length: 3
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                3, // array length: 3
                3715, // index: 3715, string: "booknum"
                3723, // index: 3723, string: "length"
                3730, // index: 3730, string: "startPos"
                3, // array length: 3
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                4, // array length: 4
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                4, // array length: 4
                3723, // index: 3723, string: "length"
                3746, // index: 3746, string: "listChapters"
                3730, // index: 3730, string: "startPos"
                3759, // index: 3759, string: "unused"
                4, // array length: 4
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                -1, // string: null
                3, // array length: 3
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                3551, // index: 3551, string: "serial"
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                3551, // index: 3551, string: "serial"
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                2, // array length: 2
                3551, // index: 3551, string: "serial"
                3766, // index: 3766, string: "DisplayText"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                5, // array length: 5
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                5, // array length: 5
                3551, // index: 3551, string: "serial"
                3778, // index: 3778, string: "BookMarksToShow"
                3794, // index: 3794, string: "ShowDate"
                3803, // index: 3803, string: "_title"
                3558, // index: 3558, string: "serial2"
                5, // array length: 5
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                12, // array length: 12
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                12, // array length: 12
                3551, // index: 3551, string: "serial"
                3766, // index: 3766, string: "DisplayText"
                3810, // index: 3810, string: "DisplayTextHtmlBody"
                3830, // index: 3830, string: "Found"
                3836, // index: 3836, string: "SearchChapter"
                3850, // index: 3850, string: "SearchText"
                3861, // index: 3861, string: "SearchTypeIndex"
                3877, // index: 3877, string: "translationFound"
                3894, // index: 3894, string: "translationNewTestement"
                3918, // index: 3918, string: "translationOldTestement"
                3942, // index: 3942, string: "translationSearch"
                3960, // index: 3960, string: "translationWholeBible"
                12, // array length: 12
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                3, // array length: 3
                -1, // string: null
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                3, // array length: 3
                3551, // index: 3551, string: "serial"
                3558, // index: 3558, string: "serial2"
                3982, // index: 3982, string: "titleBrowserWindow"
                3, // array length: 3
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                7, // array length: 7
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                3, // array length: 3
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                7, // array length: 7
                3551, // index: 3551, string: "serial"
                3778, // index: 3778, string: "BookMarksToShow"
                3794, // index: 3794, string: "ShowDate"
                3803, // index: 3803, string: "_title"
                3558, // index: 3558, string: "serial2"
                4001, // index: 4001, string: "LocalDisplaySettings"
                4022, // index: 4022, string: "NotesToShow"
                7, // array length: 7
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                2, // array length: 2
                1420, // index: 1420, string: "Key"
                1424, // index: 1424, string: "Value"
                2, // array length: 2
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                2, // array length: 2
                1420, // index: 1420, string: "Key"
                1424, // index: 1424, string: "Value"
                2, // array length: 2
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                2, // array length: 2
                -1, // string: null
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                2, // array length: 2
                1420, // index: 1420, string: "Key"
                1424, // index: 1424, string: "Value"
                2, // array length: 2
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                2, // array length: 2
                -1, // string: null
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                2, // array length: 2
                4034, // index: 4034, string: "key"
                4038, // index: 4038, string: "value"
                2, // array length: 2
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                2, // array length: 2
                -1, // string: null
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                1, // array length: 1
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                2, // array length: 2
                4034, // index: 4034, string: "key"
                4038, // index: 4038, string: "value"
                2, // array length: 2
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                2, // array length: 2
                -1, // string: null
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                1, // array length: 1
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                2, // array length: 2
                4034, // index: 4034, string: "key"
                4038, // index: 4038, string: "value"
                2, // array length: 2
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                3, // array length: 3
                -1, // string: null
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                3, // array length: 3
                3551, // index: 3551, string: "serial"
                4044, // index: 4044, string: "Link"
                4049, // index: 4049, string: "TitleBar"
                3, // array length: 3
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                763, // index: 763, string: "http://schemas.datacontract.org/2004/07/CrossConnect.readers"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                3551, // index: 3551, string: "serial"
                4044, // index: 4044, string: "Link"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                12, // array length: 12
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                12, // array length: 12
                4058, // index: 4058, string: "Book"
                4063, // index: 4063, string: "Chapter"
                4071, // index: 4071, string: "Code"
                4076, // index: 4076, string: "Icon"
                4081, // index: 4081, string: "IconLink"
                3395, // index: 3395, string: "IsNtOnly"
                4090, // index: 4090, string: "Language"
                4099, // index: 4099, string: "Name"
                3404, // index: 3404, string: "Pattern"
                3412, // index: 3412, string: "Src"
                4104, // index: 4104, string: "Verse"
                3433, // index: 3433, string: "VoiceName"
                12, // array length: 12
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1, // array length: 1
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                1, // array length: 1
                3551, // index: 3551, string: "serial"
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                5, // array length: 5
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                5, // array length: 5
                4110, // index: 4110, string: "Length"
                4117, // index: 4117, string: "PositionInDat"
                4131, // index: 4131, string: "Title"
                4137, // index: 4137, string: "WindowMatchingKey"
                3551, // index: 3551, string: "serial"
                5, // array length: 5
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                4137, // index: 4137, string: "WindowMatchingKey"
                3551, // index: 3551, string: "serial"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                4137, // index: 4137, string: "WindowMatchingKey"
                3551, // index: 3551, string: "serial"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                5, // array length: 5
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                5, // array length: 5
                4110, // index: 4110, string: "Length"
                4117, // index: 4117, string: "PositionInDat"
                4131, // index: 4131, string: "Title"
                4137, // index: 4137, string: "WindowMatchingKey"
                3551, // index: 3551, string: "serial"
                5, // array length: 5
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                4137, // index: 4137, string: "WindowMatchingKey"
                3551, // index: 3551, string: "serial"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                2, // array length: 2
                2802, // index: 2802, string: "chapterNum"
                2827, // index: 2827, string: "when"
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                9, // array length: 9
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                2, // array length: 2
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                9, // array length: 9
                3551, // index: 3551, string: "serial"
                3766, // index: 3766, string: "DisplayText"
                3810, // index: 3810, string: "DisplayTextHtmlBody"
                3830, // index: 3830, string: "Found"
                3836, // index: 3836, string: "SearchChapter"
                3850, // index: 3850, string: "SearchText"
                3861, // index: 3861, string: "SearchTypeIndex"
                3877, // index: 3877, string: "translationFound"
                3942, // index: 3942, string: "translationSearch"
                9, // array length: 9
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                7, // array length: 7
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                7, // array length: 7
                4155, // index: 4155, string: "Children"
                4164, // index: 4164, string: "NextBrother"
                4176, // index: 4176, string: "NumCharacters"
                4190, // index: 4190, string: "Parent"
                4197, // index: 4197, string: "PositionInBdt"
                4211, // index: 4211, string: "PositionInChapters"
                4131, // index: 4131, string: "Title"
                7, // array length: 7
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                383, // index: 383, string: "http://schemas.datacontract.org/2004/07/Sword.reader"
                10, // array length: 10
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                -1, // string: null
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                10, // array length: 10
                4230, // index: 4230, string: "PersonalNotesVersified"
                4253, // index: 4253, string: "PlanBible"
                4263, // index: 4263, string: "PlanBibleDescription"
                4284, // index: 4284, string: "PlanTextSize"
                4297, // index: 4297, string: "currentChapterNumber"
                4318, // index: 4318, string: "currentVerseNumber"
                4337, // index: 4337, string: "personalNotes"
                4351, // index: 4351, string: "planDayNumber"
                4365, // index: 4365, string: "planNumber"
                4376, // index: 4376, string: "planStartDate"
                10, // array length: 10
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                284, // index: 284, string: "http://schemas.datacontract.org/2004/07/CrossConnect"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                2, // array length: 2
                1420, // index: 1420, string: "Key"
                1424, // index: 1424, string: "Value"
                2, // array length: 2
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                1264, // index: 1264, string: "http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                2, // array length: 2
                -1, // string: null
                -1, // string: null
                1, // array length: 1
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                2, // array length: 2
                4034, // index: 4034, string: "key"
                4038, // index: 4038, string: "value"
                2, // array length: 2
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                1699, // index: 1699, string: "http://schemas.datacontract.org/2004/07/System.Collections.Generic"
                1, // array length: 1
                -1, // string: null
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                4390, // index: 4390, string: "procent"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                -1, // string: null
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                4398, // index: 4398, string: "message"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                2, // array length: 2
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                -1, // string: null
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                2, // array length: 2
                4406, // index: 4406, string: "audioModel"
                4417, // index: 4417, string: "title"
                2, // array length: 2
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                -1, // string: null
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                4423, // index: 4423, string: "Timestamp"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                -1, // string: null
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                4423, // index: 4423, string: "Timestamp"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                1991, // index: 1991, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                4406, // index: 4406, string: "audioModel"
                1, // array length: 1
                2423, // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
                1, // array length: 1
                2423  // index: 2423, string: "http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages"
        };
        // Count = 15
        static global::MemberEntry[] s_dataMemberLists = new global::MemberEntry[] {
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 504, // WindowBible
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 516, // WindowBibleNotes
                    Value = 1,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 533, // WindowBook
                    Value = 2,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 544, // WindowSearch
                    Value = 3,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 557, // WindowHistory
                    Value = 4,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 571, // WindowBookmarks
                    Value = 5,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 587, // WindowSelectedVerses
                    Value = 6,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 608, // WindowDailyPlan
                    Value = 7,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 624, // WindowAddedNotes
                    Value = 8,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 641, // WindowCommentary
                    Value = 9,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 658, // WindowTranslator
                    Value = 10,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 675, // WindowInternetLink
                    Value = 11,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 694, // WindowLexiconLink
                    Value = 12,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 712, // WindowMediaPlayer
                    Value = 13,
                }, 
                new global::MemberEntry() {
                    EmitDefaultValue = true,
                    NameIndex = 730, // WindowDictionary
                    Value = 14,
                }
        };
        static readonly byte[] s_dataContractMap_Hashtable = null;
        // Count=108
        [global::System.Runtime.CompilerServices.PreInitialized]
        static readonly global::DataContractMapEntry[] s_dataContractMap = new global::DataContractMapEntry[] {
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 0, // 0x0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]" +
                                ", mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 0, // 0x0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 16, // 0x10
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Char, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 32, // 0x20
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Char, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], m" +
                                "scorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 32, // 0x20
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 48, // 0x30
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]" +
                                "], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 48, // 0x30
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 64, // 0x40
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]" +
                                ", mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 64, // 0x40
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 80, // 0x50
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]," +
                                " mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 80, // 0x50
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 96, // 0x60
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]," +
                                " mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 96, // 0x60
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 112, // 0x70
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], m" +
                                "scorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 112, // 0x70
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 128, // 0x80
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], " +
                                "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 128, // 0x80
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 144, // 0x90
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], " +
                                "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 144, // 0x90
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 160, // 0xa0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Xml.XmlQualifiedName, System.Xml.ReaderWriter, Version=4.0.11.0, Culture=neutral, PublicKeyToken=b03f5f7f" +
                                "11d50a3a")),
                    TableIndex = 176, // 0xb0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 192, // 0xc0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], " +
                                "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 192, // 0xc0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.SByte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 208, // 0xd0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.SByte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], " +
                                "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 208, // 0xd0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 224, // 0xe0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.TimeSpan, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 240, // 0xf0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.TimeSpan, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]" +
                                "], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 240, // 0xf0
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 256, // 0x100
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], m" +
                                "scorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 256, // 0x100
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 272, // 0x110
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]," +
                                " mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 272, // 0x110
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 288, // 0x120
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]," +
                                " mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 288, // 0x120
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 304, // 0x130
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[System.UInt16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]," +
                                " mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 304, // 0x130
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Uri, System.Private.Uri, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 320, // 0x140
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.BackupRestore+BackupManifest, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 1, // 0x1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.App+BiblePlaceMarkers, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 17, // 0x11
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, Publi" +
                                "cKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 2, // 0x2
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 33, // 0x21
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DisplaySettings, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 49, // 0x31
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.SerializableWindowState, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 65, // 0x41
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.WindowType, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 3, // 0x3
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[CrossConnect.WindowType, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]" +
                                "], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 3, // 0x3
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.DailyPlanReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 81, // 0x51
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 97, // 0x61
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReaderSerialData, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 113, // 0x71
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.CommentZtextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 129, // 0x81
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+ChapterPos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 145, // 0x91
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BibleZtextReader+VersePos, Sword, Version=1.0.0.0, Culture=neutr" +
                                "al, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a" +
                                "3a")),
                    TableIndex = 18, // 0x12
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+VersePos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 161, // 0xa1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Nullable`1[[Sword.reader.BibleZtextReader+VersePos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyTok" +
                                "en=null]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 161, // 0xa1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+BookPos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 177, // 0xb1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BibleZtextReader+ChapterPos, Sword, Version=1.0.0.0, Culture=neu" +
                                "tral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d5" +
                                "0a3a")),
                    TableIndex = 34, // 0x22
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.CommentRawComReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 193, // 0xc1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleRawTextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 209, // 0xd1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.TranslatorReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 225, // 0xe1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BiblePlaceMarkReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 241, // 0xf1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.SearchReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 257, // 0x101
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.BibleNoteReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 273, // 0x111
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.PersonalNotesReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 289, // 0x121
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 50, // 0x32
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 305, // 0x131
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Nullable`1[[System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 305, // 0x131
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 66, // 0x42
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 321, // 0x141
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Nullable`1[[System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 321, // 0x141
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 82, // 0x52
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 337, // 0x151
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Nullable`1[[System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 337, // 0x151
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 353, // 0x161
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Nullable`1[[System.Collections.Generic.KeyValuePair`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 353, // 0x161
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 369, // 0x171
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Nullable`1[[System.Collections.Generic.KeyValuePair`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 369, // 0x171
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 385, // 0x181
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Nullable`1[[System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 385, // 0x181
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.InternetLinkReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 401, // 0x191
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.GreekHebrewDictReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 417, // 0x1a1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.AudioModel, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 433, // 0x1b1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 449, // 0x1c1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRawDefReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 465, // 0x1d1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRawIndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 481, // 0x1e1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRaw4IndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 497, // 0x1f1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryZldDefReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 513, // 0x201
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryZldIndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 529, // 0x211
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextPlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 545, // 0x221
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenSearchReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 561, // 0x231
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextReader+ChapterData, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 577, // 0x241
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.RawGenTextReader+ChapterData, Sword, Version=1.0.0.0, Culture=ne" +
                                "utral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d" +
                                "50a3a")),
                    TableIndex = 98, // 0x62
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.App+SerializableDailyPlan, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    TableIndex = 593, // 0x251
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 114, // 0x72
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 609, // 0x261
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Nullable`1[[System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 609, // 0x261
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 625, // 0x271
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Nullable`1[[System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    TableIndex = 625, // 0x271
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.PositionMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicK" +
                                "eyToken=null")),
                    TableIndex = 641, // 0x281
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.ErrorMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyT" +
                                "oken=null")),
                    TableIndex = 657, // 0x291
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.TrackChangedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                "licKeyToken=null")),
                    TableIndex = 673, // 0x2a1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.AppSuspendedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                "licKeyToken=null")),
                    TableIndex = 689, // 0x2b1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.AppResumedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Publi" +
                                "cKeyToken=null")),
                    TableIndex = 705, // 0x2c1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.StartPlaybackMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pu" +
                                "blicKeyToken=null")),
                    TableIndex = 721, // 0x2d1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.SkipNextMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicK" +
                                "eyToken=null")),
                    TableIndex = 737, // 0x2e1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.SkipPreviousMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                "licKeyToken=null")),
                    TableIndex = 753, // 0x2f1
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.KillMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyTo" +
                                "ken=null")),
                    TableIndex = 769, // 0x301
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.UpdateStartPointMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral," +
                                " PublicKeyToken=null")),
                    TableIndex = 785, // 0x311
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.BackgroundAudioTaskStartedMessage, BackgroundAudioShared, Version=1.0.0.0, Cultur" +
                                "e=neutral, PublicKeyToken=null")),
                    TableIndex = 801, // 0x321
                }, 
                new global::DataContractMapEntry() {
                    UserCodeType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Object[], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    TableIndex = 130, // 0x82
                }
        };
        static readonly byte[] s_dataContracts_Hashtable = null;
        // Count=21
        [global::System.Runtime.CompilerServices.PreInitialized]
        static readonly global::DataContractEntry[] s_dataContracts = new global::DataContractEntry[] {
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 0, // boolean
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 0, // boolean
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 0, // boolean
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.BooleanDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        NameIndex = 93, // base64Binary
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 93, // base64Binary
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 93, // base64Binary
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.ByteArrayDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 106, // char
                        NamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        StableNameIndex = 106, // char
                        StableNameNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        TopLevelElementNameIndex = 106, // char
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Char, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Char, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.CharDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 111, // dateTime
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 111, // dateTime
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 111, // dateTime
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.DateTimeDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 120, // decimal
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 120, // decimal
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 120, // decimal
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.DecimalDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 128, // double
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 128, // double
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 128, // double
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.DoubleDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 135, // float
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 135, // float
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 135, // float
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.FloatDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 141, // guid
                        NamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        StableNameIndex = 141, // guid
                        StableNameNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        TopLevelElementNameIndex = 141, // guid
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.GuidDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 146, // int
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 146, // int
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 146, // int
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.IntDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 150, // long
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 150, // long
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 150, // long
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.LongDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        NameIndex = 155, // anyType
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 155, // anyType
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 155, // anyType
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    },
                    Kind = global::DataContractKind.ObjectDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        NameIndex = 163, // QName
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 163, // QName
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 163, // QName
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Xml.XmlQualifiedName, System.Xml.ReaderWriter, Version=4.0.11.0, Culture=neutral, PublicKeyToken=b03f5f7f" +
                                    "11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Xml.XmlQualifiedName, System.Xml.ReaderWriter, Version=4.0.11.0, Culture=neutral, PublicKeyToken=b03f5f7f" +
                                    "11d50a3a")),
                    },
                    Kind = global::DataContractKind.QNameDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 169, // short
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 169, // short
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 169, // short
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.ShortDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 175, // byte
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 175, // byte
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 175, // byte
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.SByte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.SByte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.SignedByteDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        NameIndex = 180, // string
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 180, // string
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 180, // string
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.StringDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 187, // duration
                        NamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        StableNameIndex = 187, // duration
                        StableNameNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        TopLevelElementNameIndex = 187, // duration
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.TimeSpan, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.TimeSpan, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.TimeSpanDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 196, // unsignedByte
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 196, // unsignedByte
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 196, // unsignedByte
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.UnsignedByteDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 209, // unsignedInt
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 209, // unsignedInt
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 209, // unsignedInt
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.UnsignedIntDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 221, // unsignedLong
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 221, // unsignedLong
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 221, // unsignedLong
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.UnsignedLongDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        IsValueType = true,
                        NameIndex = 234, // unsignedShort
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 234, // unsignedShort
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 234, // unsignedShort
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.UInt16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    },
                    Kind = global::DataContractKind.UnsignedShortDataContract,
                }, 
                new global::DataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsBuiltInDataContract = true,
                        NameIndex = 248, // anyURI
                        NamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        StableNameIndex = 248, // anyURI
                        StableNameNamespaceIndex = 8, // http://www.w3.org/2001/XMLSchema
                        TopLevelElementNameIndex = 248, // anyURI
                        TopLevelElementNamespaceIndex = 41, // http://schemas.microsoft.com/2003/10/Serialization/
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Uri, System.Private.Uri, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Uri, System.Private.Uri, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    },
                    Kind = global::DataContractKind.UriDataContract,
                }
        };
        static readonly byte[] s_classDataContracts_Hashtable = null;
        // Count=51
        [global::System.Runtime.CompilerServices.PreInitialized]
        static readonly global::ClassDataContractEntry[] s_classDataContracts = new global::ClassDataContractEntry[] {
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 255, // BackupRestore.BackupManifest
                        NamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        StableNameIndex = 255, // BackupRestore.BackupManifest
                        StableNameNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        TopLevelElementNameIndex = 255, // BackupRestore.BackupManifest
                        TopLevelElementNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.BackupRestore+BackupManifest, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.BackupRestore+BackupManifest, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type0.ReadBackupRestore_BackupManifestFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type1.WriteBackupRestore_BackupManifestToXml),
                    ChildElementNamespacesListIndex = 1,
                    ContractNamespacesListIndex = 9,
                    MemberNamesListIndex = 11,
                    MemberNamespacesListIndex = 19,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsReference = true,
                        NameIndex = 337, // App.BiblePlaceMarkers
                        NamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        StableNameIndex = 337, // App.BiblePlaceMarkers
                        StableNameNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        TopLevelElementNameIndex = 337, // App.BiblePlaceMarkers
                        TopLevelElementNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.App+BiblePlaceMarkers, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.App+BiblePlaceMarkers, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 1,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type2.ReadApp_BiblePlaceMarkersFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type3.WriteApp_BiblePlaceMarkersToXml),
                    ChildElementNamespacesListIndex = 27,
                    ContractNamespacesListIndex = 30,
                    MemberNamesListIndex = 32,
                    MemberNamespacesListIndex = 35,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 436, // BiblePlaceMarker
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 436, // BiblePlaceMarker
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 436, // BiblePlaceMarker
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type7.ReadBiblePlaceMarkerFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type8.WriteBiblePlaceMarkerToXml),
                    ChildElementNamespacesListIndex = 38,
                    ContractNamespacesListIndex = 44,
                    MemberNamesListIndex = 46,
                    MemberNamespacesListIndex = 52,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 453, // DisplaySettings
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 453, // DisplaySettings
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 453, // DisplaySettings
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DisplaySettings, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DisplaySettings, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type9.ReadDisplaySettingsFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type10.WriteDisplaySettingsToXml),
                    ChildElementNamespacesListIndex = 58,
                    ContractNamespacesListIndex = 91,
                    MemberNamesListIndex = 93,
                    MemberNamespacesListIndex = 126,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 469, // SerializableWindowState
                        NamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        StableNameIndex = 469, // SerializableWindowState
                        StableNameNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        TopLevelElementNameIndex = 469, // SerializableWindowState
                        TopLevelElementNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.SerializableWindowState, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.SerializableWindowState, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 3,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type11.ReadSerializableWindowStateFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type12.WriteSerializableWindowStateToXml),
                    ChildElementNamespacesListIndex = 159,
                    ContractNamespacesListIndex = 176,
                    MemberNamesListIndex = 178,
                    MemberNamespacesListIndex = 195,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 747, // DailyPlanReader
                        NamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        StableNameIndex = 747, // DailyPlanReader
                        StableNameNamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        TopLevelElementNameIndex = 747, // DailyPlanReader
                        TopLevelElementNamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.DailyPlanReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.DailyPlanReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type13.ReadDailyPlanReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type14.WriteDailyPlanReaderToXml),
                    ChildElementNamespacesListIndex = 228,
                    ContractNamespacesListIndex = 231,
                    MemberNamesListIndex = 234,
                    MemberNamespacesListIndex = 237,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 824, // BibleZtextReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 824, // BibleZtextReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 824, // BibleZtextReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type15.ReadBibleZtextReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type16.WriteBibleZtextReaderToXml),
                    ChildElementNamespacesListIndex = 240,
                    ContractNamespacesListIndex = 242,
                    MemberNamesListIndex = 244,
                    MemberNamespacesListIndex = 246,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 841, // BibleZtextReaderSerialData
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 841, // BibleZtextReaderSerialData
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 841, // BibleZtextReaderSerialData
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReaderSerialData, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReaderSerialData, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type17.ReadBibleZtextReaderSerialDataFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type18.WriteBibleZtextReaderSerialDataToXml),
                    ChildElementNamespacesListIndex = 248,
                    ContractNamespacesListIndex = 258,
                    MemberNamesListIndex = 260,
                    MemberNamespacesListIndex = 270,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 868, // CommentZtextReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 868, // CommentZtextReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 868, // CommentZtextReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.CommentZtextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.CommentZtextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 16,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type19.ReadCommentZtextReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type20.WriteCommentZtextReaderToXml),
                    ChildElementNamespacesListIndex = 280,
                    ContractNamespacesListIndex = 282,
                    MemberNamesListIndex = 285,
                    MemberNamespacesListIndex = 287,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsReference = true,
                        NameIndex = 887, // BibleZtextReader.ChapterPos
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 887, // BibleZtextReader.ChapterPos
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 887, // BibleZtextReader.ChapterPos
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+ChapterPos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+ChapterPos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 20,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type21.ReadBibleZtextReader_ChapterPosFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type22.WriteBibleZtextReader_ChapterPosToXml),
                    ChildElementNamespacesListIndex = 289,
                    ContractNamespacesListIndex = 296,
                    MemberNamesListIndex = 298,
                    MemberNamespacesListIndex = 305,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 948, // BibleZtextReader.VersePos
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 948, // BibleZtextReader.VersePos
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 948, // BibleZtextReader.VersePos
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+VersePos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+VersePos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type26.ReadBibleZtextReader_VersePosFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type27.WriteBibleZtextReader_VersePosToXml),
                    ChildElementNamespacesListIndex = 312,
                    ContractNamespacesListIndex = 316,
                    MemberNamesListIndex = 318,
                    MemberNamespacesListIndex = 322,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsReference = true,
                        NameIndex = 974, // BibleZtextReader.BookPos
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 974, // BibleZtextReader.BookPos
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 974, // BibleZtextReader.BookPos
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+BookPos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+BookPos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 22,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type28.ReadBibleZtextReader_BookPosFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type29.WriteBibleZtextReader_BookPosToXml),
                    ChildElementNamespacesListIndex = 326,
                    ContractNamespacesListIndex = 331,
                    MemberNamesListIndex = 333,
                    MemberNamespacesListIndex = 338,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1034, // CommentRawComReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 1034, // CommentRawComReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 1034, // CommentRawComReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.CommentRawComReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.CommentRawComReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 25,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type33.ReadCommentRawComReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type34.WriteCommentRawComReaderToXml),
                    ChildElementNamespacesListIndex = 343,
                    ContractNamespacesListIndex = 345,
                    MemberNamesListIndex = 349,
                    MemberNamespacesListIndex = 351,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1054, // BibleRawComReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 1054, // BibleRawComReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 1054, // BibleRawComReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleRawTextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleRawTextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 29,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type35.ReadBibleRawComReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type36.WriteBibleRawComReaderToXml),
                    ChildElementNamespacesListIndex = 353,
                    ContractNamespacesListIndex = 355,
                    MemberNamesListIndex = 358,
                    MemberNamespacesListIndex = 360,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1072, // TranslatorReader
                        NamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        StableNameIndex = 1072, // TranslatorReader
                        StableNameNamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        TopLevelElementNameIndex = 1072, // TranslatorReader
                        TopLevelElementNamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.TranslatorReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.TranslatorReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 33,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type37.ReadTranslatorReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type38.WriteTranslatorReaderToXml),
                    ChildElementNamespacesListIndex = 362,
                    ContractNamespacesListIndex = 365,
                    MemberNamesListIndex = 368,
                    MemberNamespacesListIndex = 371,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1089, // BiblePlaceMarkReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 1089, // BiblePlaceMarkReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 1089, // BiblePlaceMarkReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BiblePlaceMarkReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BiblePlaceMarkReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 37,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type39.ReadBiblePlaceMarkReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type40.WriteBiblePlaceMarkReaderToXml),
                    ChildElementNamespacesListIndex = 374,
                    ContractNamespacesListIndex = 380,
                    MemberNamesListIndex = 383,
                    MemberNamespacesListIndex = 389,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1110, // SearchReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 1110, // SearchReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 1110, // SearchReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.SearchReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.SearchReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 41,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type41.ReadSearchReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type42.WriteSearchReaderToXml),
                    ChildElementNamespacesListIndex = 395,
                    ContractNamespacesListIndex = 408,
                    MemberNamesListIndex = 411,
                    MemberNamespacesListIndex = 424,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1123, // BibleNoteReader
                        NamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        StableNameIndex = 1123, // BibleNoteReader
                        StableNameNamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        TopLevelElementNameIndex = 1123, // BibleNoteReader
                        TopLevelElementNamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.BibleNoteReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.BibleNoteReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 45,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type43.ReadBibleNoteReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type44.WriteBibleNoteReaderToXml),
                    ChildElementNamespacesListIndex = 437,
                    ContractNamespacesListIndex = 441,
                    MemberNamesListIndex = 444,
                    MemberNamespacesListIndex = 448,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1139, // PersonalNotesReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 1139, // PersonalNotesReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 1139, // PersonalNotesReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.PersonalNotesReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.PersonalNotesReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 49,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type45.ReadPersonalNotesReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type46.WritePersonalNotesReaderToXml),
                    ChildElementNamespacesListIndex = 452,
                    ContractNamespacesListIndex = 460,
                    MemberNamesListIndex = 464,
                    MemberNamespacesListIndex = 472,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 1322, // KeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 1322, // KeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 1322, // KeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Runtime.Serialization.KeyValue`2, System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neut" +
                                    "ral, PublicKeyToken=b03f5f7f11d50a3a")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type50.ReadKeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1FromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type51.WriteKeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1ToXml),
                    ChildElementNamespacesListIndex = 480,
                    ContractNamespacesListIndex = 483,
                    MemberNamesListIndex = 485,
                    MemberNamespacesListIndex = 488,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 1504, // KeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 1504, // KeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 1504, // KeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Runtime.Serialization.KeyValue`2, System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neut" +
                                    "ral, PublicKeyToken=b03f5f7f11d50a3a")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type55.ReadKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1FromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type56.WriteKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ToXml),
                    ChildElementNamespacesListIndex = 491,
                    ContractNamespacesListIndex = 494,
                    MemberNamesListIndex = 496,
                    MemberNamespacesListIndex = 499,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 1617, // KeyValueOfintBiblePlaceMarker47_PGvv1Z
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 1617, // KeyValueOfintBiblePlaceMarker47_PGvv1Z
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 1617, // KeyValueOfintBiblePlaceMarker47_PGvv1Z
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Runtime.Serialization.KeyValue`2, System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neut" +
                                    "ral, PublicKeyToken=b03f5f7f11d50a3a")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type60.ReadKeyValueOfintBiblePlaceMarker47_PGvv1ZFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type61.WriteKeyValueOfintBiblePlaceMarker47_PGvv1ZToXml),
                    ChildElementNamespacesListIndex = 502,
                    ContractNamespacesListIndex = 505,
                    MemberNamesListIndex = 507,
                    MemberNamespacesListIndex = 510,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 1656, // KeyValuePairOfintBiblePlaceMarker47_PGvv1Z
                        NamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        StableNameIndex = 1656, // KeyValuePairOfintBiblePlaceMarker47_PGvv1Z
                        StableNameNamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        TopLevelElementNameIndex = 1656, // KeyValuePairOfintBiblePlaceMarker47_PGvv1Z
                        TopLevelElementNamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.KeyValuePair`2, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyTo" +
                                    "ken=b03f5f7f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type62.ReadKeyValuePairOfintBiblePlaceMarker47_PGvv1ZFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type63.WriteKeyValuePairOfintBiblePlaceMarker47_PGvv1ZToXml),
                    ChildElementNamespacesListIndex = 513,
                    ContractNamespacesListIndex = 516,
                    MemberNamesListIndex = 518,
                    MemberNamespacesListIndex = 521,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 1766, // KeyValuePairOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        NamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        StableNameIndex = 1766, // KeyValuePairOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        StableNameNamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        TopLevelElementNameIndex = 1766, // KeyValuePairOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        TopLevelElementNamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.KeyValuePair`2, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyTo" +
                                    "ken=b03f5f7f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type64.ReadKeyValuePairOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1FromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type65.WriteKeyValuePairOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ToXml),
                    ChildElementNamespacesListIndex = 524,
                    ContractNamespacesListIndex = 527,
                    MemberNamesListIndex = 529,
                    MemberNamespacesListIndex = 532,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 1837, // KeyValuePairOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        NamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        StableNameIndex = 1837, // KeyValuePairOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        StableNameNamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        TopLevelElementNameIndex = 1837, // KeyValuePairOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        TopLevelElementNamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.KeyValuePair`2, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyTo" +
                                    "ken=b03f5f7f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type66.ReadKeyValuePairOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1FromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type67.WriteKeyValuePairOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1ToXml),
                    ChildElementNamespacesListIndex = 535,
                    ContractNamespacesListIndex = 538,
                    MemberNamesListIndex = 540,
                    MemberNamespacesListIndex = 543,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1939, // InternetLinkReader
                        NamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        StableNameIndex = 1939, // InternetLinkReader
                        StableNameNamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        TopLevelElementNameIndex = 1939, // InternetLinkReader
                        TopLevelElementNamespaceIndex = 763, // http://schemas.datacontract.org/2004/07/CrossConnect.readers
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.InternetLinkReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.readers.InternetLinkReader, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 53,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type68.ReadInternetLinkReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type69.WriteInternetLinkReaderToXml),
                    ChildElementNamespacesListIndex = 546,
                    ContractNamespacesListIndex = 550,
                    MemberNamesListIndex = 553,
                    MemberNamespacesListIndex = 557,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1958, // GreekHebrewDictReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 1958, // GreekHebrewDictReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 1958, // GreekHebrewDictReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.GreekHebrewDictReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.GreekHebrewDictReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 57,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type70.ReadGreekHebrewDictReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type71.WriteGreekHebrewDictReaderToXml),
                    ChildElementNamespacesListIndex = 561,
                    ContractNamespacesListIndex = 564,
                    MemberNamesListIndex = 567,
                    MemberNamespacesListIndex = 570,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1980, // AudioModel
                        NamespaceIndex = 1991, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared
                        StableNameIndex = 1980, // AudioModel
                        StableNameNamespaceIndex = 1991, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared
                        TopLevelElementNameIndex = 1980, // AudioModel
                        TopLevelElementNamespaceIndex = 1991, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.AudioModel, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.AudioModel, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type72.ReadAudioModelFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type73.WriteAudioModelToXml),
                    ChildElementNamespacesListIndex = 573,
                    ContractNamespacesListIndex = 586,
                    MemberNamesListIndex = 588,
                    MemberNamespacesListIndex = 601,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2053, // RawGenTextReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2053, // RawGenTextReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2053, // RawGenTextReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type74.ReadRawGenTextReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type75.WriteRawGenTextReaderToXml),
                    ChildElementNamespacesListIndex = 614,
                    ContractNamespacesListIndex = 616,
                    MemberNamesListIndex = 618,
                    MemberNamespacesListIndex = 620,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2070, // DictionaryRawDefReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2070, // DictionaryRawDefReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2070, // DictionaryRawDefReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRawDefReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRawDefReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type76.ReadDictionaryRawDefReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type77.WriteDictionaryRawDefReaderToXml),
                    ChildElementNamespacesListIndex = 622,
                    ContractNamespacesListIndex = 628,
                    MemberNamesListIndex = 630,
                    MemberNamespacesListIndex = 636,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2093, // DictionaryRawIndexReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2093, // DictionaryRawIndexReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2093, // DictionaryRawIndexReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRawIndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRawIndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type78.ReadDictionaryRawIndexReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type79.WriteDictionaryRawIndexReaderToXml),
                    ChildElementNamespacesListIndex = 642,
                    ContractNamespacesListIndex = 645,
                    MemberNamesListIndex = 647,
                    MemberNamespacesListIndex = 650,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2118, // DictionaryRaw4IndexReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2118, // DictionaryRaw4IndexReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2118, // DictionaryRaw4IndexReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRaw4IndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryRaw4IndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type80.ReadDictionaryRaw4IndexReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type81.WriteDictionaryRaw4IndexReaderToXml),
                    ChildElementNamespacesListIndex = 653,
                    ContractNamespacesListIndex = 656,
                    MemberNamesListIndex = 659,
                    MemberNamespacesListIndex = 662,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2144, // DictionaryZldDefReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2144, // DictionaryZldDefReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2144, // DictionaryZldDefReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryZldDefReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryZldDefReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type82.ReadDictionaryZldDefReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type83.WriteDictionaryZldDefReaderToXml),
                    ChildElementNamespacesListIndex = 665,
                    ContractNamespacesListIndex = 671,
                    MemberNamesListIndex = 674,
                    MemberNamespacesListIndex = 680,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2167, // DictionaryZldIndexReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2167, // DictionaryZldIndexReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2167, // DictionaryZldIndexReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryZldIndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.DictionaryZldIndexReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type84.ReadDictionaryZldIndexReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type85.WriteDictionaryZldIndexReaderToXml),
                    ChildElementNamespacesListIndex = 686,
                    ContractNamespacesListIndex = 689,
                    MemberNamesListIndex = 692,
                    MemberNamespacesListIndex = 695,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2192, // RawGenTextPlaceMarker
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2192, // RawGenTextPlaceMarker
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2192, // RawGenTextPlaceMarker
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextPlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextPlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type86.ReadRawGenTextPlaceMarkerFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type87.WriteRawGenTextPlaceMarkerToXml),
                    ChildElementNamespacesListIndex = 698,
                    ContractNamespacesListIndex = 701,
                    MemberNamesListIndex = 703,
                    MemberNamespacesListIndex = 706,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2214, // RawGenSearchReader
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2214, // RawGenSearchReader
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2214, // RawGenSearchReader
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenSearchReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenSearchReader, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        KnownDataContractsListIndex = 61,
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type88.ReadRawGenSearchReaderFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type89.WriteRawGenSearchReaderToXml),
                    ChildElementNamespacesListIndex = 709,
                    ContractNamespacesListIndex = 719,
                    MemberNamesListIndex = 722,
                    MemberNamespacesListIndex = 732,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2233, // RawGenTextReader.ChapterData
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2233, // RawGenTextReader.ChapterData
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2233, // RawGenTextReader.ChapterData
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextReader+ChapterData, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextReader+ChapterData, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type90.ReadRawGenTextReader_ChapterDataFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type91.WriteRawGenTextReader_ChapterDataToXml),
                    ChildElementNamespacesListIndex = 742,
                    ContractNamespacesListIndex = 750,
                    MemberNamesListIndex = 752,
                    MemberNamespacesListIndex = 760,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2298, // App.SerializableDailyPlan
                        NamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        StableNameIndex = 2298, // App.SerializableDailyPlan
                        StableNameNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        TopLevelElementNameIndex = 2298, // App.SerializableDailyPlan
                        TopLevelElementNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.App+SerializableDailyPlan, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.App+SerializableDailyPlan, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type95.ReadApp_SerializableDailyPlanFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type96.WriteApp_SerializableDailyPlanToXml),
                    ChildElementNamespacesListIndex = 768,
                    ContractNamespacesListIndex = 779,
                    MemberNamesListIndex = 781,
                    MemberNamespacesListIndex = 792,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 2355, // KeyValueOfstringanyType
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 2355, // KeyValueOfstringanyType
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 2355, // KeyValueOfstringanyType
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Runtime.Serialization.KeyValue`2, System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neut" +
                                    "ral, PublicKeyToken=b03f5f7f11d50a3a")),
                    },
                    HasDataContract = true,
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type100.ReadKeyValueOfstringanyTypeFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type101.WriteKeyValueOfstringanyTypeToXml),
                    ChildElementNamespacesListIndex = 803,
                    ContractNamespacesListIndex = 806,
                    MemberNamesListIndex = 808,
                    MemberNamespacesListIndex = 811,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 2379, // KeyValuePairOfstringanyType
                        NamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        StableNameIndex = 2379, // KeyValuePairOfstringanyType
                        StableNameNamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        TopLevelElementNameIndex = 2379, // KeyValuePairOfstringanyType
                        TopLevelElementNamespaceIndex = 1699, // http://schemas.datacontract.org/2004/07/System.Collections.Generic
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.KeyValuePair`2, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyTo" +
                                    "ken=b03f5f7f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassReaderDelegate>(global::Type102.ReadKeyValuePairOfstringanyTypeFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatClassWriterDelegate>(global::Type103.WriteKeyValuePairOfstringanyTypeToXml),
                    ChildElementNamespacesListIndex = 814,
                    ContractNamespacesListIndex = 817,
                    MemberNamesListIndex = 819,
                    MemberNamespacesListIndex = 822,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2407, // PositionMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2407, // PositionMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2407, // PositionMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.PositionMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicK" +
                                    "eyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.PositionMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicK" +
                                    "eyToken=null")),
                    },
                    HasDataContract = true,
                    ChildElementNamespacesListIndex = 825,
                    ContractNamespacesListIndex = 827,
                    MemberNamesListIndex = 829,
                    MemberNamespacesListIndex = 831,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2494, // ErrorMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2494, // ErrorMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2494, // ErrorMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.ErrorMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyT" +
                                    "oken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.ErrorMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyT" +
                                    "oken=null")),
                    },
                    HasDataContract = true,
                    ChildElementNamespacesListIndex = 833,
                    ContractNamespacesListIndex = 835,
                    MemberNamesListIndex = 837,
                    MemberNamespacesListIndex = 839,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2507, // TrackChangedMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2507, // TrackChangedMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2507, // TrackChangedMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.TrackChangedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                    "licKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.TrackChangedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                    "licKeyToken=null")),
                    },
                    HasDataContract = true,
                    ChildElementNamespacesListIndex = 841,
                    ContractNamespacesListIndex = 844,
                    MemberNamesListIndex = 846,
                    MemberNamespacesListIndex = 849,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2527, // AppSuspendedMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2527, // AppSuspendedMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2527, // AppSuspendedMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.AppSuspendedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                    "licKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.AppSuspendedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                    "licKeyToken=null")),
                    },
                    HasDataContract = true,
                    ChildElementNamespacesListIndex = 852,
                    ContractNamespacesListIndex = 854,
                    MemberNamesListIndex = 856,
                    MemberNamespacesListIndex = 858,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2547, // AppResumedMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2547, // AppResumedMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2547, // AppResumedMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.AppResumedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Publi" +
                                    "cKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.AppResumedMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Publi" +
                                    "cKeyToken=null")),
                    },
                    HasDataContract = true,
                    ChildElementNamespacesListIndex = 860,
                    ContractNamespacesListIndex = 862,
                    MemberNamesListIndex = 864,
                    MemberNamespacesListIndex = 866,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2565, // StartPlaybackMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2565, // StartPlaybackMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2565, // StartPlaybackMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.StartPlaybackMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pu" +
                                    "blicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.StartPlaybackMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pu" +
                                    "blicKeyToken=null")),
                    },
                    HasDataContract = true,
                    ContractNamespacesListIndex = 868,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2586, // SkipNextMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2586, // SkipNextMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2586, // SkipNextMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.SkipNextMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicK" +
                                    "eyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.SkipNextMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicK" +
                                    "eyToken=null")),
                    },
                    HasDataContract = true,
                    ContractNamespacesListIndex = 870,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2602, // SkipPreviousMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2602, // SkipPreviousMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2602, // SkipPreviousMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.SkipPreviousMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                    "licKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.SkipPreviousMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, Pub" +
                                    "licKeyToken=null")),
                    },
                    HasDataContract = true,
                    ContractNamespacesListIndex = 872,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2622, // KillMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2622, // KillMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2622, // KillMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.KillMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyTo" +
                                    "ken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.KillMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral, PublicKeyTo" +
                                    "ken=null")),
                    },
                    HasDataContract = true,
                    ContractNamespacesListIndex = 874,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2634, // UpdateStartPointMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2634, // UpdateStartPointMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2634, // UpdateStartPointMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.UpdateStartPointMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral," +
                                    " PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.UpdateStartPointMessage, BackgroundAudioShared, Version=1.0.0.0, Culture=neutral," +
                                    " PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    ChildElementNamespacesListIndex = 876,
                    ContractNamespacesListIndex = 878,
                    MemberNamesListIndex = 880,
                    MemberNamespacesListIndex = 882,
                }, 
                new global::ClassDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2658, // BackgroundAudioTaskStartedMessage
                        NamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        StableNameIndex = 2658, // BackgroundAudioTaskStartedMessage
                        StableNameNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        TopLevelElementNameIndex = 2658, // BackgroundAudioTaskStartedMessage
                        TopLevelElementNamespaceIndex = 2423, // http://schemas.datacontract.org/2004/07/BackgroundAudioShared.Messages
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.BackgroundAudioTaskStartedMessage, BackgroundAudioShared, Version=1.0.0.0, Cultur" +
                                    "e=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("BackgroundAudioShared.Messages.BackgroundAudioTaskStartedMessage, BackgroundAudioShared, Version=1.0.0.0, Cultur" +
                                    "e=neutral, PublicKeyToken=null")),
                    },
                    HasDataContract = true,
                    ContractNamespacesListIndex = 884,
                }
        };
        static readonly byte[] s_collectionDataContracts_Hashtable = null;
        // Count=9
        [global::System.Runtime.CompilerServices.PreInitialized]
        static readonly global::CollectionDataContractEntry[] s_collectionDataContracts = new global::CollectionDataContractEntry[] {
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 359, // ArrayOfBiblePlaceMarker
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 359, // ArrayOfBiblePlaceMarker
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 359, // ArrayOfBiblePlaceMarker
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, Publi" +
                                    "cKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, Publi" +
                                    "cKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1, System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7" +
                                    "f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionReaderDelegate>(global::Type4.ReadArrayOfBiblePlaceMarkerFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionWriterDelegate>(global::Type5.WriteArrayOfBiblePlaceMarkerToXml),
                    XmlFormatGetOnlyCollectionReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatGetOnlyCollectionReaderDelegate>(global::Type6.ReadArrayOfBiblePlaceMarkerFromXmlIsGetOnly),
                    CollectionItemNameIndex = 436, // BiblePlaceMarker
                    KeyNameIndex = -1,
                    ItemNameIndex = 436, // BiblePlaceMarker
                    ValueNameIndex = -1,
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.GenericList,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                }, 
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 915, // ArrayOfBibleZtextReader.VersePos
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 915, // ArrayOfBibleZtextReader.VersePos
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 915, // ArrayOfBibleZtextReader.VersePos
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BibleZtextReader+VersePos, Sword, Version=1.0.0.0, Culture=neutr" +
                                    "al, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a" +
                                    "3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BibleZtextReader+VersePos, Sword, Version=1.0.0.0, Culture=neutr" +
                                    "al, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a" +
                                    "3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1, System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7" +
                                    "f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionReaderDelegate>(global::Type23.ReadArrayOfBibleZtextReader_VersePosFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionWriterDelegate>(global::Type24.WriteArrayOfBibleZtextReader_VersePosToXml),
                    XmlFormatGetOnlyCollectionReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatGetOnlyCollectionReaderDelegate>(global::Type25.ReadArrayOfBibleZtextReader_VersePosFromXmlIsGetOnly),
                    CollectionItemNameIndex = 948, // BibleZtextReader.VersePos
                    KeyNameIndex = -1,
                    ItemNameIndex = 948, // BibleZtextReader.VersePos
                    ValueNameIndex = -1,
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.GenericList,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+VersePos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                }, 
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 999, // ArrayOfBibleZtextReader.ChapterPos
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 999, // ArrayOfBibleZtextReader.ChapterPos
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 999, // ArrayOfBibleZtextReader.ChapterPos
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BibleZtextReader+ChapterPos, Sword, Version=1.0.0.0, Culture=neu" +
                                    "tral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d5" +
                                    "0a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.BibleZtextReader+ChapterPos, Sword, Version=1.0.0.0, Culture=neu" +
                                    "tral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d5" +
                                    "0a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1, System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7" +
                                    "f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionReaderDelegate>(global::Type30.ReadArrayOfBibleZtextReader_ChapterPosFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionWriterDelegate>(global::Type31.WriteArrayOfBibleZtextReader_ChapterPosToXml),
                    XmlFormatGetOnlyCollectionReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatGetOnlyCollectionReaderDelegate>(global::Type32.ReadArrayOfBibleZtextReader_ChapterPosFromXmlIsGetOnly),
                    CollectionItemNameIndex = 887, // BibleZtextReader.ChapterPos
                    KeyNameIndex = -1,
                    ItemNameIndex = 887, // BibleZtextReader.ChapterPos
                    ValueNameIndex = -1,
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.GenericList,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.BibleZtextReader+ChapterPos, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                }, 
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1159, // ArrayOfKeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 1159, // ArrayOfKeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 1159, // ArrayOfKeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.Dictionary`2, System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b" +
                                    "03f5f7f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionReaderDelegate>(global::Type47.ReadArrayOfKeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1FromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionWriterDelegate>(global::Type48.WriteArrayOfKeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1ToXml),
                    XmlFormatGetOnlyCollectionReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatGetOnlyCollectionReaderDelegate>(global::Type49.ReadArrayOfKeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1FromXmlIsGetOnly),
                    CollectionItemNameIndex = 1322, // KeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                    KeyNameIndex = 1420, // Key
                    ItemNameIndex = 1322, // KeyValueOfstringArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ty7Ep6D1
                    ValueNameIndex = 1424, // Value
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.GenericDictionary,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                }, 
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1430, // ArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 1430, // ArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 1430, // ArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.Dictionary`2, System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b" +
                                    "03f5f7f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionReaderDelegate>(global::Type52.ReadArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1FromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionWriterDelegate>(global::Type53.WriteArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1ToXml),
                    XmlFormatGetOnlyCollectionReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatGetOnlyCollectionReaderDelegate>(global::Type54.ReadArrayOfKeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1FromXmlIsGetOnly),
                    CollectionItemNameIndex = 1504, // KeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                    KeyNameIndex = 1420, // Key
                    ItemNameIndex = 1504, // KeyValueOfintArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Zty7Ep6D1
                    ValueNameIndex = 1424, // Value
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.GenericDictionary,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                }, 
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 1571, // ArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Z
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 1571, // ArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Z
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 1571, // ArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1Z
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.Dictionary`2, System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b" +
                                    "03f5f7f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionReaderDelegate>(global::Type57.ReadArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1ZFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionWriterDelegate>(global::Type58.WriteArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1ZToXml),
                    XmlFormatGetOnlyCollectionReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatGetOnlyCollectionReaderDelegate>(global::Type59.ReadArrayOfKeyValueOfintBiblePlaceMarker47_PGvv1ZFromXmlIsGetOnly),
                    CollectionItemNameIndex = 1617, // KeyValueOfintBiblePlaceMarker47_PGvv1Z
                    KeyNameIndex = 1420, // Key
                    ItemNameIndex = 1617, // KeyValueOfintBiblePlaceMarker47_PGvv1Z
                    ValueNameIndex = 1424, // Value
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.GenericDictionary,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[Sword.reader.BiblePlaceMarker, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                }, 
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2262, // ArrayOfRawGenTextReader.ChapterData
                        NamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        StableNameIndex = 2262, // ArrayOfRawGenTextReader.ChapterData
                        StableNameNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        TopLevelElementNameIndex = 2262, // ArrayOfRawGenTextReader.ChapterData
                        TopLevelElementNamespaceIndex = 383, // http://schemas.datacontract.org/2004/07/Sword.reader
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.RawGenTextReader+ChapterData, Sword, Version=1.0.0.0, Culture=ne" +
                                    "utral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d" +
                                    "50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1[[Sword.reader.RawGenTextReader+ChapterData, Sword, Version=1.0.0.0, Culture=ne" +
                                    "utral, PublicKeyToken=null]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d" +
                                    "50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.List`1, System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7" +
                                    "f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionReaderDelegate>(global::Type92.ReadArrayOfRawGenTextReader_ChapterDataFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionWriterDelegate>(global::Type93.WriteArrayOfRawGenTextReader_ChapterDataToXml),
                    XmlFormatGetOnlyCollectionReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatGetOnlyCollectionReaderDelegate>(global::Type94.ReadArrayOfRawGenTextReader_ChapterDataFromXmlIsGetOnly),
                    CollectionItemNameIndex = 2233, // RawGenTextReader.ChapterData
                    KeyNameIndex = -1,
                    ItemNameIndex = 2233, // RawGenTextReader.ChapterData
                    ValueNameIndex = -1,
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.GenericList,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("Sword.reader.RawGenTextReader+ChapterData, Sword, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                }, 
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2324, // ArrayOfKeyValueOfstringanyType
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 2324, // ArrayOfKeyValueOfstringanyType
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 2324, // ArrayOfKeyValueOfstringanyType
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        GenericTypeDefinition = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Collections.Generic.Dictionary`2, System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b" +
                                    "03f5f7f11d50a3a")),
                    },
                    XmlFormatReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionReaderDelegate>(global::Type97.ReadArrayOfKeyValueOfstringanyTypeFromXml),
                    XmlFormatWriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatCollectionWriterDelegate>(global::Type98.WriteArrayOfKeyValueOfstringanyTypeToXml),
                    XmlFormatGetOnlyCollectionReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.XmlFormatGetOnlyCollectionReaderDelegate>(global::Type99.ReadArrayOfKeyValueOfstringanyTypeFromXmlIsGetOnly),
                    CollectionItemNameIndex = 2355, // KeyValueOfstringanyType
                    KeyNameIndex = 1420, // Key
                    ItemNameIndex = 2355, // KeyValueOfstringanyType
                    ValueNameIndex = 1424, // Value
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.GenericDictionary,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf(@"System.Runtime.Serialization.KeyValue`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Private.DataContractSerialization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                }, 
                new global::CollectionDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        NameIndex = 2692, // ArrayOfanyType
                        NamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        StableNameIndex = 2692, // ArrayOfanyType
                        StableNameNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        TopLevelElementNameIndex = 2692, // ArrayOfanyType
                        TopLevelElementNamespaceIndex = 1264, // http://schemas.microsoft.com/2003/10/Serialization/Arrays
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Object[], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Object[], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                    },
                    CollectionItemNameIndex = 155, // anyType
                    KeyNameIndex = -1,
                    ItemNameIndex = 155, // anyType
                    ValueNameIndex = -1,
                    CollectionContractKind = global::System.Runtime.Serialization.CollectionKind.Array,
                    ItemType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                }
        };
        static readonly byte[] s_enumDataContracts_Hashtable = null;
        // Count=1
        [global::System.Runtime.CompilerServices.PreInitialized]
        static readonly global::EnumDataContractEntry[] s_enumDataContracts = new global::EnumDataContractEntry[] {
                new global::EnumDataContractEntry() {
                    Common = new global::CommonContractEntry() {
                        HasRoot = true,
                        IsValueType = true,
                        NameIndex = 493, // WindowType
                        NamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        StableNameIndex = 493, // WindowType
                        StableNameNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        TopLevelElementNameIndex = 493, // WindowType
                        TopLevelElementNamespaceIndex = 284, // http://schemas.datacontract.org/2004/07/CrossConnect
                        OriginalUnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.WindowType, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                        UnderlyingType = new global::Internal.Runtime.CompilerServices.FixupRuntimeTypeHandle(global::System.Runtime.InteropServices.TypeOfHelper.RuntimeTypeHandleOf("CrossConnect.WindowType, CrossConnect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")),
                    },
                    BaseContractNameIndex = -1,
                    BaseContractNamespaceIndex = -1,
                    ChildElementNamesListIndex = 212,
                    MemberCount = 15,
                }
        };
        static readonly byte[] s_xmlDataContracts_Hashtable = null;
        // Count=0
        [global::System.Runtime.CompilerServices.PreInitialized]
        static readonly global::XmlDataContractEntry[] s_xmlDataContracts = new global::XmlDataContractEntry[0];
        static readonly byte[] s_jsonDelegatesList_Hashtable = null;
        // Count=13
        [global::System.Runtime.CompilerServices.PreInitialized]
        static readonly global::JsonDelegateEntry[] s_jsonDelegatesList = new global::JsonDelegateEntry[] {
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 96,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type107.WritePositionMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type106.ReadPositionMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 97,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type111.WriteErrorMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type110.ReadErrorMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 98,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type115.WriteTrackChangedMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type114.ReadTrackChangedMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 79,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type117.WriteAudioModelToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type116.ReadAudioModelFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 99,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type121.WriteAppSuspendedMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type120.ReadAppSuspendedMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 100,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type125.WriteAppResumedMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type124.ReadAppResumedMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 101,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type129.WriteStartPlaybackMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type128.ReadStartPlaybackMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 102,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type133.WriteSkipNextMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type132.ReadSkipNextMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 103,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type137.WriteSkipPreviousMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type136.ReadSkipPreviousMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 104,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type141.WriteKillMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type140.ReadKillMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 105,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type145.WriteUpdateStartPointMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type144.ReadUpdateStartPointMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 106,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassWriterDelegate>(global::Type149.WriteBackgroundAudioTaskStartedMessageToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatClassReaderDelegate>(global::Type148.ReadBackgroundAudioTaskStartedMessageFromJson),
                }, 
                new global::JsonDelegateEntry() {
                    DataContractMapIndex = 107,
                    IsCollection = true,
                    WriterDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatCollectionWriterDelegate>(global::Type154.WriteArrayOfanyTypeToJson),
                    ReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatCollectionReaderDelegate>(global::Type153.ReadArrayOfanyTypeFromJson),
                    GetOnlyReaderDelegate = global::SgIntrinsics.AddrOf<global::System.Runtime.Serialization.Json.JsonFormatGetOnlyCollectionReaderDelegate>(global::Type155.ReadArrayOfanyTypeFromJsonIsGetOnly),
                }
        };
        static char[] s_stringPool = new char[] {
            'b','o','o','l','e','a','n','\0','h','t','t','p',':','/','/','w','w','w','.','w','3','.','o','r','g','/','2','0','0','1',
            '/','X','M','L','S','c','h','e','m','a','\0','h','t','t','p',':','/','/','s','c','h','e','m','a','s','.','m','i','c','r',
            'o','s','o','f','t','.','c','o','m','/','2','0','0','3','/','1','0','/','S','e','r','i','a','l','i','z','a','t','i','o',
            'n','/','\0','b','a','s','e','6','4','B','i','n','a','r','y','\0','c','h','a','r','\0','d','a','t','e','T','i','m','e','\0',
            'd','e','c','i','m','a','l','\0','d','o','u','b','l','e','\0','f','l','o','a','t','\0','g','u','i','d','\0','i','n','t','\0',
            'l','o','n','g','\0','a','n','y','T','y','p','e','\0','Q','N','a','m','e','\0','s','h','o','r','t','\0','b','y','t','e','\0',
            's','t','r','i','n','g','\0','d','u','r','a','t','i','o','n','\0','u','n','s','i','g','n','e','d','B','y','t','e','\0','u',
            'n','s','i','g','n','e','d','I','n','t','\0','u','n','s','i','g','n','e','d','L','o','n','g','\0','u','n','s','i','g','n',
            'e','d','S','h','o','r','t','\0','a','n','y','U','R','I','\0','B','a','c','k','u','p','R','e','s','t','o','r','e','.','B',
            'a','c','k','u','p','M','a','n','i','f','e','s','t','\0','h','t','t','p',':','/','/','s','c','h','e','m','a','s','.','d',
            'a','t','a','c','o','n','t','r','a','c','t','.','o','r','g','/','2','0','0','4','/','0','7','/','C','r','o','s','s','C',
            'o','n','n','e','c','t','\0','A','p','p','.','B','i','b','l','e','P','l','a','c','e','M','a','r','k','e','r','s','\0','A',
            'r','r','a','y','O','f','B','i','b','l','e','P','l','a','c','e','M','a','r','k','e','r','\0','h','t','t','p',':','/','/',
            's','c','h','e','m','a','s','.','d','a','t','a','c','o','n','t','r','a','c','t','.','o','r','g','/','2','0','0','4','/',
            '0','7','/','S','w','o','r','d','.','r','e','a','d','e','r','\0','B','i','b','l','e','P','l','a','c','e','M','a','r','k',
            'e','r','\0','D','i','s','p','l','a','y','S','e','t','t','i','n','g','s','\0','S','e','r','i','a','l','i','z','a','b','l',
            'e','W','i','n','d','o','w','S','t','a','t','e','\0','W','i','n','d','o','w','T','y','p','e','\0','W','i','n','d','o','w',
            'B','i','b','l','e','\0','W','i','n','d','o','w','B','i','b','l','e','N','o','t','e','s','\0','W','i','n','d','o','w','B',
            'o','o','k','\0','W','i','n','d','o','w','S','e','a','r','c','h','\0','W','i','n','d','o','w','H','i','s','t','o','r','y',
            '\0','W','i','n','d','o','w','B','o','o','k','m','a','r','k','s','\0','W','i','n','d','o','w','S','e','l','e','c','t','e',
            'd','V','e','r','s','e','s','\0','W','i','n','d','o','w','D','a','i','l','y','P','l','a','n','\0','W','i','n','d','o','w',
            'A','d','d','e','d','N','o','t','e','s','\0','W','i','n','d','o','w','C','o','m','m','e','n','t','a','r','y','\0','W','i',
            'n','d','o','w','T','r','a','n','s','l','a','t','o','r','\0','W','i','n','d','o','w','I','n','t','e','r','n','e','t','L',
            'i','n','k','\0','W','i','n','d','o','w','L','e','x','i','c','o','n','L','i','n','k','\0','W','i','n','d','o','w','M','e',
            'd','i','a','P','l','a','y','e','r','\0','W','i','n','d','o','w','D','i','c','t','i','o','n','a','r','y','\0','D','a','i',
            'l','y','P','l','a','n','R','e','a','d','e','r','\0','h','t','t','p',':','/','/','s','c','h','e','m','a','s','.','d','a',
            't','a','c','o','n','t','r','a','c','t','.','o','r','g','/','2','0','0','4','/','0','7','/','C','r','o','s','s','C','o',
            'n','n','e','c','t','.','r','e','a','d','e','r','s','\0','B','i','b','l','e','Z','t','e','x','t','R','e','a','d','e','r',
            '\0','B','i','b','l','e','Z','t','e','x','t','R','e','a','d','e','r','S','e','r','i','a','l','D','a','t','a','\0','C','o',
            'm','m','e','n','t','Z','t','e','x','t','R','e','a','d','e','r','\0','B','i','b','l','e','Z','t','e','x','t','R','e','a',
            'd','e','r','.','C','h','a','p','t','e','r','P','o','s','\0','A','r','r','a','y','O','f','B','i','b','l','e','Z','t','e',
            'x','t','R','e','a','d','e','r','.','V','e','r','s','e','P','o','s','\0','B','i','b','l','e','Z','t','e','x','t','R','e',
            'a','d','e','r','.','V','e','r','s','e','P','o','s','\0','B','i','b','l','e','Z','t','e','x','t','R','e','a','d','e','r',
            '.','B','o','o','k','P','o','s','\0','A','r','r','a','y','O','f','B','i','b','l','e','Z','t','e','x','t','R','e','a','d',
            'e','r','.','C','h','a','p','t','e','r','P','o','s','\0','C','o','m','m','e','n','t','R','a','w','C','o','m','R','e','a',
            'd','e','r','\0','B','i','b','l','e','R','a','w','C','o','m','R','e','a','d','e','r','\0','T','r','a','n','s','l','a','t',
            'o','r','R','e','a','d','e','r','\0','B','i','b','l','e','P','l','a','c','e','M','a','r','k','R','e','a','d','e','r','\0',
            'S','e','a','r','c','h','R','e','a','d','e','r','\0','B','i','b','l','e','N','o','t','e','R','e','a','d','e','r','\0','P',
            'e','r','s','o','n','a','l','N','o','t','e','s','R','e','a','d','e','r','\0','A','r','r','a','y','O','f','K','e','y','V',
            'a','l','u','e','O','f','s','t','r','i','n','g','A','r','r','a','y','O','f','K','e','y','V','a','l','u','e','O','f','i',
            'n','t','A','r','r','a','y','O','f','K','e','y','V','a','l','u','e','O','f','i','n','t','B','i','b','l','e','P','l','a',
            'c','e','M','a','r','k','e','r','4','7','_','P','G','v','v','1','Z','t','y','7','E','p','6','D','1','t','y','7','E','p',
            '6','D','1','\0','h','t','t','p',':','/','/','s','c','h','e','m','a','s','.','m','i','c','r','o','s','o','f','t','.','c',
            'o','m','/','2','0','0','3','/','1','0','/','S','e','r','i','a','l','i','z','a','t','i','o','n','/','A','r','r','a','y',
            's','\0','K','e','y','V','a','l','u','e','O','f','s','t','r','i','n','g','A','r','r','a','y','O','f','K','e','y','V','a',
            'l','u','e','O','f','i','n','t','A','r','r','a','y','O','f','K','e','y','V','a','l','u','e','O','f','i','n','t','B','i',
            'b','l','e','P','l','a','c','e','M','a','r','k','e','r','4','7','_','P','G','v','v','1','Z','t','y','7','E','p','6','D',
            '1','t','y','7','E','p','6','D','1','\0','K','e','y','\0','V','a','l','u','e','\0','A','r','r','a','y','O','f','K','e','y',
            'V','a','l','u','e','O','f','i','n','t','A','r','r','a','y','O','f','K','e','y','V','a','l','u','e','O','f','i','n','t',
            'B','i','b','l','e','P','l','a','c','e','M','a','r','k','e','r','4','7','_','P','G','v','v','1','Z','t','y','7','E','p',
            '6','D','1','\0','K','e','y','V','a','l','u','e','O','f','i','n','t','A','r','r','a','y','O','f','K','e','y','V','a','l',
            'u','e','O','f','i','n','t','B','i','b','l','e','P','l','a','c','e','M','a','r','k','e','r','4','7','_','P','G','v','v',
            '1','Z','t','y','7','E','p','6','D','1','\0','A','r','r','a','y','O','f','K','e','y','V','a','l','u','e','O','f','i','n',
            't','B','i','b','l','e','P','l','a','c','e','M','a','r','k','e','r','4','7','_','P','G','v','v','1','Z','\0','K','e','y',
            'V','a','l','u','e','O','f','i','n','t','B','i','b','l','e','P','l','a','c','e','M','a','r','k','e','r','4','7','_','P',
            'G','v','v','1','Z','\0','K','e','y','V','a','l','u','e','P','a','i','r','O','f','i','n','t','B','i','b','l','e','P','l',
            'a','c','e','M','a','r','k','e','r','4','7','_','P','G','v','v','1','Z','\0','h','t','t','p',':','/','/','s','c','h','e',
            'm','a','s','.','d','a','t','a','c','o','n','t','r','a','c','t','.','o','r','g','/','2','0','0','4','/','0','7','/','S',
            'y','s','t','e','m','.','C','o','l','l','e','c','t','i','o','n','s','.','G','e','n','e','r','i','c','\0','K','e','y','V',
            'a','l','u','e','P','a','i','r','O','f','i','n','t','A','r','r','a','y','O','f','K','e','y','V','a','l','u','e','O','f',
            'i','n','t','B','i','b','l','e','P','l','a','c','e','M','a','r','k','e','r','4','7','_','P','G','v','v','1','Z','t','y',
            '7','E','p','6','D','1','\0','K','e','y','V','a','l','u','e','P','a','i','r','O','f','s','t','r','i','n','g','A','r','r',
            'a','y','O','f','K','e','y','V','a','l','u','e','O','f','i','n','t','A','r','r','a','y','O','f','K','e','y','V','a','l',
            'u','e','O','f','i','n','t','B','i','b','l','e','P','l','a','c','e','M','a','r','k','e','r','4','7','_','P','G','v','v',
            '1','Z','t','y','7','E','p','6','D','1','t','y','7','E','p','6','D','1','\0','I','n','t','e','r','n','e','t','L','i','n',
            'k','R','e','a','d','e','r','\0','G','r','e','e','k','H','e','b','r','e','w','D','i','c','t','R','e','a','d','e','r','\0',
            'A','u','d','i','o','M','o','d','e','l','\0','h','t','t','p',':','/','/','s','c','h','e','m','a','s','.','d','a','t','a',
            'c','o','n','t','r','a','c','t','.','o','r','g','/','2','0','0','4','/','0','7','/','B','a','c','k','g','r','o','u','n',
            'd','A','u','d','i','o','S','h','a','r','e','d','\0','R','a','w','G','e','n','T','e','x','t','R','e','a','d','e','r','\0',
            'D','i','c','t','i','o','n','a','r','y','R','a','w','D','e','f','R','e','a','d','e','r','\0','D','i','c','t','i','o','n',
            'a','r','y','R','a','w','I','n','d','e','x','R','e','a','d','e','r','\0','D','i','c','t','i','o','n','a','r','y','R','a',
            'w','4','I','n','d','e','x','R','e','a','d','e','r','\0','D','i','c','t','i','o','n','a','r','y','Z','l','d','D','e','f',
            'R','e','a','d','e','r','\0','D','i','c','t','i','o','n','a','r','y','Z','l','d','I','n','d','e','x','R','e','a','d','e',
            'r','\0','R','a','w','G','e','n','T','e','x','t','P','l','a','c','e','M','a','r','k','e','r','\0','R','a','w','G','e','n',
            'S','e','a','r','c','h','R','e','a','d','e','r','\0','R','a','w','G','e','n','T','e','x','t','R','e','a','d','e','r','.',
            'C','h','a','p','t','e','r','D','a','t','a','\0','A','r','r','a','y','O','f','R','a','w','G','e','n','T','e','x','t','R',
            'e','a','d','e','r','.','C','h','a','p','t','e','r','D','a','t','a','\0','A','p','p','.','S','e','r','i','a','l','i','z',
            'a','b','l','e','D','a','i','l','y','P','l','a','n','\0','A','r','r','a','y','O','f','K','e','y','V','a','l','u','e','O',
            'f','s','t','r','i','n','g','a','n','y','T','y','p','e','\0','K','e','y','V','a','l','u','e','O','f','s','t','r','i','n',
            'g','a','n','y','T','y','p','e','\0','K','e','y','V','a','l','u','e','P','a','i','r','O','f','s','t','r','i','n','g','a',
            'n','y','T','y','p','e','\0','P','o','s','i','t','i','o','n','M','e','s','s','a','g','e','\0','h','t','t','p',':','/','/',
            's','c','h','e','m','a','s','.','d','a','t','a','c','o','n','t','r','a','c','t','.','o','r','g','/','2','0','0','4','/',
            '0','7','/','B','a','c','k','g','r','o','u','n','d','A','u','d','i','o','S','h','a','r','e','d','.','M','e','s','s','a',
            'g','e','s','\0','E','r','r','o','r','M','e','s','s','a','g','e','\0','T','r','a','c','k','C','h','a','n','g','e','d','M',
            'e','s','s','a','g','e','\0','A','p','p','S','u','s','p','e','n','d','e','d','M','e','s','s','a','g','e','\0','A','p','p',
            'R','e','s','u','m','e','d','M','e','s','s','a','g','e','\0','S','t','a','r','t','P','l','a','y','b','a','c','k','M','e',
            's','s','a','g','e','\0','S','k','i','p','N','e','x','t','M','e','s','s','a','g','e','\0','S','k','i','p','P','r','e','v',
            'i','o','u','s','M','e','s','s','a','g','e','\0','K','i','l','l','M','e','s','s','a','g','e','\0','U','p','d','a','t','e',
            'S','t','a','r','t','P','o','i','n','t','M','e','s','s','a','g','e','\0','B','a','c','k','g','r','o','u','n','d','A','u',
            'd','i','o','T','a','s','k','S','t','a','r','t','e','d','M','e','s','s','a','g','e','\0','A','r','r','a','y','O','f','a',
            'n','y','T','y','p','e','\0','I','s','W','i','n','d','o','w','s','P','h','o','n','e','\0','b','i','b','l','e','s','\0','b',
            'o','o','k','m','a','r','k','s','\0','h','i','g','h','l','i','g','h','t','i','n','g','\0','s','e','t','t','i','n','g','s',
            '\0','t','h','e','m','e','s','\0','w','i','n','d','o','w','S','e','t','u','p','\0','h','i','s','t','o','r','y','\0','B','o',
            'o','k','S','h','o','r','t','N','a','m','e','\0','c','h','a','p','t','e','r','N','u','m','\0','n','o','t','e','\0','v','e',
            'r','s','e','N','u','m','\0','w','h','e','n','\0','A','d','d','L','i','n','e','B','e','t','w','e','e','n','N','o','t','e',
            's','\0','H','i','g','h','l','i','g','h','t','N','a','m','e','1','\0','H','i','g','h','l','i','g','h','t','N','a','m','e',
            '2','\0','H','i','g','h','l','i','g','h','t','N','a','m','e','3','\0','H','i','g','h','l','i','g','h','t','N','a','m','e',
            '4','\0','H','i','g','h','l','i','g','h','t','N','a','m','e','5','\0','H','i','g','h','l','i','g','h','t','N','a','m','e',
            '6','\0','M','a','r','g','i','n','I','n','s','i','d','e','T','e','x','t','W','i','n','d','o','w','\0','N','u','m','b','e',
            'r','O','f','S','c','r','e','e','n','s','\0','O','n','e','D','r','i','v','e','F','o','l','d','e','r','\0','R','e','m','o',
            'v','e','S','c','r','e','e','n','T','r','a','n','s','i','t','i','o','n','s','\0','S','h','o','w','2','t','i','t','l','e',
            'R','o','w','s','\0','S','y','n','c','M','e','d','i','a','V','e','r','s','e','s','\0','U','s','e','H','i','g','h','l','i',
            'g','h','t','s','\0','c','u','s','t','o','m','B','i','b','l','e','D','o','w','n','l','o','a','d','L','i','n','k','s','\0',
            'e','a','c','h','V','e','r','s','e','N','e','w','L','i','n','e','\0','g','r','e','e','k','D','i','c','t','i','o','n','a',
            'r','y','L','i','n','k','\0','h','e','b','r','e','w','D','i','c','t','i','o','n','a','r','y','L','i','n','k','\0','h','i',
            'g','h','l','i','g','h','t','M','a','r','k','i','n','g','s','\0','s','h','o','w','A','d','d','e','d','N','o','t','e','s',
            'B','y','C','h','a','p','t','e','r','\0','s','h','o','w','B','o','o','k','N','a','m','e','\0','s','h','o','w','C','h','a',
            'p','t','e','r','N','u','m','b','e','r','\0','s','h','o','w','H','e','a','d','i','n','g','s','\0','s','h','o','w','M','o',
            'r','p','h','o','l','o','g','y','\0','s','h','o','w','N','o','t','e','P','o','s','i','t','i','o','n','s','\0','s','h','o',
            'w','S','t','r','o','n','g','s','N','u','m','b','e','r','s','\0','s','h','o','w','V','e','r','s','e','N','u','m','b','e',
            'r','\0','s','m','a','l','l','V','e','r','s','e','N','u','m','b','e','r','s','\0','s','o','u','n','d','L','i','n','k','\0',
            'u','s','e','I','n','t','e','r','n','e','t','G','r','e','e','k','H','e','b','r','e','w','D','i','c','t','\0','u','s','e',
            'r','U','n','i','q','u','e','G','u','u','i','d','\0','w','o','r','d','s','O','f','C','h','r','i','s','t','R','e','d','\0',
            'F','o','n','t','\0','I','s','N','t','O','n','l','y','\0','P','a','t','t','e','r','n','\0','S','r','c','\0','V','S','c','h',
            'r','o','l','l','P','o','s','i','t','i','o','n','\0','V','o','i','c','e','N','a','m','e','\0','W','i','n','d','o','w','\0',
            'b','i','b','l','e','D','e','s','c','r','i','p','t','i','o','n','\0','b','i','b','l','e','T','o','L','o','a','d','\0','c',
            'o','d','e','\0','c','u','r','I','n','d','e','x','\0','h','t','m','l','F','o','n','t','S','i','z','e','\0','i','s','S','y',
            'n','c','h','r','o','n','i','z','e','d','\0','n','u','m','R','o','w','s','I','o','w','n','\0','s','o','u','r','c','e','\0',
            'w','i','n','d','o','w','T','y','p','e','\0','s','e','r','i','a','l','\0','s','e','r','i','a','l','2','\0','C','i','p','h',
            'e','r','K','e','y','\0','C','o','n','f','i','g','P','a','t','h','\0','V','e','r','s','i','f','i','c','a','t','i','o','n',
            '\0','i','s','I','s','o','E','n','c','o','d','i','n','g','\0','i','s','o','2','D','i','g','i','t','L','a','n','g','C','o',
            'd','e','\0','p','a','t','h','\0','p','o','s','B','o','o','k','S','h','o','r','t','N','a','m','e','\0','p','o','s','C','h',
            'a','p','t','N','u','m','\0','p','o','s','V','e','r','s','e','N','u','m','\0','b','o','o','k','R','e','l','a','t','i','v',
            'e','C','h','a','p','t','e','r','N','u','m','\0','b','o','o','k','S','t','a','r','t','P','o','s','\0','b','o','o','k','n',
            'u','m','\0','l','e','n','g','t','h','\0','s','t','a','r','t','P','o','s','\0','v','e','r','s','e','s','\0','l','i','s','t',
            'C','h','a','p','t','e','r','s','\0','u','n','u','s','e','d','\0','D','i','s','p','l','a','y','T','e','x','t','\0','B','o',
            'o','k','M','a','r','k','s','T','o','S','h','o','w','\0','S','h','o','w','D','a','t','e','\0','_','t','i','t','l','e','\0',
            'D','i','s','p','l','a','y','T','e','x','t','H','t','m','l','B','o','d','y','\0','F','o','u','n','d','\0','S','e','a','r',
            'c','h','C','h','a','p','t','e','r','\0','S','e','a','r','c','h','T','e','x','t','\0','S','e','a','r','c','h','T','y','p',
            'e','I','n','d','e','x','\0','t','r','a','n','s','l','a','t','i','o','n','F','o','u','n','d','\0','t','r','a','n','s','l',
            'a','t','i','o','n','N','e','w','T','e','s','t','e','m','e','n','t','\0','t','r','a','n','s','l','a','t','i','o','n','O',
            'l','d','T','e','s','t','e','m','e','n','t','\0','t','r','a','n','s','l','a','t','i','o','n','S','e','a','r','c','h','\0',
            't','r','a','n','s','l','a','t','i','o','n','W','h','o','l','e','B','i','b','l','e','\0','t','i','t','l','e','B','r','o',
            'w','s','e','r','W','i','n','d','o','w','\0','L','o','c','a','l','D','i','s','p','l','a','y','S','e','t','t','i','n','g',
            's','\0','N','o','t','e','s','T','o','S','h','o','w','\0','k','e','y','\0','v','a','l','u','e','\0','L','i','n','k','\0','T',
            'i','t','l','e','B','a','r','\0','B','o','o','k','\0','C','h','a','p','t','e','r','\0','C','o','d','e','\0','I','c','o','n',
            '\0','I','c','o','n','L','i','n','k','\0','L','a','n','g','u','a','g','e','\0','N','a','m','e','\0','V','e','r','s','e','\0',
            'L','e','n','g','t','h','\0','P','o','s','i','t','i','o','n','I','n','D','a','t','\0','T','i','t','l','e','\0','W','i','n',
            'd','o','w','M','a','t','c','h','i','n','g','K','e','y','\0','C','h','i','l','d','r','e','n','\0','N','e','x','t','B','r',
            'o','t','h','e','r','\0','N','u','m','C','h','a','r','a','c','t','e','r','s','\0','P','a','r','e','n','t','\0','P','o','s',
            'i','t','i','o','n','I','n','B','d','t','\0','P','o','s','i','t','i','o','n','I','n','C','h','a','p','t','e','r','s','\0',
            'P','e','r','s','o','n','a','l','N','o','t','e','s','V','e','r','s','i','f','i','e','d','\0','P','l','a','n','B','i','b',
            'l','e','\0','P','l','a','n','B','i','b','l','e','D','e','s','c','r','i','p','t','i','o','n','\0','P','l','a','n','T','e',
            'x','t','S','i','z','e','\0','c','u','r','r','e','n','t','C','h','a','p','t','e','r','N','u','m','b','e','r','\0','c','u',
            'r','r','e','n','t','V','e','r','s','e','N','u','m','b','e','r','\0','p','e','r','s','o','n','a','l','N','o','t','e','s',
            '\0','p','l','a','n','D','a','y','N','u','m','b','e','r','\0','p','l','a','n','N','u','m','b','e','r','\0','p','l','a','n',
            'S','t','a','r','t','D','a','t','e','\0','p','r','o','c','e','n','t','\0','m','e','s','s','a','g','e','\0','a','u','d','i',
            'o','M','o','d','e','l','\0','t','i','t','l','e','\0','T','i','m','e','s','t','a','m','p','\0'};
    }
}
