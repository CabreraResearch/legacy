
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
namespace ChemSW.Nbt.ImportExport
{

    public class CswNbtMetaDataForSpreadSheetCol
    {
        public CswNbtMetaDataNodeType CswNbtMetaDataNodeType = null;
        public CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp = null;

        public bool IsValid
        {
            get { return ( ( null != CswNbtMetaDataNodeTypeProp ) && ( null != CswNbtMetaDataNodeType ) ); }
        }

        private List<string> _FieldTypeColNames = new List<string>();
        public List<string> FieldTypeColNames
        {
            get
            {
                if( ( null != CswNbtMetaDataNodeTypeProp ) && ( 0 == _FieldTypeColNames.Count ) )
                {

                    ICswNbtFieldTypeRule CswNbtFieldTypeRule = CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
                    CswNbtSubFieldColl CswNbtSubFieldColl = CswNbtFieldTypeRule.SubFields;
                    foreach( CswNbtSubField CurrentSubField in CswNbtSubFieldColl )
                    {
                        _FieldTypeColNames.Add( CurrentSubField.ToXmlNodeName() );
                    }
                }//if we haven't populated yet

                return ( _FieldTypeColNames );
            }//get

        }//FieldTypeColNames

    } // CswNbtMetaDataForSpreadSheetCol


} // namespace ChemSW.Nbt
