using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDSDPhrase: CswNbtPropertySetPhrase
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

        public CswNbtObjClassDSDPhrase( CswNbtResources CswNbtResources, CswNbtNode Node ): base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DSDPhraseClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDSDPhrase
        /// </summary>
        public static implicit operator CswNbtObjClassDSDPhrase( CswNbtNode Node )
        {
            CswNbtObjClassDSDPhrase ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DSDPhraseClass ) )
            {
                ret = (CswNbtObjClassDSDPhrase) Node.ObjClass;
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
