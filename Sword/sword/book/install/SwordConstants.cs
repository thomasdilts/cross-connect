///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
/// the terms of the GNU Lesser General Public License, version 2.1 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/lgpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
///
/// Copyright: 2005
///     The copyright to this program is held by it's authors.
///
/// ID: $Id: SwordConstants.java 1995 2010-10-23 20:39:04Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
namespace book.install
{

	//using Verse = org.crosswire.jsword.passage.Verse;
	//using BibleNames = org.crosswire.jsword.versification.BibleNames;

	///
	/// <summary> A Constants to help the SwordBookDriver to read Sword format data.
	///  </summary>
	/// <seealso cref= gnu.lgpl.License for license details.<br>
	///      The copyright to this program is held by it's authors.
	/// @author Mark Goodwin [mark at thorubio dot org]
	/// @author Joe Walker [joe at eireneh dot com]
	/// @author The Sword project (don't know who - no credits in original files
	///         (canon.h)) </seealso>
	/// 
	public class SwordConstants
	{
	///    
	/// <summary>* Prevent instantiation </summary>
	///     
		private SwordConstants()
		{
		}

	///    
	/// <summary>* New testament data files </summary>
	///     
		public const string FILE_NT = "nt";

	///    
	/// <summary>* Old testament data files </summary>
	///     
		public const string FILE_OT = "ot";

	///    
	/// <summary>* Index file extensions </summary>
	///     
		public const string EXTENSION_VSS = ".vss";

	///    
	/// <summary>* Extension for index files </summary>
	///     
		public const string EXTENSION_INDEX = ".idx";

	///    
	/// <summary>* Extension for data files </summary>
	///     
		public const string EXTENSION_DATA = ".dat";

	///    
	/// <summary>* Extension for config files </summary>
	///     
		public const string EXTENSION_CONF = ".conf";

	///    
	/// <summary>* The data directory </summary>
	///     
		public const string DIR_DATA = "modules";

	///    
	/// <summary>* The configuration directory </summary>
	///     
		public const string DIR_CONF = "mods.d";

	///    
	/// <summary>* constant for the introduction </summary>
	///     
		public const int TESTAMENT_INTRO = 0;

	///    
	/// <summary>* constant for the old testament </summary>
	///     
		public const int TESTAMENT_OLD = 1;

	///    
	/// <summary>* constant for the new testament </summary>
	///     
		public const int TESTAMENT_NEW = 2;

