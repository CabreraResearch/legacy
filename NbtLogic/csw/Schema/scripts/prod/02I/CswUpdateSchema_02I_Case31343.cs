using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31343 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31343; }
        }

        public override string Title
        {
            get { return "Create new ChemWatch action; set permissions"; }
        }

        public override void update()
        {
            // Create new action 'ChemWatch'
            _CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.ChemWatch, true, "", "System" );

            // Grant permission to all administrators
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( forceReInit: false, IncludeDefaultFilters: false, IncludeHiddenNodes: true, includeSystemNodes: true ) )
            {
                if( RoleNode.Administrator.Checked == CswEnumTristate.True )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.ChemWatch, RoleNode, true );
                }
            } // foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( forceReInit: false, IncludeDefaultFilters: false, IncludeHiddenNodes: true, includeSystemNodes: true ) )

        } // update()

    }

}//namespace ChemSW.Nbt.Schema