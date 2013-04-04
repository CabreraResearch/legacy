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

        private readonly CswNbtMetaDataResources _CswNbtMetaDataResources;

        public CswNbtMetaDataPropertySet( CswNbtMetaDataResources Resources, DataRow Row )
        {
            _CswNbtMetaDataResources = Resources;
            Reassign( Row );
        }

        public void Reassign( DataRow NewRow )
        {
            _PropertySetRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }

        public CswEnumNbtPropertySetName Name
        {
            get
            {
                return (CswEnumNbtPropertySetName) _DataRow["name"];
            }
        }

        public Int32 PropertySetId
        {
            get { return _UniqueId; }
        }
        
        public string IconFileName
        {
            get { return _DataRow["iconfilename"].ToString(); }
        }

        public CswNbtView CreateDefaultView( bool IncludeDefaultFilters = true )
        {
            CswNbtView DefaultView = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
            DefaultView.ViewName = this.Name;

            CswNbtViewRelationship RelationshipToMe = DefaultView.AddViewRelationship( this, IncludeDefaultFilters );
            return DefaultView;
        }

    }//CswNbtMetaDataPropertySet

}//namespace ChemSW.Nbt.MetaData
