using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ImportExport
{

    public class CswImporterDbTables
    {

        private CswNbtResources _CswNbtResources = null;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;


        public const string TblName_ImportNodes = "import_nodes";
        public const string TblName_ImportProps = "import_props";

        public const string _ColName_ProcessStatus = "processstatus";
        public const string _ColName_StatusMessage = "statusmessage";
        public const string _ColName_Source = "source";
        public const string _ColName_ImportNodeId = "importnodeid";

        public const string _ColName_Infra_BothTables_Node_Id = "Node_Id";

        public const string _ColName_Infra_Nodes_NodeTypeName = "nodetypename";

        public const string _ColName_Infra_Nodes_NodeTypePropName = "nodetypepropname";

        public const string _ColName_Props_ImportTargetNodeIdUnique = "importtargetnodeid";
        public const string _ColName_Props_ImportTargetNodeIdOriginal = "NodeID";
        public const string _ColName_Nodes_NodeName = "nodename";

        public const string _ColName_Infra_Nodes_LegacyNodeId = "Nodes_Id";


        public const string ColName_ImportNodesTablePk = "importnodespk";
        public const string ColName_ImportPropsTablePk = "importpropspk";
        public const string Colname_NbtNodeId = "nbtnodeid";



        public const string ColName_ImportPropsRealPropId = "nbtnodepropid";

        private Collection<string> _AdditonalColumns = new Collection<string>();
        private Collection<string> _IndexColumns = new Collection<string>();

        private Collection<string> _NodeTableInfrastructureColumns = new Collection<string>();
        private Collection<string> _PropTableInfrastructureColumns = new Collection<string>();
        private Collection<string> _BothTablesInfrastructureColumns = new Collection<string>();

        private CswNbtImportOptions _CswNbtImportOptions = null;

        public CswImporterDbTables( CswNbtResources CswNbtResources, CswNbtImportOptions CswNbtImportOptions )
        {

            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new Schema.CswNbtSchemaModTrnsctn( _CswNbtResources );

            _CswNbtImportOptions = CswNbtImportOptions;

            //*************** import_nodes columns
            _NodeTableInfrastructureColumns.Add( _ColName_Infra_Nodes_LegacyNodeId ); //cruft from original xml import
            _NodeTableInfrastructureColumns.Add( _ColName_Infra_Nodes_NodeTypeName );
            _NodeTableInfrastructureColumns.Add( _ColName_Nodes_NodeName );

            //*************** import_props columns (there will be additional ones per each prop's field values)
            _PropTableInfrastructureColumns.Add( _ColName_Infra_Nodes_NodeTypePropName );
            _PropTableInfrastructureColumns.Add( ColName_ImportPropsRealPropId );
            _PropTableInfrastructureColumns.Add( _ColName_Props_ImportTargetNodeIdUnique );
            _PropTableInfrastructureColumns.Add( ColName_ImportNodesTablePk ); //for fk references


            //*************** columns for both import_nodes and import_props
            _BothTablesInfrastructureColumns.Add( _ColName_ProcessStatus );
            _BothTablesInfrastructureColumns.Add( _ColName_StatusMessage );
            _BothTablesInfrastructureColumns.Add( _ColName_Source );
            _BothTablesInfrastructureColumns.Add( _ColName_ImportNodeId );
            _BothTablesInfrastructureColumns.Add( _ColName_Infra_BothTables_Node_Id );
            _BothTablesInfrastructureColumns.Add( Colname_NbtNodeId );



            _IndexColumns.Add( _ColName_ProcessStatus );
            _IndexColumns.Add( _ColName_ImportNodeId );


        }//ctor


        public void reset()
        {
            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TblName_ImportNodes ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TblName_ImportNodes ) ) //belt and suspenders
            {
                _CswNbtSchemaModTrnsctn.dropTable( TblName_ImportNodes );
            }

            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TblName_ImportProps ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TblName_ImportProps ) ) //belt and suspenders
            {
                _CswNbtSchemaModTrnsctn.dropTable( TblName_ImportProps );
            }


        }//reset()


        public bool areImportTablesAbsent( ref string ImportTableInconsistencyMessage )
        {

            bool ReturnVal = true;

            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TblName_ImportNodes ) )
            {
                if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TblName_ImportNodes ) )
                {
                    if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TblName_ImportProps ) )
                    {
                        if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TblName_ImportProps ) )
                        {
                            ReturnVal = false;
                            ImportTableInconsistencyMessage = string.Empty;
                        }
                        else
                        {
                            ImportTableInconsistencyMessage = "The import nodes table is defined properly; however, the import props table is defined in the database but not in meta data";
                        }//if-else propstable is defined in meta data
                    }
                    else
                    {
                        ImportTableInconsistencyMessage = "The import nodes nodes table is defined properly; however the import props table is not defined in the database";
                    }//if-else props table is defined in db
                }
                else
                {
                    ImportTableInconsistencyMessage = "The import nodes table is defined in the database, but not in the meta data";
                }//if-else Nodes table is defined in meta data
            }
            else
            {
                ImportTableInconsistencyMessage = string.Empty; //not necessarily an error condition: it just aint there
            }//if-else Nodes table is defined in db

            return ( ReturnVal );

        }//areImportTablesAbsent()




        public void makeImportTables( List<string> NodePropColumnNames )
        {


            _CswNbtSchemaModTrnsctn.beginTransaction();

            List<string> NodesColumns = new List<string>();

            foreach( string CurrentColName in _BothTablesInfrastructureColumns )
            {
                NodesColumns.Add( CurrentColName );
            }

            foreach( string CurrentColName in _NodeTableInfrastructureColumns )
            {
                NodesColumns.Add( CurrentColName );
            }

            _makeImportTable( TblName_ImportNodes, ColName_ImportNodesTablePk, NodesColumns, 512, _IndexColumns );



            //_CswNbtSchemaModTrnsctn.addLongColumn( TblName_ImportNodes, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );

            List<string> AllPropColumns = new List<string>();

            foreach( string CurrentColName in _BothTablesInfrastructureColumns )
            {
                AllPropColumns.Add( CurrentColName );
            }

            foreach( string CurrentColName in _PropTableInfrastructureColumns )
            {
                AllPropColumns.Add( CurrentColName );
            }


            foreach( string CurrentColName in NodePropColumnNames )
            {

                if( false == AllPropColumns.Contains( CurrentColName ) )
                {
                    AllPropColumns.Add( CurrentColName );
                }
            }//add node prop columns

            _makeImportTable( TblName_ImportProps, ColName_ImportPropsTablePk, AllPropColumns, 512, _IndexColumns );


            //_CswNbtSchemaModTrnsctn.addLongColumn( TblName_ImportProps, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );
            //_CswNbtSchemaModTrnsctn.addLongColumn( TblName_ImportProps, ColName_ImportPropsRealPropId, "to be filled in when the node is actually created", false, false );


            _CswNbtSchemaModTrnsctn.commitTransaction();

        }


        private void _makeImportTable( string TableName, string PkColumnName, List<string> ColumnsToAdd, Int32 ArbitraryStringColumnLength, Collection<string> IndexColumns )
        {

            _CswNbtSchemaModTrnsctn.addTable( TableName, PkColumnName );



            foreach( string CurrentColumn in ColumnsToAdd )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( TableName, CurrentColumn, string.Empty, false, false, ArbitraryStringColumnLength );
            }

            foreach( string CurrentColumnName in IndexColumns )
            {
                _CswNbtSchemaModTrnsctn.indexColumn( TableName, CurrentColumnName );
            }

        }//_makeImportTable() 

    } // class CswImporterExperimental

} // namespace ChemSW.Nbt


