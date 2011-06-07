
using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-46
    /// </summary>
    public class CswUpdateSchemaTo01H46 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 46 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H46( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            //            CswNbtMetaDataNodeType EquipmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment" );
            //            CswNbtMetaDataNodeType AssemblyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly" );

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

            CswNbtMetaDataObjectClass ProblemsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass );

            foreach( CswNbtMetaDataNodeType ProblemNT in ProblemsOC.NodeTypes )
            {
                CswNbtViewRelationship ProblemRel = ProblemsOpen.AddViewRelationship( ProblemNT, false );
                CswNbtMetaDataNodeTypeProp ClosedNtp = ProblemNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ClosedPropertyName );
                CswNbtMetaDataNodeTypeProp OwnerNtp = ProblemNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.OwnerPropertyName );
                CswNbtMetaDataNodeTypeProp StartDateNtp = ProblemNT.getNodeTypeProp( "Start Date" );
                CswNbtMetaDataNodeTypeProp SummaryNtp = ProblemNT.getNodeTypeProp( "Summary" );
                CswNbtMetaDataNodeTypeProp TechnicianNtp = ProblemNT.getNodeTypeProp( "Technician" );

                Int32 Order = 0;

                if( null != StartDateNtp )
                {
                    CswNbtViewProperty StartDateVp = ProblemsOpen.AddViewProperty( ProblemRel, StartDateNtp );
                    StartDateVp.Order = Order++;
                    //ProblemsOpen.AddViewPropertyFilter( StartDateVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, DateTime.MinValue.ToString(), false );
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

                CswNbtViewProperty EquipmentVp = ProblemsOpen.AddViewProperty( ProblemRel, OwnerNtp );
                EquipmentVp.Order = Order++;
                ProblemsOpen.AddViewPropertyFilter( EquipmentVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                CswNbtViewRelationship EquipmentRel = ProblemsOpen.AddViewRelationship( ProblemRel,
                                                                                        CswNbtViewRelationship.PropOwnerType.First,
                                                                                        OwnerNtp,
                                                                                        false );

                Int32 TargetId = OwnerNtp.DefaultValue.AsRelationship.TargetId;
                if( Int32.MinValue != TargetId )
                {
                    switch( OwnerNtp.DefaultValue.AsRelationship.TargetType )
                    {
                        case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                            {
                                CswNbtMetaDataNodeType OwnerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( TargetId );

                                CswNbtMetaDataNodeTypeProp TypeNtp = OwnerNT.getNodeTypeProp( "Type" ) ?? OwnerNT.getNodeTypeProp( "Assembly Type" );
                                if( null != TypeNtp )
                                {
                                    CswNbtViewProperty TypeVp = ProblemsOpen.AddViewProperty( EquipmentRel, TypeNtp );
                                    TypeVp.Name = "Type";
                                    TypeVp.Order = Order++;
                                    ProblemsOpen.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                                }

                                CswNbtMetaDataNodeTypeProp SerialNtp = OwnerNT.getNodeTypeProp( "Serial No" ) ?? OwnerNT.getNodeTypeProp( "Assembly Serial No" );
                                if( null != SerialNtp )
                                {
                                    CswNbtViewProperty SerialNoVp = ProblemsOpen.AddViewProperty( EquipmentRel, SerialNtp );
                                    SerialNoVp.Name = "Serial No";
                                    SerialNoVp.Order = Order++;
                                    ProblemsOpen.AddViewPropertyFilter( SerialNoVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                                }

                                CswNbtMetaDataNodeTypeProp ManufacturerNtp = OwnerNT.getNodeTypeProp( "Manufacturer" ) ?? OwnerNT.getNodeTypeProp( "Assembly Manufacturer" );
                                if( null != ManufacturerNtp )
                                {
                                    CswNbtViewProperty ManufacturerVp = ProblemsOpen.AddViewProperty( EquipmentRel, ManufacturerNtp );
                                    ManufacturerVp.Name = "Manufacturer";
                                    ManufacturerVp.Order = Order++;
                                    ProblemsOpen.AddViewPropertyFilter( ManufacturerVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                                }

                                CswNbtMetaDataNodeTypeProp StatusNtp = OwnerNT.getNodeTypeProp( "Status" );
                                if( null != StatusNtp )
                                {
                                    CswNbtViewProperty StatusVp = ProblemsOpen.AddViewProperty( EquipmentRel, StatusNtp );
                                    StatusVp.Name = "Status";
                                    StatusVp.Order = Order++;
                                    ProblemsOpen.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, "Available", false );
                                }
                                break;
                            } // case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                        case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                            {
                                CswNbtMetaDataObjectClass OwnerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( TargetId );

                                CswNbtViewProperty TypeVp = null;
                                CswNbtViewProperty SerialNoVp = null;
                                CswNbtViewProperty ManufacturerVp = null;
                                CswNbtViewProperty StatusVp = null;

                                foreach( CswNbtMetaDataNodeType OwnerNT in OwnerOC.NodeTypes )
                                {
                                    if( null == TypeVp )
                                    {
                                        CswNbtMetaDataNodeTypeProp TypeNtp = OwnerNT.getNodeTypeProp( "Type" ) ?? OwnerNT.getNodeTypeProp( "Assembly Type" );
                                        if( null != TypeNtp )
                                        {
                                            TypeVp = ProblemsOpen.AddViewProperty( EquipmentRel, TypeNtp );
                                            TypeVp.Name = "Type";
                                            TypeVp.Order = Order++;
                                            ProblemsOpen.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                                        }
                                    }

                                    if( null == SerialNoVp )
                                    {
                                        CswNbtMetaDataNodeTypeProp SerialNtp = OwnerNT.getNodeTypeProp( "Serial No" ) ?? OwnerNT.getNodeTypeProp( "Assembly Serial No" );
                                        if( null != SerialNtp )
                                        {
                                            SerialNoVp = ProblemsOpen.AddViewProperty( EquipmentRel, SerialNtp );
                                            SerialNoVp.Name = "Serial No";
                                            SerialNoVp.Order = Order++;
                                            ProblemsOpen.AddViewPropertyFilter( SerialNoVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                                        }
                                    }

                                    if( null == ManufacturerVp )
                                    {
                                        CswNbtMetaDataNodeTypeProp ManufacturerNtp = OwnerNT.getNodeTypeProp( "Manufacturer" ) ?? OwnerNT.getNodeTypeProp( "Assembly Manufacturer" );
                                        if( null != ManufacturerNtp )
                                        {
                                            ManufacturerVp = ProblemsOpen.AddViewProperty( EquipmentRel, ManufacturerNtp );
                                            ManufacturerVp.Name = "Manufacturer";
                                            ManufacturerVp.Order = Order++;
                                            ProblemsOpen.AddViewPropertyFilter( ManufacturerVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                                        }
                                    }

                                    if( null == StatusVp )
                                    {
                                        CswNbtMetaDataNodeTypeProp StatusNtp = OwnerNT.getNodeTypeProp( "Status" ) ?? OwnerNT.getNodeTypeProp( "Assembly Status" );
                                        if( null != StatusNtp )
                                        {
                                            StatusVp = ProblemsOpen.AddViewProperty( EquipmentRel, StatusNtp );
                                            StatusVp.Name = "Status";
                                            StatusVp.Order = Order++;
                                            ProblemsOpen.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, "Available", false );
                                        }
                                    }
                                }
                                break;
                            } // case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                    } //switch( OwnerNtp.DefaultValue.AsRelationship.TargetType )
                } // if( Int32.MinValue != TargetId  )
            } // foreach( CswNbtMetaDataNodeType ProblemNT in ProblemsOC.NodeTypes )

            ProblemsOpen.save();

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

            //CswNbtMetaDataNodeType TaskNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Task" );
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );

            foreach( CswNbtMetaDataNodeType TaskNT in TaskOC.NodeTypes )
            {
                CswNbtViewRelationship TaskRel = TasksOpen.AddViewRelationship( TaskNT, false );
                CswNbtMetaDataNodeTypeProp CompletedNtp = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.CompletedPropertyName );
                CswNbtMetaDataNodeTypeProp OwnerNtp = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.OwnerPropertyName );
                CswNbtMetaDataNodeTypeProp DueDateNtp = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.DueDatePropertyName );
                CswNbtMetaDataNodeTypeProp SummaryNtp = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.SummaryPropertyName );
                CswNbtMetaDataNodeTypeProp TechnicianNtp = TaskNT.getNodeTypeProp( "Technician" );

                Int32 Order = 0;

                CswNbtViewProperty DueDateVp = TasksOpen.AddViewProperty( TaskRel, DueDateNtp );
                DueDateVp.Order = Order++;
                //TasksOpen.AddViewPropertyFilter( DueDateVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, DateTime.MinValue.ToString(), false );

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

                CswNbtViewProperty EquipmentVp = TasksOpen.AddViewProperty( TaskRel, OwnerNtp );
                EquipmentVp.Order = Order++;
                TasksOpen.AddViewPropertyFilter( EquipmentVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                CswNbtViewRelationship EquipmentRel = TasksOpen.AddViewRelationship( TaskRel,
                                                                                     CswNbtViewRelationship.PropOwnerType.First,
                                                                                     OwnerNtp,
                                                                                     false );

                Int32 TargetId = OwnerNtp.DefaultValue.AsRelationship.TargetId;
                if( Int32.MinValue != TargetId )
                {
                    switch( OwnerNtp.DefaultValue.AsRelationship.TargetType )
                    {
                        case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                            {
                                CswNbtMetaDataNodeType OwnerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( TargetId );

                                CswNbtMetaDataNodeTypeProp TypeNtp = OwnerNT.getNodeTypeProp( "Type" ) ?? OwnerNT.getNodeTypeProp( "Assembly Type" );
                                if( null != TypeNtp )
                                {
                                    CswNbtViewProperty TypeVp = TasksOpen.AddViewProperty( EquipmentRel, TypeNtp );
                                    TypeVp.Name = "Type";
                                    TypeVp.Order = Order++;
                                    TasksOpen.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                                }

                                CswNbtMetaDataNodeTypeProp SerialNtp = OwnerNT.getNodeTypeProp( "Serial No" ) ?? OwnerNT.getNodeTypeProp( "Assembly Serial No" );
                                if( null != SerialNtp )
                                {
                                    CswNbtViewProperty SerialNoVp = TasksOpen.AddViewProperty( EquipmentRel, SerialNtp );
                                    SerialNoVp.Name = "Serial No";
                                    SerialNoVp.Order = Order++;
                                    TasksOpen.AddViewPropertyFilter( SerialNoVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                                }

                                CswNbtMetaDataNodeTypeProp StatusNtp = OwnerNT.getNodeTypeProp( "Status" );
                                if( null != StatusNtp )
                                {
                                    CswNbtViewProperty StatusVp = TasksOpen.AddViewProperty( EquipmentRel, StatusNtp );
                                    StatusVp.Name = "Status";
                                    StatusVp.Order = Order++;
                                    TasksOpen.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, "Available", false );
                                }
                                break;
                            } // case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                        case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                            {
                                CswNbtMetaDataObjectClass OwnerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( TargetId );

                                CswNbtViewProperty TypeVp = null;
                                CswNbtViewProperty SerialNoVp = null;
                                CswNbtViewProperty StatusVp = null;

                                foreach( CswNbtMetaDataNodeType OwnerNT in OwnerOC.NodeTypes )
                                {
                                    if( null == TypeVp )
                                    {
                                        CswNbtMetaDataNodeTypeProp TypeNtp = OwnerNT.getNodeTypeProp( "Type" ) ?? OwnerNT.getNodeTypeProp( "Assembly Type" );
                                        if( null != TypeNtp )
                                        {
                                            TypeVp = TasksOpen.AddViewProperty( EquipmentRel, TypeNtp );
                                            TypeVp.Name = "Type";
                                            TypeVp.Order = Order++;
                                            TasksOpen.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                                        }
                                    }

                                    if( null == SerialNoVp )
                                    {
                                        CswNbtMetaDataNodeTypeProp SerialNtp = OwnerNT.getNodeTypeProp( "Serial No" ) ?? OwnerNT.getNodeTypeProp( "Assembly Serial No" );
                                        if( null != SerialNtp )
                                        {
                                            SerialNoVp = TasksOpen.AddViewProperty( EquipmentRel, SerialNtp );
                                            SerialNoVp.Name = "Serial No";
                                            SerialNoVp.Order = Order++;
                                            TasksOpen.AddViewPropertyFilter( SerialNoVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                                        }
                                    }

                                    if( null == StatusVp )
                                    {
                                        CswNbtMetaDataNodeTypeProp StatusNtp = OwnerNT.getNodeTypeProp( "Status" ) ?? OwnerNT.getNodeTypeProp( "Assembly Status" );
                                        if( null != StatusNtp )
                                        {
                                            StatusVp = TasksOpen.AddViewProperty( EquipmentRel, StatusNtp );
                                            StatusVp.Name = "Status";
                                            StatusVp.Order = Order++;
                                            TasksOpen.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, "Available", false );
                                        }
                                    }
                                }
                                break;
                            } // case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                    } //switch( OwnerNtp.DefaultValue.AsRelationship.TargetType )
                }

                TasksOpen.save();
            } //if( null != ProblemNT )

        } // update()

    }//class CswUpdateSchemaTo01H46

}//namespace ChemSW.Nbt.Schema

