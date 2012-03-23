using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25519
    /// </summary>
    public class CswUpdateSchemaCase25519 : CswUpdateSchemaTo
    {

        public override void update()
        {
            CswNbtMetaDataNodeType InspectionScheduleNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            CswNbtMetaDataNodeTypeProp TypeNtp = InspectionScheduleNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.TargetTypePropertyName );
            TypeNtp.ServerManaged = false;

        }//Update()

    }//class CswUpdateSchemaCase25519

}//namespace ChemSW.Nbt.Schema