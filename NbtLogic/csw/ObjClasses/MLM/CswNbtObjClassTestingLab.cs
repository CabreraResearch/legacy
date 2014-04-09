using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassTestingLab : CswNbtObjClass
    {
        /// <summary>
        /// Property names on the TestingLab class
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string SampleDeliveryRequired = "Sample Delivery Required";
            public const string SampleDeliveryLocation = "Sample Delivery Location";
        }


        public CswNbtObjClassTestingLab( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingLabClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassTestingLab
        /// </summary>
        public static implicit operator CswNbtObjClassTestingLab( CswNbtNode Node )
        {
            CswNbtObjClassTestingLab ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.TestingLabClass ) )
            {
                ret = (CswNbtObjClassTestingLab) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events    
        
        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropLogical SampleDeliveryRequired { get { return _CswNbtNode.Properties[PropertyName.SampleDeliveryRequired]; } }
        public CswNbtNodePropLocation SampleDeliveryLocation { get { return _CswNbtNode.Properties[PropertyName.SampleDeliveryLocation]; } }

        #endregion

    }//CswNbtObjClassTestingLab

}//namespace ChemSW.Nbt.ObjClasses
