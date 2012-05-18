using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26217
    /// </summary>
    public class CswUpdateSchemaCase26217 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // fix category on Lab Safety (demo) views
            List<CswNbtView> theViewList = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Problems: Open" );
            if( theViewList.Count > 0 )
            {
                addDateFilter( theViewList[0], "Start Date" );
            }

            List<CswNbtView> theViewList2 = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Tasks: Open" );
            if( theViewList2.Count > 0 )
            {
                addDateFilter( theViewList2[0], "Due Date" );
            }


        } //update()

        void addDateFilter( CswNbtView aView, string propname )
        {
            //add the filter
            foreach( CswNbtViewRelationship arel in aView.Root.ChildRelationships )
            {
                foreach( CswNbtViewProperty aprop in arel.Properties )
                {
                    if( aprop.Name == propname )
                    {
                        CswNbtViewPropertyFilter afilter = aView.AddViewPropertyFilter( aprop, CswNbtSubField.SubFieldName.Value,
                                                       CswNbtPropFilterSql.PropertyFilterMode.GreaterThan, "today-90", false );
                        afilter.ShowAtRuntime = true;
                        aView.save();
                    }
                }
            }
            aView.save();

        }
    }//class CswUpdateSchemaCase26217

}//namespace ChemSW.Nbt.Schema