using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02E_Case29847 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29847; }
        }

        public override string ScriptName
        {
            get { throw new NotImplementedException(); }
        }

        public override bool AlwaysRun
        {
            get { throw new NotImplementedException(); }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp EmailNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Email );
                CswNbtMetaDataNodeTypeProp PhoneNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Phone );
                EmailNTP.updateLayout( CswEnumNbtLayoutType.Add, PhoneNTP, false );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema