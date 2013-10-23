using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30400 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {                
            get { return 30400; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "_MD"; }
        }

        public override string Title
        {
            get { return "Rename User Work Unit to Current Work Unit"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp WorkUnitOCP = UserOC.getObjectClassProp( "Work Unit" );
            if( null != WorkUnitOCP )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( WorkUnitOCP, CswEnumNbtObjectClassPropAttributes.propname, CswNbtObjClassUser.PropertyName.CurrentWorkUnit );
            }
            
        } // update()

    }

}//namespace ChemSW.Nbt.Schema