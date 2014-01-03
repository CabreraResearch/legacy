using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterialSynonym : CswNbtObjClass
    {
        public CswNbtObjClassMaterialSynonym( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialSynonymClass ); }
        }

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string Name = "Name";
            public const string Type = "Type";
            public const string Language = "Language";
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterialSynonym
        /// </summary>
        public static implicit operator CswNbtObjClassMaterialSynonym( CswNbtNode Node )
        {
            CswNbtObjClassMaterialSynonym ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MaterialSynonymClass ) )
            {
                ret = (CswNbtObjClassMaterialSynonym) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropList Type { get { return ( _CswNbtNode.Properties[PropertyName.Type] ); } }
        public CswNbtNodePropList Language { get { return ( _CswNbtNode.Properties[PropertyName.Language] ); } }

        #endregion

    }//CswNbtObjClassMaterialSynonym

}//namespace ChemSW.Nbt.ObjClasses
