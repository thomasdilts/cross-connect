#region Header

// <copyright file="MorphologyTranslator.cs" company="Thomas Dilts">
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

namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public static class MorphologyTranslator
    {
        #region Fields

        private static readonly Dictionary<string, string> Case = new Dictionary<string, string> {
                { "N", "Nominative" },
                { "V", "Vocative" },
                { "G", "Genitive" },
                { "D", "Dative" },
                { "A", "Accusative" },
            };
        private static readonly Dictionary<string, string> Gender = new Dictionary<string, string> { { "M", "Masculine" }, { "F", "Feminine" }, { "N", "Neuter" }, };
        private static readonly Dictionary<string, string> Number = new Dictionary<string, string> { { "S", "Singular" }, { "P", "Plural" }, };
        private static readonly Dictionary<string, string> Person = new Dictionary<string, string> { { "1", "First person" }, { "2", "Second person" }, { "3", "Third person" } };
        private static readonly Dictionary<string, string> Suffixes = new Dictionary<string, string> {
                { "S", "Superlative" },
                { "C", "Comparative" },
                { "ABB", "Abbreviated form" },
                { "I", "Interrogative" },
                { "N", "Negative" },
                { "K", "Contracted form, or two words merged by crasis" },
                { "ATT", "Attic Greek form" },
                { "P", "Particle attached (with relative pronoun)" }
            };
        private static readonly Dictionary<string, MorphTrans> UndeclinedForms = new Dictionary<string, MorphTrans> {
                { "ADV", new MorphTrans(GetSuffix, "Adverb or adverb and particle combined") },
                { "CONJ", new MorphTrans(GetSuffix, "Conjunction or conjunctive particle") },
                { "COND", new MorphTrans(GetSuffix, "Conditional particle or conjunction") },
                { "PRT", new MorphTrans(GetSuffix, "Particle, disjunctive particle") },
                { "PREP", new MorphTrans(GetSuffix, "Preposition") },
                { "INJ", new MorphTrans(GetSuffix, "Interjection") },
                { "ARAM", new MorphTrans(GetSuffix, "Aramaic transliterated word (indeclinable)") },
                { "HEB", new MorphTrans(GetSuffix, "Hebrew transliterated word (indeclinable)") },
                { "N-PRI", new MorphTrans(GetSuffix, "Indeclinable Proper Noun") },
                { "A-NUI", new MorphTrans(GetSuffix, "Indeclinable Numeral (Adjective)") },
                { "N-LI", new MorphTrans(GetSuffix, "Indeclinable Letter (Noun)") },
                { "N-OI", new MorphTrans(GetSuffix, "Indeclinable Noun of Other type") },
                { "N", new MorphTrans(GetDecline, "Noun") },
                { "A", new MorphTrans(GetDecline, "Adjective") },
                { "R", new MorphTrans(GetDecline, "Relative pronoun") },
                { "C", new MorphTrans(GetDecline, "Reciprocal pronoun") },
                { "D", new MorphTrans(GetDecline, "Demonstrative pronoun") },
                { "T", new MorphTrans(GetDecline, "Definite article") },
                { "K", new MorphTrans(GetDecline, "Correlative pronoun") },
                { "I", new MorphTrans(GetDecline, "Interrogative pronoun") },
                { "X", new MorphTrans(GetDecline, "Indefinite pronoun") },
                { "Q", new MorphTrans(GetDecline, "Correlative or interrogative pronoun") },
                { "F", new MorphTrans(GetPerson, "Reflexive pronoun") },
                { "S", new MorphTrans(GetPerson, "Possessive pronoun") },
                { "P", new MorphTrans(GetPerson, "Personal pronoun") },
                { "V", new MorphTrans(GetVerb, "Verb") },
            };
        private static readonly Dictionary<string, string> VerbMood = new Dictionary<string, string> {
                { "I", "Indicative" },
                { "S", "Subjunctive" },
                { "O", "Optative" },
                { "M", "Imperative" },
                { "N", "Infinitive" },
                { "P", "Participle" },
                { "R", "Imperative-sense participle" },
            };
        private static readonly Dictionary<string, string> VerbSuffix = new Dictionary<string, string> {
                { "M", "Middle significance" },
                { "C", "Contracted form" },
                { "T", "Transitive" },
                { "A", "Aeolic" },
                { "ATT", "Attic" },
                { "AP", "Apocopated form" },
                { "IRR", "Irregular or Impure form" },
            };
        private static readonly Dictionary<string, string> VerbTense = new Dictionary<string, string> {
                { "P", "Present" },
                { "I", "Imperfect" },
                { "F", "Future" },
                { "2F", "Second Future" },
                { "A", "Aorist" },
                { "2A", "Second Aorist" },
                { "R", "Perfect" },
                { "2R", "Second perfect" },
                { "L", "Pluperfect" },
                { "2L", "Second pluperfect" },
                { "X", "No tense stated (adverbial imperative)" },
            };
        private static readonly Dictionary<string, string> VerbVoice = new Dictionary<string, string> {
                { "A", "Active" },
                { "M", "Middle" },
                { "P", "Passive" },
                { "E", "Either middle or passive" },
                { "D", "Middle Deponent" },
                { "O", "Passive deponent" },
                { "N", "Middle or passive deponent" },
                { "Q", "Impersonal active" },
                { "X", "No voice stated" },
            };

        #endregion Fields

        #region Methods

        public static string ParseRobinson(string toParse)
        {
            string retInfo = string.Empty;
            try
            {
                string[] pieces = toParse.Split(new[] { '-' });
                if (pieces.Length >= 2 && UndeclinedForms.ContainsKey(pieces[0] + "-" + pieces[1]))
                {
                    retInfo +=
                        Translations.Translate(
                            "Morphology-" + UndeclinedForms[pieces[0] + "-" + pieces[1]].DescriptiveText) + "\n";
                    retInfo +=
                        UndeclinedForms[pieces[0] + "-" + pieces[1]].ToDoNextStage(
                            (pieces[0] + "-" + pieces[1]).Length + 1, toParse);
                }
                else if (UndeclinedForms.ContainsKey(pieces[0]))
                {
                    retInfo += Translations.Translate("Morphology-" + UndeclinedForms[pieces[0]].DescriptiveText) + "\n";
                    retInfo += UndeclinedForms[pieces[0]].ToDoNextStage(pieces[0].Length + 1, toParse);
                }

                Debug.WriteLine(toParse + ";" + retInfo);
            }
            catch (Exception e2)
            {
                Debug.WriteLine(e2.StackTrace);
            }

            return retInfo;
        }

        private static string GetDecline(int len, string toParse)
        {
            string retInfo = string.Empty;
            string[] pieces = toParse.Substring(len).Split(new[] { '-' });

            // prefix-case-number-gender-(suffix)
            retInfo += Translations.Translate("Morphology-" + Case[toParse.Substring(len, 1)]) + "\n";
            retInfo += Translations.Translate("Morphology-" + Number[toParse.Substring(len + 1, 1)]) + "\n";
            if (pieces[0].Length > 2)
            {
                retInfo += Translations.Translate("Morphology-" + Gender[toParse.Substring(len + 2, 1)]) + "\n";
            }

            if (pieces.Length > 1)
            {
                retInfo += Translations.Translate("Morphology-" + Suffixes[pieces[1]]) + "\n";
            }

            return retInfo;
        }

        private static string GetPerson(int len, string toParse)
        {
            string retInfo = string.Empty;
            if (Person.ContainsKey(toParse.Substring(len, 1)))
            {
                retInfo += Translations.Translate("Morphology-" + Person[toParse.Substring(len, 1)]) + "\n";
                len++;
            }

            if (Number.ContainsKey(toParse.Substring(len, 1)))
            {
                retInfo += Translations.Translate("Morphology-" + Number[toParse.Substring(len, 1)]) + "\n";
                len++;
            }

            retInfo += GetDecline(len, toParse);
            return retInfo;
        }

        private static string GetSuffix(int len, string toParse)
        {
            var pieces = new string[0];
            if (len <= toParse.Length)
            {
                pieces = toParse.Substring(len).Split(new[] { '-' });
            }

            string retInfo = string.Empty;
            if (pieces.Length > 1)
            {
                retInfo += Translations.Translate("Morphology-" + Suffixes[pieces[1]]) + "\n";
            }

            return retInfo;
        }

        private static string GetVerb(int len, string toParse)
        {
            string retInfo = string.Empty;
            string[] pieces = toParse.Substring(len).Split(new[] { '-' });
            int numPieces = pieces.Length;
            if (VerbSuffix.ContainsKey(pieces[pieces.Length - 1]))
            {
                numPieces--;

                // it has a suffix. Get it first
                retInfo += Translations.Translate("Morphology-" + VerbSuffix[pieces[pieces.Length - 1]]) + "\n";
            }

            // get the tense-voice-mood
            if (VerbTense.ContainsKey(toParse.Substring(len, 2)))
            {
                retInfo += Translations.Translate("Morphology-" + VerbTense[toParse.Substring(len, 2)]) + "\n";
                len += 2;
            }
            else
            {
                retInfo += Translations.Translate("Morphology-" + VerbTense[toParse.Substring(len, 1)]) + "\n";
                len++;
            }

            retInfo += Translations.Translate("Morphology-" + VerbVoice[toParse.Substring(len, 1)]) + "\n";
            len++;
            retInfo += Translations.Translate("Morphology-" + VerbMood[toParse.Substring(len, 1)]) + "\n";
            if (numPieces > 1)
            {
                switch (pieces[1].Length)
                {
                    case 2: // V-tense-voice-mood-person-number
                        retInfo += Translations.Translate("Morphology-" + Person[pieces[1].Substring(0, 1)]) + "\n";
                        retInfo += Translations.Translate("Morphology-" + Number[pieces[1].Substring(1, 1)]) + "\n";
                        break;
                    case 3: // V-tense-voice-mood-case-number-gender
                        retInfo += Translations.Translate("Morphology-" + Case[pieces[1].Substring(0, 1)]) + "\n";
                        retInfo += Translations.Translate("Morphology-" + Number[pieces[1].Substring(1, 1)]) + "\n";
                        retInfo += Translations.Translate("Morphology-" + Gender[pieces[1].Substring(2, 1)]) + "\n";
                        break;
                }
            }

            return retInfo;
        }

        #endregion Methods
    }

    public class MorphTrans
    {
        #region Fields

        public string DescriptiveText;
        public NextStage ToDoNextStage;

        #endregion Fields

        #region Constructors

        public MorphTrans(NextStage nextStage, string descriptiveText)
        {
            ToDoNextStage = nextStage;
            DescriptiveText = descriptiveText;
        }

        #endregion Constructors

        #region Delegates

        public delegate string NextStage(int pos, string toParse);

        #endregion Delegates
    }
}