using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31406 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31406; }
        }

        public override string Title
        {
            get { return "Update My Request History View"; }
        }

        public override void update()
        {
            CswNbtView RequestView = _CswNbtSchemaModTrnsctn.restoreView( "My Request History", CswEnumNbtViewVisibility.Global );
            if( null != RequestView )
            {
                CswNbtMetaDataObjectClass RequestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
                RequestView.Root.ChildRelationships.Clear();
                RequestView.AddViewRelationship( RequestOC, true );
                RequestView.save();
            }
        }
    }
}