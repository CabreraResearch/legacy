using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30875 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {                
            get { return 30875; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Put Work Unit on User Add Layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            if( null != UserOC )
            {
                foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp WorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
                    WorkUnitNTP.updateLayout( CswEnumNbtLayoutType.Add, true );
                }
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema