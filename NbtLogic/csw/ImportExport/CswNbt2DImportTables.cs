using System;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DImportTables
    {
        public class ImportDef
        {
            public const string TableName = "import_def";
            public const string PkColumnName = importdefinitionid;

            public const string importdefinitionid = "importdefinitionid";
            public const string definitionname = "definitionname";
            public const string sheetname = "sheetname";
            public const string sheetorder = "sheetorder";
        }
        
        public class ImportDefOrder
        {
            public const string TableName = "import_def_order";
            public const string PkColumnName = importorderid;

            public const string importorderid = "importorderid";
            public const string importdefinitionid = "importdefinitionid";
            public const string importorder = "importorder";
            public const string nodetypename = "nodetypename";
            public const string instance = "instance";
        }

        public class ImportDefBindings
        {
            public const string TableName = "import_def_bindings";
            public const string PkColumnName = importbindingid;

            public const string importbindingid = "importbindingid";
            public const string importdefinitionid = "importdefinitionid";
            public const string sourcecolumnname = "sourcecolumnname";
            public const string destnodetypename = "destnodetypename";
            public const string destpropname = "destpropname";
            public const string destsubfield = "destsubfield";
            public const string instance = "instance";
        }

        public class ImportDefRelationships
        {
            public const string TableName = "import_def_relationships";
            public const string PkColumnName = importrelationshipid;

            public const string importrelationshipid = "importrelationshipid";
            public const string importdefinitionid = "importdefinitionid";
            public const string nodetypename = "nodetypename";
            public const string relationship = "relationship";
            public const string instance = "instance";
        }

        public class ImportDataMap
        {
            public const string TableName = "import_data_map";
            public const string PkColumnName = importdatamapid;

            public const string importdatamapid = "importdatamapid";
            public const string importdefinitionid = "importdefinitionid";
            public const string datatablename = "datatablename";
            public const string overwrite = "overwrite";
            public const string completed = "completed";
        }

        public class ImportDataN
        {
            public const string TableNamePrefix = "import_data";
            public const string PkColumnName = importdataid;

            public const string importdataid = "importdataid";
            public const string error = "error";
            public const string errorlog = "errorlog";
            // plus a lot more configurable columns
        }
    }
}
