using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    class CswNbtObjClassCertDefSpec: CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string NameForTestingConditions = "Name For Testing Conditions";
            public const string Method = "Method";
            public const string CertDef = "Cert Def";
            public const string Conditions = "Conditions";
            public const string Characteristics = "Characteristics";
        }

        public CswNbtObjClassCertDefSpec( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassCertDefSpec
        /// </summary>
        public static implicit operator CswNbtObjClassCertDefSpec( CswNbtNode Node )
        {
            CswNbtObjClassCertDefSpec ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CertDefSpecClass ) )
            {
                ret = (CswNbtObjClassCertDefSpec) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforePromoteNodeLogic( bool OverrideUniqueValidation = false )
        {
            base.beforePromoteNodeLogic( OverrideUniqueValidation );
            _createCertDefSpecsAndCharacteristics();
        }

        #endregion

        #region private methods
       
        /// <summary>
        /// for each method condition and method characteristic attached to this
        /// certDef, this method created a corresponding certDef condition
        /// and certDef characteristic. 
        /// </summary>
        private void _createCertDefSpecsAndCharacteristics()
        {
            
            CswNbtMetaDataObjectClass MethodOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass );
            CswNbtMetaDataObjectClass MethodConditionOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodConditionClass );
            CswNbtMetaDataObjectClass MethodCharacteristicOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodCharacteristicClass);
            CswNbtMetaDataObjectClass CertDefConditionOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefConditionClass );
            CswNbtMetaDataObjectClass CertDefCharacteristicOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefCharacteristicLimitClass);

            CswNbtMetaDataNodeType CertDefConditionDefaultNT = CertDefConditionOC.FirstNodeType;
            CswNbtMetaDataNodeType CertDefCharacteristicDefaultNT = CertDefCharacteristicOC.FirstNodeType;

            CswNbtMetaDataObjectClassProp MethodConditionMethodOCP = _CswNbtResources.MetaData.getObjectClassProp(MethodConditionOC.ObjectClassId, 
                                                                                                                  CswNbtObjClassMethodCondition.PropertyName.Method);
            CswNbtMetaDataObjectClassProp MethodCharacteristicMethodOCP = _CswNbtResources.MetaData.getObjectClassProp(MethodCharacteristicOC.ObjectClassId, 
                                                                                                                  CswNbtObjClassMethodCharacteristic.PropertyName.Method);

            //create a view of all methodcondition and methodcharacteristics
            //attached to this method
            CswNbtView MethodConditionAndCharacteristicsView = new CswNbtView( _CswNbtResources );
            MethodConditionAndCharacteristicsView.ViewName = "View of all method characteristics and method conditions attached to a method";

            CswNbtViewRelationship MethodAsRootRelationship = MethodConditionAndCharacteristicsView.AddViewRelationship( MethodOC, false );
            MethodAsRootRelationship.NodeIdsToFilterIn.Add( Method.RelatedNodeId );

            MethodConditionAndCharacteristicsView.AddViewRelationship( MethodAsRootRelationship,
                                                                       CswEnumNbtViewPropOwnerType.Second,
                                                                       MethodConditionMethodOCP,
                                                                       false );
            MethodConditionAndCharacteristicsView.AddViewRelationship( MethodAsRootRelationship,
                                                                       CswEnumNbtViewPropOwnerType.Second,
                                                                       MethodCharacteristicMethodOCP,
                                                                       false );

            ICswNbtTree MethodConditionAndCharacteristicsTree = _CswNbtResources.Trees.getTreeFromView( MethodConditionAndCharacteristicsView, false, true, true, true );
            if( MethodConditionAndCharacteristicsTree.getChildNodeCount() > 0 )
            {
                MethodConditionAndCharacteristicsTree.goToNthChild( 0 );

                for( int i = 0; i < MethodConditionAndCharacteristicsTree.getChildNodeCount(); i++ )
                {
                    MethodConditionAndCharacteristicsTree.goToNthChild( i );
                    CswNbtNode thisNode = MethodConditionAndCharacteristicsTree.getNodeForCurrentPosition();

                    CswEnumNbtObjectClass thisNodeOC = thisNode.getObjectClass().ObjectClass;

                    if( thisNodeOC == CswEnumNbtObjectClass.MethodConditionClass)
                    {
                        if( CertDefConditionDefaultNT != null )
                        {
                        CswNbtObjClassMethodCondition thisMethodCondition = thisNode;
                        CswNbtObjClassCertDefCondition correspondingCertDefCondition = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CertDefConditionDefaultNT.NodeTypeId );

                        correspondingCertDefCondition.MethodCondition.RelatedNodeId = thisMethodCondition.NodeId;
                        correspondingCertDefCondition.CertDefSpec.RelatedNodeId = this.NodeId;
                        
                        correspondingCertDefCondition.postChanges( false );

                           
                        }

                        else
                        {
                            throw new CswDniException(CswEnumErrorType.Error,
                                                      "A CertDef Condition corresponding to an existing Method Condition could not be created because there is no CertDef Condition NodeType",
                                                      "CertDef Condition NodeType not found, so CertDef Condition cannot be created");
                        }
                    }

                    else if( thisNodeOC == CswEnumNbtObjectClass.MethodCharacteristicClass)
                    {
                        if( CertDefConditionDefaultNT != null )
                        {
                        CswNbtObjClassMethodCharacteristic thisMethodCharacteristic = thisNode;
                        CswNbtObjClassCertDefCharacteristicLimit correspondingCertDefCharacteristic = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CertDefCharacteristicDefaultNT.NodeTypeId );

                        correspondingCertDefCharacteristic.MethodCharacteristic.RelatedNodeId = thisMethodCharacteristic.NodeId;
                        correspondingCertDefCharacteristic.CertDefSpec.RelatedNodeId = this.NodeId;

                        correspondingCertDefCharacteristic.postChanges( false );

                        }
                        else 
                        {
                            throw new CswDniException(CswEnumErrorType.Error,
                                                      "A CertDef Characteristic Limit corresponding to an existing Method Characteristic Limit could not be created because there is no CertDef Characteristic Limit NodeType",
                                                      "CertDef Characteristic Limit NodeType not found, so CertDef Characteristic Limit cannot be created");
                        }
                    }

                    MethodConditionAndCharacteristicsTree.goToParentNode();

                }
            }
           
        }

       #endregion

        #region Object class specific properties

        public CswNbtNodePropText NameForTestingConditions
        {
            get { return _CswNbtNode.Properties[PropertyName.NameForTestingConditions]; }
        }
        public CswNbtNodePropRelationship Method
        {
            get { return _CswNbtNode.Properties[PropertyName.Method]; }
        }
        public CswNbtNodePropRelationship CertDef
        {
            get { return _CswNbtNode.Properties[PropertyName.CertDef]; }
        }
        public CswNbtNodePropGrid Conditions
        {
            get { return _CswNbtNode.Properties[PropertyName.Conditions]; }
        }
        public CswNbtNodePropGrid Characteristics
        {
            get { return _CswNbtNode.Properties[PropertyName.Characteristics]; }
        }

        #endregion

    }//CswNbtObjClassMethod

}//namespace ChemSW.Nbt.ObjClasses