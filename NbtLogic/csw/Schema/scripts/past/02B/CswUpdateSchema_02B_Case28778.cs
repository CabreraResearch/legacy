using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28778
    /// </summary>
    public class CswUpdateSchema_02B_Case28778 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 28778; }
        }

        public override void update()
        {
            // Default GHS label
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass PrintLabelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
            if( null != PrintLabelOC )
            {
                CswNbtMetaDataNodeType PrintLabelNT = PrintLabelOC.FirstNodeType;
                if( null != PrintLabelNT )
                {
                    CswNbtObjClassPrintLabel GhsLabel = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( PrintLabelNT.NodeTypeId );
                    GhsLabel.LabelName.Text = "Example GHS Label";
                    if( null != ContainerOC )
                    {
                        foreach(CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes())
                        {
                            GhsLabel.NodeTypes.SelectedNodeTypeIds.Add( ContainerNT.NodeTypeId.ToString() );
                        }
                    }
                    GhsLabel.EplText.Text = @"I8,0,001
S2
OD
JF
WN
D12
ZB
q375
N
A25,15,0,2,1,1,N,""{Material}""
B25,45,0,1,3,6,40,N,""{Barcode}""
A25,80,0,2,1,1,N,""{Barcode}""
A25,130,0,2,1,1,N,""{NBTGHSB}""
GW0,160,{NBTGHSPICTOS}
GW128,160,{NBTGHSPICTOS_2}
GW256,160,{NBTGHSPICTOS_3}
P1";
                    GhsLabel.postChanges( false );
                }
            }

        } // update()

    }//class CswUpdateSchema_02B_Case28778

}//namespace ChemSW.Nbt.Schema