using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDocument : CswNbtPropertySetDocument
    {
        #region Enums
        /// <summary>
        /// Object Class Property Names
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetDocument.PropertyName
        {
            /// <summary>
            /// Expiration Date, if any
            /// </summary>
            public const string ExpirationDate = "Expiration Date";
        }

        #endregion Enums

        #region Base

        public CswNbtObjClassDocument( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDocument
        /// </summary>
        public static implicit operator CswNbtObjClassDocument( CswNbtNode Node )
        {
            CswNbtObjClassDocument ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DocumentClass ) )
            {
                ret = (CswNbtObjClassDocument) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassDocument fromPropertySet( CswNbtPropertySetDocument PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetDocument toPropertySet( CswNbtObjClassDocument ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Inherited Events

        //Extend CswNbtPropertySetDocument events here

        #endregion Inherited Events

        #region Object class specific properties

        public CswNbtNodePropDateTime ExpirationDate { get { return _CswNbtNode.Properties[PropertyName.ExpirationDate]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassDocument

}//namespace ChemSW.Nbt.ObjClasses