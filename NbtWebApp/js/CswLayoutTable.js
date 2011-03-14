; (function ($) {
    $.fn.CswLayoutTable = function (method) {
        var PluginName = "CswLayoutTable";

        var methods = {
            'init': function(options) {
                        var o = {
                            ID: '',
                            cellset: { rows: 1, columns: 1 },
                            onSwap: function(e, onSwapData){ }
                        };
                        if (options) {
                            $.extend(o, options);
                        }
                        var $parent = $(this);
                        var $table = $parent.CswTable('init', { 
                                                  'ID': o.ID + '_tbl', 
                                                  'TableCssClass': 'CswLayoutTable_table',
                                                  'CellCssClass': 'CswLayoutTable_cell',
                                                  'onCreateCell': function(ev, $table, $newcell, realrow, realcolumn) { 
                                                                    onCreateCell($table, $newcell, realrow, realcolumn, o.cellset.rows, o.cellset.columns);
                                                                  }
                                                })
                                       .attr('cellset_rows', o.cellset.rows)
                                       .attr('cellset_columns', o.cellset.columns);

                        setConfigMode($table, 'false');
                        $table.bind('CswLayoutTable_onSwap', o.onSwap);
                        return $table;
                    },

            'cellset': function(row, column) {
                        var $table = $(this);
                        return _getCellSet($table, row, column);
                    },

            'toggleConfig': function() {
                        var $table = $(this);
                        if(isConfigMode($table))
                        {
                            $table.CswTable('findCell', '.CswLayoutTable_cell')
                                .removeClass('CswLayoutTable_configcell');

                            setConfigMode($table, 'false');
                        } else {
                            var cellsetrows = parseInt($table.attr('cellset_rows'));
                            var cellsetcolumns = parseInt($table.attr('cellset_columns'));
                            var tablemaxrows = $table.CswTable('maxrows');
                            var tablemaxcolumns = $table.CswTable('maxcolumns');
                        
                            // add an extra row and column
                            //var $newcell = getCell($table, (tablemaxrows / cellsetrows ) + 1, (tablemaxcolumns / cellsetcolumns) + 1, cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
                            
                            $table.CswTable('finish', null);
                            
                            $table.CswTable('findCell', '.CswLayoutTable_cell')
                                .addClass('CswLayoutTable_configcell');

                            setConfigMode($table, 'true');
                        }
                    },
            
            'isConfig': function() {
                        var $table = $(this);
                        return isConfigMode($table);
                    }
        };

        function _getCellSet($table, row, column)
        {
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
        }


        function isConfigMode($table)
        {
            return ($table.attr('configmode') == "true");
        }
        function setConfigMode($table, mode)
        {
            $table.attr('configmode', mode);
        }

        function getCell($table, row, column, cellsetrow, cellsetcolumn, cellsetrows, cellsetcolumns)
        {
            var realrow = ((row - 1) * cellsetrows) + cellsetrow;
            var realcolumn = ((column - 1) * cellsetcolumns) + cellsetcolumn;
            var $cell = $table.CswTable('cell', realrow, realcolumn);
            return $cell;
        }

        function onCreateCell($table, $cell, realrow, realcolumn, cellsetrows, cellsetcolumns)
        {
            var row = Math.ceil(realrow / cellsetrows);
            var column = Math.ceil(realcolumn / cellsetcolumns);
            var cellsetrow = parseInt(cellsetrows - realrow % cellsetrows);
            var cellsetcolumn = parseInt(cellsetcolumns - realcolumn % cellsetcolumns);

            $cell.attr('row', row)
                 .attr('column', column)
                 .attr('cellsetrow', cellsetrow)
                 .attr('cellsetcolumn', cellsetcolumn)
                 .drag(function(ev, dd) { onDrag(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); })
                 .drop(function(ev, dd) { onDrop(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); })
                 .hover(function(ev, dd) { onHoverIn(ev, dd, $table, $(this)); },
                        function(ev, dd) { onHoverOut(ev, dd, $table, $(this)); } );
        }

        function onHoverIn(ev, dd, $table, $cell)
        {
            if(isConfigMode($table))
                $cell.addClass('CswLayoutTable_hover');
        }
        function onHoverOut(ev, dd, $table, $cell)
        {
            if(isConfigMode($table))
                $cell.removeClass('CswLayoutTable_hover');
        }

        function onDrag(ev, dd, $table, row, column, cellsetrows, cellsetcolumns) 
        {
            if(isConfigMode($table))
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
            }
        } // onDrag

        function onDrop(ev, dd, $table, row, column, cellsetrows, cellsetcolumns)
        { 
            if(isConfigMode($table))
            {
                $table.CswTable('findCell', '.CswLayoutTable_swapcell')
                      .removeClass('CswLayoutTable_swapcell');

                var $cells = $table.CswTable('findCell', '[row="'+ row +'"][column="'+ column +'"]');
                $cells.removeClass('CswLayoutTable_dragcell');

                var $swapcells = getSwapCells($table, row, column, cellsetrows, cellsetcolumns, dd);
                if($swapcells.length > 0)
                {
                    // This must happen BEFORE we do the swap, in case the caller relies on the contents of the div being where it was
                    $table.trigger('CswLayoutTable_onSwap', { 
                                            table: $table,
                                            cellset: _getCellSet($table, row, column),
                                            swapcellset: _getCellSet($table, $swapcells.first().attr('row'), $swapcells.first().attr('column')),
                                            row: row,
                                            column: column,
                                            swaprow: $swapcells.first().attr('row'),
                                            swapcolumn: $swapcells.first().attr('column')
                                        });
                    

                    var $tempdiv = $('<div />');
                    for(r = 1; r <= cellsetrows; r++)
                    {
                        for(c = 1; c <= cellsetcolumns; c++)
                        {
                            var $cell = $cells.filter('[cellsetrow="'+ r +'"][cellsetcolumn="'+ c +'"]');
                            var $swapcell = $swapcells.filter('[cellsetrow="'+ r +'"][cellsetcolumn="'+ c +'"]');

                            // swap contents
                            $cell.contents().appendTo($tempdiv);
                            $swapcell.contents().appendTo($cell);
                            $tempdiv.contents().appendTo($swapcell);
                        }
                    }
                    

                }
            }
        } // onDrop
        
        function getSwapCells($table, row, column, cellsetrows, cellsetcolumns, dd)
        {
//            // top left cell of each cellset
//            var $cell = $table.CswTable('cell', (row - 1) * cellsetrows + 1, (column - 1) * cellsetcolumns + 1)

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

