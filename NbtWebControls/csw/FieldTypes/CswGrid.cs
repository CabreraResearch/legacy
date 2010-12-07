using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;
using ChemSW.Nbt.ObjClasses;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswGrid : CswFieldTypeWebControl, INamingContainer
    {
        //public bool AllowEdit = true;
        //private NodeEditMode _EditMode;
        public CswGrid(CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode)
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswGrid_DataBinding );
            EnsureChildControls();
        }
        
        private CswNbtView _View;
        private void CswGrid_DataBinding(object sender, EventArgs e)
        {
            try
            {
                if( Prop != null )
                {
                    _View = Prop.AsGrid.View;
                    //if(!AllowEdit)
                    //    _View.EditMode = ChemSW.Nbt.GridEditMode.None;

                    if( _View != null )
                    {
                        _Grid.ParentNodeKey = new CswNbtNodeKey( _CswNbtResources, null, string.Empty, Prop.NodeId, NodeSpecies.Plain, Prop.NodeTypeProp.NodeType.NodeTypeId, Prop.NodeTypeProp.NodeType.ObjectClass.ObjectClassId, string.Empty, string.Empty );
                        _Grid.View = _View;
                        _Grid.ReadOnly = ReadOnly;
                    }
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        private string _AddNodeActionPre = "addnode_";


        public override void Save()
        {
            // No saving!
        }
        public override void AfterSave()
        {
        }
        public override void Clear()
        {
        }

        private CswNodesGrid _Grid;
        //private CswAutoTable _Table;
        protected override void CreateChildControls()
        {
            try
            {
                _Grid = new CswNodesGrid( _CswNbtResources );
                _Grid.DisplayViewName = false;
                _Grid.ViewName = String.Empty;
                _Grid.ID = "pgrid";
                _Grid.OnError += new CswErrorHandler( HandleError );
                if( _EditMode != NodeEditMode.LowRes )
                {
                    if( _EditMode != NodeEditMode.PrintReport )   // BZ 8668
                    {
                        _Grid.ShowActionColumns = true;
                        _Grid.DisplayMenu = true;
                        _Grid.Menu.AllowAdd = true;
                        _Grid.Menu.AllowDelete = true;
                        //_Grid.Menu.AllowExport = true; //bz # 7129
                        _Grid.Menu.NbtViewRenderingMode = NbtViewRenderingMode.Grid;
                        _Grid.Menu.AllowPrint = true;
                        _Grid.Menu.AddMenuDoesntChangeView = true;
                        _Grid.Menu.AddMenuDoesntChangeSelectedNode = true;
                    }
                    else
                    {
                        _Grid.Grid.Skin = "ChemSWPrint";
                    }
                }
                else
                {
                    _Grid.ShowActionColumns = false;
                    _Grid.DisplayMenu = false;
                    _Grid.DisplayViewName = false;
                    _Grid.ShowAsHtmlTable = true;
                }

                this.Controls.Add( _Grid );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.CreateChildControls();
        }



    } // class CswGrid
} // namespace ChemSW.NbtWebControls.FieldTypes
