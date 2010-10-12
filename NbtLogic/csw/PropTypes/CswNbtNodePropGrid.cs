using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Nbt.MetaData;
using System.Xml;
using ChemSW.Core;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropGrid : CswNbtNodeProp
    {

        public CswNbtNodePropGrid(CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp)
            : base(CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp)
        {
        }//generic

        override public bool Empty
        {
            get
            {
                return (0 == Gestalt.Length);
            }//
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//

        }//Gestalt

        public CswNbtView View
        {
            get
            {
                //CswNbtView Ret = new CswNbtView(_CswNbtResources);
                CswNbtView Ret = null;
                if( _CswNbtMetaDataNodeTypeProp.ViewId != Int32.MinValue )
                    //Ret.LoadXml(_CswNbtMetaDataNodeTypeProp.ViewId);
                    Ret = ( CswNbtView ) CswNbtViewFactory.restoreView( _CswNbtResources, _CswNbtMetaDataNodeTypeProp.ViewId );
                return Ret;
            }
        }
        public Int32 Width
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaRows = value;
            //}
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

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
