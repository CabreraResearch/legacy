using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_Case52300: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52300; }
        }

        public override string Title
        {
            get { return "MLM2: CIS-52300: Add obsolete property to method OC"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MethodOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass );

            CswNbtMetaDataObjectClassProp ObsoleteOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethod.PropertyName.Obsolete,
                FieldType = CswEnumNbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ObsoleteOCP, CswEnumTristate.False );

            //set existing OCP Method no's unique property to 
            //false
            CswNbtMetaDataObjectClassProp MethodNoOCP = MethodOC.getObjectClassProp( CswNbtObjClassMethod.PropertyName.MethodNo );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MethodNoOCP, CswEnumNbtObjectClassPropAttributes.isunique, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MethodNoOCP, CswEnumNbtObjectClassPropAttributes.iscompoundunique, false );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema