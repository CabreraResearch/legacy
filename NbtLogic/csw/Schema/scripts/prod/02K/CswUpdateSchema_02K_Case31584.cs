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
    public class CswUpdateSchema_02K_Case31584 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31584; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DesignSequenceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );
            
            // Make 'Name' unique
            CswNbtMetaDataObjectClassProp DesignSequenceNameOCP = DesignSequenceOC.getObjectClassProp( CswNbtObjClassDesignSequence.PropertyName.Name );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DesignSequenceNameOCP, CswEnumNbtObjectClassPropAttributes.isunique, true );

            // Set name template
            foreach( CswNbtMetaDataNodeType DesignSequenceNT in  DesignSequenceOC.getNodeTypes() )
            {
                DesignSequenceNT.DesignNode.NameTemplateText.Text = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignSequence.PropertyName.Name );
                DesignSequenceNT.DesignNode.postChanges( false );
            }
        } // update()


    }//class CswUpdateSchema_02K_Case31584

}//namespace ChemSW.Nbt.Schema