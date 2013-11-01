using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30017 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {                
            get { return 30017; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Lab Safety (Demo)"; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType LabSafetyChecklist = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Lab Safety Checklist (demo)" );
            CswNbtMetaDataNodeType LabSafetyGroup = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Lab Safety Group (demo)" );
            CswNbtMetaDataNodeType LabSafetyTarget = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Lab Safety (demo)" );

            if( null != LabSafetyChecklist && null != LabSafetyGroup && null != LabSafetyTarget )
            {
                CswNbtMetaDataNodeTypeTab SchedulesTab = LabSafetyGroup.getNodeTypeTab( "Schedules" );
                if( null == SchedulesTab )
                {
                    CswNbtMetaDataNodeType GeneratorNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
                    if( null != GeneratorNt )
                    {
                        SchedulesTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( LabSafetyGroup, " Schedules", 2 );
                        CswNbtMetaDataNodeTypeProp SchedulesNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( LabSafetyGroup, CswEnumNbtFieldType.Grid, "Schedules", SchedulesTab.TabId );
                        CswNbtView SchedulesView = _CswNbtSchemaModTrnsctn.makeSafeView( LabSafetyTarget.NodeTypeName + " Schedules", CswEnumNbtViewVisibility.Property );
                        SchedulesView.NbtViewMode = CswEnumNbtViewRenderingMode.Grid.ToString();

                        CswNbtViewRelationship Rel = SchedulesView.AddViewRelationship( LabSafetyGroup, IncludeDefaultFilters: true );
                        CswNbtViewRelationship SchedRel = SchedulesView.AddViewRelationship( Rel, CswEnumNbtViewPropOwnerType.Second, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner ), IncludeDefaultFilters: true );
                        SchedulesView.AddViewProperty( SchedRel, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Description ) );
                        SchedulesView.AddViewProperty( SchedRel, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.NextDueDate ) );
                        SchedulesView.AddViewProperty( SchedRel, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.RunStatus ) );
                        SchedulesView.AddViewProperty( SchedRel, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.RunTime ) );

                        SchedulesView.save();
                        SchedulesNtp.ViewId = SchedulesView.ViewId;
                        SchedulesNtp.removeFromLayout( CswEnumNbtLayoutType.Add );
                    }
                }
                CswNbtView GroupView = _CswNbtSchemaModTrnsctn.restoreView( "Groups, Lab Safety Checklist: Lab Safety (demo)" );
                if( null != GroupView )
                {
                    CswNbtView View = GroupView;
                    GroupView.Root.eachRelationship( Relationship =>
                    {
                        if( Relationship.SecondMetaDataDefinitionObject().UniqueId == LabSafetyTarget.NodeTypeId )
                        {
                            View.AddViewRelationship( Relationship, CswEnumNbtViewPropOwnerType.Second, LabSafetyChecklist.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target ), false );
                        }

                    }, null );  
                    GroupView.save();
                }

                CswNbtView DoomedView1 = _CswNbtSchemaModTrnsctn.restoreView( "Inspections, Lab Safety Checklist: Lab Safety (demo)" );
                if( null != DoomedView1 )
                {
                    DoomedView1.Delete();
                }
                
                CswNbtView DoomedView2 = _CswNbtSchemaModTrnsctn.restoreView( "Scheduling, Lab Safety Checklist: Lab Safety (demo)" );
                if( null != DoomedView2 )
                {
                    DoomedView2.Delete();
                }





            }
            

        } // update()

    }

}//namespace ChemSW.Nbt.Schema