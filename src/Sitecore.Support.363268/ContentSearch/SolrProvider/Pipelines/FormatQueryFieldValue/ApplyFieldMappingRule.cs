using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Sitecore.Support.ContentSearch.SolrProvider.Pipelines.FormatQueryFieldValue
{
    public class ApplyFieldMappingRule : Sitecore.ContentSearch.SolrProvider.Pipelines.FormatQueryFieldValue.ApplyFieldMappingRule
    {
        private static Regex defaultRegex;

        private static Regex regexWithHyphen;

        public ApplyFieldMappingRule()
        {
            if (defaultRegex == null)
            {
                HashSet<string> escapeCharacterSet = GetEscapeCharacterSet();
                StringBuilder stringBuilder = new StringBuilder(50);
                foreach (string item in escapeCharacterSet)
                {
                    stringBuilder.AppendFormat("\\{0}+|", item);
                }
                
                string text = stringBuilder.ToString();
                defaultRegex = new Regex(text.Remove(text.Length - 1));

                string textHyphen = stringBuilder.AppendFormat("\\{0}+|", "-").ToString();
                regexWithHyphen = new Regex(textHyphen.Remove(textHyphen.Length - 1));
            }
        }
        
        protected override string Escape(string fieldValue, string escapedValue, bool includeExistingSpecialCharacter, Collection<string> excludeEscapeCharacters, Collection<string> additionalEscapeCharacters = null)
        {
            // Reuse default regex instance if no custom characters are included or excluded
            if (additionalEscapeCharacters == null && excludeEscapeCharacters == null)
            {
                return defaultRegex.Replace(fieldValue, (Match match) => escapedValue + (includeExistingSpecialCharacter ? match.Value : ""));
            }
            else if (excludeEscapeCharacters == null && additionalEscapeCharacters != null && additionalEscapeCharacters.Count == 1 && additionalEscapeCharacters[0] == "-")
            {
                return regexWithHyphen.Replace(fieldValue, (Match match) => escapedValue + (includeExistingSpecialCharacter ? match.Value : ""));
            }

            HashSet<string> escapeCharacterSet = GetEscapeCharacterSet();
            StringBuilder stringBuilder = new StringBuilder(50);
            if (additionalEscapeCharacters != null)
            {
                escapeCharacterSet.UnionWith(additionalEscapeCharacters);
            }

            if (excludeEscapeCharacters != null)
            {
                escapeCharacterSet.ExceptWith(excludeEscapeCharacters);
            }

            foreach (string item in escapeCharacterSet)
            {
                stringBuilder.AppendFormat("\\{0}+|", item);
            }

            string text = stringBuilder.ToString();
            
            return new Regex(text.Remove(text.Length - 1)).Replace(fieldValue, (Match match) => escapedValue + (includeExistingSpecialCharacter ? match.Value : ""));
        }
    }
}