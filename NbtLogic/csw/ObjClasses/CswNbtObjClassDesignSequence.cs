using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignSequence : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Pre = "Pre";
            public const string Post = "Post";
            public const string Pad = "Pad";
            public const string NextValue = "Next Value";
        }


        //private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignSequence( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            //_CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        //ctor()

        /// <summary>
        /// This is the object class that OWNS this property (DesignNodeType)
        /// If you want the object class property value, look for ObjectClassProperty
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDesignNodeType
        /// </summary>
        public static implicit operator CswNbtObjClassDesignSequence( CswNbtNode Node )
        {
            CswNbtObjClassDesignSequence ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DesignSequenceClass ) )
            {
                ret = (CswNbtObjClassDesignSequence) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void afterPopulateProps()
        {
            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                CswNbtSequenceValue NextSequenceValue = new CswNbtSequenceValue( _CswNbtResources, RelationalId.PrimaryKey );
                if( null != NextSequenceValue )
                {
                    NextValue.Text = NextSequenceValue.getCurrent();
                }
            }

            //_CswNbtObjClassDefault.triggerAfterPopulateProps();
        } //afterPopulateProps()

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                /*Do Something*/
            }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropText Pre { get { return ( _CswNbtNode.Properties[PropertyName.Pre] ); } }
        public CswNbtNodePropText Post { get { return ( _CswNbtNode.Properties[PropertyName.Post] ); } }
        public CswNbtNodePropNumber Pad { get { return ( _CswNbtNode.Properties[PropertyName.Pad] ); } }
        public CswNbtNodePropText NextValue { get { return ( _CswNbtNode.Properties[PropertyName.NextValue] ); } }

        #endregion

    }//CswNbtObjClassDesignSequence

}//namespace ChemSW.Nbt.ObjClasses
