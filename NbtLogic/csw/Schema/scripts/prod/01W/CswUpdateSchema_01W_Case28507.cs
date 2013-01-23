using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using System.Data;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28507
    /// </summary>
    public class CswUpdateSchema_01W_Case28507 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28507; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass locationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );

            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp locationOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
            CswNbtMetaDataObjectClassProp barcodeOCP = containerOC.getBarcodeProp();
            CswNbtMetaDataObjectClassProp materialOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
            CswNbtMetaDataObjectClassProp ownerOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner );
            CswNbtMetaDataObjectClassProp statusOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status );
            CswNbtMetaDataObjectClassProp expDateOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );
            CswNbtMetaDataObjectClassProp quantOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );

            CswNbtView containersByLocation = _CswNbtSchemaModTrnsctn.makeView();
            containersByLocation.SetViewMode( NbtViewRenderingMode.Grid );
            CswNbtViewRelationship locationParent = containersByLocation.AddViewRelationship( locationOC, true );
            CswNbtViewRelationship containerParent = containersByLocation.AddViewRelationship( locationParent, NbtViewPropOwnerType.Second, locationOCP, true );

            containersByLocation.AddViewProperty( containerParent, barcodeOCP );
            containersByLocation.AddViewProperty( containerParent, materialOCP );
            containersByLocation.AddViewProperty( containerParent, ownerOCP );
            containersByLocation.AddViewProperty( containerParent, expDateOCP );
            containersByLocation.AddViewProperty( containerParent, statusOCP );
            containersByLocation.AddViewProperty( containerParent, quantOCP );

            CswTableUpdate tu = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "fixLocationsContainersView_28507", "node_views" );
            DataTable node_views = tu.getTable( "where viewname = 'Containers' and visibility = 'Property'" );
            foreach( DataRow row in node_views.Rows )
            {
                row["viewxml"] = containersByLocation.ToString();
            }
            tu.update( node_views );


        } //Update()

    }//class CswUpdateSchema_01W_Case28507

}//namespace ChemSW.Nbt.Schema