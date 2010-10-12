using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using System.Xml;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropImage : CswNbtNodeProp
    {

        public CswNbtNodePropImage( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FileNameSubField = ( (CswNbtFieldTypeRuleImage) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).FileNameSubField;
            _ContentTypeSubField = ( (CswNbtFieldTypeRuleImage) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).ContentTypeSubField;
        }

        private CswNbtSubField _FileNameSubField;
        private CswNbtSubField _ContentTypeSubField;

        override public bool Empty
        {
            get
            {
                return ( string.Empty == _CswNbtNodePropData.GetPropRowValue( _FileNameSubField.Column ) ||
                         string.Empty == _CswNbtNodePropData.GetPropRowValue( _ContentTypeSubField.Column ) );
            }
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        //public byte[] BlobData
        //{
        //    get
        //    {
        //        return _CswNbtNodePropData.Row["blobdata"] as byte[];
        //    }
        //    set
        //    {
        //        _CswNbtNodePropData.Row["blobdata"] = value;
        //    }
        //}
        public string FileName
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _FileNameSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _FileNameSubField.Column, value );
            }
        }
        public string ContentType
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _ContentTypeSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _ContentTypeSubField.Column, value );
            }
        }

        public Int32 JctNodePropId
        {
            get
            {
                return _CswNbtNodePropData.JctNodePropId;
            }
        }

        public Int32 Height
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

        public Int32 Width
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaColumns = value;
            //}
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode FileNameNode = CswXmlDocument.AppendXmlNode( ParentNode, _FileNameSubField.Name.ToString(), FileName );
            XmlNode ContentTypeNode = CswXmlDocument.AppendXmlNode( ParentNode, _ContentTypeSubField.Name.ToString(), ContentType );

            // TODO: We need to figure out how we want to do this, for binary data
            // Handle blob data
            //CswTableCaddy JctCaddy = _CswNbtXmlDocServices.CswNbtResources.makeCswTableCaddy( "jct_nodes_props" );
            //JctCaddy.RequireOneRow = true;
            //JctCaddy.AllowBlobColumns = true;
            //JctCaddy.FilterColumn = "jctnodepropid";
            //DataTable JctTable = JctCaddy[CswNbtNodePropWrapper.JctNodePropId].Table;

            //if( !JctTable.Rows[0].IsNull( "blobdata" ) )
            //{
            //    byte[] BlobData = new byte[0];
            //    BlobData = JctTable.Rows[0]["blobdata"] as byte[];

            //    int ArraySize = BlobData.GetUpperBound( 0 );
            //    FileStream fs = new FileStream( _CswNbtXmlDocServices.DocumentPath + CswNbtNodePropWrapper.AsImage.FileName, FileMode.OpenOrCreate, FileAccess.Write );
            //    fs.Write( BlobData, 0, ArraySize );
            //    fs.Close();
            //}
        }
        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            ContentType = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _ContentTypeSubField.Name.ToString() );
            FileName = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _FileNameSubField.Name.ToString() );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            ContentType = CswTools.XmlRealAttributeName( PropRow[_ContentTypeSubField.Name.ToString()].ToString() );
            FileName = CswTools.XmlRealAttributeName( PropRow[_FileNameSubField.Name.ToString()].ToString() );
        }

    }

}//namespace ChemSW.Nbt.PropTypes
