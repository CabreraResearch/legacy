using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Printer Object Class
    /// </summary>
    public class CswNbtObjClassPrinter : CswNbtObjClass
    {
        /// <summary>
        /// Property names on the Printer class
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Description = "Description";
            public const string Enabled = "Enabled";
            public const string LastJobRequest = "Last Job Request";
            public const string Jobs = "Jobs";
        }

        public CswNbtObjClassPrinter( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrinterClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassPrinter
        /// </summary>
        public static implicit operator CswNbtObjClassPrinter( CswNbtNode Node )
        {
            CswNbtObjClassPrinter ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.PrinterClass ) )
            {
                ret = (CswNbtObjClassPrinter) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtMetaDataObjectClassProp EnabledOcp = ObjectClass.getObjectClassProp( PropertyName.Enabled );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, EnabledOcp, Value: CswEnumTristate.True.ToString(), ShowInGrid: false );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropMemo Description { get { return _CswNbtNode.Properties[PropertyName.Description]; } }
        public CswNbtNodePropLogical Enabled { get { return _CswNbtNode.Properties[PropertyName.Enabled]; } }
        public CswNbtNodePropDateTime LastJobRequest { get { return _CswNbtNode.Properties[PropertyName.LastJobRequest]; } }
        public CswNbtNodePropGrid Jobs { get { return _CswNbtNode.Properties[PropertyName.Jobs]; } }

        #endregion

    }//CswNbtObjClassPrinter

}//namespace ChemSW.Nbt.ObjClasses
