using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30562 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30562; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30562"; }
        }

        public override string Title
        {
            get { return "Edit 'Due Inspections (all)' view in nbt_master"; }
        }

        public override void update()
        {
            CswNbtView DueInspectionsAllView = _CswNbtSchemaModTrnsctn.restoreView( "Due Inspections (all)" );
            if( null != DueInspectionsAllView )
            {
                // Set the view name
                DueInspectionsAllView.ViewName = "Due Inspections";

                // Show the due date filter at runtime
                CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
                CswNbtMetaDataObjectClassProp DueDateOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate );

                foreach( CswNbtViewRelationship ViewRelationship in DueInspectionsAllView.Root.ChildRelationships )
                {
                    foreach( CswNbtViewProperty ViewProperty in ViewRelationship.Properties )
                    {
                        if( ViewProperty.ObjectClassPropId == DueDateOCP.ObjectClassPropId )
                        {
                            foreach( CswNbtViewPropertyFilter ViewPropertyFilter in ViewProperty.Filters )
                            {
                                if( ViewPropertyFilter.Value == "today+7" )
                                {
                                    ViewPropertyFilter.ShowAtRuntime = CswConvert.ToBoolean( CswEnumTristate.True );
                                    break;
                                }
                            }
                        }
                    }
                }

                DueInspectionsAllView.save();
            }


        } // update()

    }

}//namespace ChemSW.Nbt.Schema