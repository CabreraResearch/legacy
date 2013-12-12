using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31086 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31086; }
        }

        public override string Title
        {
            get { return "Create view for impersonation nodeselect"; }
        }

        public override void update()
        {
            // Create a view that the impersonation nodeselect uses
            CswNbtView ImpersonationView = _CswNbtSchemaModTrnsctn.makeNewView( "Impersonation NodeSelect View", CswEnumNbtViewVisibility.Hidden );
            ImpersonationView.IsSystem = true;

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp UsernameOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Username );
            CswNbtMetaDataObjectClassProp RoleOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Role );
            CswNbtViewRelationship Parent = ImpersonationView.AddViewRelationship( UserOC, true );
            ImpersonationView.AddViewPropertyAndFilter(
                ParentViewRelationship: Parent,
                MetaDataProp: UsernameOCP,
                Conjunction: CswEnumNbtFilterConjunction.And,
                Value: CswNbtObjClassUser.ChemSWAdminUsername,
                FilterMode: CswEnumNbtFilterMode.NotEquals
                );
            ImpersonationView.AddViewPropertyAndFilter(
                ParentViewRelationship: Parent,
                MetaDataProp: RoleOCP,
                Conjunction: CswEnumNbtFilterConjunction.And,
                Value: CswNbtObjClassRole.ChemSWAdminRoleName,
                FilterMode: CswEnumNbtFilterMode.NotEquals,
                SubFieldName: CswEnumNbtSubFieldName.Name
                );
            ImpersonationView.save();
        } // update()
    }

}//namespace ChemSW.Nbt.Schema