using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28108
    /// </summary>
    public class CswUpdateSchemaCase_01T_LocationLabel_Case28108 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 28108; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataNodeType PrintLabelNt = PrintLabelOc.getLatestVersionNodeTypes().FirstOrDefault();
            CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            if( null != PrintLabelNt )
            {
                CswNbtObjClassPrintLabel LocationLabel = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( PrintLabelNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                LocationLabel.IsDemo = true;
                LocationLabel.LabelName.Text = "Location Label";
                foreach( CswNbtMetaDataNodeType LocationNt in LocationOc.getLatestVersionNodeTypes() )
                {
                    LocationLabel.NodeTypes.SelectedNodeTypeIds.Add( LocationNt.NodeTypeId.ToString() );
                }
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
P1";
                LocationLabel.EplText.Text = EplText;
                LocationLabel.postChanges( ForceUpdate: false );
            }
        }

        //Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema