using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-42
    /// </summary>
    public class CswUpdateSchemaTo01H50 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 50 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H50( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            CswNbtView OocInspectionsView = _CswNbtSchemaModTrnsctn.makeView();
            OocInspectionsView.makeNew( "OOC Inspections", NbtViewVisibility.Global, null, null, null );
            OocInspectionsView.Category = "Inspections";
            OocInspectionsView.ViewMode = NbtViewRenderingMode.Grid;

            CswNbtMetaDataObjectClass InspectionsOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            foreach( CswNbtMetaDataNodeType InspectionNT in InspectionsOc.NodeTypes )
            {
                Int32 Order = 0;
                CswNbtMetaDataNodeType FirstInspectionNt = InspectionNT.FirstVersionNodeType;
                CswNbtViewRelationship InspectionRel = OocInspectionsView.AddViewRelationship( FirstInspectionNt, false );

                CswNbtMetaDataNodeTypeProp NameNtp = FirstInspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
                CswNbtViewProperty NameVp = OocInspectionsView.AddViewProperty( InspectionRel, NameNtp );
                NameVp.Order = Order++;
                NameVp.Name = "Name";
                OocInspectionsView.AddViewPropertyFilter( NameVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                CswNbtMetaDataNodeTypeProp DateNtp = FirstInspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
                CswNbtViewProperty DateVp = OocInspectionsView.AddViewProperty( InspectionRel, DateNtp );
                DateVp.Order = Order++;
                DateVp.Name = "Due Date";
                OocInspectionsView.AddViewPropertyFilter( DateVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, DateTime.MinValue.ToString(), false );

                CswNbtMetaDataNodeTypeProp LocationNtp = FirstInspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.LocationPropertyName );
                CswNbtViewProperty LocationVp = OocInspectionsView.AddViewProperty( InspectionRel, LocationNtp );
                LocationVp.Order = Order++;
                LocationVp.Name = "Location";
                OocInspectionsView.AddViewPropertyFilter( LocationVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                CswNbtMetaDataNodeTypeProp TargetNtp = FirstInspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                CswNbtViewProperty TargetVp = OocInspectionsView.AddViewProperty( InspectionRel, TargetNtp );
                TargetVp.Order = Order++;
                TargetVp.Name = "Inspection Point";
                OocInspectionsView.AddViewPropertyFilter( TargetVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                string ActionRequired = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Action_Required );

                CswNbtMetaDataNodeTypeProp StatusNtp = FirstInspectionNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );
                CswNbtViewProperty StatusVp = OocInspectionsView.AddViewProperty( InspectionRel, StatusNtp );
                StatusVp.Order = Order++;
                StatusVp.Name = "Status";
                OocInspectionsView.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, ActionRequired, false );
            }

            OocInspectionsView.save();

        } // update()

    }//class CswUpdateSchemaTo01H45

}//namespace ChemSW.Nbt.Schema

