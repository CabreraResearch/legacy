using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateMetaData_02K_Case31545 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31545; }
        }

        public override string Title
        {
            get { return "Add C3ACDPreferredSuppliers property to UserOC"; }
        }

        public override void update()
        {
            // Add new property to the User object class
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( UserOC )
                {
                    PropName = CswNbtObjClassUser.PropertyName.C3ACDPreferredSuppliers,
                    FieldType = CswEnumNbtFieldType.Memo,
                    ReadOnly = true,
                    ServerManaged = true
                } );

        } // update()

    }//class CswUpdateMetaData_02K_Case31545
}//namespace ChemSW.Nbt.Schema


