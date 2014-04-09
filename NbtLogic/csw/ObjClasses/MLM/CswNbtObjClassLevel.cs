using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassLevel: CswNbtObjClass
    {
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string LevelNumber = "Level Number";
            public const string LevelName = "Level Name";
            public const string LevelSuffix = "Level Suffix";
            public const string OnlyQualifiedCofA = "Only Qualified C of A";
            public const string LabUseOnly = "Lab Use Only";
            public const string Enterprise = "Enterprise";
        }

        public CswNbtObjClassLevel( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.Level ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassLevel
        /// </summary>
        public static implicit operator CswNbtObjClassLevel( CswNbtNode Node )
        {
            CswNbtObjClassLevel ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.Level ) )
            {
                ret = (CswNbtObjClassLevel) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText LevelNumber { get { return _CswNbtNode.Properties[PropertyName.LevelNumber]; } }
        public CswNbtNodePropText LevelName { get { return _CswNbtNode.Properties[PropertyName.LevelName]; } }
        public CswNbtNodePropText LevelSuffix { get { return _CswNbtNode.Properties[PropertyName.LevelSuffix]; } }
        public CswNbtNodePropLogical OnlyQualifiedCofA { get { return _CswNbtNode.Properties[PropertyName.OnlyQualifiedCofA]; } }
        public CswNbtNodePropLogical LabUseOnly { get { return _CswNbtNode.Properties[PropertyName.LabUseOnly]; } }
        public CswNbtNodePropLogical Enterprise { get { return _CswNbtNode.Properties[PropertyName.Enterprise]; } }

        #endregion


    }//CswNbtObjClassLevel

}//namespace ChemSW.Nbt.ObjClasses
