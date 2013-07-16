using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateSchema_02D_Case29311_Sequences : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
        }

        public override void update()
        {
            CswTableUpdate jctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29311_jctddntp_update", "jct_dd_ntp" );
            DataTable jctTable = jctUpdate.getEmptyTable();

            // Set up Sequence Nodetype
            CswNbtMetaDataObjectClass SequenceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );
            CswNbtMetaDataNodeType SequenceNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeNew( new CswNbtWcfMetaDataModel.NodeType( SequenceOC )
                {
                    NodeTypeName = "Design Sequence",
                    Category = "Design",
                    IconFileName = "wrench.png"
                } );
            SequenceNT.addNameTemplateText( CswNbtObjClassDesignSequence.PropertyName.Name );
            Int32 TabId = SequenceNT.getFirstNodeTypeTab().TabId;

            CswNbtMetaDataNodeTypeProp SeqNameNTP = SequenceNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignSequence.PropertyName.Name );
            CswNbtMetaDataNodeTypeProp SeqNextValueNTP = SequenceNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignSequence.PropertyName.NextValue );
            CswNbtMetaDataNodeTypeProp SeqPadNTP = SequenceNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignSequence.PropertyName.Pad );
            CswNbtMetaDataNodeTypeProp SeqPostNTP = SequenceNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignSequence.PropertyName.Post );
            CswNbtMetaDataNodeTypeProp SeqPreNTP = SequenceNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignSequence.PropertyName.Pre );

            // Edit Layout
            SeqNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
            SeqPreNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
            SeqPostNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
            SeqPadNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
            SeqNextValueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 5, DisplayColumn: 1 );

            // Add Layout
            SeqNameNTP.updateLayout( CswEnumNbtLayoutType.Add, false, Int32.MinValue, DisplayRow: 1, DisplayColumn: 1 );
            SeqPreNTP.updateLayout( CswEnumNbtLayoutType.Add, false, Int32.MinValue, DisplayRow: 2, DisplayColumn: 1 );
            SeqPostNTP.updateLayout( CswEnumNbtLayoutType.Add, false, Int32.MinValue, DisplayRow: 3, DisplayColumn: 1 );
            SeqPadNTP.updateLayout( CswEnumNbtLayoutType.Add, false, Int32.MinValue, DisplayRow: 4, DisplayColumn: 1 );
            SeqNextValueNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

            // Populate nodes
            // Very important that this happens BEFORE we map to the nodetypes table, or else we'll end up duplicating rows!
            Dictionary<Int32, CswNbtObjClassDesignSequence> SequenceNodeMap = new Dictionary<int, CswNbtObjClassDesignSequence>();
            foreach( DataRow SeqRow in _CswNbtSchemaModTrnsctn.getAllSequences().Rows )
            {
                CswNbtObjClassDesignSequence node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SequenceNT.NodeTypeId, OverrideUniqueValidation: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassDesignSequence NewSeqNode = NewNode;
                        NewSeqNode.Name.Text = SeqRow["sequencename"].ToString();
                        NewSeqNode.Pad.Value = CswConvert.ToInt32( SeqRow["pad"] );
                        NewSeqNode.Post.Text = SeqRow["post"].ToString();
                        NewSeqNode.Pre.Text = SeqRow["prep"].ToString();
                    } );
                node.RelationalId = new CswPrimaryKey( "sequences", CswConvert.ToInt32( SeqRow["sequenceid"] ) );
                node.postChanges( false );
                SequenceNodeMap.Add( node.RelationalId.PrimaryKey, node );
            }

            // Here's where the extra special super-secret magic comes in

            //SequenceNT.TableName = "sequences";
            SequenceNT._DataRow["tablename"] = "sequences";

            _addJctRow( jctTable, SeqNameNTP, SequenceNT.TableName, "sequencename" );
            _addJctRow( jctTable, SeqPadNTP, SequenceNT.TableName, "pad" );
            _addJctRow( jctTable, SeqPostNTP, SequenceNT.TableName, "post" );
            _addJctRow( jctTable, SeqPreNTP, SequenceNT.TableName, "prep" );
            jctUpdate.update( jctTable );

            // Set up existing relationships to sequences
            Dictionary<Int32, CswNbtObjClassDesignSequence> SequenceValueMap = new Dictionary<Int32, CswNbtObjClassDesignSequence>();
            string Sql = @"select n.nodeid, r.sequenceid
                             from nodetype_props p
                             join nodetypes t on p.nodetypeid = t.nodetypeid
                             join nodes n on t.nodetypeid = n.nodetypeid
                             join nodetype_props r on n.relationalid = r.nodetypepropid
                            where p.propname = 'Sequence' and t.nodetypename like 'Design%'";
            CswArbitrarySelect ExistingSequenceIdSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "getExistingSequenceIds", Sql );
            DataTable ExistingSequenceIdTable = ExistingSequenceIdSelect.getTable();
            foreach( DataRow row in ExistingSequenceIdTable.Rows )
            {
                Int32 thisSeqId = CswConvert.ToInt32( row["sequenceid"] );
                if( Int32.MinValue != thisSeqId && SequenceNodeMap.ContainsKey( thisSeqId ))
                {
                    SequenceValueMap.Add( CswConvert.ToInt32( row["nodeid"] ), SequenceNodeMap[thisSeqId] );
                }
            }

            foreach( CswEnumNbtFieldType ft in new CswEnumNbtFieldType[] { CswEnumNbtFieldType.Barcode, CswEnumNbtFieldType.Sequence } )
            {
                CswNbtMetaDataNodeType PropNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassDesignNodeTypeProp.getNodeTypeName( ft ) );
                //CswNbtFieldTypeRuleSequence Rule = (CswNbtFieldTypeRuleSequence) _CswNbtSchemaModTrnsctn.MetaData.getFieldTypeRule( ft );
                //CswNbtFieldTypeAttribute SequenceAttribute = Rule.getAttributes().FirstOrDefault( a => a.Name == CswEnumNbtPropertyAttributeName.Sequence );
                CswNbtMetaDataNodeTypeProp SequenceNTP = PropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Sequence );
                SequenceNTP.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), SequenceOC.ObjectClassId );

                foreach( CswNbtObjClassDesignNodeTypeProp PropNode in PropNT.getNodes( false, true ) )
                {
                    if( SequenceValueMap.ContainsKey( PropNode.NodeId.PrimaryKey ) )
                    {
                        CswNbtObjClassDesignSequence SeqNode = SequenceValueMap[PropNode.NodeId.PrimaryKey];
                        if( null != SeqNode )
                        {
                            PropNode.Node.Properties[SequenceNTP].AsRelationship.RelatedNodeId = SeqNode.NodeId;
                            PropNode.postChanges( false );
                        }
                    }
                }
            } // foreach( CswEnumNbtFieldType ft in new CswEnumNbtFieldType[] {CswEnumNbtFieldType.Barcode, CswEnumNbtFieldType.Sequence} )

        } // update()


        private void _addJctRow( DataTable JctTable, CswNbtMetaDataNodeTypeProp Prop, string TableName, string ColumnName, CswEnumNbtSubFieldName SubFieldName = null )
        {
            _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( TableName, ColumnName );
            DataRow NodeTypeNameRow = JctTable.NewRow();
            NodeTypeNameRow["nodetypepropid"] = Prop.PropId;
            NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
            if( null != SubFieldName )
            {
                NodeTypeNameRow["subfieldname"] = SubFieldName.ToString();
            }
            else if( null != Prop.getFieldTypeRule().SubFields.Default )
            {
                NodeTypeNameRow["subfieldname"] = Prop.getFieldTypeRule().SubFields.Default.Name;
            }
            JctTable.Rows.Add( NodeTypeNameRow );
        }


    }//class CswUpdateSchema_02D_Case29311_Design

}//namespace ChemSW.Nbt.Schema