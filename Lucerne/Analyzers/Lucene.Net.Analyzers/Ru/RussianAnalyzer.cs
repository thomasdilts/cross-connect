using System;
using System.Text;
using System.IO;
using System.Collections.Generic;using System.Collections;
using Lucene.Net.Analysis;

namespace Lucene.Net.Analysis.Ru
{
	/// <summary>
	/// Analyzer for Russian language. Supports an external list of stopwords (words that
	/// will not be indexed at all).
	/// A default set of stopwords is used unless an alternative list is specified.
	/// </summary>
	public sealed class RussianAnalyzer : Analyzer
	{
		// letters
		private static char A = (char)0;
		private static char B = (char)1;
		private static char V = (char)2;
		private static char G = (char)3;
		private static char D = (char)4;
		private static char E = (char)5;
		private static char ZH = (char)6;
		private static char Z = (char)7;
		private static char I = (char)8;
		private static char I_ = (char)9;
		private static char K = (char)10;
		private static char L = (char)11;
		private static char M = (char)12;
		private static char N = (char)13;
		private static char O = (char)14;
		private static char P = (char)15;
		private static char R = (char)16;
		private static char S = (char)17;
		private static char T = (char)18;
		private static char U = (char)19;
		//private static char F = (char)20;
		private static char X = (char)21;
		//private static char TS = (char)22;
		private static char CH = (char)23;
		private static char SH = (char)24;
		private static char SHCH = (char)25;
		//private static char HARD = (char)26;
		private static char Y = (char)27;
		private static char SOFT = (char)28;
		private static char AE = (char)29;
		private static char IU = (char)30;
		private static char IA = (char)31;

		/// <summary>
		/// List of typical Russian stopwords.
		/// </summary>
		private static char[][] RUSSIAN_STOP_WORDS = {
		new char[] {A},
		new char[] {B, E, Z},
		new char[] {B, O, L, E, E},
		new char[] {B, Y},
		new char[] {B, Y, L},
		new char[] {B, Y, L, A},
		new char[] {B, Y, L, I},
		new char[] {B, Y, L, O},
		new char[] {B, Y, T, SOFT},
		new char[] {V},
		new char[] {V, A, M},
		new char[] {V, A, S},
		new char[] {V, E, S, SOFT},
		new char[] {V, O},
		new char[] {V, O, T},
		new char[] {V, S, E},
		new char[] {V, S, E, G, O},
		new char[] {V, S, E, X},
		new char[] {V, Y},
		new char[] {G, D, E},
		new char[] {D, A},
		new char[] {D, A, ZH, E},
		new char[] {D, L, IA},
		new char[] {D, O},
		new char[] {E, G, O},
		new char[] {E, E},
		new char[] {E, I_,},
		new char[] {E, IU},
		new char[] {E, S, L, I},
		new char[] {E, S, T, SOFT},
		new char[] {E, SHCH, E},
		new char[] {ZH, E},
		new char[] {Z, A},
		new char[] {Z, D, E, S, SOFT},
		new char[] {I},
		new char[] {I, Z},
		new char[] {I, L, I},
		new char[] {I, M},
		new char[] {I, X},
		new char[] {K},
		new char[] {K, A, K},
		new char[] {K, O},
		new char[] {K, O, G, D, A},
		new char[] {K, T, O},
		new char[] {L, I},
		new char[] {L, I, B, O},
		new char[] {M, N, E},
		new char[] {M, O, ZH, E, T},
		new char[] {M, Y},
		new char[] {N, A},
		new char[] {N, A, D, O},
		new char[] {N, A, SH},
		new char[] {N, E},
		new char[] {N, E, G, O},
		new char[] {N, E, E},
		new char[] {N, E, T},
		new char[] {N, I},
		new char[] {N, I, X},
		new char[] {N, O},
		new char[] {N, U},
		new char[] {O},
		new char[] {O, B},
		new char[] {O, D, N, A, K, O},
		new char[] {O, N},
		new char[] {O, N, A},
		new char[] {O, N, I},
		new char[] {O, N, O},
		new char[] {O, T},
		new char[] {O, CH, E, N, SOFT},
		new char[] {P, O},
		new char[] {P, O, D},
		new char[] {P, R, I},
		new char[] {S},
		new char[] {S, O},
		new char[] {T, A, K},
		new char[] {T, A, K, ZH, E},
		new char[] {T, A, K, O, I_},
		new char[] {T, A, M},
		new char[] {T, E},
		new char[] {T, E, M},
		new char[] {T, O},
		new char[] {T, O, G, O},
		new char[] {T, O, ZH, E},
		new char[] {T, O, I_},
		new char[] {T, O, L, SOFT, K, O},
		new char[] {T, O, M},
		new char[] {T, Y},
		new char[] {U},
		new char[] {U, ZH, E},
		new char[] {X, O, T, IA},
		new char[] {CH, E, G, O},
		new char[] {CH, E, I_},
		new char[] {CH, E, M},
		new char[] {CH, T, O},
		new char[] {CH, T, O, B, Y},
		new char[] {CH, SOFT, E},
		new char[] {CH, SOFT, IA},
		new char[] {AE, T, A},
		new char[] {AE, T, I},
		new char[] {AE, T, O},
		new char[] {IA}
													 };

