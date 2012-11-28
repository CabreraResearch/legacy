using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28249
    /// </summary>
    public class CswUpdateSchema_01U_Case28249 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtView BelowInventoryView = _CswNbtSchemaModTrnsctn.restoreView("Below Minimum Inventory");
            if( null != BelowInventoryView )
            {
                CswNbtMetaDataObjectClass InvLevelClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryLevelClass );
                CswNbtMetaDataObjectClassProp MaterialProp = InvLevelClass.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Material );
                CswNbtMetaDataObjectClass MaterialClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClassProp RequestProp = MaterialClass.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Request );
                CswNbtViewRelationship InvLevelRelationship = BelowInventoryView.Root.ChildRelationships[0];
                if( InvLevelRelationship.TextLabel == "InventoryLevelClass" )
                {
                    CswNbtViewRelationship MaterialRelationship = BelowInventoryView.AddViewRelationship(InvLevelRelationship, NbtViewPropOwnerType.First, MaterialProp, true);
                    BelowInventoryView.AddViewProperty( MaterialRelationship, RequestProp );
                    BelowInventoryView.save();
                }
            }
        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28249; }
        }

    }//class CswUpdateSchema_01U_Case28249

}//namespace ChemSW.Nbt.Schema