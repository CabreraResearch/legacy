/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../controls/CswGrid.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswAuditHistoryGrid';

    var methods = {
        init: function(options) {
            var o = {
                Url: '/NbtWebApp/wsNBT.asmx/getAuditHistoryGrid',
                ID: '',
                nodeid: '',
                EditMode: EditMode.Edit.name,
                JustDateColumn: false,
                onEditRow: null, //function(date) {},
                onSelectRow: null, //function(date) {},
                selectedDate: '',
                allowEditRow: true
            };
            if(options) $.extend(o, options);

            var $Div = $(this);
            $Div.contents().remove();

            CswAjaxJson({
                url: o.Url,
                data: {
                    NodeId: o.nodeid, 
                    JustDateColumn: o.JustDateColumn 
                },
                success: function(gridJson) {
                
                    var preventSelectTrigger = false;
                    
                    var auditGridId = o.ID + '_csw_auditGrid_outer';
                    var $auditGrid = $Div.find('#' + auditGridId);
                    if (isNullOrEmpty($auditGrid) || $auditGrid.length === 0) {
                        $auditGrid = $('<div id="' + o.ID + '"></div>').appendTo($Div);
                    } else {
                        $auditGrid.empty();
                    }
                    
                    var g = {
                        ID: o.ID,
                        canEdit: isTrue(o.allowEditRow),
                        pagermode: 'default',
                        gridOpts: {
                            height: 180,
                            rowNum: 10,
                            onSelectRow: function(rowid) {
                                if (!preventSelectTrigger && false === isNullOrEmpty(rowid)) {
                                    var cellVal = grid.getValueForColumn('CHANGEDATE', rowid);
                                    if (isFunction(o.onSelectRow)) {
                                        o.onSelectRow(cellVal);
                                    }
                                }
                            },
                            add: false,
                            view: false,
                            del: false,
                            refresh: false,
                            search: false
                        }
                    };

                    $.extend(g.gridOpts, gridJson);

                    if (o.EditMode === EditMode.PrintReport.name) {
                        g.gridOpts.caption = '';
                        g.hasPager = false;
                    }
                    else {
                        g.optNavEdit = {
                            editfunc: function(rowid) {
                                if (false === isNullOrEmpty(rowid)) {
                                    var cellVal = grid.getValueForColumn('CHANGEDATE', rowid);
                                    if (isFunction(o.onEditRow)) {
                                        o.onEditRow(cellVal);
                                    }
                                } else {
                                    $.CswDialog('AlertDialog', 'Please select a row to edit');
                                }
                            }
                        };
                    }
                    
                    var grid = CswGrid(g, $auditGrid);
                    grid.$gridPager.css({ width: '100%', height: '20px' });
                    
                    // set selected row by date
                    
                    if(!isNullOrEmpty(o.selectedDate))
                    {
                        preventSelectTrigger = true;
                        var rowid = grid.getRowIdForVal('CHANGEDATE', o.selectedDate.toString());
                        grid.setSelection(rowid);
                        preventSelectTrigger = false;
                    }
                }
            });


        }
    };
    
    // Method calling logic
    $.fn.CswAuditHistoryGrid = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
