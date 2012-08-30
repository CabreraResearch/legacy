using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27488
    /// </summary>
    public class CswUpdateSchema_RequestItem_Case27488 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            // Set the Request Item's Size Relationship View
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp RequestItemMaterialOCP = RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Material );
            CswNbtMetaDataObjectClassProp SizeMaterialOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );

            foreach( CswNbtMetaDataNodeType RequestItemNT in RequestItemOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp RequestItemSizeNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Size );

                CswNbtView SizeView = _CswNbtSchemaModTrnsctn.restoreView( RequestItemSizeNTP.ViewId );
                SizeView.Root.ChildRelationships.Clear();

                CswNbtViewRelationship RequestItemViewRel = SizeView.AddViewRelationship( RequestItemOC, false );
                CswNbtViewRelationship MaterialViewRel = SizeView.AddViewRelationship( RequestItemViewRel, NbtViewPropOwnerType.First, RequestItemMaterialOCP, true );
                CswNbtViewRelationship SizeViewRel = SizeView.AddViewRelationship( MaterialViewRel, NbtViewPropOwnerType.Second, SizeMaterialOCP, true );

                //Case 27438 
                CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
                CswNbtMetaDataNodeType SizeNt = SizeOc.FirstNodeType;
                CswNbtMetaDataNodeTypeProp DispensableNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.DispensablePropertyName );
                SizeView.AddViewPropertyAndFilter(
                        ParentViewRelationship: SizeViewRel,
                        MetaDataProp: DispensableNtp,
                        Value: Tristate.False.ToString(),
                        SubFieldName: CswNbtSubField.SubFieldName.Checked,
                        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals
                        );

                SizeView.save();
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema