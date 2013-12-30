using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGHSClassification : CswNbtPropertySetPhrase
    {
        #region Enums
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetPhrase.PropertyName
        {
            public const string Category = "Category";
        }

        #endregion Enums

        public CswNbtObjClassGHSClassification( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClassificationClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGHSClassification
        /// </summary>
        public static implicit operator CswNbtObjClassGHSClassification( CswNbtNode Node )
        {
            CswNbtObjClassGHSClassification ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.GHSClassificationClass ) )
            {
                ret = (CswNbtObjClassGHSClassification) Node.ObjClass;
            }
            return ret;
        }

        #region Property Set Methods

        //Extend CswNbtPropertySetPhase events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }

        #endregion

    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
