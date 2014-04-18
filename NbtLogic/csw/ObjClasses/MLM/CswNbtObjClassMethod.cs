using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMethod : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string MethodNo = "Method No";
            public const string MethodDescription = "Method Description";
            public const string Obsolete = "Obsolete";
        }

        public CswNbtObjClassMethod( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

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

        protected override void afterPopulateProps()
        {
            base.afterPopulateProps();
            MethodNo.SetOnPropChange( _onAfterMethodNumberChange );
            Obsolete.SetOnPropChange( _onAfterObsoleteChecked ); 
        }

        //Extend CswNbtObjClass events here
        //CIS-52300: MLM2 Create method object class
        //Set default view filter of obsolete to false
        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            base.addDefaultViewFilters( ParentRelationship );
            CswNbtMetaDataObjectClassProp ObsoleteClassProp = ObjectClass.getObjectClassProp( PropertyName.Obsolete );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, ObsoleteClassProp,
                                                              Value: CswEnumTristate.True.ToString(), FilterMode: CswEnumNbtFilterMode.NotEquals );

        }

        #endregion

        #region ObjectClass-Specific Properties

        /// <summary>
        /// When obsolete checkbox is checked, marks the obsolete prop as servermanaged, preventing
        /// further modification of obsolete status. This helps prevent having more than one obsolete
        /// method at the same time
        /// </summary>
        /// <param name="NodeProp"></param>
        /// <param name="Creating"></param>
        private void _onAfterObsoleteChecked( CswNbtNodeProp NodeProp, bool Creating )
        {
            NodeProp.setReadOnly(true, true);
        }

        /// <summary>
        /// Throws dni exception if other non-obsolete methods exist with the same prop number  CIS-52300
        /// </summary>
        /// <param name="NodeProp"></param>
        /// <param name="Creating"></param>
        private void _onAfterMethodNumberChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            string thisMethodNo = MethodNo.Text;

            //create a view of all Method nodes
            CswNbtView MethodView = new CswNbtView( _CswNbtResources );
            MethodView.ViewName = "Check for Duplicate Method View";

            //ignore already obsolete methods
            CswNbtViewRelationship MethodRelationship = MethodView.AddViewRelationship( ObjectClass, true );
            CswNbtMetaDataObjectClassProp MethodNoOCP = ObjectClass.getObjectClassProp( PropertyName.MethodNo );
            MethodView.AddViewPropertyAndFilter( MethodRelationship, MethodNoOCP,
                                                 Value: thisMethodNo,
                                                 FilterMode: CswEnumNbtFilterMode.Equals );

            ICswNbtTree MethodNodesTree = _CswNbtResources.Trees.getTreeFromView( MethodView, false, false, false );
            if( MethodNodesTree.getChildNodeCount() > 0 )
            {
                throw new CswDniException( CswEnumErrorType.Warning, 
                    "Method number must be unique", 
                    "Found existing non-obsolete method with method number: " + thisMethodNo);
            }

        }//_onAfterMethodChange()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText MethodNo { get { return _CswNbtNode.Properties[PropertyName.MethodNo]; } }
        public CswNbtNodePropText MethodDescription { get { return _CswNbtNode.Properties[PropertyName.MethodDescription]; } }
        public CswNbtNodePropLogical Obsolete { get { return _CswNbtNode.Properties[PropertyName.Obsolete]; } }

        #endregion

    }//CswNbtObjClassMethod

}//namespace ChemSW.Nbt.ObjClasses
