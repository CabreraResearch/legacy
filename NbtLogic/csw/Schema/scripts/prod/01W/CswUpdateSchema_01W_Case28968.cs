using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28968
    /// </summary>
    public class CswUpdateSchema_01W_Case28968 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28968; }
        }

        public override void update()
        {

            //Make the Container.Missing OCP default to false and set IsRequired = true
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp missingOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Missing );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( missingOCP, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( missingOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            //set all existing containers.missing to false if null
            foreach( CswNbtObjClassContainer containerNode in containerOC.getNodes( false, false, false, true ) )
            {
                if( Tristate.Null == containerNode.Missing.Checked )
                {
                    containerNode.Missing.Checked = Tristate.False;
                    containerNode.postChanges( false );
                }
            }

            //Update the Missing Containers View to have a filter Missing = True
            CswNbtView missingContainersView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( "Missing Containers", NbtViewVisibility.Global );

            if( null != missingContainersView )
            {
                CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClassProp tradeNameOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Tradename );
                CswNbtMetaDataObjectClassProp partNoOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.PartNumber );
                CswNbtMetaDataObjectClassProp supplierOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );

                CswNbtMetaDataObjectClassProp disposedOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Disposed );
                CswNbtMetaDataObjectClassProp barcodeOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );
                CswNbtMetaDataObjectClassProp expDateOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );
                CswNbtMetaDataObjectClassProp quantityOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );
                CswNbtMetaDataObjectClassProp locationOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
                CswNbtMetaDataObjectClassProp materialOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );

                missingContainersView.Clear();
                missingContainersView.SetViewMode( NbtViewRenderingMode.Grid );
                missingContainersView.Category = "Containers";
                missingContainersView.ViewName = "Missing Containers";
                missingContainersView.Visibility = NbtViewVisibility.Global;

                CswNbtViewRelationship containerParent = missingContainersView.AddViewRelationship( containerOC, true );
                missingContainersView.AddViewPropertyAndFilter( containerParent,
                    MetaDataProp: missingOCP,
                    Value: true.ToString(),
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                    ShowInGrid: false );

                missingContainersView.AddViewPropertyAndFilter( containerParent,
                    MetaDataProp: disposedOCP,
                    Value: false.ToString(),
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                    ShowInGrid: false );

                missingContainersView.AddViewProperty( containerParent, barcodeOCP );
                missingContainersView.AddViewProperty( containerParent, expDateOCP );
                missingContainersView.AddViewProperty( containerParent, quantityOCP );
                missingContainersView.AddViewProperty( containerParent, locationOCP );

                CswNbtViewRelationship materialParent = missingContainersView.AddViewRelationship( containerParent, NbtViewPropOwnerType.First, materialOCP, true );
                missingContainersView.AddViewProperty( materialParent, tradeNameOCP );
                missingContainersView.AddViewProperty( materialParent, partNoOCP );
                missingContainersView.AddViewProperty( materialParent, supplierOCP );

                missingContainersView.save();
            }


        } //Update()

    }//class CswUpdateSchema_01V_Case28968

}//namespace ChemSW.Nbt.Schema