using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27513
    /// </summary>
    public class CswUpdateSchemaCase27513 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //get all views and find the dispense requests view
            DataTable Views = _CswNbtSchemaModTrnsctn.ViewSelect.getAllViews();
            CswNbtViewId dispenseRequestsViewId = new CswNbtViewId();
            foreach( DataRow row in Views.Rows )
            {
                if( row["viewname"].Equals( "Dispense Requests: Open" ) )
                {
                    dispenseRequestsViewId.set( CswConvert.ToInt32( row["nodeviewid"] ) );
                }
            }

            /*
             * The dispense requests view is broken. It has THREE 'Status' properties, each with different filters
             */

            if( dispenseRequestsViewId.isSet() )
            {
                CswNbtView DispenseRequestsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( dispenseRequestsViewId );
                LinkedList<CswNbtViewProperty> viewProps = DispenseRequestsView.getOrderedViewProps( false );
                CswNbtViewRelationship parent = null;
                foreach( CswNbtViewProperty viewProp in viewProps ) //delete the 3 duplicate 'Status' view props from this view
                {
                    if( viewProp.Name.Equals( "Status" ) )
                    {
                        DispenseRequestsView.removeViewProperty( viewProp.MetaDataProp );
                    }
                    else
                    {
                        parent = (CswNbtViewRelationship) viewProp.Parent; //get the view parent to add the Status prop later (in this specific case this get the root)
                    }
                }

                //add the property to the view ONCE
                CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
                CswNbtViewProperty statusVP = DispenseRequestsView.AddViewProperty( parent, requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status ) );

                //now add the filters
                DispenseRequestsView.AddViewPropertyFilter( statusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: "Completed" );
                DispenseRequestsView.AddViewPropertyFilter( statusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: "Cancelled" );

                DispenseRequestsView.save();
            }

        }//Update()

    }//class CswUpdateSchemaCase27513

}//namespace ChemSW.Nbt.Schema