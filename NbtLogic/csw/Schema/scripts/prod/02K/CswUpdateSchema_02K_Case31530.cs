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
    /// Schema Update for case 31530
    /// </summary>
    public class CswUpdateSchema_02K_Case31530 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31530; }
        }

        public override void update()
        {
            // Set icons for Design object classes and nodetypes
            CswNbtMetaDataObjectClass DesignTabOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass );
            CswNbtMetaDataObjectClass DesignPropOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
            CswNbtMetaDataObjectClass DesignSequenceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );

            DesignTabOC._DataRow["iconfilename"] = "folder.png";
            DesignPropOC._DataRow["iconfilename"] = "right.png";
            DesignSequenceOC._DataRow["iconfilename"] = "sequence.png";

            foreach( CswNbtMetaDataNodeType TabNT in DesignTabOC.getNodeTypes() )
            {
                TabNT.DesignNode.IconFileName.Value = new CswCommaDelimitedString() {"folder.png"};
                TabNT.DesignNode.postChanges( false );
            }
            foreach( CswNbtMetaDataNodeType PropNT in DesignPropOC.getNodeTypes() )
            {
                PropNT.DesignNode.IconFileName.Value = new CswCommaDelimitedString() { "right.png" };
                PropNT.DesignNode.postChanges( false );
            }
            foreach( CswNbtMetaDataNodeType SequenceNT in DesignSequenceOC.getNodeTypes() )
            {
                SequenceNT.DesignNode.IconFileName.Value = new CswCommaDelimitedString() { "sequence.png" };
                SequenceNT.DesignNode.postChanges( false );
            }

        } // update()


    }//class CswUpdateSchema_02K_Case31530

}//namespace ChemSW.Nbt.Schema