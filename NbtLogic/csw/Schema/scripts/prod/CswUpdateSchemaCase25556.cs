using System;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25556
    /// </summary>
    public class CswUpdateSchemaCase25556 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // New View: Due Inspections (all)
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            CswNbtMetaDataObjectClassProp DateOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.DatePropertyName );
            CswNbtMetaDataObjectClassProp StatusOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );
            CswNbtMetaDataObjectClassProp NameOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.NamePropertyName );
            CswNbtMetaDataObjectClassProp TargetOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );
            CswNbtMetaDataObjectClassProp LocationOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.LocationPropertyName );

            string DueInspViewName = "Due Inspections (all)";

            CswNbtView DueInspView = _CswNbtSchemaModTrnsctn.restoreView( DueInspViewName );
            if( DueInspView == null )
            {
                DueInspView = _CswNbtSchemaModTrnsctn.makeView();
                DueInspView.makeNew( DueInspViewName, NbtViewVisibility.Global, null, null, null );
            }

            DueInspView.Root.ChildRelationships.Clear();
            DueInspView.ViewMode = NbtViewRenderingMode.Grid;
            DueInspView.Category = "SI_Example";

            CswNbtViewRelationship InspDesignViewRel = DueInspView.AddViewRelationship( InspectionDesignOC, true );

            CswNbtViewProperty DateViewProp = DueInspView.AddViewProperty( InspDesignViewRel, DateOCP );
            CswNbtViewProperty StatusViewProp = DueInspView.AddViewProperty( InspDesignViewRel, StatusOCP );
            CswNbtViewProperty NameViewProp = DueInspView.AddViewProperty( InspDesignViewRel, NameOCP );
            CswNbtViewProperty TargetViewProp = DueInspView.AddViewProperty( InspDesignViewRel, TargetOCP );
            CswNbtViewProperty LocationViewProp = DueInspView.AddViewProperty( InspDesignViewRel, LocationOCP );

            DueInspView.AddViewPropertyFilter( DateViewProp, DateOCP.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, "today+7", false );

            DueInspView.AddViewPropertyFilter( StatusViewProp, DateOCP.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled ), false );
            DueInspView.AddViewPropertyFilter( StatusViewProp, DateOCP.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ), false );
            DueInspView.AddViewPropertyFilter( StatusViewProp, DateOCP.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late ), false );
            DueInspView.AddViewPropertyFilter( StatusViewProp, DateOCP.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed ), false );

            DueInspView.save();

        }//Update()

    }//class CswUpdateSchemaCase25556

}//namespace ChemSW.Nbt.Schema