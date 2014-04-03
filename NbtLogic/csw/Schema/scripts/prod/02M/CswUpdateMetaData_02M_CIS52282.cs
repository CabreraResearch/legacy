using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52282 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52282; }
        }

        public override string Title
        {
            get { return "Add new Location OCP"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp RDLLOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( LocationOC, new CswNbtWcfMetaDataModel.ObjectClassProp( LocationOC )
            {
                PropName = CswNbtObjClassLocation.PropertyName.RequestDeliveryLocation,
                FieldType = CswEnumNbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( RDLLOCP, CswEnumTristate.False, CswEnumNbtSubFieldName.Checked );
        }
    }
}