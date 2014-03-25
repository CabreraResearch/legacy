using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMethod : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string MethodNo = "Method No";
            public const string MethodDescription = "Method Description";
            public const string Obsolete = "Method is Obsolete";
        }

        public CswNbtObjClassMethod( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMethod
        /// </summary>
        public static implicit operator CswNbtObjClassMethod( CswNbtNode Node )
        {
            CswNbtObjClassMethod ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MethodClass ) )
            {
                ret = (CswNbtObjClassMethod) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        
        protected override void  afterPopulateProps()
        {
           base.afterPopulateProps();
           MethodNo.SetOnPropChange( _onAfterMethodNumberChange);
        }

        //Extend CswNbtObjClass events here
        //CIS-52300: MLM2 Create method object class
        //Set default view filter of obsolete to false
        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
           base.addDefaultViewFilters( ParentRelationship );
            CswNbtMetaDataObjectClassProp ObsoleteClassProp = ObjectClass.getObjectClassProp( PropertyName.Obsolete );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, ObsoleteClassProp,
                                                              Value: CswEnumTristate.True.ToString(),                                                                  FilterMode: CswEnumNbtFilterMode.NotEquals );

        }

        #endregion

        #region ObjectClass-Specific Properties

        /// <summary>
        /// Throws dni exception if other non-obsolete methods exist with the same prop number  CIS-52300
        /// </summary>
        /// <param name="NodeProp"></param>
        /// <param name="Creating"></param>
        private void _onAfterMethodNumberChange (CswNbtNodeProp NodeProp, bool Creating)
        {
            string thisMethodNo = ((CswNbtNodePropText) NodeProp).Text;

            //create a view of all Method nodes
            CswNbtView MethodView = new CswNbtView(_CswNbtResources);
            MethodView.ViewName = "Check for Duplicate Method View";
            //ignore already obsolete methods
            CswNbtViewRelationship MethodRelationship = MethodView.AddViewRelationship( ObjectClass, true );

            ICswNbtTree MethodNodesTree = _CswNbtResources.Trees.getTreeFromView( MethodView, false, false, false );
            for( int i = 0; i < MethodNodesTree.getChildNodeCount(); i++ )
            {
                MethodNodesTree.goToNthChild( i );
                CswNbtObjClassMethod CurrentNode = MethodNodesTree.getCurrentNode();
                if( CurrentNode.MethodNo.ToString() == thisMethodNo )
                {
                    throw new CswDniException(CswEnumErrorType.Error, "Error: There is an existing non-obsolete method with this method number", "Error: method number must be unique if method is not obsolete");
                }
            }

        }//_onAfterMethodChange()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText MethodNo { get { return _CswNbtNode.Properties[PropertyName.MethodNo]; } }
        public CswNbtNodePropText MethodDescription { get { return _CswNbtNode.Properties[PropertyName.MethodDescription]; } }
        public CswNbtNodePropLogical Obsolete { get { return _CswNbtNode.Properties[PropertyName.Obsolete]; }}

        #endregion

    }//CswNbtObjClassMethod

}//namespace ChemSW.Nbt.ObjClasses
