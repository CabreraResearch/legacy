﻿using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_Case52297C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52297; }
        }

        public override string Title
        {
            get { return "MLM2: Add default NT for CertDef Spec"; }
        }

        public override string AppendToScriptName()
        {
            return "BF";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecClass );

            CswNbtMetaDataNodeType CertDefSpecDefaultNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( CertDefSpecOC )
                {
                    Category = "MLM",
                    ObjectClass = CertDefSpecOC,
                    ObjectClassId = CertDefSpecOC.ObjectClassId,
                    NodeTypeName = "CertDef Spec",
                    Searchable = true,
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefSpec.PropertyName.NameForTestingConditions)
                } );

            CswNbtMetaDataNodeTypeProp ContainerGridPropOnCertDefSpecNTP = CertDefSpecDefaultNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCertDefSpec.PropertyName.Conditions );
            CswNbtView ContainerViewOnCertDefSpec = _CswNbtSchemaModTrnsctn.restoreView( "CertDefConditionsGridOnCertDefSpec" );
            ContainerGridPropOnCertDefSpecNTP.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.View].AsViewReference.ViewId = ContainerViewOnCertDefSpec.ViewId;

        }

    }
}

//namespace ChemSW.Nbt.Schema