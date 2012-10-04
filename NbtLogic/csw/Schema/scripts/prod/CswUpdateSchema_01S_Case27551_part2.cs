using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27551_part2
    /// </summary>
    public class CswUpdateSchema_01S_Case27551_part2 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp mixtureOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27551; }
        }

        //Update()

    }//class CswUpdateSchemaCase27551_part2

}//namespace ChemSW.Nbt.Schema