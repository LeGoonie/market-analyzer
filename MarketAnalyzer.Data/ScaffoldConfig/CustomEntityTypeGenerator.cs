using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using System;
using System.Linq;

namespace Blockbase.Web.Data.ScaffoldConfig
{
    internal class CustomEntityTypeGenerator : CSharpEntityTypeGenerator
    {
        public CustomEntityTypeGenerator(ICSharpHelper cSharpHelper) : base(cSharpHelper)
        {
        }

        public override string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            //por reflection, detetar as propriedades correspondentes às interfaces e alterar a escrita dos ficheiros para escrever que as classes implementam as interfaces correspondentes
            string code = base.WriteCode(entityType, @namespace, useDataAnnotations);
            // Cross-Platform
            string newLine = Environment.NewLine;

            int index = code.IndexOf(newLine + newLine + "namespace");
            code = code.Insert(index, "\nusing MarketAnalyzer.Data.Interfaces;");

            string oldString = "public partial class " + entityType.Name;
            string newString = "";

            if (HasProperty(entityType, "Id", typeof(Guid)))
            {
                newString = oldString + " : IEntity ";
            }

            if (HasProperty(entityType, "CreatedBy", typeof(byte[])))
            {
                newString += ", ICreatedBy";
            }

            if (HasProperty(entityType, "DateCreated", typeof(DateTime)))
            {
                newString += ", ICreatedTime";
            }

            if (HasProperty(entityType, "UpdatedBy", typeof(byte[])))
            {
                newString += ", IUpdatedBy";
            }

            if (HasProperty(entityType, "DateUpdated", typeof(DateTime)))
            {
                newString += ", IUpdatedTime";
            }

            if (HasProperty(entityType, "Year", typeof(int)))
            {
                newString += ", IFInfo";
            }

            if (HasProperty(entityType, "DataSourceId", typeof(Guid)))
            {
                newString += ", IFScrappedInfo";
            }

            if (HasProperty(entityType, "VoidedBy", typeof(byte[])))
            {
                newString += ", IVoidedBy";
            }

            if (HasProperty(entityType, "VoidedAt", typeof(DateTime)))
            {
                newString += ", IVoidedTime";
            }

            //  check with Ricardo if this is needed
            if (HasProperty(entityType, "FacilitySignature", typeof(byte[])))
            {
                newString += ", IFacilitySignature";
            }

            code = code.Replace(oldString, newString);
            return code;
        }

        private bool HasProperty(IEntityType entityType, string propertyName, Type propertyType)
        {
            var hasProp = entityType.GetProperties().Where(p => p.Name == propertyName
                                                           && (p.ClrType == propertyType || Nullable.GetUnderlyingType(p.ClrType) == propertyType))
                                                           .Any();

            return hasProp;
        }
    }
}