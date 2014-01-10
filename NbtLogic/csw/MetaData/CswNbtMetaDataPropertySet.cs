using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Represents a Property Set
    /// </summary>
    public class CswNbtMetaDataPropertySet : ICswNbtMetaDataObject, ICswNbtMetaDataDefinitionObject
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
        private CswDateTime _Date;
        public CswNbtMetaDataPropertySet( CswNbtMetaDataResources Resources, DataRow Row, CswDateTime Date = null )
        {
            _CswNbtMetaDataResources = Resources;
            _Date = Date;
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
                return (CswEnumNbtPropertySetName) _DataRow["name"].ToString();
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

        public IEnumerable<CswNbtMetaDataObjectClass> getObjectClasses()
        {
            return _CswNbtMetaDataResources.ObjectClassesCollection.getObjectClassesByPropertySetId( PropertySetId );
        }
        // for ICswNbtMetaDataDefinitionObject
        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypes()
        {
            Collection<CswNbtMetaDataNodeType> ret = new Collection<CswNbtMetaDataNodeType>();
            foreach( CswNbtMetaDataNodeType nt in getObjectClasses().SelectMany( oc => oc.getNodeTypes() ) )
            {
                ret.Add( nt );
            }
            return ret;
        }

        private CswNbtMetaDataObjectClassProp _BarcodeProp = null;
        public ICswNbtMetaDataProp getBarcodeProperty()
        {
            if( null == _BarcodeProp )
            {
                _BarcodeProp = ( from _Prop
                                     in _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropsByPropertySetId( PropertySetId )
                                 where _Prop.getFieldTypeValue() == CswEnumNbtFieldType.Barcode
                                 select _Prop ).FirstOrDefault();
            }
            return _BarcodeProp;
        }
        public bool HasLabel { get { return false; } }

        public IEnumerable<CswNbtMetaDataObjectClassProp> getPropertySetProps()
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropsByPropertySetId( PropertySetId );
        }

    }//CswNbtMetaDataPropertySet

}//namespace ChemSW.Nbt.MetaData
