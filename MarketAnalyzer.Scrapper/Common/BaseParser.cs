using HtmlAgilityPack;
using MarketAnalyzer.Data.Interfaces;
using MarketAnalyzer.Scrapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Scrapper.Common
{
    public abstract class BaseParser<TOut> where TOut : ICreatedTime, IUpdatedTime
    {
        protected readonly Dictionary<string, string> _propertyMappings;

        public BaseParser()
        {
            _propertyMappings = new Dictionary<string, string>();
        }

        public abstract List<TOut> ParseTable(HtmlNode table);

        protected virtual List<TOut> ParseTable(HtmlNode table, int leftColumnsToIgnore = 0, int rightColumnsToIgnore = 0)
        {
            var fInfos = new List<TOut>();

            var rows = table.Descendants().Where(n => n.Name == "tr");

            var rowsCount = rows.Count();
            var columnsCount = rows.First().Descendants().Where(n => n.Name == "td").Skip(leftColumnsToIgnore).Count() - rightColumnsToIgnore;

            for (int j = 0; j < columnsCount; j++)
            {
                string currentParent = string.Empty;
                var fInfo = Activator.CreateInstance<TOut>();
                fInfo.DateCreated = DateTime.UtcNow;
                fInfo.DateUpdated = DateTime.UtcNow;
                for (int i = 0; i < rowsCount; i++)
                {
                    var row = rows.ElementAt(i);

                    var cell = row.Descendants().Where(n => n.Name == "td").Skip(leftColumnsToIgnore).Take(columnsCount).ElementAt(j);
                    var key = row.Descendants().First().InnerHtml;

                    if (row.Descendants().First().HasClass("labelHeader"))
                    {
                        currentParent = key;
                        continue;
                    }

                    if (!_propertyMappings.ContainsKey(currentParent + key)) continue;
                    var propertyName = _propertyMappings[currentParent + key];

                    var property = fInfo.GetType().GetProperty(propertyName);

                    if (decimal.TryParse(GetCellValue(cell), out var decimalValue) && property.TrySetValue(fInfo, decimalValue)) continue;
                    if (double.TryParse(GetCellValue(cell), out var doubleValue) && property.TrySetValue(fInfo, doubleValue)) continue;
                    if (int.TryParse(GetCellValue(cell), out var intValue) && property.TrySetValue(fInfo, intValue)) continue;
                }

                fInfos.Add(fInfo);
            }

            return fInfos;
        }

        private string GetCellValue(HtmlNode cell)
        {
            var dataValue = cell.Attributes.Where(a => a.Name == "data-value").SingleOrDefault();
            if (dataValue != null) return dataValue.Value;
            else return cell.InnerHtml;
        }

        protected class RowInfo
        {
            public string Name { get; set; }
            public string ParentName { get; set; }
        }
    }
}