using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Sword.versification
{
    public static class CanonManager
    {
        /* this is the true list of allowed book names. It is complete but unnecessary and therefore commented out
        public static Dictionary<string, string> AllowedBookNames = new Dictionary<string, string>
        {
            {"Genesis","Gen"},
            {"Exodus","Exod"},
            {"Leviticus","Lev"},
            {"Numbers","Num"},
            {"Deuteronomy","Deut"},
            {"Joshua","Josh"},
            {"Judges","Judg"},
            {"Ruth","Ruth"},
            {"I Samuel","1Sam"},
            {"II Samuel","2Sam"},
            {"I Kings","1Kgs"},
            {"II Kings","2Kgs"},
            {"I Chronicles","1Chr"},
            {"II Chronicles","2Chr"},
            {"Ezra","Ezra"},
            {"Nehemiah","Neh"},
            {"Esther","Esth"},
            {"Job","Job"},
            {"Psalms","Ps"},
            {"Proverbs","Prov"},
            {"Ecclesiastes","Eccl"},
            {"Song of Solomon","Song"},
            {"Isaiah","Isa"},
            {"Jeremiah","Jer"},
            {"Lamentations","Lam"},
            {"Ezekiel","Ezek"},
            {"Daniel","Dan"},
            {"Hosea","Hos"},
            {"Joel","Joel"},
            {"Amos","Amos"},
            {"Obadiah","Obad"},
            {"Jonah","Jonah"},
            {"Micah","Mic"},
            {"Nahum","Nah"},
            {"Habakkuk","Hab"},
            {"Zephaniah","Zeph"},
            {"Haggai","Hag"},
            {"Zechariah","Zech"},
            {"Malachi","Mal"},
            {"I Esdras","1Esd"},
            {"II Esdras","2Esd"},
            {"Tobit","Tob"},
            {"Judith","Jdt"},
            {"Additions to Esther","AddEsth"},
            {"Wisdom","Wis"},
            {"Sirach","Sir"},
            {"Baruch","Bar"},
            {"Prayer of Azariah","PrAzar"},
            {"Susanna","Sus"},
            {"Bel and the Dragon","Bel"},
            {"Prayer of Manasses","PrMan"},
            {"I Maccabees","1Macc"},
            {"II Maccabees","2Macc"},
            {"Esther (Greek)","EsthGr"},
            {"Additional Psalm","AddPs"},
            {"III Maccabees","3Macc"},
            {"IV Maccabees","4Macc"},
            {"Epistle of Jeremiah","EpJer"},
            {"Additions to Daniel","AddDan"},
            {"Psalms of Solomon","PssSol"},
            {"I Enoch","1En"},
            {"Odes","Odes"},
            {"Matthew","Matt"},
            {"Mark","Mark"},
            {"Luke","Luke"},
            {"John","John"},
            {"Acts","Acts"},
            {"Romans","Rom"},
            {"I Corinthians","1Cor"},
            {"II Corinthians","2Cor"},
            {"Galatians","Gal"},
            {"Ephesians","Eph"},
            {"Philippians","Phil"},
            {"Colossians","Col"},
            {"I Thessalonians","1Thess"},
            {"II Thessalonians","2Thess"},
            {"I Timothy","1Tim"},
            {"II Timothy","2Tim"},
            {"Titus","Titus"},
            {"Philemon","Phlm"},
            {"Hebrews","Heb"},
            {"James","Jas"},
            {"I Peter","1Pet"},
            {"II Peter","2Pet"},
            {"I John","1John"},
            {"II John","2John"},
            {"III John","3John"},
            {"Jude","Jude"},
            {"Revelation of John","Rev"},
            {"Laodiceans","EpLao"}
        };*/
        private static Dictionary<string, Canon> canons = new Dictionary<string, Canon> 
        {                 
            {"KJV", new Canon()},
            {"Leningrad", null},
            {"MT", null},
            {"KJVA", null},
            {"NRSV", null},
            {"NRSVA", null},
            {"Synodal", null},
            {"SynodalProt", null},
            {"Vulg", null},
            {"German", null},
            {"Luther", null},
            {"Catholic", null},
            {"Catholic2", null},
            {"LXX", null},
            {"Orthodox", null}
        };

        public static void debug()
        {
            Dictionary<string, string> OTBookByLongName = new Dictionary<string,string>();
            Dictionary<string, string> NTBookByLongName = new Dictionary<string,string>();
            foreach (var cc in canons)
            {
                var canon = GetCanon(cc.Key);
                var oldTestBooks = canon.OldTestBooks;
                foreach (var book in oldTestBooks)
                {
                    OTBookByLongName[book.FullName] = book.ShortName1;
                }

                var newTestBooks = canon.NewTestBooks;
                foreach (var book in newTestBooks)
                {
                    NTBookByLongName[book.FullName] = book.ShortName1;
                }
            }

           

            foreach (var item in OTBookByLongName)
	        {
		        Debug.WriteLine("<book key=\""+ item.Value +"\" full=\""+ item.Key +"\" />");
	        }
            foreach (var item in NTBookByLongName)
	        {
		        Debug.WriteLine("<book key=\""+ item.Value +"\" full=\""+ item.Key +"\" />");
	        }
        }


        public static Canon GetCanon(string crossWireName)
        {
            Canon canon;
            if(string.IsNullOrEmpty(crossWireName) || !canons.TryGetValue(crossWireName, out canon))
            {
                return canons["KJV"];
            }

            // this is so that we don't need to load everything into memory at start
            if(canon == null)
            {
                switch(crossWireName)
                {
                    case "Leningrad":
                        canons[crossWireName] = new CanonLeningrad();
                        break;
                    case "MT":
                        canons[crossWireName] = new CanonMt();
                        break;
                    case "KJVA":
                        canons[crossWireName] = new CanonKjva();
                        break;
                    case "NRSV":
                        canons[crossWireName] = new CanonNrsv();
                        break;
                    case "NRSVA":
                        canons[crossWireName] = new CanonNrsva();
                        break;
                    case "Synodal":
                        canons[crossWireName] = new CanonSynodal();
                        break;
                    case "SynodalProt":
                        canons[crossWireName] = new CanonSynodalProt();
                        break;
                    case "Vulg":
                        canons[crossWireName] = new CanonVulg();
                        break;
                    case "German":
                        canons[crossWireName] = new CanonGerman();
                        break;
                    case "Luther":
                        canons[crossWireName] = new CanonLuther();
                        break;
                    case "Catholic":
                        canons[crossWireName] = new CanonCatholic();
                        break;
                    case "Catholic2":
                        canons[crossWireName] = new CanonCatholic2();
                        break;
                    case "LXX":
                        canons[crossWireName] = new CanonLxx();
                        break;
                    case "Orthodox":
                        canons[crossWireName] = new CanonOrthodox();
                        break;
                }

                return canons[crossWireName];
            }

            return canon;
        }

        private static Dictionary<string, string> AllAbbreviations = null;
        public static string GetShortNameFromAbbreviation(string abbrev)
        {
            if(AllAbbreviations==null)
            {
                AllAbbreviations = new Dictionary<string, string>();
                var canon = GetCanon("NRSVA");
                foreach (var book in canon.BookByShortName)
                {
                    // these 2 might be the same. On second try we force it
                    AllAbbreviations.Add(book.Value.ShortName1.ToLower(), book.Value.ShortName1);
                    AllAbbreviations[book.Value.FullName.ToLower()] = book.Value.ShortName1;
                    var allNames = book.Value.ShortName2.Split(',');
                    if (allNames != null && allNames.Any())
                    {
                        foreach (var name in allNames)
                        {
                            AllAbbreviations.Add(name.ToLower(), book.Value.ShortName1);
                        }
                    }
                }
            }

            var foundShortName = string.Empty;
            AllAbbreviations.TryGetValue(abbrev.ToLower(), out foundShortName);
            return foundShortName;
        }
    }
}
