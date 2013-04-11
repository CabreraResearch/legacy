using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29245
    /// </summary>
    public class CswUpdateSchema_02A_Case29245B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29245; }
        }

        public override void update()
        {
            // Remove the HazardClassesNTP from all layouts for any MaterialNTs != Chemical
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialClass );
            if( null != MaterialOC )
            {
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    if( false == MaterialNT.NodeTypeName.Equals( "Chemical" ) )
                    {
                        CswNbtMetaDataNodeTypeProp HazardClassesNTP = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.HazardClasses );
                        HazardClassesNTP.removeFromAllLayouts();
                    }
                }
            }

        } // update()

    }//class CswUpdateSchema_02A_Case29245B

}//namespace ChemSW.Nbt.Schema