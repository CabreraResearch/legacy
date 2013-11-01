using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30379 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Correct Required Logicals"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30379; }
        }

        public override string ScriptName
        {
            get { return "Elementary"; }
        }

        public override void update()
        {
            {
                CswCommaDelimitedString LogicalsMissingValidDefaultValues = new CswCommaDelimitedString();
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    foreach( CswNbtMetaDataNodeTypeProp Logical in NodeType.getNodeTypeProps( CswEnumNbtFieldType.Logical ) )
                    {
                        if( Logical.IsRequired &&
                            ( false == Logical.HasDefaultValue() ||
                              ( CswEnumTristate.False != Logical.DefaultValue.AsLogical.Checked &&
                                CswEnumTristate.True != Logical.DefaultValue.AsLogical.Checked ) ) )
                        {
                            LogicalsMissingValidDefaultValues.Add( Logical.PropId.ToString() );
                            Logical.DefaultValue.AsLogical.Checked = CswEnumTristate.False;
                        }
                    }
                }
                if( LogicalsMissingValidDefaultValues.Count > 0 )
                {
                    string NodesToUpdateSql = @" select nodeid, nodetypepropid from jct_nodes_props where nodetypepropid in (" + LogicalsMissingValidDefaultValues.ToString() + ")";
                    CswArbitrarySelect ArbSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "logicals_missing_default_values", NodesToUpdateSql );
                    DataTable NodeTable = ArbSelect.getTable();
                    foreach( DataRow Row in NodeTable.Rows )
                    {
                        CswPrimaryKey NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Row[ "nodeid" ] ) );
                        CswNbtNode Node = _CswNbtSchemaModTrnsctn.Nodes[ NodeId ];
                        if( null != Node )
                        {
                            Int32 NodeTypePropId = CswConvert.ToInt32( Row[ "nodetypepropid" ] );

                            CswNbtNodePropWrapper PropWrapper = Node.Properties[ NodeTypePropId ];
                            if( null != PropWrapper )
                            {
                                if( CswEnumTristate.False != PropWrapper.AsLogical.Checked &&
                                    CswEnumTristate.True != PropWrapper.AsLogical.Checked )
                                {
                                    PropWrapper.SetDefaultValue();
                                }
                            }
                            Node.postChanges( ForceUpdate: false );
                        }
                    }
                }
            }

            {
                CswCommaDelimitedString ImageListsWithDefaultValue = new CswCommaDelimitedString();
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    foreach( CswNbtMetaDataNodeTypeProp ImageList in NodeType.getNodeTypeProps( CswEnumNbtFieldType.ImageList ) )
                    {
                        if( ImageList.IsRequired &&
                            ImageList.HasDefaultValue() )
                        {
                            ImageListsWithDefaultValue.Add( ImageList.PropId.ToString() );
                        }
                    }
                }

                if( ImageListsWithDefaultValue.Count > 0 )
                {
                    string NodesToUpdateSql = @" select nodeid, nodetypepropid from jct_nodes_props where nodetypepropid in (" + ImageListsWithDefaultValue.ToString() + ")";
                    CswArbitrarySelect ArbSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "imagelists_default_values", NodesToUpdateSql );
                    DataTable NodeTable = ArbSelect.getTable();
                    foreach( DataRow Row in NodeTable.Rows )
                    {
                        CswPrimaryKey NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Row[ "nodeid" ] ) );
                        CswNbtNode Node = _CswNbtSchemaModTrnsctn.Nodes[ NodeId ];
                        if( null != Node )
                        {
                            Int32 NodeTypePropId = CswConvert.ToInt32( Row[ "nodetypepropid" ] );

                            CswNbtNodePropWrapper PropWrapper = Node.Properties[ NodeTypePropId ];
                            if( null != PropWrapper )
                            {
                                if( PropWrapper.Empty )
                                {
                                    PropWrapper.SetDefaultValue();
                                }
                            }
                            Node.postChanges( ForceUpdate: false );
                        }
                    }
                }
            }
        }
    }
}