using System;
using System.IO;
using System.Text;
using System.Collections.Generic;using System.Collections;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.De;
using Lucene.Net.Analysis.Standard;

namespace Lucene.Net.Analysis.Fr
{
	/* ====================================================================
	 * The Apache Software License, Version 1.1
	 *
	 * Copyright (c) 2004 The Apache Software Foundation.  All rights
	 * reserved.
	 *
	 * Redistribution and use in source and binary forms, with or without
	 * modification, are permitted provided that the following conditions
	 * are met:
	 *
	 * 1. Redistributions of source code must retain the above copyright
	 *    notice, this list of conditions and the following disclaimer.
	 *
	 * 2. Redistributions in binary form must reproduce the above copyright
	 *    notice, this list of conditions and the following disclaimer in
	 *    the documentation and/or other materials provided with the
	 *    distribution.
	 *
	 * 3. The end-user documentation included with the redistribution,
	 *    if any, must include the following acknowledgment:
	 *       "This product includes software developed by the
	 *        Apache Software Foundation (http://www.apache.org/)."
	 *    Alternately, this acknowledgment may appear in the software itself,
	 *    if and wherever such third-party acknowledgments normally appear.
	 *
	 * 4. The names "Apache" and "Apache Software Foundation" and
	 *    "Apache Lucene" must not be used to endorse or promote products
	 *    derived from this software without prior written permission. For
	 *    written permission, please contact apache@apache.org.
	 *
	 * 5. Products derived from this software may not be called "Apache",
	 *    "Apache Lucene", nor may "Apache" appear in their name, without
	 *    prior written permission of the Apache Software Foundation.
	 *
	 * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
	 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
	 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
	 * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
	 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
	 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
	 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
	 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
	 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
	 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
	 * SUCH DAMAGE.
	 * ====================================================================
	 *
	 * This software consists of voluntary contributions made by many
	 * individuals on behalf of the Apache Software Foundation.  For more
	 * information on the Apache Software Foundation, please see
	 * <http://www.apache.org/>.
	 */

	/// <summary>
	/// Analyzer for french language. Supports an external list of stopwords (words that
	/// will not be indexed at all) and an external list of exclusions (word that will
	/// not be stemmed, but indexed).
	/// A default set of stopwords is used unless an other list is specified, the
	/// exclusionlist is empty by default.
	/// 
	/// <author>Patrick Talbot (based on Gerhard Schwarz work for German)</author>
	/// <version>$Id: FrenchAnalyzer.java,v 1.9 2004/10/17 11:41:40 dnaber Exp $</version>
	/// </summary>
	public sealed class FrenchAnalyzer : Analyzer 
	{

		/// <summary>
		/// Extended list of typical french stopwords.
		/// </summary>
		public static String[] FRENCH_STOP_WORDS = 
				 {
					 "a", "afin", "ai", "ainsi", "après", "attendu", "au", "aujourd", "auquel", "aussi",
					 "autre", "autres", "aux", "auxquelles", "auxquels", "avait", "avant", "avec", "avoir",
					 "c", "car", "ce", "ceci", "cela", "celle", "celles", "celui", "cependant", "certain",
					 "certaine", "certaines", "certains", "ces", "cet", "cette", "ceux", "chez", "ci",
					 "combien", "comme", "comment", "concernant", "contre", "d", "dans", "de", "debout",
					 "dedans", "dehors", "delà", "depuis", "derrière", "des", "désormais", "desquelles",
					 "desquels", "dessous", "dessus", "devant", "devers", "devra", "divers", "diverse",
					 "diverses", "doit", "donc", "dont", "du", "duquel", "durant", "dès", "elle", "elles",
					 "en", "entre", "environ", "est", "et", "etc", "etre", "eu", "eux", "excepté", "hormis",
					 "hors", "hélas", "hui", "il", "ils", "j", "je", "jusqu", "jusque", "l", "la", "laquelle",
					 "le", "lequel", "les", "lesquelles", "lesquels", "leur", "leurs", "lorsque", "lui", "là",
					 "ma", "mais", "malgré", "me", "merci", "mes", "mien", "mienne", "miennes", "miens", "moi",
					 "moins", "mon", "moyennant", "même", "mêmes", "n", "ne", "ni", "non", "nos", "notre",
					 "nous", "néanmoins", "nôtre", "nôtres", "on", "ont", "ou", "outre", "où", "par", "parmi",
					 "partant", "pas", "passé", "pendant", "plein", "plus", "plusieurs", "pour", "pourquoi",
					 "proche", "près", "puisque", "qu", "quand", "que", "quel", "quelle", "quelles", "quels",
					 "qui", "quoi", "quoique", "revoici", "revoilà", "s", "sa", "sans", "sauf", "se", "selon",
					 "seront", "ses", "si", "sien", "sienne", "siennes", "siens", "sinon", "soi", "soit",
					 "son", "sont", "sous", "suivant", "sur", "ta", "te", "tes", "tien", "tienne", "tiennes",
					 "tiens", "toi", "ton", "tous", "tout", "toute", "toutes", "tu", "un", "une", "va", "vers",
					 "voici", "voilà", "vos", "votre", "vous", "vu", "vôtre", "vôtres", "y", "à", "ça", "ès",
					 "été", "être", "ô"
				 };

