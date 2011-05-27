/// <reference path="../jquery/jquery-1.6.1-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
    $.fn.CswLayoutTable = function (method) {
        var PluginName = "CswLayoutTable";

        var methods = {
            'init': function(options) {
                        var o = {
                            ID: '',
                            cellset: { rows: 1, columns: 1 },
                            onSwap: function(event, onSwapData)
								{ 
									var s = { 
                                        table: '',
                                        cellset: '',
                                        swapcellset: '',
                                        row: '',
                                        column: '',
                                        swaprow: '',
                                        swapcolumn: ''
                                    };
								},
							onAddClick: function() { },
							onConfigOn: function($buttontable) { },
							onConfigOff: function($buttontable) { },
                            onRemove: function(event, onRemoveData)
								{ 
									var r = { 
                                        table: '',
                                        cellset: '',
                                        row: '',
                                        column: ''
                                    };
								},
							TableCssClass: '',
                            CellCssClass: '',
                            cellpadding: 0,
                            cellspacing: 0,
                            width: '',
                            align: '',
							showConfigButton: false,
							showAddButton: false,
							showRemoveButton: false,
							OddCellRightAlign: false,
							ReadOnly: false
						};
                        if (options) {
                            $.extend(o, options);
                        }
                        var $parent = $(this);

                        var $buttondiv = $('<div />')
                                            .appendTo($parent)
                                            .css({ 'float': 'right' });

						if(o.ReadOnly) $buttondiv.hide();

                        var $table = $parent.CswTable('init', { 
                                                  'ID': o.ID, 
                                                  'TableCssClass': o.TableCssClass + ' CswLayoutTable_table',
                                                  'CellCssClass': o.CellCssClass + ' CswLayoutTable_cell',
                                                  'cellpadding': o.cellpadding,
                                                  'cellspacing': o.cellspacing,
												  'OddCellRightAlign': o.OddCellRightAlign,
                                                  'width': o.width,
                                                  'align': o.align,
                                                  'onCreateCell': function(ev, $table, $newcell, realrow, realcolumn) { 
                                                                    onCreateCell($table, $newcell, realrow, realcolumn, o.cellset.rows, o.cellset.columns);
                                                                  }
                                                })
                                       .CswAttrDom('cellset_rows', o.cellset.rows)
                                       .CswAttrDom('cellset_columns', o.cellset.columns);

                        setConfigMode($table, 'false');
                        $table.bind(o.ID + 'CswLayoutTable_onSwap', o.onSwap);
						$table.bind(o.ID + 'CswLayoutTable_onRemove', o.onRemove);
						$table.bind(o.ID + 'CswLayoutTable_onConfigOn', o.onConfigOn);
						$table.bind(o.ID + 'CswLayoutTable_onConfigOff', o.onConfigOff);

						var $buttontable = $buttondiv.CswTable('init', { ID: o.ID + '_buttontbl' });
						if(o.showAddButton)
						{
							$buttontable.CswTable('cell', 1, 1).CswImageButton({
                        							ButtonType: CswImageButton_ButtonType.Add,
                        							AlternateText: 'Add',
                        							ID: o.ID + 'addbtn',
                        							onClick: function ($ImageDiv)
                        							{
                        								o.onAddClick();
														return CswImageButton_ButtonType.None;
                        							}
												}).hide();
                        }
                        if(o.showRemoveButton)
						{
							$buttontable.CswTable('cell', 1, 2).CswImageButton({
                        							ButtonType: CswImageButton_ButtonType.Delete,
                        							AlternateText: 'Remove',
                        							ID: o.ID + 'rembtn',
                        							onClick: function ($ImageDiv)
                        							{
														_toggleRemove($table, $(this));
														return CswImageButton_ButtonType.None;
                        							}
												}).hide();
						}
						if(o.showConfigButton)
						{
							$buttontable.CswTable('cell', 1, 3).CswImageButton({
                                                    ButtonType: CswImageButton_ButtonType.ArrowEast,
                                                    AlternateText: 'Add Column',
                                                    ID: o.ID + 'addcolumnbtn',
                                                    onClick: function ($ImageDiv) 
													{ 
														_addColumn($table);
                                                        return CswImageButton_ButtonType.None; 
                                                    }
                                                }).hide();
							$buttontable.CswTable('cell', 1, 4).CswImageButton({
                                                    ButtonType: CswImageButton_ButtonType.ArrowSouth,
                                                    AlternateText: 'Add Row',
                                                    ID: o.ID + 'addrowbtn',
                                                    onClick: function ($ImageDiv) 
													{ 
														_addRow($table);
                                                        return CswImageButton_ButtonType.None; 
                                                    }
                                                }).hide();
							$buttontable.CswTable('cell', 1, 5).CswImageButton({
                                                    ButtonType: CswImageButton_ButtonType.Configure,
                                                    AlternateText: 'Configure',
                                                    ID: o.ID + 'configbtn',
                                                    onClick: function ($ImageDiv) 
													{ 
														_toggleConfig($table, $buttontable);
                                                        return CswImageButton_ButtonType.None; 
                                                    }
                                                });
                        }
						    
                        return $table;
                    },

            'cellset': function(row, column) 
					{
                        var $table = $(this);
                        return _getCellSet($table, row, column);
                    },

			'isConfig': function ()
					{
						var $table = $(this);
						return isConfigMode($table);
					},

			'toggleConfig': function() 
					{
                        var $table = $(this);
                        var $buttontable = $('#' + $table.CswAttrDom('id') + '_buttontbl');
						_toggleConfig($table, $buttontable);
                    },
            
			'ConfigOn': function() 
					{
                        var $table = $(this);
                        var $buttontable = $('#' + $table.CswAttrDom('id') + '_buttontbl');
						_configOn($table, $buttontable); 
                    },
            
			'ConfigOff': function() 
					{
                        var $table = $(this);
                        var $buttontable = $('#' + $table.CswAttrDom('id') + '_buttontbl');
						_configOff($table, $buttontable);
                    }
        };

        function _getCellSet($table, row, column)
        {
            var cellsetrows = parseInt($table.CswAttrDom('cellset_rows'));
            var cellsetcolumns = parseInt($table.CswAttrDom('cellset_columns'));
            var cellset = new Array();
            for(var r = 1; r <= cellsetrows; r++)
            {
                for(var c = 1; c <= cellsetcolumns; c++)
                {
                    if(cellset[r] === undefined) 
					{
						cellset[r] = new Array();
                    }
					cellset[r][c] = getCell( $table, row, column, r, c, cellsetrows, cellsetcolumns);
                }
            }
            return cellset;
        }

		function isRemoveMode($table)
		{
            return ($table.CswAttrDom('removemode') === "true");
		}
	    function setRemoveMode($table, mode)
        {
            $table.CswAttrDom('removemode', mode);
        }

        function isConfigMode($table)
        {
            return ($table.CswAttrDom('configmode') === "true");
        }
	    function setConfigMode($table, mode)
        {
            $table.CswAttrDom('configmode', mode);
        }
		function _toggleRemove($table, $rembtn)
		{
			if(isRemoveMode($table))
			{
				setRemoveMode($table, 'false');
				$rembtn.removeClass('CswLayoutTable_removeEnabled');
			}
			else
			{
				setRemoveMode($table, 'true');
				$rembtn.addClass('CswLayoutTable_removeEnabled');
			}
		}
        function _toggleConfig($table, $buttontable)
        {
            if(isConfigMode($table))
            {
				_configOff($table, $buttontable);
			} else {
				_configOn($table, $buttontable);
			}
		}

		function _configOff($table, $buttontable)
		{
			$buttontable.find('#' + $table.CswAttrDom('id') + 'addbtn').hide();
			$buttontable.find('#' + $table.CswAttrDom('id') + 'rembtn').hide();
			$buttontable.find('#' + $table.CswAttrDom('id') + 'addcolumnbtn').hide();
			$buttontable.find('#' + $table.CswAttrDom('id') + 'addrowbtn').hide();

            $table.CswTable('findCell', '.CswLayoutTable_cell')
				.removeClass('CswLayoutTable_configcell');

            setConfigMode($table, 'false');
            $table.trigger($table.CswAttrDom('id') + 'CswLayoutTable_onConfigOff', $buttontable);
        } // _configOff()
		
		function _configOn($table, $buttontable)
		{
			$buttontable.find('#' + $table.CswAttrDom('id') + 'addbtn').show();
			$buttontable.find('#' + $table.CswAttrDom('id') + 'rembtn').show();
			$buttontable.find('#' + $table.CswAttrDom('id') + 'addcolumnbtn').show();
			$buttontable.find('#' + $table.CswAttrDom('id') + 'addrowbtn').show();

            var cellsetrows = parseInt($table.CswAttrDom('cellset_rows'));
            var cellsetcolumns = parseInt($table.CswAttrDom('cellset_columns'));

            $table.CswTable('finish', null);

            $table.CswTable('findCell', '.CswLayoutTable_cell')
				.addClass('CswLayoutTable_configcell');

            setConfigMode($table, 'true');
            $table.trigger($table.CswAttrDom('id') + 'CswLayoutTable_onConfigOn', $buttontable);
        } // _configOn()

		function _addRow($table)
		{
            var cellsetrows = parseInt($table.CswAttrDom('cellset_rows'));
            var cellsetcolumns = parseInt($table.CswAttrDom('cellset_columns'));
            var tablemaxrows = $table.CswTable('maxrows');
            var tablemaxcolumns = $table.CswTable('maxcolumns');

            // add a row and column
            var $newcell = getCell($table, (tablemaxrows / cellsetrows ) + 1, (tablemaxcolumns / cellsetcolumns), cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
            $table.CswTable('finish', null);

            if(isConfigMode($table))
			{
				$table.CswTable('findCell', '.CswLayoutTable_cell')
					.addClass('CswLayoutTable_configcell');
			}
		} // _addRowAndColumn()

		function _addColumn($table)
		{
            var cellsetrows = parseInt($table.CswAttrDom('cellset_rows'));
            var cellsetcolumns = parseInt($table.CswAttrDom('cellset_columns'));
            var tablemaxrows = $table.CswTable('maxrows');
            var tablemaxcolumns = $table.CswTable('maxcolumns');

            // add a row and column
            var $newcell = getCell($table, (tablemaxrows / cellsetrows ), (tablemaxcolumns / cellsetcolumns) + 1, cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
            $table.CswTable('finish', null);

            if(isConfigMode($table))
			{
				$table.CswTable('findCell', '.CswLayoutTable_cell')
					.addClass('CswLayoutTable_configcell');
			}
		} // _addRowAndColumn()

        function getCell($table, row, column, cellsetrow, cellsetcolumn, cellsetrows, cellsetcolumns)
        {
            if(row < 1) row = 1;
            if(column < 1) column = 1;
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

            $cell.CswAttrDom('row', row)
                 .CswAttrDom('column', column)
                 .CswAttrDom('cellsetrow', cellsetrow)
                 .CswAttrDom('cellsetcolumn', cellsetcolumn)
				 .click(function(ev, dd) { onClick(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); })
                 .draggable({	//start: function(ev, dd) { onDrag(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); },
								drag: function(ev, dd) { onDrag(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); },
								stop: function(ev, dd) { onDrop(ev, dd, $table, row, column, cellsetrows, cellsetcolumns); }
							})
                 .hover(function(ev, dd) { onHoverIn(ev, dd, $table, $(this)); },
                        function(ev, dd) { onHoverOut(ev, dd, $table, $(this)); } );
        }

        function onHoverIn(ev, dd, $table, $cell)
        {
            if(isConfigMode($table))
                $cell.addClass('CswLayoutTable_hover');
			if(isRemoveMode($table))
			{
				var $cellset = $table.CswTable('findCell', '[row="'+ $cell.CswAttrDom('row') +'"][column="'+ $cell.CswAttrDom('column') +'"]');
				$cellset.addClass('CswLayoutTable_remove');
			}
		} // onHoverIn()

        function onHoverOut(ev, dd, $table, $cell)
        {
            if(isConfigMode($table))
                $cell.removeClass('CswLayoutTable_hover');
			if(isRemoveMode($table))
			{
				var $cellset = $table.CswTable('findCell', '[row="'+ $cell.CswAttrDom('row') +'"][column="'+ $cell.CswAttrDom('column') +'"]');
				$cellset.removeClass('CswLayoutTable_remove');
	        }
		} // onHoverOut()

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
                    $table.trigger($table.CswAttrDom('id') + 'CswLayoutTable_onSwap', { 
                                            table: $table,
                                            cellset: _getCellSet($table, row, column),
                                            swapcellset: _getCellSet($table, $swapcells.first().CswAttrDom('row'), $swapcells.first().CswAttrDom('column')),
                                            row: row,
                                            column: column,
                                            swaprow: $swapcells.first().CswAttrDom('row'),
                                            swapcolumn: $swapcells.first().CswAttrDom('column')
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
                } // if($swapcells.length > 0)
            } // if(isConfigMode($table))
        } // onDrop()

        function onClick(ev, dd, $table, row, column, cellsetrows, cellsetcolumns)
		{
			if(isRemoveMode($table))
			{
				var $removecells = $table.find('.CswLayoutTable_remove');
				if($removecells.length > 0)
				{
					$table.trigger($table.CswAttrDom('id') + 'CswLayoutTable_onRemove', { 
                                            table: $table,
                                            cellset: _getCellSet($table, row, column),
                                            row: $removecells.CswAttrDom('row'),
                                            column: $removecells.CswAttrDom('column')
                                        });
				}
				// contents().hide() doesn't work, jQuery ticket #8586: http://bugs.jquery.com/ticket/8586
				$removecells.children().hide();
				
				$removecells.removeClass('CswLayoutTable_remove');
			} // if(isRemoveMode($table))
		} // onClick()
        
        function getSwapCells($table, row, column, cellsetrows, cellsetcolumns, dd)
        {
            var $hovercell = $table.find('.CswLayoutTable_hover');
			return $table.CswTable('findCell', '[row="'+ $hovercell.CswAttrDom('row') +'"][column="'+ $hovercell.CswAttrDom('column') +'"]');
        } // getSwapCells()


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

