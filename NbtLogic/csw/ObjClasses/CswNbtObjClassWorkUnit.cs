using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassWorkUnit : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string AuditingEnabled = "Auditing Enabled";
            public const string SignatureRequired = "Signature Required";
            public const string Name = "Name";
        }

        public CswNbtObjClassWorkUnit( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassWorkUnit
        /// </summary>
        public static implicit operator CswNbtObjClassWorkUnit( CswNbtNode Node )
        {
            CswNbtObjClassWorkUnit ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.WorkUnitClass ) )
            {
                ret = (CswNbtObjClassWorkUnit) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropNodeTypeSelect AuditingEnabled { get { return ( _CswNbtNode.Properties[PropertyName.AuditingEnabled] ); } }
        public CswNbtNodePropNodeTypeSelect SignatureRequired { get { return ( _CswNbtNode.Properties[PropertyName.SignatureRequired] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }

        #endregion

    }//CswNbtObjClassWorkUnit

}//namespace ChemSW.Nbt.ObjClasses