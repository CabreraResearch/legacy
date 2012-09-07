using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Print Label Object Class
    /// </summary>
    public class CswNbtObjClassPrintLabel : CswNbtObjClass
    {
        /// <summary>
        /// Property names on the Print Label class
        /// </summary>
        public sealed class PropertyName
        {
            public const string EplText = "epltext";
            public const string Params = "params";
            public const string NodeTypes = "NodeTypes";
            public const string ControlType = "Control Type";
            public const string LabelName = "Label Name";
        }

        /// <summary>
        /// Supported UI Controls to Print from
        /// </summary>
        public sealed class ControlTypes
        {
            public const string ActiveX = "ActiveX";
            public const string jZebra = "jZebra";
            public static CswCommaDelimitedString Options = new CswCommaDelimitedString { ActiveX, jZebra };
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassPrintLabel( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassPrintLabel
        /// </summary>
        public static implicit operator CswNbtObjClassPrintLabel( CswNbtNode Node )
        {
            CswNbtObjClassPrintLabel ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass ) )
            {
                ret = (CswNbtObjClassPrintLabel) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        
        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo EplText { get { return _CswNbtNode.Properties[PropertyName.EplText]; } }
        public CswNbtNodePropMemo Params { get { return _CswNbtNode.Properties[PropertyName.Params]; } }
        public CswNbtNodePropNodeTypeSelect NodeTypes { get { return _CswNbtNode.Properties[PropertyName.NodeTypes]; } }
        public CswNbtNodePropList ControlType { get { return _CswNbtNode.Properties[PropertyName.ControlType]; } }
        public CswNbtNodePropText LabelName { get { return _CswNbtNode.Properties[PropertyName.LabelName]; } }

        #endregion

    }//CswNbtObjClassPrintLabel

}//namespace ChemSW.Nbt.ObjClasses
