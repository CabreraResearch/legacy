using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Grid.ExtJs
{
    [DataContract]
    public class CswNbtGridExtJsColumn
    {
        /// <summary>
        /// Display name for the column
        /// </summary>
        [DataMember( EmitDefaultValue = false, IsRequired = false )]
        public string header;
        /// <summary>
        /// Internal column name (matches field definition's dataIndex)
        /// </summary>
        public CswNbtGridExtJsDataIndex dataIndex;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "dataIndex" )]
        public string ExtDataIndex
        {
            get { return dataIndex.ToString(); }
            set { dataIndex = new CswNbtGridExtJsDataIndex( "", value ); }
        }

        /// <summary>
        /// Width in pixels (default is 100)
        /// </summary>
        [DataMember( EmitDefaultValue = false, IsRequired = false )]
        public Int32 width;
        
        /// <summary>
        /// Whether the column can be resized
        /// </summary>
        [DataMember( EmitDefaultValue = false, IsRequired = false )]
        public bool resizable = true;
        
        /// <summary>
        /// Whether this column will widen to encompass all extra space in the grid
        /// </summary>
        [DataMember( EmitDefaultValue = false, IsRequired = false )]
        public bool flex = false;

        /// <summary>
        /// Whether the menu is enabled for this column
        /// </summary>
        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "menuDisabled" )] 
        public bool MenuDisabled;
        
        /// <summary>
        /// Hide this column (can be manually displayed on client)
        /// </summary>
        [DataMember( EmitDefaultValue = false, IsRequired = false )]
        public bool hidden = false;
        
        /// <summary>
        /// Determine whether the column can be manually hidden/displayed on client
        /// </summary>
        [DataMember( EmitDefaultValue = false, IsRequired = false )]
        public bool hideable = true;
        
        /// <summary>
        /// Data type for grid column
        /// </summary>
        public extJsXType xtype = extJsXType.Unknown;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "xtype" )]
        public string ExtJsXType
        {
            get { return xtype.ToString(); }
            set { xtype = (extJsXType) value; }
        }


        /// <summary>
        /// Date format (if xtype == datecolumn)
        /// </summary>
        [DataMember( EmitDefaultValue = false, IsRequired = false )]
        public string dateformat;

        public JObject ToJson()
        {
            JObject Jcol = new JObject();
            Jcol["id"] = dataIndex.safeString();
            Jcol["header"] = header;
            Jcol["dataIndex"] = dataIndex.ToString();
            if( width > 0 )
            {
                Jcol["width"] = width;
            }
            if( flex )
            {
                Jcol["flex"] = flex;
            }
            if( false == resizable )
            {
                Jcol["resizable"] = resizable;
            }
            if( hidden )
            {
                Jcol["hidden"] = hidden;
            }
            if( false == hideable )
            {
                Jcol["hideable"] = hideable;
            }
            if( xtype != extJsXType.Unknown )
            {
                Jcol["xtype"] = xtype.ToString();
            }
            if( dateformat != string.Empty )
            {
                Jcol["format"] = dateformat;
            }
            return Jcol;
        } // ToJson()

    } // class CswNbtGridExtJsColumn

} // namespace ChemSW.Nbt.Grid.ExtJs
