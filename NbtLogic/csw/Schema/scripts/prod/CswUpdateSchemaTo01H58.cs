using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-58
    /// </summary>
    public class CswUpdateSchemaTo01H58 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 58 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H58( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            _CswNbtSchemaModTrnsctn.deleteView( "Inspections Due This Week", true );
            CswNbtMetaDataObjectClass InspectionsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            Collection<CswNbtMetaDataNodeType> InspectionNTs = new Collection<CswNbtMetaDataNodeType>();
            foreach( CswNbtMetaDataNodeType InspectionNT in InspectionsOC.NodeTypes
                                                                         .Cast<CswNbtMetaDataNodeType>()
                                                                         .Where( InspectionNT => InspectionNT.IsLatestVersion ) )
            {
                InspectionNTs.Add( InspectionNT );
            }

            CswNbtView InspectionsDueWithinSeven = _CswNbtSchemaModTrnsctn.makeView();
            InspectionsDueWithinSeven.makeNew( "Inspections Due Within 7 Days", NbtViewVisibility.Global, null, null, null );
            InspectionsDueWithinSeven.Category = "Inspections";
            InspectionsDueWithinSeven.ForMobile = true;

            CswNbtView InspectionsDueToday = _CswNbtSchemaModTrnsctn.makeView();
            InspectionsDueToday.makeNew( "Inspections Due Today", NbtViewVisibility.Global, null, null, null );
            InspectionsDueToday.Category = "Inspections";
            InspectionsDueToday.ForMobile = true;

            CswNbtView InspectionsDueTomorrow = _CswNbtSchemaModTrnsctn.makeView();
            InspectionsDueTomorrow.makeNew( "Inspections Due Tomorrow", NbtViewVisibility.Global, null, null, null );
            InspectionsDueTomorrow.Category = "Inspections";
            InspectionsDueTomorrow.ForMobile = true;

            foreach( CswNbtMetaDataNodeType InspectionNt in InspectionNTs )
            {
                CswNbtViewRelationship InspectionSevenRel = InspectionsDueWithinSeven.AddViewRelationship( InspectionNt, false );
                CswNbtViewRelationship InspectionTodayRel = InspectionsDueToday.AddViewRelationship( InspectionNt, false );
                CswNbtViewRelationship InspectionTomorrowRel = InspectionsDueTomorrow.AddViewRelationship( InspectionNt, false );

                Int32 Order = 0;

                CswNbtMetaDataNodeTypeProp DueDateNtp = InspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
                if( DueDateNtp != null ) //this is aesthetic, it won't be null
                {
                    Order++;
                    CswNbtViewProperty DueDateSevenVp = InspectionsDueWithinSeven.AddViewProperty( InspectionSevenRel, DueDateNtp );
                    DueDateSevenVp.Order = Order;
                    DueDateSevenVp.Name = "Due Date";
                    InspectionsDueWithinSeven.AddViewPropertyFilter( DueDateSevenVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, "today+7", false );

                    CswNbtViewProperty DueDateTodayVp = InspectionsDueToday.AddViewProperty( InspectionTodayRel, DueDateNtp );
                    DueDateTodayVp.Order = Order;
                    DueDateTodayVp.Name = "Due Date";
                    InspectionsDueToday.AddViewPropertyFilter( DueDateTodayVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, "today+0", false );

                    CswNbtViewProperty DueDateTomorrowVp = InspectionsDueTomorrow.AddViewProperty( InspectionTomorrowRel, DueDateNtp );
                    DueDateTomorrowVp.Order = Order;
                    DueDateTomorrowVp.Name = "Due Date";
                    InspectionsDueTomorrow.AddViewPropertyFilter( DueDateTomorrowVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, "today+1", false );
                }

                CswNbtMetaDataNodeTypeProp LocationNtp = InspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.LocationPropertyName );
                if( LocationNtp != null ) //this is aesthetic, it won't be null
                {
                    Order++;
                    CswNbtViewProperty LocationSevenVp = InspectionsDueWithinSeven.AddViewProperty( InspectionSevenRel, LocationNtp );
                    LocationSevenVp.Order = Order;
                    LocationSevenVp.Name = "Location";
                    InspectionsDueWithinSeven.AddViewPropertyFilter( LocationSevenVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty LocationTodayVp = InspectionsDueToday.AddViewProperty( InspectionTodayRel, LocationNtp );
                    LocationTodayVp.Order = Order;
                    LocationTodayVp.Name = "Location";
                    InspectionsDueToday.AddViewPropertyFilter( LocationTodayVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty LocationTomorrowVp = InspectionsDueTomorrow.AddViewProperty( InspectionTomorrowRel, LocationNtp );
                    LocationTomorrowVp.Order = Order;
                    LocationTomorrowVp.Name = "Location";
                    InspectionsDueTomorrow.AddViewPropertyFilter( LocationTomorrowVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

                CswNbtMetaDataNodeTypeProp BarcodeNtp = InspectionNt.BarcodeProperty;
                if( null != BarcodeNtp ) //might be null
                {
                    Order++;
                    CswNbtViewProperty BarcodeSevenVp = InspectionsDueWithinSeven.AddViewProperty( InspectionSevenRel, BarcodeNtp );
                    BarcodeSevenVp.Order = Order;
                    BarcodeSevenVp.Name = "Barcode";
                    InspectionsDueWithinSeven.AddViewPropertyFilter( BarcodeSevenVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty BarcodeTodayVp = InspectionsDueToday.AddViewProperty( InspectionTodayRel, BarcodeNtp );
                    BarcodeTodayVp.Order = Order;
                    BarcodeTodayVp.Name = "Barcode";
                    InspectionsDueToday.AddViewPropertyFilter( BarcodeTodayVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty BarcodeTomorrowVp = InspectionsDueTomorrow.AddViewProperty( InspectionTomorrowRel, BarcodeNtp );
                    BarcodeTomorrowVp.Order = Order;
                    BarcodeTomorrowVp.Name = "Barcode";
                    InspectionsDueTomorrow.AddViewPropertyFilter( BarcodeTomorrowVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

                CswNbtMetaDataNodeTypeProp FinishedNtp = InspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.FinishedPropertyName );
                if( FinishedNtp != null ) //this is aesthetic, it won't be null
                {
                    Order++;
                    CswNbtViewProperty FinishedSevenVp = InspectionsDueWithinSeven.AddViewProperty( InspectionSevenRel, FinishedNtp );
                    FinishedSevenVp.Order = Order;
                    FinishedSevenVp.Name = "Finished";
                    InspectionsDueWithinSeven.AddViewPropertyFilter( FinishedSevenVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );

                    CswNbtViewProperty FinishedTodayVp = InspectionsDueToday.AddViewProperty( InspectionTodayRel, FinishedNtp );
                    FinishedTodayVp.Order = Order;
                    FinishedTodayVp.Name = "Finished";
                    InspectionsDueToday.AddViewPropertyFilter( FinishedTodayVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );

                    CswNbtViewProperty FinishedTomorrowVp = InspectionsDueTomorrow.AddViewProperty( InspectionTomorrowRel, FinishedNtp );
                    FinishedTomorrowVp.Order = Order;
                    FinishedTomorrowVp.Name = "Finished";
                    InspectionsDueTomorrow.AddViewPropertyFilter( FinishedTomorrowVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );
                }

                CswNbtMetaDataNodeTypeProp GeneratorNtp = InspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
                if( GeneratorNtp != null ) //this is aesthetic, it won't be null
                {
                    Order++;
                    CswNbtViewProperty GeneratorSevenVp = InspectionsDueWithinSeven.AddViewProperty( InspectionSevenRel, GeneratorNtp );
                    GeneratorSevenVp.Order = Order;
                    GeneratorSevenVp.Name = "Inspection Schedule";
                    InspectionsDueWithinSeven.AddViewPropertyFilter( GeneratorSevenVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty GeneratorTodayVp = InspectionsDueToday.AddViewProperty( InspectionTodayRel, GeneratorNtp );
                    GeneratorTodayVp.Order = Order;
                    GeneratorTodayVp.Name = "Inspection Schedule";
                    InspectionsDueToday.AddViewPropertyFilter( GeneratorTodayVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty GeneratorTomorrowVp = InspectionsDueTomorrow.AddViewProperty( InspectionTomorrowRel, GeneratorNtp );
                    GeneratorTomorrowVp.Order = Order;
                    GeneratorTomorrowVp.Name = "Inspection Schedule";
                    InspectionsDueTomorrow.AddViewPropertyFilter( GeneratorTomorrowVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

                CswNbtMetaDataNodeTypeProp NameNtp = InspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
                if( NameNtp != null ) //this is aesthetic, it won't be null
                {
                    Order++;
                    CswNbtViewProperty NameSevenVp = InspectionsDueWithinSeven.AddViewProperty( InspectionSevenRel, NameNtp );
                    NameSevenVp.Order = Order;
                    NameSevenVp.Name = "Name";
                    InspectionsDueWithinSeven.AddViewPropertyFilter( NameSevenVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty NameTodayVp = InspectionsDueToday.AddViewProperty( InspectionTodayRel, NameNtp );
                    NameTodayVp.Order = Order;
                    NameTodayVp.Name = "Name";
                    InspectionsDueToday.AddViewPropertyFilter( NameTodayVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty NameTomorrowVp = InspectionsDueTomorrow.AddViewProperty( InspectionTomorrowRel, NameNtp );
                    NameTomorrowVp.Order = Order;
                    NameTomorrowVp.Name = "Name";
                    InspectionsDueTomorrow.AddViewPropertyFilter( NameTomorrowVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

                CswNbtMetaDataNodeTypeProp StatusNtp = InspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );
                if( StatusNtp != null ) //this is aesthetic, it won't be null
                {
                    Order++;
                    CswNbtViewProperty StatusSevenVp = InspectionsDueWithinSeven.AddViewProperty( InspectionSevenRel, StatusNtp );
                    StatusSevenVp.Order = Order;
                    StatusSevenVp.Name = "Status";
                    InspectionsDueWithinSeven.AddViewPropertyFilter( StatusSevenVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed ), false );
                    InspectionsDueWithinSeven.AddViewPropertyFilter( StatusSevenVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ), false );
                    InspectionsDueWithinSeven.AddViewPropertyFilter( StatusSevenVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late ), false );
                    InspectionsDueWithinSeven.AddViewPropertyFilter( StatusSevenVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled ), false );

                    CswNbtViewProperty StatusTodayVp = InspectionsDueToday.AddViewProperty( InspectionTodayRel, StatusNtp );
                    StatusTodayVp.Order = Order;
                    StatusTodayVp.Name = "Status";
                    InspectionsDueToday.AddViewPropertyFilter( StatusTodayVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed ), false );
                    InspectionsDueToday.AddViewPropertyFilter( StatusTodayVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ), false );
                    InspectionsDueToday.AddViewPropertyFilter( StatusTodayVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late ), false );
                    InspectionsDueToday.AddViewPropertyFilter( StatusTodayVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled ), false );

                    CswNbtViewProperty StatusTomorrowVp = InspectionsDueTomorrow.AddViewProperty( InspectionTomorrowRel, StatusNtp );
                    StatusTomorrowVp.Order = Order;
                    StatusTomorrowVp.Name = "Status";
                    InspectionsDueTomorrow.AddViewPropertyFilter( StatusTomorrowVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed ), false );
                    InspectionsDueTomorrow.AddViewPropertyFilter( StatusTomorrowVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ), false );
                    InspectionsDueTomorrow.AddViewPropertyFilter( StatusTomorrowVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late ), false );
                    InspectionsDueTomorrow.AddViewPropertyFilter( StatusTomorrowVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled ), false );
                }

                CswNbtMetaDataNodeTypeProp TargetNtp = InspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                if( TargetNtp != null ) //this is aesthetic, it won't be null
                {
                    Order++;
                    CswNbtViewProperty TargetSevenVp = InspectionsDueWithinSeven.AddViewProperty( InspectionSevenRel, TargetNtp );
                    TargetSevenVp.Order = Order;
                    TargetSevenVp.Name = "Target";
                    InspectionsDueWithinSeven.AddViewPropertyFilter( TargetSevenVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty TargetTodayVp = InspectionsDueToday.AddViewProperty( InspectionTodayRel, TargetNtp );
                    TargetTodayVp.Order = Order;
                    TargetTodayVp.Name = "Target";
                    InspectionsDueToday.AddViewPropertyFilter( TargetTodayVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                    CswNbtViewProperty TargetTomorrowVp = InspectionsDueTomorrow.AddViewProperty( InspectionTomorrowRel, TargetNtp );
                    TargetTomorrowVp.Order = Order;
                    TargetTomorrowVp.Name = "Target";
                    InspectionsDueTomorrow.AddViewPropertyFilter( TargetTomorrowVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

            } //foreach( CswNbtMetaDataNodeType InspectionNt in InspectionsOC.NodeTypes )

            InspectionsDueToday.save();
            InspectionsDueTomorrow.save();
            InspectionsDueWithinSeven.save();

        } // update()


    }//class CswUpdateSchemaTo01H58

}//namespace ChemSW.Nbt.Schema

