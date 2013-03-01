using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28724
    /// </summary>
    public class CswUpdateSchema_01W_Case28724 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28724; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
            {
                if (MaterialNT.NodeTypeName != "Chemical" )
                {
                    CswNbtMetaDataNodeTypeProp CasNoNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( MaterialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.CasNo );
                    CasNoNTP.removeFromAllLayouts();
                    CswNbtMetaDataNodeTypeProp IsTierIINTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( MaterialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.IsTierII );
                    IsTierIINTP.removeFromAllLayouts();
                }
            }
        }//Update()
    }//class CswUpdateSchemaCase_01W_28724
}//namespace ChemSW.Nbt.Schema