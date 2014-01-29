using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateSchema_02K_Case31329B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31329; }
        }

        public override string Title
        {
            get { return "Add DesignSequence.CacheSize to add layout; Property sequences have no cache"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            //Add CacheSize to layouts
            CswNbtMetaDataObjectClass DesignSequenceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );
            foreach( CswNbtMetaDataNodeType DesignSequenceNT in DesignSequenceOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp CacheSizeNT = DesignSequenceNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignSequence.PropertyName.CacheSize );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, DesignSequenceNT.NodeTypeId, CacheSizeNT, true );

                CswNbtMetaDataNodeTypeTab FirstTab = DesignSequenceNT.getFirstNodeTypeTab();
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, DesignSequenceNT.NodeTypeId, CacheSizeNT, true, TabId : FirstTab.TabId );
            }

            //Update existing DesignSequences to have a cache of zero (this will also update the sequences in the DB
            foreach( CswNbtObjClassDesignSequence Sequence in DesignSequenceOC.getNodes( false, true, false, true ) )
            {
                Sequence.CacheSize.Value = 0;
                Sequence.postChanges( true ); 
            }

            //Set the CacheSize to default to zero
            CswNbtMetaDataObjectClassProp CacheSizeOCP = DesignSequenceOC.getObjectClassProp( CswNbtObjClassDesignSequence.PropertyName.CacheSize );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CacheSizeOCP, 0, CswNbtFieldTypeRuleNumber.SubFieldName.Value );

        }

    }//class CswUpdateMetaData_02K_Case31517B

}//namespace ChemSW.Nbt.Schema