		/// <summary>
		/// Contains the stopwords used with the StopFilter.
		/// </summary>
		private Dictionary<object,object> stoptable = new Dictionary<object,object>();

		/// <summary>
		/// Contains words that should be indexed but not stemmed.
		/// </summary>
		private Dictionary<object,object> excltable = new Dictionary<object,object>();

		/// <summary>
		/// Builds an analyzer.
		/// </summary>
		public FrenchAnalyzer() 
		{
			stoptable = StopFilter.MakeStopSet( FRENCH_STOP_WORDS );
		}

		/// <summary>
		/// Builds an analyzer with the given stop words.
		/// </summary>
		public FrenchAnalyzer( String[] stopwords ) 
		{
			stoptable = StopFilter.MakeStopSet( stopwords );
		}

		/// <summary>
		/// Builds an analyzer with the given stop words.
		/// </summary>
        public FrenchAnalyzer(Dictionary<object, object> stopwords) 
		{
			stoptable = stopwords;
		}

		/// <summary>
		/// Builds an analyzer with the given stop words.
		/// </summary>
		public FrenchAnalyzer( FileInfo stopwords ) 
		{
			stoptable = WordlistLoaderForAnalyzer.GetWordtable( stopwords );
		}

		/// <summary>
		/// Builds an exclusionlist from an array of Strings.
		/// </summary>
		public void SetStemExclusionTable( String[] exclusionlist ) 
		{
			excltable = StopFilter.MakeStopSet( exclusionlist );
		}

		/// <summary>
		/// Builds an exclusionlist from a Dictionary.
		/// </summary>
		public void SetStemExclusionTable(  Dictionary<object,object> exclusionlist ) 
		{
			excltable = exclusionlist;
		}

		/// <summary>
		/// Builds an exclusionlist from the words contained in the given file.
		/// </summary>
		public void SetStemExclusionTable( FileInfo exclusionlist ) 
		{
			excltable = WordlistLoaderForAnalyzer.GetWordtable( exclusionlist );
		}

		/// <summary>
		/// Creates a TokenStream which tokenizes all the text in the provided Reader.
		/// </summary>
		/// <returns>
		/// A TokenStream build from a StandardTokenizer filtered with
		/// 	StandardFilter, StopFilter, FrenchStemFilter and LowerCaseFilter
		/// </returns>
		public override TokenStream TokenStream( String fieldName, TextReader reader ) 
		{
		
			if (fieldName==null) throw new ArgumentException("fieldName must not be null");
			if (reader==null) throw new ArgumentException("readermust not be null");
				
			TokenStream result = new StandardTokenizer( reader );
			result = new StandardFilter( result );
			result = new StopFilter( result, stoptable );
			result = new FrenchStemFilter( result, excltable );
			// Convert to lowercase after stemming!
			result = new LowerCaseFilter( result );
			return result;
		}
	}

}
