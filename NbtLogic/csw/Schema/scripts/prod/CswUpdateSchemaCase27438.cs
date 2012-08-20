using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27438
    /// </summary>
    public class CswUpdateSchemaCase27438 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataNodeType SizeNt = SizeOc.FirstNodeType;
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType RequestItemNt = RequestItemOC.FirstNodeType;
            CswNbtMetaDataNodeTypeProp SizeNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Size );

            if( SizeNt != null )
            {
                //Hide Dispensable and QuantityEditable properties
                CswNbtMetaDataNodeTypeProp DispensableNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.DispensablePropertyName );
                DispensableNtp.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp QuantityEditableNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName );
                QuantityEditableNtp.removeFromAllLayouts();

                //Update RequestItem's Size View to consider Dispensable value
                if( RequestItemNt != null )
                {
                    CswNbtView SizeView = _CswNbtSchemaModTrnsctn.restoreView( SizeNtp.ViewId );
                    SizeView.Root.ChildRelationships.Clear();
                    CswNbtViewRelationship Root = SizeView.AddViewRelationship( SizeNt, true );
                    SizeView.AddViewPropertyAndFilter(
                        ParentViewRelationship: Root,
                        MetaDataProp: DispensableNtp,
                        Value: Tristate.True.ToString(),
                        SubFieldName: CswNbtSubField.SubFieldName.Checked,
                        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals
                        );
                    SizeView.save();
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase27438

}//namespace ChemSW.Nbt.Schema