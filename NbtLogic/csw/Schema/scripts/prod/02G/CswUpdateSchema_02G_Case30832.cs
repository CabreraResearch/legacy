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
            get { return CswEnumDeveloper.CM; }
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

            // Update existing data
            CswNbtMetaDataNodeType UserNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "User" );
            if( null != UserNT )
            {
                CswNbtMetaDataNodeTypeProp PageSizeNTP = UserNT.getNodeTypePropByObjectClassProp( PageSizeOCP );
                if( null != PageSizeNTP )
                {
                    foreach( CswNbtNode UserNode in UserNT.getNodes( false, true ) )
                    {
                        if( UserNode.Properties[PageSizeNTP].AsNumber.Value < 5 )
                        {
                            UserNode.Properties[PageSizeNTP].AsNumber.Value = 5;
                        }
                        else if( UserNode.Properties[PageSizeNTP].AsNumber.Value > 50 )
                        {
                            UserNode.Properties[PageSizeNTP].AsNumber.Value = 50;
                        }
                    }
                }
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema