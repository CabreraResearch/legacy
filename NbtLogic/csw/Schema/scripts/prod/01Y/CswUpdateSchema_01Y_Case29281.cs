using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29281
    /// </summary>
    public class CswUpdateSchema_01Y_Case29281 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29281; }
        }

        public override void update()
        {
            CswNbtView TargetView = _getInvalidTargetsView();

            ICswNbtTree TargetTree = _CswNbtSchemaModTrnsctn.getTreeFromView( TargetView, false );
            Int32 ContainerCount = TargetTree.getChildNodeCount();
            for( int j = 0; j < ContainerCount; j++ ) //Target Nodes
            {
                TargetTree.goToNthChild( j );
                if( TargetTree.getChildNodeCount() == 0 ) //No valid inspection exists - status needs to change
                {
                    CswNbtObjClassInspectionTarget TargetNode = _CswNbtSchemaModTrnsctn.Nodes[TargetTree.getNodeIdForCurrentPosition()];
                    TargetNode.Status.Value = CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected );
                    TargetNode.postChanges( false );
                }
                TargetTree.goToParentNode();
            }
        } //Update()

        //Get all Inspection targets with Status = OK, and any inspection design nodes that been touched (i.e. - not pending or overdue)
        //If no designs exist in the view for the given target, then the target should have a status of Not Inspected instead.
        private CswNbtView _getInvalidTargetsView()
        {
            CswNbtView myView = _CswNbtSchemaModTrnsctn.makeView();
            myView.Visibility = NbtViewVisibility.Global;
            myView.ViewMode = NbtViewRenderingMode.Tree;

            CswNbtMetaDataObjectClass Rel1SecondOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InspectionTargetClass );
            CswNbtViewRelationship Rel1 = myView.AddViewRelationship( Rel1SecondOC, true );

            CswNbtViewProperty Prop2 = null;
            foreach( CswNbtMetaDataNodeType Rel1NT in Rel1SecondOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp Prop2NTP = Rel1NT.getNodeTypeProp( "Status" );
                if( null != Prop2NTP )
                {
                    Prop2 = myView.AddViewProperty( Rel1, Prop2NTP );
                    break;
                }
            }
            myView.AddViewPropertyFilter( Prop2,
                                        CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                        CswNbtPropFilterSql.FilterResultMode.Hide,
                                        CswNbtSubField.SubFieldName.Value,
                                        CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                        "OK" );

            CswNbtMetaDataObjectClass Rel4SecondOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp Rel4Prop = Rel4SecondOC.getObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
            CswNbtViewRelationship Rel4 = myView.AddViewRelationship( Rel1, NbtViewPropOwnerType.Second, Rel4Prop, true );

            CswNbtViewProperty Prop5 = null;
            foreach( CswNbtMetaDataNodeType Rel4NT in Rel4SecondOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp Prop5NTP = Rel4NT.getNodeTypeProp( "Status" );
                if( null != Prop5NTP )
                {
                    Prop5 = myView.AddViewProperty( Rel4, Prop5NTP );
                    break;
                }
            }
            myView.AddViewPropertyFilter( Prop5,
                                        CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                        CswNbtPropFilterSql.FilterResultMode.Hide,
                                        CswNbtSubField.SubFieldName.Value,
                                        CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                        "Pending" );

            myView.AddViewPropertyFilter( Prop5,
                                        CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                        CswNbtPropFilterSql.FilterResultMode.Hide,
                                        CswNbtSubField.SubFieldName.Value,
                                        CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                        "Overdue" );
            return myView;
        }

    }//class CswUpdateSchema_01Y_Case29281

}//namespace ChemSW.Nbt.Schema
