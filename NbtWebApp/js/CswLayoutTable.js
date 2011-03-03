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
                        var $table = $.CswTable({ 
                                                  ID: o.ID + '_tbl', 
                                                  TableCssClass: 'CswLayoutTable_table',
                                                  CellCssClass: 'CswLayoutTable_cell'
                                                })
                                       .appendTo($div)
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
                        var tablemaxrows = $table.CswTable('maxrows');
                        var tablemaxcolumns = $table.CswTable('maxcolumns');
                        
                        // add an extra row and column
                        var $newcell = getCell($table, (tablemaxrows / cellsetrows ) + 1, (tablemaxcolumns / cellsetcolumns) + 1, cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
                        $table.CswTable('finish', function($cell, row, column) {

                                $cell.append('&nbsp;');

                                treatCell($table, $cell, 
                                          Math.ceil(row / cellsetrows), 
                                          Math.ceil(column / cellsetcolumns), 
                                          parseInt(parseInt(cellsetrows - row % cellsetrows)),
                                          parseInt(parseInt(cellsetcolumns - column % cellsetcolumns)),
                                          cellsetrows,
                                          cellsetcolumns);
                            });
                    }
        };

        function getCell($table, row, column, cellsetrow, cellsetcolumn, cellsetrows, cellsetcolumns)
        {
            var realrow = ((row - 1) * cellsetrows) + cellsetrow;
            var realcolumn = ((column - 1) * cellsetcolumns) + cellsetcolumn;
            var $cell = $table.CswTable('cell', realrow, realcolumn);

            treatCell($table, $cell, row, column, cellsetrow, cellsetcolumn, cellsetrows, cellsetcolumns)

            return $cell;
        }

        function treatCell($table, $cell, row, column, cellsetrow, cellsetcolumn, cellsetrows, cellsetcolumns)
        {
            $cell.attr('row', row)
                 .attr('column', column)
                 .attr('cellsetrow', cellsetrow)
                 .attr('cellsetcolumn', cellsetcolumn)
                 .drag(function(ev, dd) { onDrag(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); })
                 .drop(function(ev, dd) { onDrop(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); })
                 .hover(onHoverIn, onHoverOut);
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
            $table.CswTable('findCell', '.CswLayoutTable_swapcell')
                  .removeClass('CswLayoutTable_swapcell');

            var $cells = $table.CswTable('findCell', '[row="'+ row +'"][column="'+ column +'"]');
            $cells.addClass('CswLayoutTable_dragcell');

            var $swapcells = getSwapCells($table, row, column, cellsetrows, cellsetcolumns, dd);
            if($swapcells.length > 0)
            {
                $swapcells.addClass('CswLayoutTable_swapcell');
            }
        } // onDrag

        function onDrop(ev, dd, $table, row, column, cellsetrows, cellsetcolumns)
        { 
            $table.CswTable('findCell', '.CswLayoutTable_swapcell')
                  .removeClass('CswLayoutTable_swapcell');

            var $cells = $table.CswTable('findCell', '[row="'+ row +'"][column="'+ column +'"]');
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
            return $table.CswTable('findCell', '[row="'+ $hovercell.attr('row') +'"][column="'+ $hovercell.attr('column') +'"]');
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

