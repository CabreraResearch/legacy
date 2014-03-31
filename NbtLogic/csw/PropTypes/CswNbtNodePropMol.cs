using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropMol: CswNbtNodeProp
    {
        public static readonly string MolImgFileName = "mol.jpeg";
        public static readonly string MolImgFileContentType = "image/jpeg";

        public static implicit operator CswNbtNodePropMol( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsMol;
        }

        public CswNbtNodePropMol( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _MolSubField = ( (CswNbtFieldTypeRuleMol) _FieldTypeRule ).MolSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _MolSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => getMol(), x => setMol( CswConvert.ToString( x ) ) ) );
        }

        private readonly CswNbtSubField _MolSubField;

        override public bool Empty
        {
            get { return MolHasContent; }
        }


        public void setMol( string MolString )
        {
            CswTableUpdate molDataUpdate = _CswNbtResources.makeCswTableUpdate( "NodePropMol.setMol", "mol_data" );
            DataTable molDataTbl = molDataUpdate.getTable( "where jctnodepropid = " + JctNodePropId );

            if( molDataTbl.Rows.Count == 0 )
            {
                DataRow newMolDataRow = molDataTbl.NewRow();
                newMolDataRow["jctnodepropid"] = JctNodePropId;
                newMolDataRow["orginalmol"] = Encoding.UTF8.GetBytes( MolString );
                newMolDataRow["contenttype"] = ".mol";
                newMolDataRow["nodeid"] = this.NodeId.PrimaryKey;
                molDataTbl.Rows.Add( newMolDataRow );
            }
            else
            {
                DataRow existingMolDataRow = molDataTbl.Rows[0];
                existingMolDataRow["orginalmol"] = Encoding.UTF8.GetBytes( MolString );
            }
            molDataUpdate.update( molDataTbl );

            //Update jct_nodes_props to specify whether there's mol content (this part is for making views filterable on this property)
            if( string.Empty != MolString )
            {
                SetPropRowValue( _MolSubField, "1" ); //This means the property has a value
            }
            else
            {
                SetPropRowValue( _MolSubField, string.Empty ); //This means the property doesnt have a value
            }

        }

        public string getMol()
        {
            string ret = string.Empty;
            if( MolHasContent )
            {
                CswTableSelect molDataUpdate = _CswNbtResources.makeCswTableSelect( "NodePropMol.setMol", "mol_data" );
                DataTable molDataTbl = molDataUpdate.getTable( "where jctnodepropid = " + JctNodePropId );
                if( molDataTbl.Rows.Count > 0 && null != molDataTbl.Rows[0]["orginalmol"] )
                {
                    ret = Encoding.UTF8.GetString( molDataTbl.Rows[0]["orginalmol"] as byte[] );
                }
            }
            return ret;
        }

        public bool MolHasContent
        {
            //For NodePropMol the mol subfield just represents if there is a row in Mol_Data.
            get { return null != GetPropRowValue( _MolSubField ); }
            //No setter - setMol() sets this property.
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public static string getLink( Int32 JctNodePropId, CswPrimaryKey NodeId )
        {
            string ret = string.Empty;
            if( JctNodePropId != Int32.MinValue && NodeId != null )
            {
                ret = CswNbtNodePropBlob.getLink( JctNodePropId, NodeId, UseNodeTypeAsPlaceholder : true );
            }
            return ret;
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_MolSubField.ToXmlNodeName( true )] = getMol();
            ParentObject["column"] = _MolSubField.Column.ToString().ToLower();
            ParentObject["href"] = getLink( JctNodePropId, NodeId );
            ParentObject["placeholder"] = "Images/icons/300/_placeholder.gif";
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            setMol( CswTools.XmlRealAttributeName( PropRow[_MolSubField.ToXmlNodeName()].ToString() ) );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_MolSubField.ToXmlNodeName( true )] )
            {
                setMol( JObject[_MolSubField.ToXmlNodeName( true )].ToString() );
            }
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, getMol() );
        }

        /// <summary>
        /// Formats a mol file in the format of: 3 lines with optional text, atom/bond count line, atoms table, bonds table, "M  END"
        /// </summary>
        /// <returns></returns>
        public static string FormatMolFile( string OrginalMolFile )
        {
            //strip out any "$$$$"
            OrginalMolFile = OrginalMolFile.Replace( "$$$$", "" );

            List<string> fixedLines = new List<string>()
                {
                    "", //for optional comment 1
                    "", //for optional comment 2
                    "", //for optional comment 3
                    ""  //for atom/bond count line
                };

            string[] lines = OrginalMolFile.Split( new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None );

            int commentsAdded = 0;
            bool firstAtomTblLine = true;

            for( int i = 0; i < lines.Length; i++ )
            {
                string line = lines[i];
                if( Regex.IsMatch( line, @"^(\s{1,5}(-|)[0-9]{1,3}.[0-9]{3,4}){3}\s[aA-zZ*]{1,2}" ) ) //atom table line
                {
                    fixedLines.Add( line );
                    if( firstAtomTblLine ) //if this is the first AtomTbl line, assume the line before it is the Atom/Bond Count line
                    {
                        firstAtomTblLine = false;
                        fixedLines[3] = lines[i - 1];
                    }
                }
                else if( Regex.IsMatch( line, @"^\s{0,3}([0-9]{1,3})\s{0,2}([1-9]|[1-9][0-9])\s{2}[1-3]\s{2}|\s{0,2}[0-9]{5,6}\s{2}[1-3]\s{2}" ) && false == firstAtomTblLine )
                {
                    fixedLines.Add( line );
                }
                else if( commentsAdded < 3 )
                {
                    fixedLines[commentsAdded] = line;
                    commentsAdded++;
                }
            }
            fixedLines.Add( "M  END" ); //this is always the last line in a Mol file

            string ret = "";
            fixedLines.ForEach( line => ret += line + "\n" );
            return ret;
        }

    }//CswNbtNodePropMol

}//namespace ChemSW.Nbt.PropTypes
