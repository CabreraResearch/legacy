


using System;
using ChemSW.Core;

namespace ChemSW.Nbt.ViewEditor
{
    public class CswNbtViewEditorRuleViewAttributes: CswNbtViewEditorRule
    {
        public CswNbtViewEditorRuleViewAttributes( CswNbtResources CswNbtResources, CswNbtViewEditorData IncomingRequest )
            : base( CswNbtResources, IncomingRequest )
        {
            RuleName = CswEnumNbtViewEditorRuleName.ViewAttributes;
        }

        public override CswNbtViewEditorData GetStepData()
        {
            //This step just allows the user to edit View properties such as View Name, Category, Width, ect.
            //The client already has this information in the current view, if we get a request to get this step data, just return the view
            CswNbtViewEditorData Return = new CswNbtViewEditorData();
            base.Finalize( Return );
            return Return;
        }

        public override CswNbtViewEditorData HandleAction()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();

            CurrentView.ViewName = Request.NewViewName;
            CurrentView.Visibility = (CswEnumNbtViewVisibility) Request.NewViewVisibility;
            CurrentView.VisibilityRoleId = CswConvert.ToPrimaryKey( Request.NewVisibilityRoleId );
            CurrentView.VisibilityUserId = CswConvert.ToPrimaryKey( Request.NewVisbilityUserId );
            CurrentView.Category = Request.NewViewCategory;
            if( Int32.MinValue != Request.NewViewWidth )
            {
                CurrentView.Width = Request.NewViewWidth;
            }

            base.Finalize( Return );
            return Return;
        }

        
    }
}
