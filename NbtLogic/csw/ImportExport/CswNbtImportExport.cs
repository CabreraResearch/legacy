using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.ImportExport
{
    /// <summary>
    /// Controls data exports and imports
    /// </summary>
    /// 
    public delegate void StatusUpdateHandler( string Message );
    public delegate void ImportPhaseHandler( CswNbtImportStatus CswNbtImportStatus );

    public class CswNbtImportExport
    {


        CswNbtImportStatus _CswNbtImportStatus = null;

        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Constructor, requires a CswNbtResources object
        /// </summary>
        public CswNbtImportExport( CswNbtResources R, CswNbtImportStatus CswNbtImportStatus )
        {
            _CswNbtResources = R;
            _CswNbtImportStatus = CswNbtImportStatus;
        }

        /// <summary>
        /// Describes how data is to be treated when importing
        /// </summary>


        #region Import


        public event ImportPhaseHandler OnImportPhaseChange = null;
        private void _ImportPhaseChange( CswNbtImportStatus CswNbtImportStatus )
        {
            if( null != OnImportPhaseChange )
            {
                OnImportPhaseChange( CswNbtImportStatus );
            }
        }//_importPhaseChange



        public event StatusUpdateHandler OnStatusUpdate = null;
        private void _StatusUpdate( string Msg )
        {
            if( OnStatusUpdate != null )
                OnStatusUpdate( Msg );
        }

        ICswImporter _CswImporter = null;
        public void stopImport()
        {
            if( null != _CswImporter )
            {
                _CswImporter.stop();
            }
        }//stopImport()


        public void reset()
        {

            ICswImporter CswImporter = CswImporterFactory.make( ImportAlgorithm.Experimental, _CswNbtResources, null, OnStatusUpdate, OnImportPhaseChange, _CswNbtImportStatus );
            CswImporter.reset();
            _CswNbtResources.finalize();

        }//reset()

        /// <summary>
        /// Imports data from an Xml String
        /// </summary>
        /// <param name="IMode">Describes how data is to be treated when importing</param>
        /// <param name="CswNbtImportExportFrame">Source XML</param>
        /// <param name="ViewXml">Will be filled with the exported view's XML as String </param>
        /// <param name="ResultXml">Will be filled with an XML String record of new primary keys and references</param>
        /// <param name="ErrorLog">Will be filled with a summary of recoverable errors</param>
        public void ImportXml( ImportMode IMode, CswNbtImportExportFrame CswNbtImportExportFrame, ref string ViewXml, ref string ResultXml, ref string ErrorLog )
        {
            _StatusUpdate( "Starting Import" );

            ErrorLog = string.Empty;

            //DataSet XmlData = new DataSet();
            //XmlData.ReadXml( new System.IO.StringReader( XmlStr ) );

            _StatusUpdate( "Initializing Import Data" );


            _CswImporter = CswImporterFactory.make( ImportAlgorithm.Experimental, _CswNbtResources, CswNbtImportExportFrame, OnStatusUpdate, OnImportPhaseChange, _CswNbtImportStatus );

            _CswImporter.ImportXml( IMode, ref ViewXml, ref ResultXml, ref ErrorLog );

        } // ImportXml()

        private void _addEntryToErrorLog( ref string ErrorLog, DataRow ArbitraryDataRow, string Error )
        {
            string FullError = string.Empty;
            FullError += Error + "\n";
            FullError += "DataRow Info:\n";
            foreach( DataColumn Column in ArbitraryDataRow.Table.Columns )
            {
                FullError += Column.ColumnName + " = " + ArbitraryDataRow[Column].ToString() + "\n";
            }
            ErrorLog += FullError + "\n";

            ArbitraryDataRow["error"] += Error;
        } // _addEntryToErrorLog()

        private CswNbtMetaDataNodeType _decodeNodeType( DataRow NodeRow, DataTable NodeTypesTable, DataRelation NodeTypeToNodeRelation )
        {
            CswNbtMetaDataNodeType NodeType = null;
            if( NodeTypesTable != null )
            {
                DataRow NodeTypeRow = NodeRow.GetParentRow( NodeTypeToNodeRelation );
                NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeRow["destnodetypeid"].ToString() ) );
            }
            else if( NodeRow.Table.Columns.Contains( CswNbtImportExportFrame._Attribute_NodeTypeId ) &&
                     NodeRow[CswNbtImportExportFrame._Attribute_NodeTypeId] != null &&
                     NodeRow[CswNbtImportExportFrame._Attribute_NodeTypeId].ToString() != string.Empty )
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeRow[CswNbtImportExportFrame._Attribute_NodeTypeId].ToString() ) );
            }
            else if( NodeRow.Table.Columns.Contains( CswNbtImportExportFrame._Attribute_NodeTypeName ) &&
                     NodeRow[CswNbtImportExportFrame._Attribute_NodeTypeName] != null &&
                     NodeRow[CswNbtImportExportFrame._Attribute_NodeTypeName].ToString() != string.Empty )
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( NodeRow[CswNbtImportExportFrame._Attribute_NodeTypeName].ToString() );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Invalid Node import row", "ImportXml encountered a Node row with no nodetypeid or nodetypename" );
            }
            return NodeType;
        } // _decodeNodeType()


        private CswNbtMetaDataNodeTypeProp _decodeNodeTypeProp( DataRow PropValueRow, CswNbtMetaDataNodeType NodeType, DataTable NodeTypePropsTable, DataRelation NodeTypePropToPropValueRelation )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = null;
            if( NodeTypePropsTable != null )
            {
                DataRow NodeTypePropRow = PropValueRow.GetParentRow( NodeTypePropToPropValueRelation );
                NodeTypeProp = NodeType.getNodeTypeProp( CswConvert.ToInt32( NodeTypePropRow["destnodetypepropid"].ToString() ) );
            }
            else if( PropValueRow.Table.Columns.Contains( CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId ) &&
                     PropValueRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId] != null &&
                     PropValueRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId].ToString() != string.Empty )
            {
                NodeTypeProp = NodeType.getNodeTypeProp( CswConvert.ToInt32( PropValueRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId].ToString() ) );
            }
            else if( PropValueRow.Table.Columns.Contains( CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName ) &&
                     PropValueRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName] != null &&
                     PropValueRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName].ToString() != string.Empty )
            {
                NodeTypeProp = NodeType.getNodeTypeProp( PropValueRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName].ToString() );
            }
            return NodeTypeProp;
        } // _decodeNodeTypeProp()

        #endregion Import

        #region Export

        /// <summary>
        /// Exports all content from a schema
        /// </summary>
        /// <param name="DoNodeTypes">Include all nodetypes</param>
        /// <param name="DoViews">Include all views</param>
        /// <param name="DoNodes">Include all nodes</param>
        public XmlDocument ExportAll( bool DoNodeTypes, bool DoViews, bool DoNodes )
        {
            if( DoNodeTypes )
                return ExportAll( _CswNbtResources.MetaData.getNodeTypes(), DoViews, DoNodes );
            else
                return ExportAll( new Collection<CswNbtMetaDataNodeType>(), DoViews, DoNodes );
        }
        public XmlDocument ExportAll( IEnumerable<CswNbtMetaDataNodeType> NodeTypes, bool DoViews, bool DoNodes )
        {
            _StatusUpdate( "Starting Export" );

            CswNbtImportExportFrame Frame = new CswNbtImportExportFrame( _CswNbtResources );

            //if( DoNodeTypes )
            //{
            //    foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.NodeTypes )

            _StatusUpdate( "Saving Selected NodeTypes" );
            string NodeTypeCount = NodeTypes.Count().ToString();
            Int32 t = 0;
            foreach( CswNbtMetaDataNodeType NodeType in NodeTypes )
            {
                t++;
                if( t % 10 == 1 )
                    _StatusUpdate( "Processing NodeType: " + t.ToString() + " of " + NodeTypeCount );
                Frame.AddNodeType( NodeType );
            }
            // }
            _StatusUpdate( "Finished Saving Selected NodeTypes" );


            if( DoViews )
            {
                _StatusUpdate( "Saving Views" );

                DataTable ViewsTable = _CswNbtResources.ViewSelect.getAllViews();
                Int32 v = 0;
                foreach( DataRow ViewRow in ViewsTable.Rows )
                {
                    v++;
                    if( v % 10 == 1 )
                        _StatusUpdate( "Processing View: " + v.ToString() + " of " + ViewsTable.Rows.Count.ToString() );

                    CswNbtView View = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( ViewRow["nodeviewid"] ) ) );
                    bool IncludeView = false;
                    foreach( CswNbtMetaDataNodeType NodeType in NodeTypes )
                        IncludeView = IncludeView || View.ContainsNodeType( NodeType );
                    if( IncludeView )
                        Frame.AddView( View );
                }

                _StatusUpdate( "Finished Saving Views" );
            }


            if( DoNodes )
            {
                _StatusUpdate( "Saving Nodes" );

                CswTableSelect AllNodesSelect = _CswNbtResources.makeCswTableSelect( "SchemaInitializer_nodesselect", "nodes" );
                string WhereClause = string.Empty;
                foreach( CswNbtMetaDataNodeType NodeType in NodeTypes )
                {
                    if( WhereClause != string.Empty ) WhereClause += ",";
                    WhereClause += "'" + NodeType.NodeTypeId.ToString() + "'";
                }
                WhereClause = "where nodetypeid in (" + WhereClause + ")";
                DataTable AllNodesTable = AllNodesSelect.getTable( WhereClause );
                Collection<CswPrimaryKey> NodeIds = new Collection<CswPrimaryKey>();

                Int32 n = 0;
                foreach( DataRow NodeRow in AllNodesTable.Rows )
                {
                    n++;
                    if( n % 10 == 1 )
                        _StatusUpdate( "Processing Node: " + n.ToString() + " of " + AllNodesTable.Rows.Count.ToString() );

                    CswNbtNode Node = _CswNbtResources.Nodes.GetNode( new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["nodeid"] ) ) );
                    Frame.AddNode( Node );
                }

                _StatusUpdate( "Finished Saving Nodes" );
            }

            _StatusUpdate( "Export Finished" );

            return Frame.AsXmlDoc();
        } // ExportAll()



        /// <summary>
        /// Exports a NodeType, tabs, and properties
        /// </summary>
        /// <param name="NodeType"></param>
        /// <returns></returns>
        public XmlDocument ExportNodeType( CswNbtMetaDataNodeType NodeType )
        {
            CswNbtImportExportFrame Frame = new CswNbtImportExportFrame( _CswNbtResources );
            Frame.AddNodeType( NodeType );
            return Frame.AsXmlDoc();
        } // ExportNodeType()

        /// <summary>
        /// Exports a set of nodes
        /// </summary>
        public XmlDocument ExportNodes( Collection<CswPrimaryKey> NodeIds )
        {
            Collection<CswNbtNode> NodesCol = new Collection<CswNbtNode>();
            foreach( CswPrimaryKey NodeId in NodeIds )
                NodesCol.Add( _CswNbtResources.Nodes[NodeId] );
            return ExportNodes( NodesCol );
        } // ExportNodes()

        /// <summary>
        /// Exports a set of nodes
        /// </summary>
        public XmlDocument ExportNodes( Collection<CswNbtNode> Nodes )
        {
            CswNbtImportExportFrame Frame = new CswNbtImportExportFrame( _CswNbtResources );
            foreach( CswNbtNode Node in Nodes )
                Frame.AddNode( Node );
            return Frame.AsXmlDoc();
        } // ExportNodes()

        /// <summary>
        /// Exports a view into XML
        /// </summary>
        /// <param name="ViewId">Primary key of View to Export</param>
        /// <param name="PropsInViewOnly">Include properties included in the view only</param>
        /// <returns>XmlDocument of all metadata, node, and property data from this view</returns>
        public XmlDocument ExportView( CswNbtViewId ViewId, bool PropsInViewOnly )
        {
            CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
            return ExportView( View, PropsInViewOnly );
        } // ExportView()

        /// <summary>
        /// Exports a view into XML
        /// </summary>
        /// <param name="View">View to Export</param>
        /// <param name="PropsInViewOnly">Include properties included in the view only</param>
        /// <returns>XmlDocument of all metadata, node, and property data from this view</returns>
        public XmlDocument ExportView( CswNbtView View, bool PropsInViewOnly )
        {
            CswNbtImportExportFrame Frame = new CswNbtImportExportFrame( _CswNbtResources );

            Frame.AddView( View );

            foreach( CswNbtMetaDataNodeType MetaDataNodeType in _CswNbtResources.MetaData.getNodeTypes() )
            {
                if( View.ContainsNodeType( MetaDataNodeType ) )
                    Frame.AddNodeType( MetaDataNodeType );
            }

            ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, true );
            Frame.AddTree( View, CswNbtTree, PropsInViewOnly );

            return Frame.AsXmlDoc();
        } // ExportView()

        #endregion Export

    } // class CswNbtImportExport

} // namespace ChemSW.Nbt



