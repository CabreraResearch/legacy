using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassJurisdiction : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Format = "Format";
        }

        public CswNbtObjClassJurisdiction( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.JurisdictionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassJurisdiction
        /// </summary>
        public static implicit operator CswNbtObjClassJurisdiction( CswNbtNode Node )
        {
            CswNbtObjClassJurisdiction ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.JurisdictionClass ) )
            {
                ret = (CswNbtObjClassJurisdiction) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropList Format { get { return _CswNbtNode.Properties[PropertyName.Format]; } }

        #endregion

    }//CswNbtObjClassJurisdiction

}//namespace ChemSW.Nbt.ObjClasses
