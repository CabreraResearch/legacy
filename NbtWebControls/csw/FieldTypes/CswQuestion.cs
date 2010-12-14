using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    /// <summary>
    /// Field type class for Question
    /// </summary>
    public class CswQuestion : CswFieldTypeWebControl, INamingContainer
    {
        private bool _AllowEditValue = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswQuestion( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            DataBinding += new EventHandler( CswQuestion_DataBinding );
        }//ctor

        private void CswQuestion_DataBinding( object sender, EventArgs e )
        {
            try
            {
                _AllowEditValue = ( _EditMode != NodeEditMode.Demo && _EditMode != NodeEditMode.PrintReport && !ReadOnly );

                EnsureChildControls();
                if ( Prop != null )
                {
                    _AnswerList.Items.Clear();
                    //ListItem NoneItem = new ListItem( "", "" );
                    //_AnswerList.Items.Add( NoneItem );

                    CswCommaDelimitedString _AllowedAnswers = Prop.AsQuestion.AllowedAnswers;

                    for ( int i = 0; i < _AllowedAnswers.Count; i++ )
                    {
                        _AnswerList.Items.Add( new ListItem( _AllowedAnswers[i], _AllowedAnswers[i] ) );
                    }

                    if ( null != _AnswerList.Items.FindByValue( Prop.AsQuestion.Answer ) )
                        _AnswerList.SelectedValue = Prop.AsQuestion.Answer;

                    _CorrectiveActionText.Text = this.Prop.AsQuestion.CorrectiveAction;
                    _CommentsText.Text = this.Prop.AsQuestion.Comments;
                    _CorrectedDate.Text = this.Prop.AsQuestion.DateCorrected.ToShortDateString();
                    _AnsweredDate.Text = this.Prop.AsQuestion.DateAnswered.ToShortDateString();
                }//Prop != null
            }//try
            catch ( Exception ex )
            {
                HandleError( ex );
            }
        }

        private DropDownList _AnswerList;
        private CswLiteralText _AnswerListName;
        private TextBox _CommentsText;
        private CswLiteralText _CommentsTextName;
        private TextBox _CorrectiveActionText;
        private CswLiteralText _CorrectiveActionTextName;
        private CswLiteralText _CorrectedDateName;
        private Literal _CorrectedDate;
        private CswLiteralText _AnsweredDateName;
        private Literal _AnsweredDate;

        /// <summary>
        /// Field type Question Save event
        /// </summary>
        public override void Save()
        {
            try
            {
                if ( _AllowEditValue )
                {
                    Prop.AsQuestion.Answer = _AnswerList.SelectedValue;
                    Prop.AsQuestion.Comments = _CommentsText.Text;
                    Prop.AsQuestion.CorrectiveAction = _CorrectiveActionText.Text;
                }
            }
            catch ( Exception ex )
            {
                HandleError( ex );
            }
        }

        /// <summary>
        /// Field type question after save
        /// </summary>
        public override void AfterSave()
        {
            DataBind();
        }

        /// <summary>
        /// Field type question Clear event
        /// </summary>
        public override void Clear()
        {
            try
            {
                _AnswerList.SelectedValue = String.Empty;
                _CommentsText.Text = String.Empty;
                _CorrectiveActionText.Text = String.Empty;
            }
            catch ( Exception ex )
            {
                HandleError( ex );
            }
        }

        /// <summary>
        /// Field type question ClearChildControls
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                CswAutoTable TopTable = new CswAutoTable();
                TopTable.Width = 400;
                this.Controls.Add( TopTable );

                _AnswerListName = new CswLiteralText( "Answer: " );
                TopTable.addControl( 1, 0, _AnswerListName );

                _AnswerList = new DropDownList();
                _AnswerList.ID = "ans";
                _AnswerList.CssClass = CswFieldTypeWebControl.ListOptionTextField;
                TopTable.addControl( 1, 1, _AnswerList );

                _CommentsTextName = new CswLiteralText( "Comments: " );
                TopTable.addControl( 2, 0, _CommentsTextName );

                _CommentsText = new TextBox();
                _CommentsText.ID = "comm";
                _CommentsText.Width = 200;
                _CommentsText.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
                _CommentsText.TextMode = TextBoxMode.MultiLine;
                TopTable.addControl( 2, 1, _CommentsText );

                _CorrectiveActionTextName = new CswLiteralText( "Corrective Action: " );
                TopTable.addControl( 4, 0, _CorrectiveActionTextName );

                _CorrectiveActionText = new TextBox();
                _CorrectiveActionText.ID = "actn";
                _CorrectiveActionText.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
                _CorrectiveActionText.Width = 200;
                TopTable.addControl( 4, 1, _CorrectiveActionText );

                _CorrectedDateName = new CswLiteralText( "Date Corrected: " );
                TopTable.addControl( 5, 0, _CorrectedDateName );

                _CorrectedDate = new Literal();
                _CorrectedDate.ID = "crctd";
                TopTable.addControl( 5, 1, _CorrectedDate );

                _AnsweredDateName = new CswLiteralText( "Date Answered: " );
                TopTable.addControl( 6, 0, _AnsweredDateName );

                _AnsweredDate = new Literal();
                _AnsweredDate.ID = "answd";
                TopTable.addControl( 6, 1, _AnsweredDate );
            }
            catch ( Exception ex )
            {
                HandleError( ex );
            }
            base.CreateChildControls();
        }

        /// <summary>
        /// Field type question OnPrerender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                if ( Prop != null && Prop.AsQuestion != null )
                {
                    if ( _AllowEditValue )
                    {
                        if ( null == _AnswerList.Items.FindByValue( Prop.AsQuestion.Answer ) )
                        {
                            // Add it!  This guarantees we see the data that was saved, even if the options change
                            ListItem SelectedAnswer = new ListItem( Prop.AsQuestion.Answer, Prop.AsQuestion.Answer );
                            _AnswerList.Items.Add( SelectedAnswer );
                            _AnswerList.SelectedValue = Prop.AsQuestion.Answer;
                        }
                        _AnswerList.Enabled = true;
                        _AnswerList.Attributes.Add( "onkeypress", "CswQuestion_onchange();" );
                        _AnswerList.Attributes.Add( "onchange", "CswQuestion_onchange();" );

                        _CommentsText.Enabled = true;
                        _CommentsText.Attributes.Add( "onkeypress", "CswQuestion_onchange();" );
                        _CommentsText.Attributes.Add( "onchange", "CswQuestion_onchange();" );

                        _CorrectiveActionText.Enabled = true;
                        _CorrectiveActionText.Attributes.Add( "onkeypress", "CswQuestion_onchange();" );
                        _CorrectiveActionText.Attributes.Add( "onchange", "CswQuestion_onchange();" );

                    }//_AllowedEdit
                    else
                    {
                        _AnswerList.Enabled = false;
                        _CommentsText.Enabled = false;
                        _CorrectiveActionText.Enabled = false;
                    }
                    
                    if ( ( !Prop.AsQuestion.IsCompliant && 
                            !( String.Empty == Prop.AsQuestion.Answer ) ) || 
                            !( String.Empty == Prop.AsQuestion.CorrectiveAction ) )
                    {
                        _CorrectiveActionText.Visible = true;
                        _CorrectiveActionTextName.Visible = true;
                    }
                    else
                    {
                        _CorrectiveActionText.Visible = false;
                        _CorrectiveActionTextName.Visible = false;
                    }

                    if( _EditMode == NodeEditMode.PrintReport )
                    {
                        _AnsweredDateName.Visible = true;
                        _AnsweredDate.Visible = true;
                        _CorrectedDateName.Visible = true;
                        _CorrectedDate.Visible = true;
                    }
                    else
                    {
                        _AnsweredDateName.Visible = false;
                        _AnsweredDate.Visible = false;
                        _CorrectedDateName.Visible = false;
                        _CorrectedDate.Visible = false;
                    }
                        

                }//Prop != null
            }//try
            catch ( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }

        /// <summary>
        /// Field type question RenderContents
        /// </summary>
        /// <param name="output"></param>
        protected override void RenderContents( HtmlTextWriter output )
        {
            base.RenderContents( output );
        }
    }
}

