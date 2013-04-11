using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public abstract class CswNbtKioskModeRule
    {
        public CswNbtResources _CswNbtResources;
        public string RuleName = "DefaultKioskModeRule";

        public CswNbtKioskModeRule( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        public virtual void ValidateFieldOne( ref OperationData OpData )
        {
            OpData.Field1.ServerValidated = true;
            OpData.Field1.StatusMsg = "";
        }

        public virtual void ValidateFieldTwo( ref OperationData OpData )
        {
            OpData.Field2.ServerValidated = true;
            OpData.Field2.StatusMsg = "";
        }

        public virtual void CommitOperation( ref OperationData OpData )
        {
            OpData.Field2.Value = string.Empty;
            OpData.Field2.SecondValue = string.Empty;
            OpData.Field2.ServerValidated = false;
            OpData.Field2.FoundObjClass = string.Empty;
        }

        public virtual void SetFields( ref OperationData OpData )
        {
            OpData.ModeServerValidated = true;
            OpData.ModeStatusMsg = string.Empty;
            OpData.Field1 = new Field();
            OpData.Field2 = new Field();
            OpData.ScanTextLabel = "Scan a barcode:";
        }

        #region Private Functions

        public CswNbtNode _getNodeByBarcode( CswEnumNbtObjectClass ObjClass, string Barcode, bool IncludeDefaultFilters )
        {
            CswNbtNode Ret = null;
            ICswNbtTree tree = _getTree( ObjClass, Barcode, IncludeDefaultFilters );
            int childCount = tree.getChildNodeCount();
            if( childCount > 0 )
            {
                tree.goToNthChild( 0 );
                Ret = tree.getNodeForCurrentPosition();
            }
            return Ret;
        }

        public CswPrimaryKey _getNodeIdByBarcode( CswEnumNbtObjectClass ObjClass, string Barcode, bool IncludeDefaultFilters )
        {
            CswPrimaryKey Ret = null;
            ICswNbtTree tree = _getTree( ObjClass, Barcode, IncludeDefaultFilters );
            int childCount = tree.getChildNodeCount();
            if( childCount > 0 )
            {
                tree.goToNthChild( 0 );
                Ret = tree.getNodeIdForCurrentPosition();
            }
            return Ret;
        }

        public ICswNbtTree _getTree( CswEnumNbtObjectClass ObjClass, string Barcode, bool IncludeDefaultFilters )
        {
            ICswNbtTree tree = null;
            CswNbtMetaDataObjectClass metaDataOC = _CswNbtResources.MetaData.getObjectClass( ObjClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = (CswNbtMetaDataObjectClassProp) metaDataOC.getBarcodeProperty();
            if( null != barcodeOCP )
            {
                CswNbtView view = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship parent = view.AddViewRelationship( metaDataOC, IncludeDefaultFilters );
                view.AddViewPropertyAndFilter( parent,
                    MetaDataProp : barcodeOCP,
                    Value : Barcode,
                    SubFieldName : CswEnumNbtSubFieldName.Barcode,
                    FilterMode : CswNbtPropFilterSql.PropertyFilterMode.Equals
                );

                if( ObjClass.Equals( CswEnumNbtObjectClass.ContainerClass ) )
                {
                    CswNbtMetaDataObjectClassProp disposedOCP = metaDataOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Disposed );
                    view.AddViewProperty( parent, disposedOCP );

                    CswNbtMetaDataObjectClassProp quantityOCP = metaDataOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );
                    view.AddViewProperty( parent, quantityOCP );
                }

                tree = _CswNbtResources.Trees.getTreeFromView( view, true, false, false );
            }
            return tree;
        }

        #endregion
    }
}
