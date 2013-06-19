using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropGrid : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropGrid( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsGrid;
        }

        public CswNbtNodePropGrid( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            // No subfields
        }//generic

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
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
                if( _CswNbtMetaDataNodeTypeProp.ViewId.isSet() )
                {
                    //Ret.LoadXml(_CswNbtMetaDataNodeTypeProp.ViewId);
                    Ret = _CswNbtResources.ViewSelect.restoreView( _CswNbtMetaDataNodeTypeProp.ViewId );
                    if( false == Ret.SessionViewId.isSet() )
                    {
                        Ret.SaveToCache( false );
                    }
                }
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

        public CswEnumNbtGridPropMode GridMode
        {
            get
            {
                CswEnumNbtGridPropMode Ret = (CswEnumNbtGridPropMode) _CswNbtMetaDataNodeTypeProp.Extended;
                if( Ret == CswEnumNbtGridPropMode.Unknown )
                {
                    Ret = CswEnumNbtGridPropMode.Full;
                }
                return Ret;
            }
        }

        /// <summary>
        /// If set to false, column headers will not be displayed in Small display mode.
        /// </summary>
        public bool HasHeader
        {
            get
            {
                bool ShowHeader = true;
                String Ret = _CswNbtMetaDataNodeTypeProp.Attribute1;
                if( false == String.IsNullOrEmpty( Ret ) )
                {
                    ShowHeader = CswConvert.ToBoolean( Ret );
                }
                return ShowHeader;
            }
        }

        public Int32 MaxRows
        {
            get
            {
                Int32 Ret = CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.MaxValue );
                if( Int32.MinValue == Ret )
                {
                    Ret = 3;
                }
                return Ret;
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public override void ToJSON( JObject ParentObject )
        {
            ParentObject["viewname"] = View.ViewName;
            ParentObject["gridmode"] = GridMode.ToString();
            ParentObject["maxrows"] = MaxRows;
            ParentObject["viewid"] = View.SessionViewId.ToString();
            ParentObject["width"] = Width;
            ParentObject["hasHeader"] = HasHeader;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }

        public override void SyncGestalt()
        {

        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
