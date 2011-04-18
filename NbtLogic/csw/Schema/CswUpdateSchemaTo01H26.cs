using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-26
    /// </summary>
    public class CswUpdateSchemaTo01H26 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 26 ); } }
        public CswUpdateSchemaTo01H26( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // case 20871
            List<CswNbtView> AllMountPoints = _CswNbtSchemaModTrnsctn.restoreViews( "All FE Inspection Points" );
            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.FE_Inspection_Point ) );
            if( null != MountPointNT )
            {
                CswNbtMetaDataNodeTypeProp BarcodeNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
                CswNbtMetaDataNodeTypeProp DescriptionNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
                CswNbtMetaDataNodeTypeProp StatusNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.StatusPropertyName );
                CswNbtMetaDataNodeTypeProp TypeNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.TypePropertyName );
                CswNbtMetaDataNodeTypeProp LocationNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
                CswNbtMetaDataNodeTypeProp MountPointGroupNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                foreach( CswNbtView View in AllMountPoints )
                {
                    View.Root.ChildRelationships.Clear();
                    CswNbtViewRelationship MountPointVr = View.AddViewRelationship( MountPointNT, false );
                    CswNbtViewProperty BarcodeVp = View.AddViewProperty( MountPointVr, BarcodeNtp );
                    View.AddViewPropertyFilter( BarcodeVp, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

                    CswNbtViewProperty DescriptionVp = View.AddViewProperty( MountPointVr, DescriptionNtp );
                    View.AddViewPropertyFilter( DescriptionVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

                    CswNbtViewProperty StatusVp = View.AddViewProperty( MountPointVr, StatusNtp );
                    View.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                    CswNbtViewProperty TypeVp = View.AddViewProperty( MountPointVr, TypeNtp );
                    View.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                    CswNbtViewProperty LocationVp = View.AddViewProperty( MountPointVr, LocationNtp );
                    View.AddViewPropertyFilter( LocationVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

                    CswNbtViewProperty MountPointGroupVp = View.AddViewProperty( MountPointVr, MountPointGroupNtp );
                    View.AddViewPropertyFilter( MountPointGroupVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );
                    View.save();
                }
            }

            // case 20924
            CswNbtView MyProblems = _CswNbtSchemaModTrnsctn.restoreView( "My Problems" );
            if( null != MyProblems )
            {
                MyProblems.Root.ChildRelationships.Clear();

                CswNbtMetaDataObjectClass ProblemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass );
                CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
                CswNbtMetaDataObjectClass AssemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );
                foreach( CswNbtMetaDataNodeType Problem in ProblemOC.NodeTypes )
                {
                    CswNbtMetaDataNodeTypeProp ReportedByNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ReportedByPropertyName );
                    CswNbtMetaDataNodeTypeProp ClosedNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ClosedPropertyName );
                    CswNbtMetaDataNodeTypeProp DateOpenedNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.DateOpenedPropertyName );
                    //CswNbtMetaDataNodeTypeProp OwnerNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.OwnerPropertyName );
                    CswNbtMetaDataNodeTypeProp ParentRelationshipNtp = null;
                    foreach( CswNbtMetaDataNodeType Equip in EquipmentOC.NodeTypes )
                    {
                       if( null != Problem.NodeTypeProps.)
                    }
                    //CswNbtMetaDataNodeTypeProp ReportedByNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ReportedByPropertyName );
                    //CswNbtMetaDataNodeTypeProp ReportedByNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ReportedByPropertyName );
                }

            }


            //No else, just leave the OC-based views
        } // update()

    }//class CswUpdateSchemaTo01H26

}//namespace ChemSW.Nbt.Schema