	///    
	/// <summary>* Get the testament of a given verse </summary>
	///     
        /*
		public static int getTestament(Verse v)
		{
			int ord = v.Ordinal;

			if (ord >= SwordConstants.ORDINAL_MAT11)
			{
				// This is an NT verse
				return SwordConstants.TESTAMENT_NEW;
			}
			// This is an OT verse
			return SwordConstants.TESTAMENT_OLD;
		}

	///    
	/// <summary>* Get the sword index of the given verse </summary>
	///     
		internal static int getIndex(Verse v)
		{
			int ord = v.Ordinal;
			int book = v.Book;
			int chapter = v.Chapter;
			int verse = v.Verse;
			int testament = -1;

			if (ord >= SwordConstants.ORDINAL_MAT11)
			{
				// This is an NT verse
				testament = SwordConstants.TESTAMENT_NEW;
				book = book - BibleNames.MALACHI;
			}
			else
			{
				// This is an OT verse
				testament = SwordConstants.TESTAMENT_OLD;
			}

			short bookOffset = SwordConstants.bks[testament][book];
			short chapOffset = SwordConstants.cps[testament][bookOffset + chapter];

			return verse + chapOffset;
		}

	///    
	/// <summary>* The start of the new testament </summary>
	///     
		internal const int ORDINAL_MAT11 = new Verse(BibleNames.MATTHEW, 1, 1, true).Ordinal;
        */
        /*
	///    
	/// <summary>* array containing LUT of offsets in the chapter table. </summary>
	///     
		private static short[][] bks;

	///    
	/// <summary>* array containing LUT of positions of initial verses per chapter. This and
	/// all the cps* below were longs and then ints This was an artifact of a
	/// port from C/C++ where int/long vary in size depending on architecture. </summary>
	///     
		private static short[][] cps;

	///    
	/// <summary>* Initialize our LUTs with data shamelessly stolen from our sister project
	/// (Sword) taken from canon.h.
	/// 
	/// The basic feature of an index is that starting at 4 for Gen 1.1 and Mat
	/// 1.1, increment 1 for each subsequent verse. At a chapter boundary, skip
	/// 1. At a book boundary skip 1 for the book and 1 for the chapter.
	/// 
	/// Book 0.0 gives the index for the book's info. ( == index of Book 1.1 - 2)
	/// Book ch.0 gives the index for the chapter's info in the book. ( == index
	/// of Book 1.1 - 1)
	/// 
	/// There are arrays of data like this in BibleInfo. I guess we could merge
	/// them at some stage.
	///  </summary>
	/// <seealso cref= org.crosswire.jsword.passage.BibleInfo </seealso>
	///     
		static SwordConstants()
		{
			bks = new short[3][];
			cps = new short[3][];

			short[] bksot = { 0, 1, 52, 93, 121, 158, 193, 218, 240, 245, 277, 302, 325, 351, 381, 418, 429, 443, 454, 497, 648, 680, 693, 702, 769, 822, 828, 877, 890, 905, 909, 919, 921, 926, 934, 938, 942, 946, 949, 964 };

			short[] bksnt = { 0, 1, 30, 47, 72, 94, 123, 140, 157, 171, 178, 185, 190, 195, 201, 205, 212, 217, 221, 223, 237, 243, 249, 253, 259, 261, 263, 265 };

			bks[SwordConstants.TESTAMENT_OLD] = bksot;
			bks[SwordConstants.TESTAMENT_NEW] = bksnt;

			short[] cpsot = { 0, 2, 3, 35, 61, 86, 113, 146, 169, 194, 217, 247, 280, 313, 334, 353, 378, 400, 417, 445, 479, 518, 537, 572, 597, 618, 686, 721, 757, 804, 827, 863, 907, 963, 996, 1017, 1049, 1079, 1123, 1160, 1191, 1215, 1239, 1297, 1336, 1371, 1406, 1435, 1470, 1502, 1525, 1559, 1586, 1587, 1610, 1636, 1659, 1691, 1715, 1746, 1772, 1805, 1841, 1871, 1882, 1934, 1957, 1989, 2017, 2054, 2071, 2099, 2125, 2152, 2189, 2221, 2255, 2274, 2315, 2353, 2375, 2419, 2466, 2505, 2524, 2560, 2584, 2620, 2656, 2695, 2725, 2757, 2801, 2840, 2841, 2859, 2876, 2894, 2930, 2950, 2981, 3020, 3057, 3082, 3103, 3151, 3160, 3220, 3278, 3312, 3347, 3364, 3395, 3433, 3461, 3486, 3520, 3565, 3589, 3645, 3692, 3727, 3728, 3783, 3818, 3870, 3920, 3952, 3980, 4070, 4097, 4121, 4158, 4194, 4211, 4245, 4291, 4333, 4384, 4398, 4431, 4454, 4484, 4520, 4562, 4593, 4619, 4638, 4704, 4728, 4760, 4801, 4818, 4873, 4916, 4973, 5003, 5038, 5052, 5053, 5100, 5138, 5168, 5218, 5252, 5278, 5305, 5326, 5356, 5379, 5412, 5445, 5464, 5494, 5518, 5541, 5562, 5585, 5607, 5628, 5652, 5683, 5709, 5732, 5752, 5772, 5799, 5868, 5898, 5919, 5950, 6003, 6033, 6046, 6047, 6066, 6091, 6109, 6134, 6150, 6178, 6205, 6241, 6269, 6313, 6337, 6362, 6396, 6412, 6476, 6487, 6506, 6535, 6587, 6597, 6643, 6678, 6695, 6729, 6730, 6767, 6791, 6823, 6848, 6880, 6921, 6947, 6983, 7041, 7060, 7101, 7117, 7143, 7164, 7185, 7217, 7231, 7263, 7294, 7343, 7369, 7370, 7393, 7417, 7436, 7459, 7460, 7489, 7526, 7548, 7571, 7584, 7606, 7624, 7647, 7675, 7703, 7719, 7745, 7769, 7822, 7858, 7882, 7941, 7972, 7997, 8040, 8056, 8080, 8110, 8133, 8178, 8204, 8217, 8243, 8255, 8287, 8301, 8302, 8330, 8363, 8403, 8416, 8442, 8466, 8496, 8515, 8529, 8549, 8577, 8609, 8649, 8683, 8721, 8745, 8775, 8809, 8853, 8880, 8903, 8955, 8995, 9021, 9022, 9076, 9123, 9152, 9187, 9206, 9245, 9297, 9364, 9393, 9423, 9467, 9501, 9536, 9568, 9603, 9638, 9663, 9710, 9732, 9776, 9806, 9860, 9861, 9880, 9906, 9934, 9979, 10007, 10041, 10062, 10092, 10130, 10167, 10189, 10211, 10237, 10267, 10306, 10327, 10369, 10407, 10445, 10467, 10494, 10515, 10553, 10574, 10605, 10606, 10661, 10717, 10742, 10786, 10813, 10895, 10936, 10977, 11022, 11037, 11085, 11126, 11141, 11159, 11189, 11233, 11261, 11279, 11299, 11308, 11339, 11359, 11392, 11424, 11456, 11489, 11524, 11546, 11577, 11578, 11596, 11615, 11633, 11656, 11671, 11714, 11737, 11756, 11788, 11808, 11832, 11849, 11872, 11888, 11908, 11923, 11943, 11978, 11990, 12028, 12049, 12062, 12084, 12112, 12141, 12165, 12175, 12203, 12240, 12268, 12290, 12324, 12350, 12384, 12412, 12436, 12437, 12449, 12520, 12534, 12559, 12577, 12600, 12629, 12666, 12682, 12727, 12728, 12740, 12761, 12794, 12818, 12838, 12858, 12932, 12951, 12990, 13030, 13067, 13115, 13147, 13148, 13171, 13195, 13211, 13229, 13244, 13259, 13270, 13288, 13321, 13325, 13326, 13349, 13363, 13390, 13412, 13440, 13471, 13493, 13516, 13552, 13575, 13596, 13622, 13651, 13674, 13710, 13733, 13750, 13772, 13802, 13832, 13867, 13898, 13916, 13942, 13949, 13964, 13988, 14017, 14043, 14075, 14116, 14139, 14173, 14211, 14228, 14262, 14287, 14329, 14360, 14385, 14420, 14438, 14439, 14446, 14459, 14468, 14477, 14490, 14501, 14519, 14529, 14550, 14569, 14577, 14586, 14593, 14601, 14607, 14619, 14635, 14686, 14701, 14711, 14725, 14757, 14764, 14775, 14798, 14811, 14826, 14836, 14848, 14861, 14886, 14898, 14921, 14944, 14973, 14986, 15027, 15050, 15064, 15082, 15096, 15108, 15114, 15141, 15159, 15171, 15181, 15196, 15217, 15241, 15261, 15271, 15278, 15286, 15310, 15324, 15336, 15348, 15366, 15379, 15388, 15401, 15413, 15424, 15438, 15459, 15467, 15503, 15540, 15546, 15571, 15592, 15621, 15645, 15656, 15669, 15690, 15763, 15777, 15797, 15814, 15823, 15842, 15855, 15869, 15887, 15895, 15914, 15967, 15985, 16002, 16018, 16024, 16048, 16060, 16074, 16087, 16097, 16107, 16113, 16122, 16151, 16174, 16210, 16256, 16305, 16349, 16363, 16395, 16403, 16414, 16425, 16435, 16444, 16463, 16483, 16486, 16516, 16693, 16701, 16710, 16720, 16725, 16734, 16740, 16747, 16753, 16760, 16769, 16778, 16782, 16801, 16805, 16809, 16831, 16858, 16868, 16877, 16902, 16916, 16927, 16935, 16948, 16964, 16986, 16997, 17018, 17033, 17043, 17050, 17051, 17085, 17108, 17144, 17172, 17196, 17232, 17260, 17297, 17316, 17349, 17381, 17410, 17436, 17472, 17506, 17540, 17569, 17594, 17624, 17655, 17687, 17717, 17753, 17788, 17817, 17846, 17874, 17903, 17931, 17965, 17997, 17998, 18017, 18044, 18067, 18084, 18105, 18118, 18148, 18166, 18185, 18206, 18217, 18232, 18233, 18251, 18269, 18281, 18298, 18315, 18329, 18343, 18358, 18359, 18391, 18414, 18441, 18448, 18479, 18493, 18519, 18542, 18564, 18599, 18616, 18623, 18646, 18679, 18689, 18704, 18719, 18727, 18753, 18760, 18778, 18804, 18823, 18847, 18860, 18882, 18896, 18926, 18951, 18985, 18995, 19016, 19041, 19059, 19070, 19093, 19132, 19155, 19164, 19196, 19226, 19252, 19281, 19310, 19336, 19350, 19366, 19389, 19416, 19428, 19452, 19468, 19481, 19499, 19513, 19526, 19548, 19563, 19585, 19608, 19620, 19633, 19653, 19666, 19692, 19717, 19718, 19738, 19776, 19802, 19834, 19866, 19897, 19932, 19955, 19982, 20008, 20032, 20050, 20078, 20101, 20123, 20145, 20173, 20197, 20213, 20232, 20247, 20278, 20319, 20330, 20369, 20394, 20417, 20435, 20468, 20493, 20534, 20579, 20606, 20629, 20649, 20682, 20704, 20733, 20752, 20769, 20788, 20811, 20825, 20856, 20862, 20891, 20899, 20947, 20987, 21034, 21099, 21134, 21135, 21158, 21181, 21248, 21271, 21294, 21295, 21324, 21335, 21363, 21381, 21399, 21414, 21442, 21461, 21473, 21496, 21522, 21551, 21575, 21599, 21608, 21672, 21697, 21730, 21745, 21795, 21828, 21860, 21910, 21938, 21956, 21978, 22015, 22042, 22064, 22091, 22110, 22143, 22177, 22209, 22225, 22264, 22293, 22317, 22347, 22397, 22424, 22445, 22473, 22505, 22531, 22556, 22580, 22616, 22617, 22639, 22689, 22720, 22758, 22790, 22819, 22848, 22876, 22904, 22926, 22972, 22986, 22987, 22999, 23023, 23029, 23049, 23065, 23077, 23094, 23109, 23127, 23143, 23156, 23171, 23188, 23198, 23199, 23220, 23253, 23275, 23276, 23292, 23309, 23325, 23339, 23367, 23382, 23400, 23415, 23431, 23432, 23454, 23455, 23473, 23484, 23495, 23507, 23508, 23525, 23539, 23552, 23566, 23582, 23599, 23620, 23621, 23637, 23651, 23671, 23672, 23690, 23711, 23731, 23732, 23751, 23767, 23788, 23789, 23805, 23829, 23830, 23852, 23866, 23877, 23892, 23904, 23920, 23935, 23959, 23977, 23990, 24008, 24023, 24033, 24055, 24056, 24071, 24089, 24108 };

			short[] cpsnt = { 0, 2, 3, 29, 53, 71, 97, 146, 181, 211, 246, 285, 328, 359, 410, 469, 506, 546, 575, 603, 639, 670, 705, 752, 799, 839, 891, 938, 1014, 1081, 1102, 1103, 1149, 1178, 1214, 1256, 1300, 1357, 1395, 1434, 1485, 1538, 1572, 1617, 1655, 1728, 1776, 1797, 1798, 1879, 1932, 1971, 2016, 2056, 2106, 2157, 2214, 2277, 2320, 2375, 2435, 2471, 2507, 2540, 2572, 2610, 2654, 2703, 2751, 2790, 2862, 2919, 2973, 2974, 3026, 3052, 3089, 3144, 3192, 3264, 3318, 3378, 3420, 3463, 3521, 3572, 3611, 3643, 3671, 3705, 3732, 3773, 3816, 3848, 3874, 3875, 3902, 3950, 3977, 4015, 4058, 4074, 4135, 4176, 4220, 4269, 4300, 4326, 4379, 4408, 4450, 4491, 4526, 4555, 4597, 4636, 4677, 4708, 4744, 4772, 4800, 4833, 4878, 4910, 4911, 4944, 4974, 5006, 5032, 5054, 5078, 5104, 5144, 5178, 5200, 5237, 5259, 5274, 5298, 5332, 5360, 5361, 5393, 5410, 5434, 5456, 5470, 5491, 5532, 5546, 5574, 5608, 5643, 5675, 5689, 5730, 5789, 5814, 5815, 5840, 5858, 5877, 5896, 5918, 5937, 5954, 5979, 5995, 6014, 6048, 6070, 6085, 6086, 6111, 6133, 6163, 6195, 6222, 6241, 6242, 6266, 6289, 6311, 6344, 6378, 6403, 6404, 6435, 6466, 6488, 6512, 6513, 6543, 6567, 6593, 6612, 6613, 6624, 6645, 6659, 6678, 6707, 6708, 6721, 6739, 6758, 6759, 6780, 6796, 6813, 6830, 6856, 6878, 6879, 6898, 6925, 6943, 6966, 6967, 6984, 7000, 7016, 7017, 7043, 7044, 7059, 7078, 7098, 7115, 7130, 7151, 7180, 7194, 7223, 7263, 7304, 7334, 7360, 7361, 7389, 7416, 7435, 7453, 7474, 7475, 7501, 7527, 7550, 7570, 7585, 7586, 7608, 7631, 7650, 7651, 7662, 7692, 7717, 7739, 7761, 7762, 7776, 7777, 7792, 7793, 7819, 7820, 7841, 7871, 7894, 7906, 7921, 7939, 7957, 7971, 7993, 8005, 8025, 8043, 8062, 8083, 8092, 8114, 8133, 8158, 8180, 8196, 8224 };

			cps[SwordConstants.TESTAMENT_OLD] = cpsot;
			cps[SwordConstants.TESTAMENT_NEW] = cpsnt;
		}*/
	}

}