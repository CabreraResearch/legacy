/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

function CswGrid(options, $parent) {
    ///<summary>Generates a grid</summary>
    ///<param name="options" type="Object">Object defining paramaters for jqGrid construction</param>
    ///<param name="$parent" type="JQuery">Parent element to attach grid to.</param>
    ///<returns type="CswGrid">new CswGrid()</returns>
    var $gridTable;
    var $gridPager;
    var $topPager;
    
    //#region private
    
    (function() {
        var o = {
            canEdit: false,
            canDelete: false,
            hasPager: true,
            gridTableID: 'gridTable',
            gridPagerID: 'gridPager',
            ID: '',
            gridOpts: {
                autoencode: true,
	            //autowidth: true,
                altRows: false,
                caption: '',
                datatype: 'local',
                emptyrecords: 'No Results',
                height: '300',
                loadtext: 'Loading...',
	            //multiselect: false,
                pager: $gridPager,
                rowList: [10, 25, 50],
                rowNum: 10,
                shrinkToFit: true,
                sortname: '',
                sortorder: 'asc',
                toppager: false,
                viewrecords: true,
                width: '600px'
            },
            optSearch: {
                caption: "Search...",
                Find: "Find",
                Reset: "Reset",
                odata: ['equal', 'not equal', 'less', 'less or equal', 'greater', 'greater or equal', 'begins with', 'does not begin with', 'is in', 'is not in', 'ends with', 'does not end with', 'contains', 'does not contain'],
                groupOps: [{ op: "AND", text: "all" }, { op: "OR", text: "any" }],
                matchText: "match",
                rulesText: "rules"
            },
            optNavEdit: {
                edit: true,
                edittext: "",
                edittitle: "Edit row",
                editfunc: null
            },
            optNavDelete: {
                del: true,
                deltext: "",
                deltitle: "Delete row",
                delfunc: null
            },
            optNav: {
                cloneToTop: false,

                add: false,
                del: false,
                edit: false,
						
		        //search
                search: true,
                searchtext: "",
                searchtitle: "Find records",
						
		        //refresh
                refreshtext: "",
                refreshtitle: "Reload Grid",
                alertcap: "Warning",
                alerttext: "Please, select row",
						
		        //view
                view: true,
                viewtext: "",
                viewtitle: "View row"
                //viewfunc: none--use jqGrid built-in function for read-only
            }
        };

        $.extend(true, o, options);

        var gridTableId = makeId({ ID: o.gridTableID, prefix: o.ID });
        if (isNullOrEmpty($parent)) {
            $parent = $('<div id="' + gridTableId + '_parent"></div>');
        }

        $gridTable = $parent.CswTable('init', { ID: gridTableId });

        var gridPagedId = makeId({ ID: o.gridPagerID, prefix: o.ID });
        o.gridOpts.pager = $gridPager = $parent.CswDiv('init', { ID: gridPagedId });

        if (o.canEdit) {
            $.extend(o.optNav, o.optNavEdit);
        }
        if (o.canDelete) {
            $.extend(o.optNav, o.optNavDelete);
        }
        if (o.hasPager) {
            $gridTable.jqGrid(o.gridOpts)
                      .navGrid('#' + $gridPager.CswAttrDom('id'), o.optNav, { }, { }, { }, o.optSearch, { });
        } else {
            $gridTable.jqGrid(o.gridOpts);    
        }
        $topPager = $('#' + $gridTable[0].id + '_toppager')[0];
    })();
    
  	// Row scrolling adapted from 
	// http://stackoverflow.com/questions/2549466/is-there-a-way-to-make-jqgrid-scroll-to-the-bottom-when-a-new-row-is-added/2549654#2549654
    function getGridRowHeight () {
		
        var height = null; // Default
		try{
			height = $gridTable.find('tbody').find('tr:first').outerHeight();
		}
		catch(e){
			//catch and just suppress error
		}
		return height;
	}

    function getCell (rowid, key) {
        var ret = '';
        if(false === isNullOrEmpty(rowid) && false === isNullOrEmpty(key)) {
            ret = $gridTable.jqGrid('getCell', rowid, key);
        }
        return ret;
    }
    
    function getSelectedRowId() {
        var rowid = $gridTable.jqGrid('getGridParam', 'selrow');
        return rowid;
    }
    
    //#region private
    
    //#region public, priveleged
    
    function hideColumn (id) {
        $gridTable.jqGrid('hideCol', id);
    }
    
    function scrollToRow (rowid) {
	    ///<summary>Scrolls the grid to the specified rowid</summary>
        ///<param name="rowid" type="String">Optional. jqGrid rowid. If null, selected row is assumed.</param>
        ///<returns type="Void"></returns>
        if (isNullOrEmpty(rowid)) {
	        rowid = getSelectedRowId();
	    }
        var rowHeight = getGridRowHeight($gridTable) || 23; // Default height
	    var index = $gridTable.getInd(rowid);
	    $gridTable.closest(".ui-jqgrid-bdiv").scrollTop(rowHeight * (index - 1));
    }
    
    function getRowIdForVal(value, column) {
        ///<summary>Gets a jqGrid rowid by column name and value.</summary>
        ///<param name="value" type="String">Cell value</param>
        ///<param name="column" type="String">Column name</param>
        ///<returns type="String">jqGrid row id.</returns>
        var pks = $gridTable.jqGrid('getCol', column, true);
        var rowid = 0;
        for (var pk in pks) {
            if (pks[pk].value.toString() === value.toString())
                rowid = pks[pk].id;
        }
        return rowid;
    }
    
    function getValueForColumn(columnname, rowid) {
        ///<summary>Gets a cell value by column name.</summary>
        ///<param name="columnname" type="String">Grid column name.</param>
        ///<param name="rowid" type="String">Optional. If null, selected row is assumed.</param>
        ///<returns type="String">Value of the cell.</returns>
        if (isNullOrEmpty(rowid)) {
            rowid = getSelectedRowId();
        }
        var ret = getCell(rowid, columnname);
        return ret;
    }
    
    function setSelection(rowid) {
        ///<summary>Sets the selected row by jqGrid's rowid</summary>
        if (false === isNullOrEmpty(rowid)) {
            $gridTable.setSelection(rowid);
        }
    }
    
    this.$gridTable = $gridTable;
    this.$gridPager = $gridPager;
    this.$topPager = $topPager;
    this.getGridRowHeight = getGridRowHeight;
    this.scrollToRow = scrollToRow;
    this.getCell = getCell;
    this.hideColumn = hideColumn;
    this.getRowIdForVal = getRowIdForVal;
    this.setSelection = setSelection;
    this.getValueForColumn = getValueForColumn;
    
    //#region public, priveleged
}