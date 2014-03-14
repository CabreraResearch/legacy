using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassManufacturer : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string ManufacturingSite = "Manufacturing Site";
            public const string Qualified = "Qualified";
        }

        public CswNbtObjClassManufacturer( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassManufacturer
        /// </summary>
        public static implicit operator CswNbtObjClassManufacturer( CswNbtNode Node )
        {
            CswNbtObjClassManufacturer ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ManufacturerClass ) )
            {
                ret = (CswNbtObjClassManufacturer) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        protected override void afterPopulateProps()
        {
            Qualified.SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    if( null != ManufacturingSite.RelatedNodeId )
                    {
                        CswNbtObjClassVendor ManufacturingSiteVendor = _CswNbtResources.Nodes[ManufacturingSite.RelatedNodeId];
                        Qualified.setReadOnly( ManufacturingSiteVendor.Internal.Checked != CswEnumTristate.True, true );
                    }
                });
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropRelationship ManufacturingSite { get { return _CswNbtNode.Properties[PropertyName.ManufacturingSite]; } }
        public CswNbtNodePropLogical Qualified { get { return _CswNbtNode.Properties[PropertyName.Qualified]; } }

        #endregion

    }//CswNbtObjClassManufacturer

}//namespace ChemSW.Nbt.ObjClasses
