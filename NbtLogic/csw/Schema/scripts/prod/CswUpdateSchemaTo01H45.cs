
using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-45
    /// </summary>
    public class CswUpdateSchemaTo01H45 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 45 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H45( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // Case 21219
            CswNbtView ProblemsOpen = _CswNbtSchemaModTrnsctn.restoreView( "Problems: Open" );
            if( null != ProblemsOpen )
            {
                ProblemsOpen.Clear();
            }
            else
            {
                ProblemsOpen = _CswNbtSchemaModTrnsctn.makeView();
                ProblemsOpen.makeNew( "Problems: Open", NbtViewVisibility.Global, null, null, null );
            }
            ProblemsOpen.Category = "Problems";
            ProblemsOpen.Visibility = NbtViewVisibility.Global;
            ProblemsOpen.ViewMode = NbtViewRenderingMode.Grid;

            CswNbtMetaDataNodeType ProblemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Problem" );
            CswNbtMetaDataNodeType EquipmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment" );
            if( null != ProblemNT )
            {
                CswNbtViewRelationship ProblemRel = ProblemsOpen.AddViewRelationship( ProblemNT, false );
                CswNbtMetaDataNodeTypeProp ClosedNtp = ProblemNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ClosedPropertyName );
                CswNbtMetaDataNodeTypeProp EquipmentNtp = ProblemNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.OwnerPropertyName );
                CswNbtMetaDataNodeTypeProp StartDateNtp = ProblemNT.getNodeTypeProp( "Start Date" );
                CswNbtMetaDataNodeTypeProp SummaryNtp = ProblemNT.getNodeTypeProp( "Summary" );
                CswNbtMetaDataNodeTypeProp TechnicianNtp = ProblemNT.getNodeTypeProp( "Technician" );

                Int32 Order = 0;

                if( null != StartDateNtp )
                {
                    CswNbtViewProperty StartDateVp = ProblemsOpen.AddViewProperty( ProblemRel, StartDateNtp );
                    StartDateVp.Order = Order++;
                    ProblemsOpen.AddViewPropertyFilter( StartDateVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, DateTime.MinValue.ToString(), false );
                }

                CswNbtViewProperty ClosedVp = ProblemsOpen.AddViewProperty( ProblemRel, ClosedNtp );
                ClosedVp.Order = Order++;
                ProblemsOpen.AddViewPropertyFilter( ClosedVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );

                if( null != SummaryNtp )
                {
                    CswNbtViewProperty SummaryVp = ProblemsOpen.AddViewProperty( ProblemRel, SummaryNtp );
                    SummaryVp.Order = Order++;
                    ProblemsOpen.AddViewPropertyFilter( SummaryVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

                if( null != TechnicianNtp )
                {
                    CswNbtViewProperty TechnicianVp = ProblemsOpen.AddViewProperty( ProblemRel, TechnicianNtp );
                    TechnicianVp.Order = Order++;
                    ProblemsOpen.AddViewPropertyFilter( TechnicianVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                }

                CswNbtViewProperty EquipmentVp = ProblemsOpen.AddViewProperty( ProblemRel, EquipmentNtp );
                EquipmentVp.Order = Order++;
                ProblemsOpen.AddViewPropertyFilter( EquipmentVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                if( null != EquipmentNT )
                {
                    CswNbtViewRelationship EquipmentRel = ProblemsOpen.AddViewRelationship( ProblemRel, CswNbtViewRelationship.PropOwnerType.First, EquipmentNtp, false );
                    CswNbtMetaDataNodeTypeProp StatusNtp = EquipmentNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassEquipment.StatusPropertyName );
                    CswNbtMetaDataNodeTypeProp TypeNtp = EquipmentNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassEquipment.TypePropertyName );
                    CswNbtMetaDataNodeTypeProp ManufacturerNtp = EquipmentNT.getNodeTypeProp( "Manufacturer" );
                    CswNbtMetaDataNodeTypeProp SerialNoNtp = EquipmentNT.getNodeTypeProp( "Serial No" );

                    CswNbtViewProperty TypeVp = ProblemsOpen.AddViewProperty( EquipmentRel, TypeNtp );
                    TypeVp.Order = Order++;
                    ProblemsOpen.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                    if( null != SerialNoNtp )
                    {
                        CswNbtViewProperty SerialNoVp = ProblemsOpen.AddViewProperty( EquipmentRel, SerialNoNtp );
                        SerialNoVp.Order = Order++;
                        ProblemsOpen.AddViewPropertyFilter( SerialNoVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                    }

                    if( null != ManufacturerNtp )
                    {
                        CswNbtViewProperty ManufacturerVp = ProblemsOpen.AddViewProperty( EquipmentRel, ManufacturerNtp );
                        ManufacturerVp.Order = Order++;
                        ProblemsOpen.AddViewPropertyFilter( ManufacturerVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                    }

                    CswNbtViewProperty StatusVp = ProblemsOpen.AddViewProperty( EquipmentRel, StatusNtp );
                    StatusVp.Order = Order++;
                    ProblemsOpen.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, "Available", false );
                } // if( null != EquipmentNT )

                ProblemsOpen.save();
            } //if( null != ProblemNT )


            // Case 21221
            CswNbtView TasksOpen = _CswNbtSchemaModTrnsctn.restoreView( "Tasks: Open" );
            if( null != TasksOpen )
            {
                TasksOpen.Clear();
            }
            else
            {
                TasksOpen = _CswNbtSchemaModTrnsctn.makeView();
                TasksOpen.makeNew( "Tasks: Open", NbtViewVisibility.Global, null, null, null );
            }
            TasksOpen.Category = "Tasks";
            TasksOpen.Visibility = NbtViewVisibility.Global;
            TasksOpen.ViewMode = NbtViewRenderingMode.Grid;

            CswNbtMetaDataNodeType TaskNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Task" );
            if( null != TaskNT )
            {
                CswNbtViewRelationship TaskRel = TasksOpen.AddViewRelationship( TaskNT, false );
                CswNbtMetaDataNodeTypeProp CompletedNtp = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.CompletedPropertyName );
                CswNbtMetaDataNodeTypeProp EquipmentNtp = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.OwnerPropertyName );
                CswNbtMetaDataNodeTypeProp DueDateNtp = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.DueDatePropertyName );
                CswNbtMetaDataNodeTypeProp SummaryNtp = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.SummaryPropertyName );
                CswNbtMetaDataNodeTypeProp TechnicianNtp = TaskNT.getNodeTypeProp( "Technician" );

                Int32 Order = 0;

                CswNbtViewProperty DueDateVp = TasksOpen.AddViewProperty( TaskRel, DueDateNtp );
                DueDateVp.Order = Order++;
                TasksOpen.AddViewPropertyFilter( DueDateVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, DateTime.MinValue.ToString(), false );

                CswNbtViewProperty CompletedVp = TasksOpen.AddViewProperty( TaskRel, CompletedNtp );
                CompletedVp.Order = Order++;
                TasksOpen.AddViewPropertyFilter( CompletedVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );

                CswNbtViewProperty SummaryVp = TasksOpen.AddViewProperty( TaskRel, SummaryNtp );
                SummaryVp.Order = Order++;
                TasksOpen.AddViewPropertyFilter( SummaryVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                if( null != TechnicianNtp )
                {
                    CswNbtViewProperty TechnicianVp = TasksOpen.AddViewProperty( TaskRel, TechnicianNtp );
                    TechnicianVp.Order = Order++;
                    TasksOpen.AddViewPropertyFilter( TechnicianVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

                CswNbtViewProperty EquipmentVp = TasksOpen.AddViewProperty( TaskRel, EquipmentNtp );
                EquipmentVp.Order = Order++;
                TasksOpen.AddViewPropertyFilter( EquipmentVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                if( null != EquipmentNT )
                {
                    CswNbtViewRelationship EquipmentRel = TasksOpen.AddViewRelationship( TaskRel, CswNbtViewRelationship.PropOwnerType.First, EquipmentNtp, false );
                    CswNbtMetaDataNodeTypeProp StatusNtp = EquipmentNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassEquipment.StatusPropertyName );
                    CswNbtMetaDataNodeTypeProp TypeNtp = EquipmentNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassEquipment.TypePropertyName );
                    CswNbtMetaDataNodeTypeProp SerialNoNtp = EquipmentNT.getNodeTypeProp( "Serial No" );

                    CswNbtViewProperty TypeVp = TasksOpen.AddViewProperty( EquipmentRel, TypeNtp );
                    TypeVp.Order = Order++;
                    TasksOpen.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                    if( null != SerialNoNtp )
                    {
                        CswNbtViewProperty SerialNoVp = TasksOpen.AddViewProperty( EquipmentRel, SerialNoNtp );
                        SerialNoVp.Order = Order++;
                        TasksOpen.AddViewPropertyFilter( SerialNoVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                    }

                    CswNbtViewProperty StatusVp = TasksOpen.AddViewProperty( EquipmentRel, StatusNtp );
                    StatusVp.Order = Order++;
                    TasksOpen.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, "Retired", false );
                } // if( null != EquipmentNT )

                TasksOpen.save();
            } //if( null != ProblemNT )

            // Case 21855
            CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp NameOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.NamePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

        } // update()

    }//class CswUpdateSchemaTo01H45

}//namespace ChemSW.Nbt.Schema

