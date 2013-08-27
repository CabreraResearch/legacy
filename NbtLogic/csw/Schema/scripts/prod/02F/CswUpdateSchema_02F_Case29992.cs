using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case29992 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29992; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RequestMaterialCreateOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialCreateClass );
            foreach( CswNbtMetaDataNodeType RequestMaterialCreateNT in RequestMaterialCreateOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationNTP = RequestMaterialCreateNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Location );
                LocationNTP.updateLayout( CswEnumNbtLayoutType.Add, true );
                CswNbtMetaDataNodeTypeProp QuantityNTP = RequestMaterialCreateNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Quantity );
                QuantityNTP.updateLayout( CswEnumNbtLayoutType.Add, true );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema