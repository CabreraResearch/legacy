/*
NOTE:
After having simply moved the import algorithm from the CswNbtImportExport class into 
this sublclass of ICswIMporter, iterator works no worse thanit did before. Meaning that 
if I export the SM IMCS test schema (behaaaave!) with the BCB exporter tool, do xsl it 
into our standard format, and do an import, the following chain of messages occur, 
which were exactly the same before I did this refactoring: 
9/20/2011 4:41:03 PM: Error: Object reference not set to an instance of an object.
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeDeleteNode() in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 175
   at ChemSW.Nbt.ObjClasses.CswNbtNode.delete() in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 530
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 513
   at ChemSW.Nbt.ImportExport.CswNbtImportExport.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswNbtImportExport.cs:line 96
   at ChemSW.Nbt.Schema.WorkerThread.DoImport(String FilePath, ImportMode Mode) in C:\allHertzel\Kiln\Nbt\Nbt\NbtSchemaImporter\WorkerThread.cs:line 167
9/20/2011 4:41:03 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:41:02 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:41:02 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:41:02 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:41:02 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:41:02 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:41:02 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:41:02 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:41:02 PM: ERROR: Could not save node: Invalid node: CswNbtNodeCaster was given a null node as a parameter; Exception occurred in NbtLogic:    at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster._Validate(CswNbtNode Node, NbtObjectClass TargetObjectClass) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 12
   at ChemSW.Nbt.ObjClasses.CswNbtNodeCaster.AsRole(CswNbtNode Node) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNodeCaster.cs:line 128
   at ChemSW.Nbt.ObjClasses.CswNbtObjClassUser.beforeWriteNode(Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtObjClassUser.cs:line 119
   at ChemSW.Nbt.ObjClasses.CswNbtNode.postChanges(Boolean ForceUpdate, Boolean IsCopy, Boolean OverrideUniqueValidation) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ObjClasses\CswNbtNode.cs:line 476
   at ChemSW.Nbt.ImportExport.CswImporterLegacy.ImportXml(ImportMode IMode, String XmlStr, String& ViewXml, String& ResultXml, String& ErrorLog) in C:\allHertzel\Kiln\Nbt\Nbt\NbtLogic\csw\ImportExport\CswImporterLegacy.cs:line 522
9/20/2011 4:40:56 PM: Processing Node: 1 of 79
9/20/2011 4:40:56 PM: Started Setting Property Values
9/20/2011 4:40:56 PM: Finished Copying Nodes
9/20/2011 4:40:45 PM: Copying Node: 1 of 79
9/20/2011 4:40:45 PM: Started Copying Nodes
9/20/2011 4:40:45 PM: Done Fixing Relationship References
9/20/2011 4:40:45 PM: Fixing Relationship References
9/20/2011 4:40:45 PM: Tables Initialized
9/20/2011 4:40:45 PM: Initializing DataSet
9/20/2011 4:40:45 PM: Starting Import
9/20/2011 4:40:45 PM: Initializing Import Data
9/20/2011 4:40:45 PM: Starting Import

*/




using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ImportExport
{


    public class CswImporterHashTables : ICswImporter
    {

        CswNbtResources _CswNbtResources = null;
        CswNbtImportExportFrame _CswNbtImportExportFrame = null;
        public CswImporterHashTables( CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, StatusUpdateHandler OnStatusUpdateIn )
        {
            _CswNbtResources = CswNbtResources;
            OnStatusUpdate = OnStatusUpdateIn;
            _CswNbtImportExportFrame = CswNbtImportExportFrame;
        }


        public void reset() { throw ( new NotImplementedException() ); }
        public void stop() { throw ( new NotImplementedException() ); }

        public event StatusUpdateHandler OnStatusUpdate = null;


        private void _StatusUpdate( string Msg )
        {
            if( OnStatusUpdate != null )
                OnStatusUpdate( Msg );
        }





        /// <summary>
        /// Imports data from an Xml String
        /// </summary>
        /// <param name="IMode">Describes how data is to be treated when importing</param>
        /// <param name="ViewXml">Will be filled with the exported view's XML as String </param>
        /// <param name="ResultXml">Will be filled with an XML String record of new primary keys and references</param>
        /// <param name="ErrorLog">Will be filled with a summary of recoverable errors</param>
        public void ImportXml( ImportMode IMode, ref string ViewXml, ref string ResultXml, ref string ErrorLog )
        {
            _StatusUpdate( "Starting Import" );

            ErrorLog = string.Empty;

            //DataSet XmlData = new DataSet();
            //XmlData.ReadXml( new System.IO.StringReader( XmlStr ) );




            _StatusUpdate( "Initializing DataSet" );

            DataSet XmlData = _CswNbtImportExportFrame.AsDataSet();

            DataTable NodeTypesTable = XmlData.Tables[CswNbtMetaDataNodeType._Element_MetaDataNodeType];
            DataTable NodeTypeTabsTable = XmlData.Tables[CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab];
            DataTable NodeTypePropsTable = XmlData.Tables[CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp];
            // TODO: we need to use this
            DataTable DefaultValueTable = XmlData.Tables[CswNbtMetaDataNodeTypeProp._Element_DefaultValue];
            // TODO: we need to use this
            DataTable SubFieldMapTable = XmlData.Tables[CswNbtMetaDataNodeTypeProp._Element_SubFieldMap];
            DataTable NodesTable = XmlData.Tables[CswNbtImportExportFrame._Element_Node];
            DataTable PropValuesTable = XmlData.Tables[CswNbtImportExportFrame._Element_PropValue];

            DataRelation NodeTypePropToDefaultValueRelation = XmlData.Relations[CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp + "_" + CswNbtMetaDataNodeTypeProp._Element_DefaultValue];
            DataRelation NodeTypeToNodeRelation = null;
            DataRelation NodeTypePropToPropValueRelation = null;

            // Setup extra relations
            if( NodeTypesTable != null )
            {
                if( NodesTable != null )
                {
                    NodeTypeToNodeRelation = new DataRelation( CswNbtMetaDataNodeType._Element_MetaDataNodeType + "_" + CswNbtImportExportFrame._Element_Node,
                                                                        NodeTypesTable.Columns[CswNbtMetaDataNodeType._Attribute_NodeTypeId],
                                                                        NodesTable.Columns[CswNbtImportExportFrame._Attribute_NodeTypeId], false );
                    XmlData.Relations.Add( NodeTypeToNodeRelation );
                }
                NodeTypesTable.Columns.Add( "destnodetypeid" );
                NodeTypesTable.Columns.Add( "destnodetypename" );
                NodeTypesTable.Columns.Add( "error" );
            }
            if( NodeTypeTabsTable != null )
            {
                NodeTypeTabsTable.Columns.Add( "destnodetypetabid" );
                NodeTypeTabsTable.Columns.Add( "destnodetypetabname" );
                NodeTypeTabsTable.Columns.Add( "error" );
            }
            if( NodeTypePropsTable != null )
            {
                if( PropValuesTable != null )
                {
                    NodeTypePropToPropValueRelation = new DataRelation( CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp + "_" + CswNbtImportExportFrame._Element_PropValue,
                                                                                 NodeTypePropsTable.Columns[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId],
                                                                                 PropValuesTable.Columns[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId], false );
                    XmlData.Relations.Add( NodeTypePropToPropValueRelation );
                }
                NodeTypePropsTable.Columns.Add( "destnodetypepropid" );
                NodeTypePropsTable.Columns.Add( "destnodetypepropname" );
                NodeTypePropsTable.Columns.Add( "error" );
            }
            if( NodesTable != null )
            {
                NodesTable.Columns.Add( "destnodeid" );
                NodesTable.Columns.Add( "error" );
            }
            if( PropValuesTable != null )
            {
                // For references
                PropValuesTable.Columns.Add( "destnodeid" );
                PropValuesTable.Columns.Add( "destnodetypeid" );
                PropValuesTable.Columns.Add( "error" );
            }

            Dictionary<CswPrimaryKey, CswNbtNode> NodeMap = new Dictionary<CswPrimaryKey, CswNbtNode>();
            Dictionary<string, Int32> NodeIdMap = new Dictionary<string, Int32>();
            Dictionary<Int32, Int32> NodeTypeMap = new Dictionary<Int32, Int32>();
            Dictionary<Int32, Int32> NodeTypePropMap = new Dictionary<Int32, Int32>();
            Dictionary<Int32, CswNbtViewId> ViewMap = new Dictionary<Int32, CswNbtViewId>();

            _StatusUpdate( "Tables Initialized" );

            //---------------------------------------------------------------------------
            // Map MetaData
            if( NodeTypesTable != null )
            {
                Int32 n = 0;
                _StatusUpdate( "Started Processing NodeTypes" );
                foreach( DataRow NodeTypeRow in NodeTypesTable.Rows )
                {
                    n++;
                    if( n % 10 == 1 )
                        _StatusUpdate( "Processing NodeType " + n.ToString() + " of " + NodeTypesTable.Rows.Count );

                    bool FixFilters = false;
                    CswNbtMetaDataNodeType DestNodeType = null;
                    string SourceNodeTypeName = CswTools.XmlRealAttributeName( NodeTypeRow[CswNbtMetaDataNodeType._Attribute_NodeTypeName].ToString() );
                    Int32 SourceNodeTypeId = CswConvert.ToInt32( NodeTypeRow[CswNbtMetaDataNodeType._Attribute_NodeTypeId] );
                    Int32 SourceObjectClassId = CswConvert.ToInt32( NodeTypeRow[CswNbtMetaDataNodeType._Attribute_ObjectClassId] );

                    //if( IMode == ImportMode.CopyOverwrite )
                    //{
                    // Source Name and Source ObjectClass have to match in order to use an existing nodetype
                    foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes() )
                    {
                        if( NodeType.NodeTypeName.ToLower() == SourceNodeTypeName.ToLower() &&
                            NodeType.ObjectClassId == SourceObjectClassId )
                        {
                            DestNodeType = NodeType;
                            break;
                        }
                    }
                    //} // if( IMode == ImportMode.CopyOverwrite )
                    //else
                    //{
                    //    if( SourceNodeTypeId != Int32.MinValue )
                    //    {
                    //        // Source Name and Source PK have to match in order to use an existing nodetype
                    //        DestNodeType = _CswNbtResources.MetaData.getNodeType( SourceNodeTypeId );
                    //        if( DestNodeType != null && DestNodeType.NodeTypeName != SourceNodeTypeName )
                    //            DestNodeType = null;
                    //    }
                    //    else
                    //    {
                    //        // No Source PK provided -- try name match
                    //        DestNodeType = _CswNbtResources.MetaData.getNodeType( SourceNodeTypeName );
                    //        if( DestNodeType != null )
                    //            SourceNodeTypeId = DestNodeType.NodeTypeId;
                    //    }
                    //}

                    // Do we update the nodetype?
                    //             match found               no match
                    //update           Yes                      No
                    //duplicate        No                       Yes (new)
                    //overwrite        Yes                      Yes (new)

                    bool updateDestNodeType = false;
                    if( DestNodeType == null && ( IMode == ImportMode.Duplicate || IMode == ImportMode.Overwrite ) )
                    {
                        // No match found - Make new nodetype
                        FixFilters = true;
                        DestNodeType = _CswNbtResources.MetaData.makeNewNodeType( NodeTypeRow );
                        updateDestNodeType = true;
                    }
                    else
                    {
                        updateDestNodeType = ( DestNodeType != null && ( IMode == ImportMode.Update || IMode == ImportMode.Overwrite ) );
                    }



                    if( updateDestNodeType )
                    {
                        // Tabs
                        Int32 t = 0;
                        DataRow[] TabRows = NodeTypeRow.GetChildRows( CswNbtMetaDataNodeType._Element_MetaDataNodeType + "_" + CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab );
                        foreach( DataRow NodeTypeTabRow in TabRows )
                        {
                            t++;
                            _StatusUpdate( "Processing NodeTypeTab " + t.ToString() + " of " + TabRows.Length );

                            CswNbtMetaDataNodeTypeTab ThisTab = null;
                            foreach( CswNbtMetaDataNodeTypeTab ExistingTab in DestNodeType.getNodeTypeTabs() )
                            {
                                if( NodeTypeTabRow[CswNbtMetaDataNodeTypeTab._Attribute_TabName].ToString() == ExistingTab.TabName )
                                {
                                    ThisTab = ExistingTab;
                                }
                            }
                            if( ThisTab == null )
                            {
                                ThisTab = _CswNbtResources.MetaData.makeNewTab( DestNodeType, NodeTypeTabRow );
                            }
                            ThisTab.TabOrder = CswConvert.ToInt32( NodeTypeTabRow[CswNbtMetaDataNodeTypeTab._Attribute_Order] );

                            // Props
                            Int32 p = 0;
                            DataRow[] PropRows = NodeTypeTabRow.GetChildRows( CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab + "_" + CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp );
                            foreach( DataRow NodeTypePropRow in PropRows )
                            {
                                p++;
                                if( p % 10 == 1 )
                                    _StatusUpdate( "Processing NodeTypeProp " + p.ToString() + " of " + PropRows.Length );
                                // The nodetype might have object class props, so we have to handle that case
                                CswNbtMetaDataNodeTypeProp ThisProp = null;
                                foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in DestNodeType.getNodeTypeProps() )
                                {
                                    if( MetaDataProp.PropName.ToLower() == NodeTypePropRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName].ToString().ToLower() &&
                                        MetaDataProp.getFieldType().FieldType.ToString() == NodeTypePropRow[CswNbtMetaDataNodeTypeProp._Attribute_fieldtype].ToString() )
                                    {
                                        ThisProp = MetaDataProp;
                                        break;
                                    }
                                }
                                if( ThisProp != null )
                                {
                                    ThisProp.SetFromXmlDataRow( NodeTypePropRow );
                                    ThisProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ThisTab.TabId, Int32.MinValue, Int32.MinValue );
                                }
                                else
                                {
                                    ThisProp = _CswNbtResources.MetaData.makeNewProp( DestNodeType, ThisTab, NodeTypePropRow );
                                }

                                // Handle default value
                                if( NodeTypePropToDefaultValueRelation != null )
                                {
                                    foreach( DataRow DefaultValueRow in NodeTypePropRow.GetChildRows( NodeTypePropToDefaultValueRelation ) )
                                    {
                                        ThisProp.DefaultValue.ReadDataRow( DefaultValueRow, null, null );
                                    }
                                }
                            } // foreach( DataRow NodeTypePropRow in NodeTypeTabRow.GetChildRows( CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab + "_" + CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp ) )
                        } // foreach( DataRow NodeTypeTabRow in NodeTypeRow.GetChildRows( CswNbtMetaDataNodeType._Element_MetaDataNodeType + "_" + CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab ) )
                    } // if( DestNodeType == null )

                    if( DestNodeType != null )
                    {
                        // We have to do this after we've restored the properties
                        DestNodeType.setNameTemplateText( NodeTypeRow[CswNbtMetaDataNodeType._Attribute_NameTemplate].ToString() );

                        // Record the matching nodetype in the table
                        NodeTypeMap.Add( SourceNodeTypeId, DestNodeType.NodeTypeId );          // for property value references
                        NodeTypeRow["destnodetypeid"] = DestNodeType.NodeTypeId.ToString();    // for posterity
                        NodeTypeRow["destnodetypename"] = DestNodeType.NodeTypeName;

                        foreach( DataRow NodeTypeTabRow in NodeTypeRow.GetChildRows( CswNbtMetaDataNodeType._Element_MetaDataNodeType + "_" + CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab ) )
                        {
                            CswNbtMetaDataNodeTypeTab DestNodeTypeTab = null;
                            string StringNodeTypeTabId = NodeTypeTabRow[CswNbtMetaDataNodeTypeTab._Attribute_TabId].ToString();
                            if( CswTools.IsInteger( StringNodeTypeTabId ) )
                                DestNodeTypeTab = DestNodeType.getNodeTypeTab( CswConvert.ToInt32( StringNodeTypeTabId ) );
                            if( DestNodeTypeTab == null )
                                DestNodeTypeTab = DestNodeType.getNodeTypeTab( NodeTypeTabRow[CswNbtMetaDataNodeTypeTab._Attribute_TabName].ToString() );

                            NodeTypeTabRow["destnodetypetabid"] = DestNodeTypeTab.TabId.ToString();
                            NodeTypeTabRow["destnodetypetabname"] = DestNodeTypeTab.TabName;

                            foreach( DataRow NodeTypePropRow in NodeTypeTabRow.GetChildRows( CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab + "_" + CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp ) )
                            {
                                CswNbtMetaDataNodeTypeProp DestNodeTypeProp = null;
                                Int32 SourceNodeTypePropId = CswConvert.ToInt32( NodeTypePropRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId] );
                                //if( CswTools.IsInteger( StringNodeTypePropId ) )
                                //    DestNodeTypeProp = DestNodeType.getNodeTypeProp( CswConvert.ToInt32( StringNodeTypePropId ) );
                                //if( DestNodeTypeProp == null )
                                DestNodeTypeProp = DestNodeType.getNodeTypeProp( NodeTypePropRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName].ToString() );

                                NodeTypePropMap.Add( SourceNodeTypePropId, DestNodeTypeProp.PropId );
                                NodeTypePropRow["destnodetypepropid"] = DestNodeTypeProp.PropId.ToString();
                                NodeTypePropRow["destnodetypepropname"] = DestNodeTypeProp.PropName;
                            }

                            if( FixFilters )
                            {
                                foreach( DataRow NodeTypePropRow in NodeTypeTabRow.GetChildRows( CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab + "_" + CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp ) )
                                {
                                    Int32 FilterPropId = CswConvert.ToInt32( NodeTypePropRow[CswNbtMetaDataNodeTypeProp._Attribute_filterpropid] );
                                    string FilterString = NodeTypePropRow[CswNbtMetaDataNodeTypeProp._Attribute_filter].ToString();
                                    if( FilterPropId != Int32.MinValue )
                                    {
                                        CswNbtMetaDataNodeTypeProp DestNodeTypeProp = DestNodeType.getNodeTypeProp( NodeTypePropRow[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName].ToString() );
                                        foreach( DataRow OtherNodeTypePropRow in NodeTypeTabRow.GetChildRows( CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab + "_" + CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp ) )
                                        {
                                            //bz # 8314: compare on firstversionpropid
                                            //if ( CswConvert.ToInt32( OtherNodeTypePropRow[ CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId ] ) == FilterPropId )
                                            if( CswConvert.ToInt32( OtherNodeTypePropRow[CswNbtMetaDataNodeTypeProp._Attribute_firstpropversionid] ) == FilterPropId )
                                            {
                                                DestNodeTypeProp.setFilter( CswConvert.ToInt32( OtherNodeTypePropRow["destnodetypepropid"] ), FilterString );
                                            }
                                        }
                                    }
                                } // foreach( DataRow NodeTypePropRow in NodeTypeTabRow.GetChildRows( CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab + "_" + CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp ) )
                            } // if( FixFilters )
                        } // foreach( DataRow NodeTypeTabRow in NodeTypeRow.GetChildRows( CswNbtMetaDataNodeType._Element_MetaDataNodeType + "_" + CswNbtMetaDataNodeTypeTab._Element_MetaDataNodeTypeTab ) )
                    } // if( DestNodeType != null )
                } // foreach( DataRow NodeTypeRow in NodeTypesTable.Rows )

                _StatusUpdate( "Done Processing NodeTypes" );

            } // if( NodeTypesTable != null )


            _StatusUpdate( "Fixing Relationship References" );

            // Fix relationship references
            //foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes() )
            //{
            foreach( CswNbtMetaDataNodeTypeProp Prop in _CswNbtResources.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Relationship ) )
            {
                //if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                //{
                if( Prop.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() )
                {
                    if( NodeTypeMap.ContainsKey( Prop.FKValue ) )
                    {
                        Prop.SetFK( Prop.FKType, NodeTypeMap[Prop.FKValue], Prop.ValuePropType, Prop.ValuePropId );
                    }
                } // if( Prop.FKType == RelatedIdType.NodeTypeId.ToString() )
                //} // if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
            } // foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.NodeTypeProps )
            //} // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.NodeTypes )

            _StatusUpdate( "Done Fixing Relationship References" );

            //---------------------------------------------------------------------------
            // Copy Nodes
            // We make the nodes first, so we can update references later

            if( IMode == ImportMode.Duplicate || IMode == ImportMode.Overwrite )
            {
                _StatusUpdate( "Started Copying Nodes" );
                Int32 n = 0;
                foreach( DataRow NodeRow in NodesTable.Rows )
                {
                    n++;
                    if( n % 100 == 1 )
                        _StatusUpdate( "Copying Node: " + n.ToString() + " of " + NodesTable.Rows.Count.ToString() );

                    CswNbtMetaDataNodeType NodeType = _decodeNodeType( NodeRow, NodeTypesTable, NodeTypeToNodeRelation );
                    if( NodeType != null )
                    {
                        // only copy nodes once, even if the node is listed more than once in the data
                        if( !NodeIdMap.ContainsKey( NodeRow[CswNbtImportExportFrame._Attribute_NodeId].ToString().ToLower() ) )
                        {
                            CswNbtNode Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
                            NodeIdMap.Add( CswTools.XmlRealAttributeName( NodeRow[CswNbtImportExportFrame._Attribute_NodeId].ToString() ).ToLower(), Node.NodeId.PrimaryKey );    // for property value references
                            NodeRow["destnodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );                       // for posterity

                            NodeMap.Add( Node.NodeId, Node );
                        }
                    }
                    else
                    {
                        _StatusUpdate( "ERROR: Could not find matching nodetype for node: " + n.ToString() + " (nodeid = " + NodeRow[CswNbtImportExportFrame._Attribute_NodeId].ToString() + ")" );
                        _addEntryToErrorLog( ref ErrorLog, NodeRow, "Could not find matching nodetype for node" );
                    }
                }
                _StatusUpdate( "Finished Copying Nodes" );
            }


            // This is a workaround to BZ 9650
            _CswNbtResources.Nodes.Clear();


            //---------------------------------------------------------------------------
            // Set property values
            CswNbtNode GeneralUserRole = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( "Equipment User" );
            Collection<CswNbtNode> ImportedNodes = new Collection<CswNbtNode>();
            if( NodesTable != null )
            {
                _StatusUpdate( "Started Setting Property Values" );
                Int32 n = 0;
                foreach( DataRow NodeRow in NodesTable.Rows )
                {
                    n++;
                    if( n % 100 == 1 )
                        _StatusUpdate( "Processing Node: " + n.ToString() + " of " + NodesTable.Rows.Count.ToString() );

                    CswNbtMetaDataNodeType NodeType = _decodeNodeType( NodeRow, NodeTypesTable, NodeTypeToNodeRelation );

                    //restructured for bz # 7816: Capture the node id for the 'doesn't exist' error message
                    CswNbtNode Node = null;
                    CswPrimaryKey NodeId = null;
                    if( NodeRow["destnodeid"] != null && CswTools.IsInteger( NodeRow["destnodeid"].ToString() ) )   // skip nodes skipped above
                    {
                        //Node = _CswNbtResources.Nodes.GetNode( CswConvert.ToInt32( NodeRow["destnodeid"].ToString() ) );
                        NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["destnodeid"].ToString() ) );
                    }
                    else if( IMode == ImportMode.Update )
                    {
                        //Node = _CswNbtResources.Nodes.GetNode( CswConvert.ToInt32( NodeRow[_Attribute_NodeId].ToString() ) );
                        NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow[CswNbtImportExportFrame._Attribute_NodeId].ToString() ) );
                    }

                    if( NodeId != null )
                    {
                        //Node = _CswNbtResources.Nodes.GetNode( NodeId );
                        Node = NodeMap[NodeId];
                        ImportedNodes.Add( Node );

                        if( Node != null )
                        {
                            Int32 p = 0;
                            DataRow[] PropValueRows = NodeRow.GetChildRows( CswNbtImportExportFrame._Element_Node + "_" + CswNbtImportExportFrame._Element_PropValue );
                            foreach( DataRow PropValueRow in PropValueRows )
                            {
                                p++;

                                CswNbtMetaDataNodeTypeProp NodeTypeProp = _decodeNodeTypeProp( PropValueRow, NodeType, NodeTypePropsTable, NodeTypePropToPropValueRelation );
                                if( NodeTypeProp != null )
                                {
                                    // BZ 10340 - Create the relationship target, if it's not there
                                    string RelatedNodeID = PropValueRow[CswNbtSubField.SubFieldName.NodeID.ToString()].ToString();
                                    if( !NodeIdMap.ContainsKey( CswTools.XmlRealAttributeName( RelatedNodeID ).ToLower() ) )
                                    {
                                        if( RelatedNodeID.StartsWith( "ND--" ) )
                                        {
                                            // Ex: ND--USER--dch
                                            string[] SplitRelatedNodeID = RelatedNodeID.Split( new string[] { "--" }, StringSplitOptions.None );
                                            CswNbtMetaDataNodeType RelatedNodeType = _CswNbtResources.MetaData.getNodeType( SplitRelatedNodeID[1] );
                                            if( RelatedNodeType != null && SplitRelatedNodeID.Length >= 2 && SplitRelatedNodeID[2] != string.Empty )
                                            {
                                                CswNbtNode RelatedNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RelatedNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
                                                NodeIdMap.Add( CswTools.XmlRealAttributeName( RelatedNodeID ).ToLower(), RelatedNode.NodeId.PrimaryKey );    // for property value references
                                                NodeRow["destnodeid"] = CswConvert.ToDbVal( RelatedNode.NodeId.PrimaryKey );       // for posterity

                                                // Kludge to make the node have a value even when no property values are supplied for it
                                                RelatedNode.NodeName = SplitRelatedNodeID[2];

                                                // Kludge specifically for 10340:
                                                if( SplitRelatedNodeID[1].ToLower() == "user" )
                                                {
                                                    RelatedNode.Properties["Username"].AsText.Text = SplitRelatedNodeID[2].ToLower();
                                                    if( GeneralUserRole != null )
                                                        RelatedNode.Properties["Role"].AsRelationship.RelatedNodeId = GeneralUserRole.NodeId;
                                                    RelatedNode.Properties["AccountLocked"].AsLogical.Checked = Tristate.True;
                                                }
                                                RelatedNode.postChanges( false, false, true );
                                            }
                                        }
                                    }


                                    if( IMode == ImportMode.Duplicate || IMode == ImportMode.Overwrite )
                                    {
                                        CswNbtNodePropWrapper PropWrapper = Node.Properties[NodeTypeProp];
                                        // Each NodeTypeProp* class handles the reference updates by itself,
                                        // using NodeMap and NodeTypeMap to map the reference values
                                        PropWrapper.ReadDataRow( PropValueRow, NodeIdMap, NodeTypeMap );
                                    }
                                    else if( !NodeTypeProp.ReadOnly ) // BZ 7388
                                    {
                                        CswNbtNodePropWrapper PropWrapper = Node.Properties[NodeTypeProp];
                                        PropWrapper.ReadDataRow( PropValueRow, null, null );
                                    }
                                }
                                else
                                {
                                    _StatusUpdate( "ERROR: Could not find matching property for property value: " + p.ToString() );
                                    _addEntryToErrorLog( ref ErrorLog, PropValueRow, "Could not find matching property for value" );
                                }
                            }

                            // Is this node redundant with another existing node?
                            foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in _CswNbtResources.MetaData.getNodeTypeProps( Node.NodeTypeId ) )
                            {
                                if( MetaDataProp.IsUnique() )
                                {
                                    CswNbtNode OtherNode = _CswNbtResources.Nodes.FindNodeByUniqueProperty( MetaDataProp, Node.Properties[MetaDataProp] );
                                    if( OtherNode != null && OtherNode.NodeId != Node.NodeId )
                                    {
                                        if( IMode == ImportMode.Overwrite )
                                        {
                                            OtherNode.delete();
                                        }
                                        else if( IMode == ImportMode.Duplicate )
                                        {
                                            // It would be better not to create the node in the first place
                                            // but we're waiting on BZ 9650 to be fixed.
                                            Node.delete();
                                            break;
                                        }
                                    } // if( OtherNode != null )
                                } // if( MetaDataProp.IsUnique )
                            } // foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in Node.NodeType.NodeTypeProps )

                            try
                            {
                                Node.postChanges( false, false, true );
                            }
                            catch( Exception ex )
                            {
                                _StatusUpdate( "ERROR: Could not save node: " + ex.Message );
                                _addEntryToErrorLog( ref ErrorLog, NodeRow, "Could not save node: " + ex.Message );
                            }
                        } // if( Node != null )
                        else
                        {
                            _StatusUpdate( "Could not import node with node id (" + NodeId.ToString() + ") because it does not exist" );
                            _addEntryToErrorLog( ref ErrorLog, NodeRow, "Could not import node with node id (" + NodeId.ToString() + ") because it does not exist" );
                        }
                    } // if( NodeId != null )
                } // foreach( DataRow NodeRow in NodesTable.Rows )

                _StatusUpdate( "Finished Setting Property Values" );

            } // if( NodesTable != null )


            // Sort-of-a-kludge for BZ 10339
            // Set Equipment Nodes with Assemblies to pendingupdate=1
            _StatusUpdate( "Setting update flag on Nodes" );

            Int32 x = 0;
            foreach( CswNbtNode Node in ImportedNodes )
            {
                x++;
                if( x % 100 == 1 )
                    _StatusUpdate( "Processing Node: " + x.ToString() + " of " + NodesTable.Rows.Count.ToString() );

                if( Node.getObjectClass().ObjectClass == NbtObjectClass.EquipmentClass )
                {
                    if( Node.Properties[_CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( Node.NodeTypeId, "Assembly" )].AsRelationship.RelatedNodeId != null )
                    {
                        Node.PendingUpdate = true;
                        Node.postChanges( false, false, true );
                    }
                }
            }

            _StatusUpdate( "Finished setting update flag on Nodes" );


            _StatusUpdate( "Processing Views" );

            // View(s)
            // We need to do this after roles and users are imported
            //bz # 7872: Make sure the viewinfo element is really there.
            DataTable ViewsTable = XmlData.Tables[CswNbtImportExportFrame._Element_ViewInfo];
            if( null != ViewsTable && 0 < ViewsTable.Rows.Count && ViewsTable.Columns.Contains( CswNbtImportExportFrame._Element_ViewXml ) )
            {
                //save the first (possibly, only) view:
                ViewXml = ViewsTable.Rows[0][CswNbtImportExportFrame._Element_ViewXml].ToString();

                // match or create the view:
                Int32 v = 0;
                foreach( DataRow ViewRow in ViewsTable.Rows )
                {
                    v++;
                    if( v % 10 == 1 )
                        _StatusUpdate( "Processing View: " + v.ToString() + " of " + ViewsTable.Rows.Count.ToString() );

                    //CswNbtViewFactory ViewFactory = new CswNbtViewFactory();
                    string ThisViewXml = ViewRow[CswNbtImportExportFrame._Element_ViewXml].ToString();
                    string ThisViewName = ViewRow[CswNbtImportExportFrame._Element_ViewName].ToString();
                    CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                    ThisView.LoadXml( ViewXml );

                    DataTable ExistingView = _CswNbtResources.ViewSelect.getView( ThisViewName, ThisView.Visibility, ThisView.VisibilityRoleId, ThisView.VisibilityUserId );
                    bool SkipView = false;
                    if( ExistingView.Rows.Count > 0 )
                    {
                        if( IMode == ImportMode.Duplicate )
                            SkipView = true;
                        else  // Update or Overwrite
                            ThisView.ViewId = new CswNbtViewId( CswConvert.ToInt32( ExistingView.Rows[0]["nodeviewid"] ) );
                    }
                    else
                    {
                        ThisView.ImportView( ThisViewName, ThisViewXml, NodeTypeMap, NodeTypePropMap, NodeIdMap );
                    }
                    if( !SkipView )
                    {
                        ViewMap.Add( CswConvert.ToInt32( ViewRow[CswNbtImportExportFrame._Element_ViewId] ), ThisView.ViewId );
                        ThisView.save();
                    }
                } // foreach( DataRow ViewRow in ViewsTable.Rows )
            } // if( null != ViewsTable && 0 < ViewsTable.Rows.Count && ViewsTable.Columns.Contains( CswNbtImportExportFrame._Element_ViewXml ) )

            _StatusUpdate( "Finished Processing Views" );

            _StatusUpdate( "Fixing ViewSelect Property Values" );

            // Fix ViewSelect property values
            Int32 i = 0;
            foreach( CswNbtNode Node in ImportedNodes )
            {
                i++;
                if( i % 100 == 1 )
                    _StatusUpdate( "Processing Node: " + i.ToString() + " of " + ImportedNodes.Count.ToString() );

                foreach( CswNbtNodePropWrapper ViewProp in Node.Properties[(CswNbtMetaDataFieldType.NbtFieldType) CswNbtMetaDataFieldType.NbtFieldType.ViewPickList] )
                {
                    CswCommaDelimitedString NewSelectedViewIds = new CswCommaDelimitedString();
                    Collection<int> SelectedViewIds = ViewProp.AsViewPickList.SelectedViewIds.ToIntCollection();
                    foreach( Int32 ViewId in SelectedViewIds )
                    {
                        if( ViewMap.ContainsKey( ViewId ) )
                            NewSelectedViewIds.Add( ViewMap[ViewId].ToString() );
                    }
                    ViewProp.AsViewPickList.SelectedViewIds = NewSelectedViewIds;
                    ViewProp.AsViewPickList.PendingUpdate = true;
                } // foreach( CswNbtNodePropWrapper ViewProp in Node.Properties[ViewPickList] )
                Node.postChanges( false, false, true );
            } // foreach( CswNbtNode Node in ImportedNodes )

            _StatusUpdate( "Done ViewSelect Property Values" );

            _StatusUpdate( "Import Finished" );

            ResultXml = XmlData.GetXml();
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

    } // class CswImporterLegacy
} // namespace ChemSW.Nbt
