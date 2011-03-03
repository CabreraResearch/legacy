; (function ($) {
    $.fn.CswLayoutTable = function (method) {
        var PluginName = "CswLayoutTable";

        var methods = {
            'init': function(options) {
                        var o = {
                            ID: '',
                            cellset: { rows: 1, columns: 1 }
                        };
                        if (options) {
                            $.extend(o, options);
                        }
                        var $div = $(this);
                        var $table = makeTable(o.ID + '_tbl')
                                       .appendTo($div)
                                       .addClass('CswLayoutTable_table')
                                       .attr('cellset_rows', o.cellset.rows)
                                       .attr('cellset_columns', o.cellset.columns);
                    },

            'cellset': function(row, column) {
                        var $table = $(this).children('table');
                        var cellsetrows = parseInt($table.attr('cellset_rows'));
                        var cellsetcolumns = parseInt($table.attr('cellset_columns'));
                        var cellset = new Array();
                        for(var r = 1; r <= cellsetrows; r++)
                        {
                            for(var c = 1; c <= cellsetcolumns; c++)
                            {
                                if(cellset[r] == undefined) 
                                    cellset[r] = new Array();
                                cellset[r][c] = getCell( $table, row, column, r, c, cellsetrows, cellsetcolumns);
                            }
                        }
                        return cellset;
                    },
            'finish': function() {
                        var $table = $(this).children('table');
                        var cellsetrows = parseInt($table.attr('cellset_rows'));
                        var cellsetcolumns = parseInt($table.attr('cellset_columns'));

                        // find maximum dimensions
                        var $rows = tableFindRow($table, '');
                        var maxcolumns = 0;
                        for(var r = 0; r < $rows.length; r++)
                        {
                            var $columns = tableRowFindCell($($rows[r]), '');
                            if($columns.length > maxcolumns) 
                                maxcolumns = $columns.length;
                        }

                        // fill out all rows and columns, and add an extra row and column
                        for(var r = 1; r <= (($rows.length + 1) / cellsetrows); r++)
                        {
                            for(var c = 1; c <= Math.ceil((maxcolumns + 1) / cellsetcolumns); c++)
                            {
                                for(var csr = 1; csr <= cellsetrows; csr++)
                                {
                                    for(var csc = 1; csc <= cellsetcolumns; csc++)
                                    {
                                        var $thiscell = getCell($table, r, c, csr, csc, cellsetrows, cellsetcolumns);
                                        if($thiscell.contents().length == 0)
                                        {
                                            $thiscell.append('&nbsp;').addClass('CswLayoutTable_emptycell');
                                        }
                                    }
                                }
                            }
                        }
                    }
        };

        function getCell($table, row, column, cellsetrow, cellsetcolumn, cellsetrows, cellsetcolumns)
        {
            var $cell = getTableCell( $table, ((row - 1) * cellsetrows) + cellsetrow, ((column - 1) * cellsetcolumns) + cellsetcolumn);

            $cell.attr('row', row)
                 .attr('column', column)
                 .attr('cellsetrow', cellsetrow)
                 .attr('cellsetcolumn', cellsetcolumn)
                 .addClass('CswLayoutTable_cell')
                 .drag(function(ev, dd) { onDrag(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); })
                 .drop(function(ev, dd) { onDrop(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); })
                 .hover(onHoverIn, onHoverOut);

            return $cell;
        }

        function onHoverIn(ev, dd)
        {
            $(this).addClass('CswLayoutTable_hover');
        }
        function onHoverOut(ev, dd)
        {
            $(this).removeClass('CswLayoutTable_hover');
        }

        function onDrag(ev, dd, $table, row, column, cellsetrows, cellsetcolumns) 
        {
            tableFindCell($table, '.CswLayoutTable_swapcell').removeClass('CswLayoutTable_swapcell');

            var $cells = tableFindCell($table, '[row="'+ row +'"][column="'+ column +'"]');
            $cells.addClass('CswLayoutTable_dragcell');

            var $swapcells = getSwapCells($table, row, column, cellsetrows, cellsetcolumns, dd);
            if($swapcells.length > 0)
            {
                $swapcells.addClass('CswLayoutTable_swapcell');
            }
        } // onDrag

        function onDrop(ev, dd, $table, row, column, cellsetrows, cellsetcolumns)
        { 
            tableFindCell($table, '.CswLayoutTable_swapcell').removeClass('CswLayoutTable_swapcell');

            var $cells = tableFindCell($table, '[row="'+ row +'"][column="'+ column +'"]');
            $cells.removeClass('CswLayoutTable_dragcell');

            var $swapcells = getSwapCells($table, row, column, cellsetrows, cellsetcolumns, dd);
            if($swapcells.length > 0)
            {
                var $tempdiv = $('<div />');
                for(r = 1; r <= cellsetrows; r++)
                {
                    for(c = 1; c <= cellsetcolumns; c++)
                    {
                        var $cell = $cells.filter('[cellsetrow="'+ r +'"][cellsetcolumn="'+ c +'"]');
                        var $swapcell = $swapcells.filter('[cellsetrow="'+ r +'"][cellsetcolumn="'+ c +'"]');

                        $cell.contents().appendTo($tempdiv);
                        $swapcell.contents().appendTo($cell);
                        $tempdiv.contents().appendTo($swapcell);
                    }
                }
            }
        } // onDrop
        
        function getSwapCells($table, row, column, cellsetrows, cellsetcolumns, dd)
        {
//            // top left cell of each cellset
//            var $cell = getTableCell( $table, (row - 1) * cellsetrows + 1, (column - 1) * cellsetcolumns + 1)

//            var thistop = $cell.offset().top;
//            var thisleft = $cell.offset().left;
//            var thisheight = $cell.outerHeight();
//            var thiswidth = $cell.outerWidth();

//            var origrow = parseInt($cell.attr('row'));
//            var origcol = parseInt($cell.attr('column'));
//            var newrow = origrow + Math.round((dd.offsetY - thistop) / (thisheight * cellsetrows));
//            var newcol = origcol + Math.round((dd.offsetX - thisleft) / (thiswidth * cellsetcolumns));

//            return $table.find('td[row="'+ newrow +'"][column="'+ newcol +'"]');
            var $hovercell = $('.CswLayoutTable_hover');
            return tableFindCell($table, '[row="'+ $hovercell.attr('row') +'"][column="'+ $hovercell.attr('column') +'"]');
        }

        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + PluginName);
        }

    }; // function(options) {
})(jQuery);

