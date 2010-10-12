using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using System.Xml;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropLocationContents : CswNbtNodeProp
    {

        public CswNbtNodePropLocationContents( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.LocationContents )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropLocationContents() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }
        }//CswNbtNodePropLocationContents()

        override public bool Empty
        {
            get { return ( 0 == Gestalt.Length ); }
        }

        override public string Gestalt
        {
            get { return _CswNbtNodePropData.Gestalt; }
        }//Gestalt

        public CswNbtView View
        {
            get
            {
                CswNbtView Ret = null;
                if( _CswNbtMetaDataNodeTypeProp.ViewId != Int32.MinValue )
                    Ret = (CswNbtView) CswNbtViewFactory.restoreView( _CswNbtResources, _CswNbtMetaDataNodeTypeProp.ViewId );
                return Ret;
            }
        }

        public override void ToXml( XmlNode Parent )
        {
            // Nothing to save
        }
        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }
    
    }

}//namespace 
