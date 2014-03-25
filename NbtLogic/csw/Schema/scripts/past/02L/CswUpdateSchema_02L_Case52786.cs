using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52786 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52786; }
        }

        public override string Title
        {
            get { return "Rename Lab Safety Group"; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType LabSafetyGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Lab Safety Group demo" );
            if( null != LabSafetyGroupNT )
            {
                LabSafetyGroupNT.DesignNode.NodeTypeName.Text = "Lab Safety demo Group";
                LabSafetyGroupNT.DesignNode.postChanges( false );
            }
        } // update()
    } // class CswUpdateSchema_02L_Case52786 

}//namespace ChemSW.Nbt.Schema