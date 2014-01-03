﻿using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30207
    /// </summary>
    public class CswUpdateSchema_02K_Case30207 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30207; }
        }

        public override void update()
        {
            // RegulatoryListListCodes should be compound unique on the following properties: RegulatoryList, LOLIListCode, and LOLIListName
            CswNbtMetaDataObjectClass RegListListCodeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
            foreach( CswNbtMetaDataNodeType RegListListCodeNT in RegListListCodeOC.getNodeTypes() )
            {
                // LOLI List Code NTP
                CswNbtMetaDataNodeTypeProp RegListListCodeLoliListCodeNTP = RegListListCodeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListCode );
                //RegListListCodeLoliListCodeNTP.setIsCompoundUnique( true );
                RegListListCodeLoliListCodeNTP._DataRow["iscompoundunique"] = CswConvert.ToDbVal( true );
                // Regulatory List NTP
                CswNbtMetaDataNodeTypeProp RegListListCodeRegListNTP = RegListListCodeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.RegulatoryList );
                //RegListListCodeRegListNTP.setIsCompoundUnique( true );
                RegListListCodeRegListNTP._DataRow["iscompoundunique"] = CswConvert.ToDbVal( true );
                // LOLI List Name NTP
                CswNbtMetaDataNodeTypeProp RegListListCodeLoliListNameNTP = RegListListCodeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListName );
                //RegListListCodeLoliListNameNTP.setIsCompoundUnique( true );
                RegListListCodeLoliListNameNTP._DataRow["iscompoundunique"] = CswConvert.ToDbVal( true );
            }

        } // update()

    }//class CswUpdateSchema_02K_Case30207

}//namespace ChemSW.Nbt.Schema