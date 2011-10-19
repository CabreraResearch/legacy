using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-06
    /// </summary>
    public class CswUpdateSchemaTo01J06 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 06 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            //Case 23888

            CswNbtMetaDataNodeType PiNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Physical Inspection" );
            CswNbtMetaDataNodeType IpNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "FE Inspection Point" );

            if( null != PiNt && null != IpNt )
            {
                List<CswNbtView> PiViews = _CswNbtSchemaModTrnsctn.restoreViews( "Physical Inspections" );
                foreach( CswNbtView PiView in PiViews )
                {
                    if( PiView.ViewMode == NbtViewRenderingMode.Grid && PiView.Visibility != NbtViewVisibility.Property )
                    {
                        PiView.Root.ChildRelationships.Clear();
                        CswNbtMetaDataNodeTypeProp DueDateNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
                        CswNbtMetaDataNodeTypeProp StatusNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );
                        CswNbtMetaDataNodeTypeProp TargetNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                        CswNbtMetaDataNodeTypeProp LocationNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.LocationPropertyName );

                        CswNbtMetaDataNodeTypeProp IpGroupNtp = IpNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );

                        CswNbtViewRelationship PiRel = PiView.AddViewRelationship( PiNt, false );
                        CswNbtViewRelationship IpRel = PiView.AddViewRelationship( PiRel, CswNbtViewRelationship.PropOwnerType.First, TargetNtp, false );

                        CswNbtViewProperty Dp = PiView.AddViewProperty( PiRel, DueDateNtp );
                        Dp.Order = 1;

                        CswNbtViewProperty Sp = PiView.AddViewProperty( PiRel, StatusNtp );
                        Sp.Order = 2;
                        PiView.AddViewPropertyFilter( Sp, StatusNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled ), false );
                        PiView.AddViewPropertyFilter( Sp, StatusNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ), false );
                        PiView.AddViewPropertyFilter( Sp, StatusNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late ), false );
                        PiView.AddViewPropertyFilter( Sp, StatusNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed ), false );

                        CswNbtViewProperty Tp = PiView.AddViewProperty( PiRel, TargetNtp );
                        Tp.Order = 3;
                        PiView.AddViewPropertyFilter( Tp, TargetNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                        CswNbtViewProperty Lp = PiView.AddViewProperty( PiRel, LocationNtp );
                        Lp.Order = 4;
                        PiView.AddViewPropertyFilter( Lp, LocationNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                        CswNbtViewProperty Ip = PiView.AddViewProperty( IpRel, IpGroupNtp );
                        Ip.Order = 5;
                        PiView.AddViewPropertyFilter( Ip, IpGroupNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                        PiView.save();
                    }
                }
            }



        }//Update()

    }//class CswUpdateSchemaTo01J06

}//namespace ChemSW.Nbt.Schema


