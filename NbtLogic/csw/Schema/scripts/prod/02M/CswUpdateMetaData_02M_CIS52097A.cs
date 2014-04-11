using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52097A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52097; }
        }

        public override string Title
        {
            get { return "Adding a grid property named containers to the Receipt Lot OC" + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "AB";
        }

        public override void update()
        {
            
            const string ViewXml = "RelationshipView|<TreeView viewname='Containers' version='1.0' iconfilename='Images/view/viewgrid.gif' selectable='true' mode='Grid' width='100' viewid='3506' category='Containers' visibility='Property' visibilityroleid='' visibilityuserid='' groupbysiblings='false' included='true' isdemo='false' issystem='false' gridgroupbycol=''><Relationship secondname='LocationClass' secondtype='ObjectClassId' secondid='5' secondiconfilename='world.png' selectable='true' arbitraryid='root_OC_5' showintree='true' allowaddchildren='True' allowview='True' allowedit='True' allowdelete='True' nodeidstofilterin='' nodeidstofilterout=''><Relationship propid='1388' propname='Location' proptype='ObjectClassPropId' propowner='Second' firstname='LocationClass' firsttype='ObjectClassId' firstid='5' secondname='ContainerClass' secondtype='ObjectClassId' secondid='148' secondiconfilename='barcode.png' selectable='true' arbitraryid='root_OC_5_OC_1481388' showintree='true' allowaddchildren='True' allowview='True' allowedit='True' allowdelete='True' nodeidstofilterin='' nodeidstofilterout=''><Property type='ObjectClassPropId' nodetypepropid='-2147483648' objectclasspropid='1392' name='Disposed' arbitraryid='root_OC_5_OC_1481388_OCP_1392' sortby='False' sortmethod='Ascending' fieldtype='Logical' order='' width='' showingrid='False'><Filter value='False' filtermode='Equals' casesensitive='False' showatruntime='True' arbitraryid='root_OC_5_OC_1481388_OCP_1392_Checked_Equals_False' subfieldname='Checked' resultmode='Hide' conjunction='And' /></Property><Property type='ObjectClassPropId' nodetypepropid='-2147483648' objectclasspropid='1385' name='Barcode' arbitraryid='root_OC_5_OC_1481388_OCP_1385' sortby='False' sortmethod='Ascending' fieldtype='Barcode' order='' width='' showingrid='True' /><Property type='ObjectClassPropId' nodetypepropid='-2147483648' objectclasspropid='1384' name='Material' arbitraryid='root_OC_5_OC_1481388_OCP_1384' sortby='False' sortmethod='Ascending' fieldtype='Relationship' order='' width='' showingrid='True' /><Property type='ObjectClassPropId' nodetypepropid='-2147483648' objectclasspropid='1520' name='Owner' arbitraryid='root_OC_5_OC_1481388_OCP_1520' sortby='False' sortmethod='Ascending' fieldtype='Relationship' order='' width='' showingrid='True' /><Property type='ObjectClassPropId' nodetypepropid='-2147483648' objectclasspropid='1393' name='Expiration Date' arbitraryid='root_OC_5_OC_1481388_OCP_1393' sortby='False' sortmethod='Ascending' fieldtype='DateTime' order='' width='' showingrid='True' /><Property type='ObjectClassPropId' nodetypepropid='-2147483648' objectclasspropid='1386' name='Status' arbitraryid='root_OC_5_OC_1481388_OCP_1386' sortby='False' sortmethod='Ascending' fieldtype='List' order='' width='' showingrid='True' /><Property type='ObjectClassPropId' nodetypepropid='-2147483648' objectclasspropid='1387' name='Quantity' arbitraryid='root_OC_5_OC_1481388_OCP_1387' sortby='False' sortmethod='Ascending' fieldtype='Quantity' order='' width='' showingrid='True' /></Relationship></Relationship></TreeView>";

            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass);

            CswNbtMetaDataObjectClassProp ContainerNTP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ReceiptLotOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.Containers,
                    FieldType = CswEnumNbtFieldType.Grid,
                    ViewXml = ViewXml
                } );

            CswNbtView ContainersView = _CswNbtSchemaModTrnsctn.makeView();
            ContainersView.SetViewMode( CswEnumNbtViewRenderingMode.Grid );

            CswNbtViewRelationship ReceiptLotRel = ContainersView.AddViewRelationship( ReceiptLotOC, false);
            CswNbtViewRelationship ContainerRel = ContainersView.AddViewRelationship( ReceiptLotRel, CswEnumNbtViewPropOwnerType.Second, ContainerNTP, true);

            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode ), 1 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material), 2 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner), 3 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate), 4 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status), 5 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity), 6);

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ContainerNTP, CswEnumNbtObjectClassPropAttributes.viewxml, ContainersView.ToString() );
        }
    }
}