		/// <summary>
		/// Contains the stopwords used with the StopFilter.
		/// </summary>
		private Dictionary<object,object> stoptable = new Dictionary<object,object>();

		/// <summary>
		/// Charset for Russian letters.
	    /// Represents encoding for 32 lowercase Russian letters.
		/// Predefined charsets can be taken from RussianCharSets class
		/// </summary>
		private char[] charset;

		/// <summary>
		/// Builds an analyzer.
		/// </summary>
		public RussianAnalyzer()
		{
			this.charset = RussianCharsets.UnicodeRussian;
			stoptable = StopFilter.MakeStopSet(MakeStopWords(RussianCharsets.UnicodeRussian));
		}

		/// <summary>
		/// Builds an analyzer.
		/// </summary>
		/// <param name="charset"></param>
		public RussianAnalyzer(char[] charset)
		{
			this.charset = charset;
			stoptable = StopFilter.MakeStopSet(MakeStopWords(charset));
		}

		/// <summary>
		/// Builds an analyzer with the given stop words.
		/// </summary>
		/// <param name="charset"></param>
		/// <param name="stopwords"></param>
		public RussianAnalyzer(char[] charset, String[] stopwords)
		{
			this.charset = charset;
			stoptable = StopFilter.MakeStopSet(stopwords);
		}

		/// <summary>
		/// Takes russian stop words and translates them to a String array, using
		/// the given charset 
		/// </summary>
		/// <param name="charset"></param>
		/// <returns></returns>
		private static String[] MakeStopWords(char[] charset)
		{
			String[] res = new String[RUSSIAN_STOP_WORDS.Length];
			for (int i = 0; i < res.Length; i++)
			{
				char[] theStopWord = RUSSIAN_STOP_WORDS[i];
				// translate the word,using the charset
				StringBuilder theWord = new StringBuilder();
				for (int j = 0; j < theStopWord.Length; j++)
				{
					theWord.Append(charset[theStopWord[j]]);
				}
				res[i] = theWord.ToString();
			}
			return res;
		}

		/// <summary>
		/// Builds an analyzer with the given stop words.
		/// </summary>
		/// <param name="charset"></param>
		/// <param name="stopwords"></param>
		public RussianAnalyzer(char[] charset,  Dictionary<object,object> stopwords)
		{
			this.charset = charset;
			stoptable = stopwords;
		}

		/// <summary>
		/// Creates a TokenStream which tokenizes all the text in the provided TextReader.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="reader"></param>
		/// <returns>
		///		A TokenStream build from a RussianLetterTokenizer filtered with
		///     RussianLowerCaseFilter, StopFilter, and RussianStemFilter
		///  </returns>
		public override TokenStream TokenStream(String fieldName, TextReader reader)
		{
			TokenStream result = new RussianLetterTokenizer(reader, charset);
			result = new RussianLowerCaseFilter(result, charset);
			result = new StopFilter(result, stoptable);
			result = new RussianStemFilter(result, charset);
			return result;
		}
	}
}