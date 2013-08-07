using System;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DImportTables
    {
        public class ImportDefinitions
        {
            public const string TableName = "import_definitions";
            public const string PkColumnName = importdefinitionid;

            public const string importdefinitionid = "importdefinitionid";
            public const string definitionname = "definitionname";
        }
        public class ImportOrder
        {
            public const string TableName = "import_order";
            public const string PkColumnName = importorderid;
            
            public const string importorderid = "importorderid";
            public const string importdefinitionid = "importdefinitionid";
            public const string importorder = "importorder";
            public const string sourcesheetname = "sourcesheetname";
            public const string nodetypename = "nodetypename";
            public const string instance = "instance";
        }
        public class ImportBindings
        {
            public const string TableName = "import_bindings";
            public const string PkColumnName = importbindingid;
            
            public const string importbindingid = "importbindingid";
            public const string importdefinitionid = "importdefinitionid";
            public const string sourcesheetname = "sourcesheetname";
            public const string sourcecolumnname = "sourcecolumnname";
            public const string destnodetypename = "destnodetypename";
            public const string destpropname = "destpropname";
            public const string destsubfield = "destsubfield";
            public const string instance = "instance";
        }

        public class ImportRelationships
        {
            public const string TableName = "import_relationships";
            public const string PkColumnName = importrelationshipid;

            public const string importrelationshipid = "importrelationshipid";
            public const string importdefinitionid = "importdefinitionid";
            public const string sourcesheetname = "sourcesheetname";
            public const string nodetypename = "nodetypename";
            public const string relationship = "relationship";
            public const string instance = "instance";
        }
    }
}
