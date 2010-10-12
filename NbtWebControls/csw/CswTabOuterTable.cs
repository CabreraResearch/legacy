using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls
{
    public class CswTabOuterTable : CompositeControl
    {
        public CswTabOuterTable()
        {
            EnsureChildControls();
        }

        public TableCell ContentCell { get { return _Center; } }


        private CswAutoTable _TabOuterTable;
        private TableCell _Center;
        protected override void CreateChildControls()
        {
            _TabOuterTable = new CswAutoTable();
            _TabOuterTable.ID = "TabOuterTable";
            _TabOuterTable.CssClass = "TabOuterTable";
            this.Controls.Add( _TabOuterTable );

            TableCell TopLeft = _TabOuterTable.getCell( 0, 0 );
            TopLeft.CssClass = "TabOuterTable_TopLeft";
            TableCell Top = _TabOuterTable.getCell( 0, 1 );
            Top.CssClass = "TabOuterTable_Top";
            TableCell TopRight = _TabOuterTable.getCell( 0, 2 );
            TopRight.CssClass = "TabOuterTable_TopRight";
            TableCell Left = _TabOuterTable.getCell( 1, 0 );
            Left.CssClass = "TabOuterTable_Left";
            _Center = _TabOuterTable.getCell( 1, 1 );
            _Center.CssClass = "TabOuterTable_Center";
            TableCell Right = _TabOuterTable.getCell( 1, 2 );
            Right.CssClass = "TabOuterTable_Right";
            TableCell BottomLeft = _TabOuterTable.getCell( 2, 0 );
            BottomLeft.CssClass = "TabOuterTable_BottomLeft";
            TableCell Bottom = _TabOuterTable.getCell( 2, 1 );
            Bottom.CssClass = "TabOuterTable_Bottom";
            TableCell BottomRight = _TabOuterTable.getCell( 2, 2 );
            BottomRight.CssClass = "TabOuterTable_BottomRight";

            Image BlankImage1 = new Image();
            BlankImage1.ImageUrl = "Images/pagelayout/blank.gif";
            BlankImage1.Width = Unit.Parse( "3px" );
            BlankImage1.Height = Unit.Parse( "5px" );
            TopLeft.Controls.Add( BlankImage1 );

            Image BlankImage2 = new Image();
            BlankImage2.ImageUrl = "Images/pagelayout/blank.gif";
            BlankImage2.Width = Unit.Parse( "4px" );
            BlankImage2.Height = Unit.Parse( "5px" );
            BottomRight.Controls.Add( BlankImage2 );

            base.CreateChildControls();
        }
    }
}
