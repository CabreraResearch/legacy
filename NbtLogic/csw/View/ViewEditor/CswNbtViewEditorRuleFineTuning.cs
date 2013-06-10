using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ViewEditor
{
    public class CswNbtViewEditorRuleFineTuning: CswNbtViewEditorRule
    {
        public CswNbtViewEditorRuleFineTuning( CswNbtResources CswNbtResources, CswNbtViewEditorData IncomingRequest )
            : base( CswNbtResources, IncomingRequest )
        {
            RuleName = CswEnumNbtViewEditorRuleName.FineTuning;
        }

        public override CswNbtViewEditorData GetStepData()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();
            Return.Step4.ViewJson = CswConvert.ToString( CurrentView.ToJson() );
            base.Finalize( Return );
            return Return;
        }

        public override CswNbtViewEditorData HandleAction()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();

            if( Request.Action == "Click" )
            {
                CswNbtViewNode foundNode = Request.CurrentView.FindViewNodeByArbitraryId( Request.ArbitraryId );
                if( null != foundNode )
                {
                    if( foundNode is CswNbtViewPropertyFilter )
                    {
                        Return.Step6.FilterNode = (CswNbtViewPropertyFilter) foundNode;
                    }
                    else if( foundNode is CswNbtViewRelationship )
                    {
                        CswNbtViewRelationship asRelationship = (CswNbtViewRelationship) foundNode;
                        CswNbtView TempView = _CswNbtResources.ViewSelect.restoreView( CurrentView.ToString() );

                        if( asRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( asRelationship.SecondId );
                            Return.Step6.Properties = _getProps( NodeType, TempView, new HashSet<string>(), asRelationship, false );
                        }
                        else if( asRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                        {
                            CswNbtMetaDataObjectClass ObjClass = _CswNbtResources.MetaData.getObjectClass( asRelationship.SecondId );
                            Return.Step6.Properties = _getProps( ObjClass, TempView, new HashSet<string>(), asRelationship, false );
                        }
                        else
                        {
                            CswNbtMetaDataPropertySet PropSet = _CswNbtResources.MetaData.getPropertySet( asRelationship.SecondId );
                            HashSet<string> seenProps = new HashSet<string>();
                            foreach( CswNbtMetaDataObjectClass ObjClass in PropSet.getObjectClasses() )
                            {
                                foreach( CswNbtViewProperty prop in _getProps( ObjClass, TempView, seenProps, asRelationship ).OrderBy( prop => prop.TextLabel ) )
                                {
                                    Return.Step6.Properties.Add( prop );
                                }
                            }
                        }

                        Return.Step6.RelationshipNode = asRelationship;
                    }
                }
            }
            else if( Request.Action == "AddProp" )
            {
                ICswNbtMetaDataProp prop = null;
                if( Request.Property.Type == CswEnumNbtViewPropType.NodeTypePropId )
                {
                    prop = _CswNbtResources.MetaData.getNodeTypeProp( Request.Property.NodeTypePropId );
                }
                else
                {
                    prop = _CswNbtResources.MetaData.getObjectClassProp( Request.Property.ObjectClassPropId );
                }
                CswNbtViewRelationship relToAddTo = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( Request.Relationship.ArbitraryId );
                CurrentView.AddViewProperty( relToAddTo, prop, CurrentView.getOrderedViewProps( false ).Count + 1 );
            }

            base.Finalize( Return );
            return Return;
        }


    }
}
