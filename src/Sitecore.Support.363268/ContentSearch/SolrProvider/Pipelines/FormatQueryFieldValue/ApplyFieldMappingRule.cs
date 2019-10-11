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
        public ApplyFieldMappingRule()
        {
        }
        
        protected override string Escape(string fieldValue, string escapedValue, bool includeExistingSpecialCharacter, Collection<string> excludeEscapeCharacters, Collection<string> additionalEscapeCharacters = null)
        {
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