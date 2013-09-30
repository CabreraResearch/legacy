using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30832 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30832; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Set Max/Min value for Page Size property"; }
        }

        public override void update()
        {
            // Page Size should have a min of 5 and a max of 50
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );

            CswNbtMetaDataObjectClassProp PageSizeOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.PageSize );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PageSizeOCP, CswEnumNbtObjectClassPropAttributes.numberminvalue, 5 );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PageSizeOCP, CswEnumNbtObjectClassPropAttributes.numbermaxvalue, 50 );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema