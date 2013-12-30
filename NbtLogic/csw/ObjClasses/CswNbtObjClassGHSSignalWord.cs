using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGHSSignalWord: CswNbtPropertySetPhrase
    {
        #region Enums
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtPropertySetPhrase.PropertyName
        {

        }

        #endregion Enums

        public CswNbtObjClassGHSSignalWord( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSSignalWordClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a GHSSignalWordClass
        /// </summary>
        public static implicit operator CswNbtObjClassGHSSignalWord( CswNbtNode Node )
        {
            CswNbtObjClassGHSSignalWord ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.GHSSignalWordClass ) )
            {
                ret = (CswNbtObjClassGHSSignalWord) Node.ObjClass;
            }
            return ret;
        }

        #region Property Set Methods

        //Extend CswNbtPropertySetPhase events here

        #endregion

        #region Object class specific properties


        #endregion


    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
