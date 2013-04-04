using System;
using System.Data;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Represents a Property Set
    /// </summary>
    public class CswNbtMetaDataPropertySet : ICswNbtMetaDataObject
    {
        private DataRow _PropertySetRow;
        public DataRow _DataRow
        {
            get { return _PropertySetRow; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
        }

        public const string MetaDataUniqueType = "propertysetid";
        public string UniqueIdFieldName { get { return MetaDataUniqueType; } }

        public CswNbtMetaDataPropertySet( CswNbtMetaDataResources Resources, DataRow Row )
        {
            Reassign( Row );
        }

        public void Reassign( DataRow NewRow )
        {
            _PropertySetRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }

        public CswEnumNbtPropertySet Name
        {
            get
            {
                return (CswEnumNbtPropertySet) _DataRow["Name"];
            }
        }

        public Int32 PropertySetId
        {
            get { return _UniqueId; }
        }

    }//CswNbtMetaDataPropertySet

}//namespace ChemSW.Nbt.MetaData
