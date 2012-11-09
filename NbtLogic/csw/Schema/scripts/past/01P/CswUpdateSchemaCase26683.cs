using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26683
    /// </summary>
    public class CswUpdateSchemaCase26683 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Clear hidden/readonly flags on certain fields:
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass FeedbackOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.FeedbackClass );

            foreach( CswNbtObjClassUser UserNode in UserOC.getNodes( false, true ) )
            {
                //Case 26866/27114
                if( false == CswTools.IsAlphaNumeric( UserNode.UsernameProperty.Text ) )
                {
                    string ValidUserName = Regex.Replace( UserNode.UsernameProperty.Text, "[^a-zA-Z0-9_]+", "" );
                    CswNbtObjClassUser ExistingUserNode = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( ValidUserName );
                    if( ExistingUserNode != null && ExistingUserNode.NodeId != null )
                    {
                        UserNode.AccountLocked.Checked = Tristate.True;
                    }
                    else
                    {
                        UserNode.UsernameProperty.Text = ValidUserName;
                    }
                }

                UserNode.AccountLocked.setReadOnly( value: false, SaveToDb: true );
                UserNode.AccountLocked.setHidden( value: false, SaveToDb: true );

                UserNode.FailedLoginCount.setReadOnly( value: false, SaveToDb: true );
                UserNode.FailedLoginCount.setHidden( value: false, SaveToDb: true );
                UserNode.postChanges( true );
            }

            foreach( CswNbtObjClassFeedback FeedbackNode in FeedbackOC.getNodes( false, true ) )
            {
                FeedbackNode.LoadUserContext.setReadOnly( value: false, SaveToDb: true );
                FeedbackNode.LoadUserContext.setHidden( value: false, SaveToDb: true );
                FeedbackNode.postChanges( true );
            }

            foreach( CswNbtObjClassInspectionDesign InspectionDesignNode in InspectionDesignOC.getNodes( false, true ) )
            {
                InspectionDesignNode.Status.setReadOnly( value: false, SaveToDb: true );
                InspectionDesignNode.Status.setHidden( value: false, SaveToDb: true );
                InspectionDesignNode.postChanges( true );
            }



        }//Update()

    }//class CswUpdateSchemaCase26683

}//namespace ChemSW.Nbt.Schema