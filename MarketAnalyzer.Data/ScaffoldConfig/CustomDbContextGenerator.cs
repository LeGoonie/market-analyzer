//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.EntityFrameworkCore.Scaffolding;
//using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

//namespace Blockbase.Web.Data.ScaffoldConfig
//{
//    internal class CustomDbContextGenerator : CSharpDbContextGenerator
//    {
//        public CustomDbContextGenerator(IProviderConfigurationCodeGenerator providerConfigurationCodeGenerator, IAnnotationCodeGenerator annotationCodeGenerator, ICSharpHelper cSharpHelper) : base(providerConfigurationCodeGenerator, annotationCodeGenerator, cSharpHelper)
//        {
//        }

//        public override string WriteCode(IModel model, string contextName, string connectionString, string contextNamespace, string modelNamespace, bool useDataAnnotations, bool suppressConnectionStringWarning)
//        {
//            var code = base.WriteCode(model, contextName, connectionString, contextNamespace, modelNamespace, useDataAnnotations, suppressConnectionStringWarning);
//            var oldString = ".ValueGeneratedOnAdd();";
//            var newString = ".ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;";
//            code = code.Replace(oldString, newString);
//            return code;
//        }

//        //public override string WriteCode(IModel model, string @namespace, string contextName, string connectionString, bool useDataAnnotations, bool suppressConnectionStringWarning)
//        //{
//        //    //por reflection, detetar as propriedades correspondentes às interfaces e alterar a escrita dos ficheiros para escrever que as classes implementam as interfaces correspondentes
//        //    string code = base.WriteCode(model, @namespace, contextName, connectionString, useDataAnnotations, suppressConnectionStringWarning);
//        //    // Cross-Platform
//        //    var oldString = ".ValueGeneratedOnAdd();";
//        //    var newString = ".ValueGeneratedOnAdd().Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;";
//        //    code = code.Replace(oldString, newString);
//        //    return code;
//        //}
//    }
//}