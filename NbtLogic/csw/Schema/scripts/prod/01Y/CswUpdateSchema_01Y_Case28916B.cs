using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28916
    /// </summary>
    public class CswUpdateSchema_01Y_Case28916B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28916; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.DocumentClass );
            CswNbtMetaDataNodeType SDSNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SDS Document" );
            if( null != SDSNT )
            {
                foreach( CswNbtObjClassDocument DocNode in DocumentOC.getNodes( false, false ) )
                {
                    if( DocNode.NodeTypeId != SDSNT.NodeTypeId && 
                        ( DocNode.DocumentClass.Value == "SDS" || 
                        ( false == string.IsNullOrEmpty( DocNode.Language.Value ) && false == string.IsNullOrEmpty( DocNode.Format.Value ) ) ) )
                    {
                        CswNbtObjClassDocument SDSNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SDSNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                        SDSNode.Title.Text = DocNode.Title.Text;
                        SDSNode.Owner.RelatedNodeId = DocNode.Owner.RelatedNodeId;
                        SDSNode.FileType.Value = DocNode.FileType.Value;
                        SDSNode.Link.Text = DocNode.Link.Text;
                        SDSNode.Link.Href = DocNode.Link.Href;
                        SDSNode.Language.Value = DocNode.Language.Value;
                        SDSNode.Format.Value = DocNode.Format.Value;
                        SDSNode.AcquiredDate.DateTimeValue = DocNode.AcquiredDate.DateTimeValue;
                        SDSNode.ExpirationDate.DateTimeValue = DocNode.ExpirationDate.DateTimeValue;
                        SDSNode.Archived.Checked = DocNode.Archived.Checked;
                        if( SDSNode.Title.Text.EndsWith( "(Archived)" ) && SDSNode.Archived.Checked == Tristate.True )
                        {
                            SDSNode.Title.Text = SDSNode.Title.Text.Replace("(Archived)", "");
                        }
                        CswNbtMetaDataNodeTypeProp RevisionDateNTP = SDSNT.getNodeTypeProp( "Revision Date" );
                        CswNbtMetaDataNodeTypeProp DocRevDateNTP = DocNode.NodeType.getNodeTypeProp( "Revision Date" );
                        if( null != RevisionDateNTP && null != DocRevDateNTP )
                        {
                            SDSNode.Node.Properties[RevisionDateNTP].AsDateTime.DateTimeValue = DocNode.Node.Properties[DocRevDateNTP].AsDateTime.DateTimeValue;
                        }
                        SDSNode.postChanges( false );
                        _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update jct_nodes_props 
set field1 = (select field1 from jct_nodes_props where nodeid = " + DocNode.NodeId.PrimaryKey + @" and nodetypepropid = " + DocNode.File.NodeTypePropId + @"),
field2 = (select field2 from jct_nodes_props where nodeid = " + DocNode.NodeId.PrimaryKey + @" and nodetypepropid = " + DocNode.File.NodeTypePropId + @"),
blobdata = (select blobdata from jct_nodes_props where nodeid = " + DocNode.NodeId.PrimaryKey + @" and nodetypepropid = " + DocNode.File.NodeTypePropId + @")
where  nodeid = " + SDSNode.NodeId.PrimaryKey + @" and nodetypepropid = " + SDSNode.File.NodeTypePropId );
                        DocNode.Node.delete( false, true );
                    }
                }
            }
        } //Update()
    }//class CswUpdateSchema_01Y_Case28916B
}//namespace ChemSW.Nbt.Schema