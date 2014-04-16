using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCertificateDefinition : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string CertDefId = "CertDef Id";
            public const string Material = "Material";
            public const string Version = "Version";
            public const string RetainCount = "Retain Count";
            public const string RetainQuantity = "Retain Quantity";
            public const string RetainExpiration = "Retain Expiration"; //Unit = Years
            public const string Approved = "Approved";
            public const string ApprovedDate = "Approved Date";
            public const string QualifiedManufacturerOnly = "Qualified Manufacturer Only"; // Multilist of Levels
            public const string CertDefSpecs = "CertDef Specs";
            public const string NewDraft = "New Draft";
            public const string CurrentApproved = "Current Approved";
            public const string Obsolete = "Obsolete";
            public const string Versions = "Versions"; // Grid of other versions of this CertDef

        }

        public CswNbtObjClassCertificateDefinition( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CertificateDefinitionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassManufacturer
        /// </summary>
        public static implicit operator CswNbtObjClassCertificateDefinition( CswNbtNode Node )
        {
            CswNbtObjClassCertificateDefinition ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CertificateDefinitionClass ) )
            {
                ret = (CswNbtObjClassCertificateDefinition) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here
        protected override void afterPopulateProps()
        {
            base.afterPopulateProps();
            QualifiedManufacturerOnly.InitOptions = _initQualifiedManufacturerOnlyOptions;
        }

        protected override void afterPromoteNodeLogic()
        {
            //TODO: Remove when we fix CIS-53405
            CertDefId.setReadOnly( value: true, SaveToDb: true );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText CertDefId { get { return _CswNbtNode.Properties[PropertyName.CertDefId]; } }
        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropNumber Version { get { return _CswNbtNode.Properties[PropertyName.Version]; } }
        public CswNbtNodePropNumber RetainCount { get { return _CswNbtNode.Properties[PropertyName.RetainCount]; } }
        public CswNbtNodePropQuantity RetainQuantity { get { return _CswNbtNode.Properties[PropertyName.RetainQuantity]; } }
        public CswNbtNodePropQuantity RetainExpiration { get { return _CswNbtNode.Properties[PropertyName.RetainExpiration]; } }
        public CswNbtNodePropLogical Approved { get { return _CswNbtNode.Properties[PropertyName.Approved]; } }
        public CswNbtNodePropDateTime ApprovedDate { get { return _CswNbtNode.Properties[PropertyName.ApprovedDate]; } }
        public CswNbtNodePropMultiList QualifiedManufacturerOnly { get { return _CswNbtNode.Properties[PropertyName.QualifiedManufacturerOnly]; } }
        public CswNbtNodePropGrid CertDefSpecs { get { return _CswNbtNode.Properties[PropertyName.CertDefSpecs]; } }
        public CswNbtNodePropButton NewDraft { get { return _CswNbtNode.Properties[PropertyName.NewDraft]; } }
        public CswNbtNodePropLogical CurrentApproved { get { return _CswNbtNode.Properties[PropertyName.CurrentApproved]; } }
        public CswNbtNodePropLogical Obsolete { get { return _CswNbtNode.Properties[PropertyName.Obsolete]; } }
        public CswNbtNodePropGrid Versions { get { return _CswNbtNode.Properties[PropertyName.Versions]; } }

        #endregion

        #region Private Helper Methods

        private Dictionary<string, string> _initQualifiedManufacturerOnlyOptions()
        {
            Dictionary<string, string> Ret = new Dictionary<string, string>();

            CswNbtMetaDataObjectClass LevelOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.Level );
            Dictionary<CswPrimaryKey, string> Levels = LevelOC.getNodeIdAndNames( false, false );
            Ret = Levels.Keys.ToDictionary( pk => pk.ToString(), pk => Levels[pk] );

            return Ret;
        } // _initQualifiedManufacturerOnlyOptions()

        #endregion

    }//CswNbtObjClassManufacturer

}//namespace ChemSW.Nbt.ObjClasses
