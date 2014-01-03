using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGHSPhrase: CswNbtPropertySetPhrase
    {
        #region Enums
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtPropertySetPhrase.PropertyName
        {
            public const string Category = "Category";
        }

        #endregion Enums

        public CswNbtObjClassGHSPhrase( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGHSPhrase
        /// </summary>
        public static implicit operator CswNbtObjClassGHSPhrase( CswNbtNode Node )
        {
            CswNbtObjClassGHSPhrase ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.GHSPhraseClass ) )
            {
                ret = (CswNbtObjClassGHSPhrase) Node.ObjClass;
            }
            return ret;
        }

        #region Property Set Methods

        //Extend CswNbtPropertySetPhase events here

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }

        #endregion

    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
