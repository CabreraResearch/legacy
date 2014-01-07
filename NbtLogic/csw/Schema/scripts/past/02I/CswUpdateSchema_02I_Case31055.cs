using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31055 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31055; }
        }

        public override string Title
        {
            get { return "Remove Location Code from layouts"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp NodetypeProp = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.LocationCode );
                NodetypeProp.removeFromAllLayouts();
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema