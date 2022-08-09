using Pluralize.NET.Core;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockbase.Web.Data.ScaffoldConfig
{
    public class CustomPluralizer : IPluralizer
    {
        public string Pluralize(string identifier)
        {
            var pluralizer = new Pluralizer();
            return pluralizer.Pluralize(identifier);
        }

        public string Singularize(string identifier)
        {
            var pluralizer = new Pluralizer();
            return pluralizer.Singularize(identifier);
        }
    }
}
