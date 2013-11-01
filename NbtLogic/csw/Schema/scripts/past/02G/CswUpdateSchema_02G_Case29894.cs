using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case29894: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 29894; }
        }

        public override string ScriptName
        {
            get { return "02G_29894"; }
        }

        public override string Title
        {
            get { return "Make Container Quantity Required"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp QuantityOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema