using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30082_UserCache : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30082; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30082_UserCache"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp CachedDataOcp = UserOc.getObjectClassProp( CswNbtObjClassUser.PropertyName.CachedData );
            foreach( CswNbtMetaDataNodeTypeProp Prop in CachedDataOcp.getNodeTypeProps() )
            {
                Prop.removeFromAllLayouts();
            }
            CswNbtActRequesting ActRequesting = new CswNbtActRequesting( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources );
            foreach( CswNbtObjClassUser User in UserOc.getNodes( forceReInit: true, includeSystemNodes: false, IncludeDefaultFilters: false, IncludeHiddenNodes: true ) )
            {
                ActRequesting.resetCartCounts( User: User );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema