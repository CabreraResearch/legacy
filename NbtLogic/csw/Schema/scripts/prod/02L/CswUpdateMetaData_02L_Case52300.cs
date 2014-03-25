using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52300: CswUpdateSchemaTo
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

            //if( null == MethodOC )
            //{
            //    MethodOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.MethodClass, "doc.png", true );
            //    _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, MethodOC.ObjectClassId );
            //} //if MethodMDOC == null

            CswNbtMetaDataObjectClassProp ObsoleteOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MethodOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMethod.PropertyName.Obsolete,
                FieldType = CswEnumNbtFieldType.Logical,
                IsRequired = true
            } );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ObsoleteOCP, CswEnumTristate.False );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema