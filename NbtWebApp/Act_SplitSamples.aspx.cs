using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

using ChemSW.NbtWebControls;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_SplitSamples : System.Web.UI.Page
    {
        protected override void OnInit( EventArgs e )
        {
            SplitSamplesWizard.onPageChange += new CswWizard.CswWizardEventHandler( SplitSamplesWizard_onPageChange );
            SplitSamplesWizard.onFinish += new CswWizard.CswWizardEventHandler( SplitSamplesWizard_onFinish );
            SplitSamplesWizard.OnError += new CswErrorHandler( Master.HandleError );
            SplitSamplesWizard.onCancel += new CswWizard.CswWizardEventHandler( SplitSamplesWizard_onCancel );
            EnsureChildControls();
            base.OnInit( e );
        }


        private Literal SampleBarcodeLiteral;
        private Literal SampleBarcodeValueLiteral;
        private Literal SampleBarcodeValue;
        private Literal SampleTotalQuantityLiteral;
        private Literal SampleTotalQuantityValue;
        private Literal NumberOfChildSamplesLiteral;
        private Literal QuantityPerChildSampleLiteral;
        private Literal QuantityPerChildSampleUnitLiteral;
        private TextBox SampleBarcodeTextBox;
        private TextBox NumberOfChildSamplesTextBox;
        private TextBox QuantityPerChildSampleTextBox;
        private CswAutoTable _Table;

        protected override void CreateChildControls()
        {
            SampleBarcodeLiteral = new Literal();
            SampleBarcodeLiteral.Text = "Sample Barcode:";

            SampleBarcodeTextBox = new TextBox();
            SampleBarcodeTextBox.ID = "SampleBarcodeTextBox";
            SampleBarcodeTextBox.CssClass = "textinput";

            _Table = new CswAutoTable();
            _Table.ID = "splitsampletable";
            _Table.FirstCellRightAlign = true;

            SampleBarcodeValueLiteral = new Literal();
            SampleBarcodeValueLiteral.Text = "Sample Barcode:";

            SampleBarcodeValue = new Literal();
            
            SampleTotalQuantityLiteral = new Literal();
            SampleTotalQuantityLiteral.Text = "Sample Total Quantity:";

            SampleTotalQuantityValue = new Literal();
            
            NumberOfChildSamplesLiteral = new Literal();
            NumberOfChildSamplesLiteral.Text = "Number of Child Samples:";

            NumberOfChildSamplesTextBox = new TextBox();
            NumberOfChildSamplesTextBox.ID = "NumberOfChildSamplesTextBox";
            NumberOfChildSamplesTextBox.CssClass = "textinput";

            QuantityPerChildSampleLiteral = new Literal();
            QuantityPerChildSampleLiteral.Text = "Quantity per Child Sample:";

            QuantityPerChildSampleUnitLiteral = new Literal();
            
            QuantityPerChildSampleTextBox = new TextBox();
            QuantityPerChildSampleTextBox.ID = "QuantityPerChildSampleTextBox";
            QuantityPerChildSampleTextBox.CssClass = "textinput";

            Step1PH.Controls.Add( SampleBarcodeLiteral );
            Step1PH.Controls.Add( SampleBarcodeTextBox );

            _Table.addControl( 0, 0, SampleBarcodeValueLiteral );
            _Table.addControl( 0, 1, SampleBarcodeValue );
            _Table.addControl( 1, 0, SampleTotalQuantityLiteral);
            _Table.addControl( 1, 1, SampleTotalQuantityValue );
            _Table.addControl( 2, 0, NumberOfChildSamplesLiteral );
            _Table.addControl( 2, 1, NumberOfChildSamplesTextBox );
            _Table.addControl( 3, 0, QuantityPerChildSampleLiteral );
            _Table.addControl( 3, 1, QuantityPerChildSampleTextBox );
            _Table.addControl( 3, 1, QuantityPerChildSampleUnitLiteral );
            Step2PH.Controls.Add( _Table );

            base.CreateChildControls();
        }

        void SplitSamplesWizard_onPageChange( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            if( CswWizardEventArgs.NewPage == 2 )
            {
                bool ValidSample = false;
                string SampleBarcode = SampleBarcodeTextBox.Text;
                if( SampleBarcode != string.Empty )
                {
                    CswNbtNode SampleNode = _getSampleNodeFromBarcode( SampleBarcode );
                    if( SampleNode != null )
                    {
                        ValidSample = true;
                        CswNbtObjClassSample SampleObjClass = CswNbtNodeCaster.AsSample( SampleNode );
                        SampleBarcodeValue.Text = SampleBarcode;
                        SampleTotalQuantityValue.Text = SampleObjClass.Quantity.Gestalt;
                        QuantityPerChildSampleUnitLiteral.Text = SampleObjClass.Quantity.Units.ToString();
                    }
                }

                if( !ValidSample )
                {
                    Literal ErrorMsg = new Literal();
                    ErrorMsg.Text = "<br>Invalid barcode.  Please enter or scan a sample barcode.";
                    Step1PH.Controls.Add( ErrorMsg );

                    SplitSamplesWizard.CurrentStep = 1;
                }
            }
        }

        void SplitSamplesWizard_onFinish( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            string SampleBarcode = SampleBarcodeTextBox.Text;
            CswNbtNode ParentSampleNode = _getSampleNodeFromBarcode( SampleBarcode );
            CswNbtObjClassSample ParentSampleObjClass = CswNbtNodeCaster.AsSample( ParentSampleNode );

            Int32 NumberOfChildren = Int32.MinValue;
            Double QuantityPerChild = Double.NaN;
            try
            {
                NumberOfChildren = CswConvert.ToInt32( NumberOfChildSamplesTextBox.Text );
                QuantityPerChild = Convert.ToDouble( QuantityPerChildSampleTextBox.Text );
            }
            catch( Exception ex )
            {
                Literal ErrorMsg = new Literal();
                ErrorMsg.Text = "<br>Invalid number.  Please enter a valid integer for 'Number of Children' and a valid number for 'Quantity Per Child'.";
                Step2PH.Controls.Add( ErrorMsg );

                SplitSamplesWizard.CurrentStep = 2;
            }

            // Create children
            Collection<CswPrimaryKey> SampleNodeIds = new Collection<CswPrimaryKey>();
            for( int i = 0; i < NumberOfChildren; i++ )
            {
                CswNbtNode ChildSampleNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ParentSampleNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                CswNbtObjClassSample ChildSampleObjClass = CswNbtNodeCaster.AsSample( ChildSampleNode );
                ChildSampleNode.copyPropertyValues( ParentSampleNode );
                ChildSampleObjClass.Quantity.Quantity = QuantityPerChild;
                ChildSampleObjClass.ParentSample.RelatedNodeId = ParentSampleNode.NodeId;
                ChildSampleObjClass.ParentSample.CachedNodeName = ParentSampleNode.NodeName;

                ChildSampleNode.postChanges( true );
                SampleNodeIds.Add( ChildSampleNode.NodeId );
            }

            // Decrement quantity from parent sample
            ParentSampleObjClass.Quantity.Quantity = ParentSampleObjClass.Quantity.Quantity - ( QuantityPerChild * Convert.ToDouble( NumberOfChildren ) );
            ParentSampleNode.postChanges( true );

            CswNbtView NewSamplesView = _makeNewSamplesView( ParentSampleNode.NodeId, SampleNodeIds );
            Master.setViewXml( NewSamplesView.ToString() );
            Master.GoMain();
        }

        void SplitSamplesWizard_onCancel( object CswWizard, CswWizardEventArgs CswWizardEventArgs )
        {
            Master.GoMain();
        }

        private CswNbtNode _getSampleNodeFromBarcode( string SampleBarcode )
        {
            CswNbtNode ret = null;

            CswNbtMetaDataObjectClass SampleObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SampleClass );
            CswNbtMetaDataObjectClassProp SampleBarcodeProp = SampleObjectClass.getObjectClassProp( CswNbtObjClassSample.BarcodePropertyName );
            
            CswNbtView SampleView = new CswNbtView( Master.CswNbtResources );
            CswNbtViewRelationship SampleRelationship = SampleView.AddViewRelationship( SampleObjectClass, false );
            CswNbtViewProperty SampleBarcodeViewProperty = SampleView.AddViewProperty( SampleRelationship, SampleBarcodeProp );
            CswNbtViewPropertyFilter BarcodeFilter = SampleView.AddViewPropertyFilter( SampleBarcodeViewProperty, SampleBarcodeProp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, SampleBarcode, false );

            ICswNbtTree SampleTree = Master.CswNbtResources.Trees.getTreeFromView( SampleView, true, true, false, false );
            if( SampleTree.getChildNodeCount() > 0 )
            {
                SampleTree.goToNthChild( 0 );
                ret = SampleTree.getNodeForCurrentPosition();
            }
            return ret;
        }

        private CswNbtView _makeNewSamplesView( CswPrimaryKey ParentSampleNodeId, Collection<CswPrimaryKey> ChildSampleNodeIds )
        {
            CswNbtMetaDataObjectClass SampleObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SampleClass );
            CswNbtMetaDataObjectClassProp SampleBarcodeProp = SampleObjectClass.getObjectClassProp( CswNbtObjClassSample.BarcodePropertyName );
            CswNbtMetaDataObjectClassProp SampleParentProp = SampleObjectClass.getObjectClassProp( CswNbtObjClassSample.ParentSamplePropertyName );

            CswNbtView SampleView = new CswNbtView( Master.CswNbtResources );
            SampleView.ViewName = "New Samples";

            CswNbtViewRelationship ParentSampleRelationship = SampleView.AddViewRelationship( SampleObjectClass, true );
            ParentSampleRelationship.NodeIdsToFilterIn.Add( ParentSampleNodeId );

            CswNbtViewRelationship ChildSampleRelationship = SampleView.AddViewRelationship( ParentSampleRelationship, CswNbtViewRelationship.PropOwnerType.Second, SampleParentProp, true );
            ChildSampleRelationship.NodeIdsToFilterIn = ChildSampleNodeIds;

            return SampleView;

        }

    }
}