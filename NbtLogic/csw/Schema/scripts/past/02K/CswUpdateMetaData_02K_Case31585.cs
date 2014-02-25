using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateMetaData_02K_Case31585 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31585; }
        }

        public override string Description
        {
            get { return "Make Design NTP NodeType ServerManaged"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DesignNodeTypePropOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
            CswNbtMetaDataObjectClassProp DesignNTPNodeTypeValueOCP = DesignNodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DesignNTPNodeTypeValueOCP, CswEnumNbtObjectClassPropAttributes.servermanaged, true );
        } // update()


    }//class CswUpdateMetaData_02K_Case31585

}//namespace ChemSW.Nbt.Schema