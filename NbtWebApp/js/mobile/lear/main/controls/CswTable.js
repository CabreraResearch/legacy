/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />


(function ($) {
    "use strict"; 
    // CswTable
    // Examples:
    //   Make the table
    //     var $table = $ParentDiv.CswTable('init', { ID: 'tableid' });
    //   Use the table
    //     var $cell12 = $table.CswTable('cell', 1, 2);

    var pluginName = "CswTable";
    
    
        
    var methods = {
            
        init: function (options) {
                    var o = {
                        ID: '',
                        prefix: '',
                        suffix: '',
                        TableCssClass: '',
                        CellCssClass: '',
                        cellpadding: 0,
                        cellspacing: 0,
                        align: '',
                        width: '',
                        cellalign: 'top',
                        cellvalign: 'top',
                        onCreateCell: function () { },
                        FirstCellRightAlign: false,
                        OddCellRightAlign: false,
                        border: 0
                    };
                    if (options) {
                        $.extend(o, options);
                    }
                    var elementId = makeId({ID: o.ID, prefix: o.prefix, suffix: o.suffix});
                    var $table = $('<table id="'+ elementId +'"></table>');
                    $table.addClass(o.TableCssClass);
                    $table.CswAttrDom('width', o.width);
                    $table.CswAttrDom('align', o.align);
                    $table.CswAttrDom('cellpadding', o.cellpadding);
                    $table.CswAttrDom('cellspacing', o.cellspacing);
                    $table.CswAttrDom('border', o.border);
                    $table.CswAttrDom('cellalign', o.cellalign);
                    $table.CswAttrDom('cellvalign', o.cellvalign);
                    $table.CswAttrDom('cellcssclass', o.CellCssClass);
                    $table.CswAttrNonDom('FirstCellRightAlign', o.FirstCellRightAlign);
                    $table.CswAttrNonDom('OddCellRightAlign', o.OddCellRightAlign);

                    $table.bind('CswTable_onCreateCell', function(e, $eTable, $cell, row, column) { 
                                                            o.onCreateCell(e, $eTable, $cell, row, column); 
                                                            e.stopPropagation();  // prevents events from triggering in nested tables
                                                            });
                    $table.trigger('CswTable_onCreateCell', [ $table, $table.find('td'), 1, 1 ]);

                    $(this).append($table);

                    return $table;
                },

        // row and col are 1-based
        cell: function (row, col) {
                    var $table = $(this);
                    return getCell($table, row, col);
                },

        maxrows: function() {
                    return getMaxRows($(this));
                },

        maxcolumns: function() {
                    return getMaxColumns($(this));
                },

        finish: function(onEmptyCell) {
                    var $table = $(this);

                    // find maximum dimensions
                    var maxrows = getMaxRows($table);
                    var maxcolumns = getMaxColumns($table);

                    // make missing cells, and add &nbsp; to empty cells
                    for(var r = 1; r <= maxrows; r += 1) {
                        for(var c = 1; c <= maxcolumns; c += 1) {
                            var $cell = getCell($table, r, c);
                            if($cell.contents().length === 0)
                            {
                                if(onEmptyCell !== null)
                                    onEmptyCell($cell, r, c);
                                else
                                    $cell.append('&nbsp;');
                            }
                        }
                    }
                },
            
        // These are safe for nested tables, since using $.find() is not
        findRow: function (criteria) {
            var $table = $(this);
            var $rows = $table.children('tbody').children('tr');
            if (false === isNullOrEmpty(criteria)) {
                $rows = $rows.filter(criteria);
            }
            return $rows;
        },
        findCell: function (criteria) {
            var $table = $(this),
                $retCell;
            
            if(contains(criteria, 'row') &&
               contains(criteria, 'column')) {
                $retCell = $( $table[0].rows[criteria.row].cells[criteria.column] );
            } else {
                var $cells = $table.children('tbody').children('tr').children('td');
                if (false === isNullOrEmpty(criteria)) {
                    $retCell = $cells.filter(criteria);
                }
            }
            return $retCell;
        },
        rowFindCell: function ($row, criteria) {
            //var $table = $(this);
            var $cells = $row.children('td');
            if (false === isNullOrEmpty(criteria)) {
                $cells = $cells.filter(criteria);
            }
            return $cells;
        }

    };

    function getMaxRows($table) {
        var $rows = $table.children('tbody').children('tr');
        return $rows.length;
    }

    function getMaxColumns($table) {
        var $rows = $table.children('tbody').children('tr');
        var maxcolumns = 0;
        for(var r = 0; r < $rows.length; r++)
        {
            var $columns = $($rows[r]).children('td');
            if($columns.length > maxcolumns)
            {
                maxcolumns = $columns.length;
            }
        }
        return maxcolumns;
    }

    // row and col are 1-based
    function getCell($table, row, col) {
        var $cell = null;
        if ($table.length > 0 &&
            row !== undefined && row !== '' &&
            col !== undefined && col !== '') 
        {
            if (row <= 0) {
                log("error: row must be greater than 1, got: " + row);
                row = 1;
            }
            if (col <= 0) {
                log("error: col must be greater than 1, got: " + col);
                col = 1;
            }

            while (row > $table.children('tbody').children('tr').length) 
            {
                $table.append('<tr></tr>');
            }
            var $row = $($table.children('tbody').children('tr')[row-1]);
            while (col > $row.children('td').length) 
            {
                var align = $table.CswAttrDom('cellalign');
                if(($row.children('td').length === 0 && isTrue($table.CswAttrNonDom('FirstCellRightAlign'))) ||
                    ($row.children('td').length % 2 === 0 && isTrue($table.CswAttrNonDom('OddCellRightAlign'))))
                {
                    align = 'right';
                }
                var $newcell = $('<td class="'+ $table.CswAttrDom('cellcssclass') +'" align="'+ align +'" valign="'+ $table.CswAttrDom('cellvalign') +'"></td>')
                                    .appendTo($row);
                $table.trigger('CswTable_onCreateCell', [ $table, $newcell, row, $row.children('td').length ]);
            }
            $cell = $($row.children('td')[col-1]);
        }
        return $cell;
    }

   $.fn.CswTable = function (method) { 
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }
       return this;
    }; // function(method) {
})(jQuery);

