using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case28768 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28768; }
        }

        public override void update()
        {
            // This is a placeholder script that does nothing.
            CswNbtMetaDataNodeType PrintLabelNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Print Label" );
            if( null != PrintLabelNodeType )
            {
                CswNbtNode PrintLabelNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( PrintLabelNodeType.NodeTypeId, CswEnumNbtMakeNodeOperation.DoNothing );

                //must have no tabs or spaces to end up looking correct in the the resutling node
                string EplText = @"I8,1,001
q664
S2
OD
JF
WN
D7
ZB
Q300,37
N
B41,85,0,3,3,8,50,N,""{Barcode}""
A41,140,0,3,1,1,N,""{Barcode}""
A41,155,0,3,1,N,""{Material}""
A41,155,0,3,1,N,""{NBTGHSA}""
A41,155,0,3,1,N,""{NBTGHSA_2}""
A41,155,0,3,1,N,""{NBTGHSA_3}""
P1";

                PrintLabelNode.Properties[CswNbtObjClassPrintLabel.PropertyName.EplText].AsMemo.Text = EplText;
                PrintLabelNode.Properties[CswNbtObjClassPrintLabel.PropertyName.LabelName].AsText.Text = "Default GHS Container Label";

                CswNbtMetaDataNodeType ContainerNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
                if( null != ContainerNodeType )
                {
                    PrintLabelNode.Properties[CswNbtObjClassPrintLabel.PropertyName.NodeTypes].AsNodeTypeSelect.SelectedNodeTypeIds.Add( ContainerNodeType.NodeTypeId.ToString() );
                }

                PrintLabelNode.postChanges( true );
            }
        } // update()

    }//class CswUpdateSchema_02B_Case28768

}//namespace ChemSW.Nbt.Schema