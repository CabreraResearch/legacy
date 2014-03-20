using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52345 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52345; }
        }

        public override string Title
        {
            get { return "Fix requisitionable default value"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp RequisitionableOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Requisitionable );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( RequisitionableOCP, CswEnumTristate.True );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema