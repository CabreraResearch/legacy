; (function ($) {
    $.fn.CswTable = function (method) {
        var PluginName = "CswTable";

        var methods = {
            'make': function (options) {
                        var o = {
                            ID: '',
                        };
                        if (options) {
                            $.extend(o, options);
                        }
                        return $('<table id="'+ o.ID +'" cellpadding="0" cellspacing="0" border="0"><tr><td></td></tr></table>');
                    },

            // row and col are 1-based
            'getCell': function (row, col) {
                        var $table = $(this);
	                    var $cell = null;
	                    if ($table.length > 0 &&
		                     row != undefined && row != '' &&
		                     col != undefined && col != '') {
		                    if (row <= 0) {
			                    log("error: row must be greater than 1, got: " + row);
			                    row = 1;
		                    }
		                    if (col <= 0) {
			                    log("error: col must be greater than 1, got: " + col);
			                    col = 1;
		                    }

		                    while (row > $table.children('tbody').children('tr').length) {
			                    $table.append('<tr></tr>');
		                    }
		                    var $row = $($table.children('tbody').children('tr')[row-1]);
		                    while (col > $row.children('td').length) {
			                    $row.append('<td></td>');
		                    }
		                    $cell = $($row.children('td')[col-1]);
	                    }
	                    return $cell;
                    },

            // These are safe for nested tables, since using $.find() is not
            'findRow': function (criteria) {
                        var $table = $(this);
                        var $rows = $table.children('tbody').children('tr');
                        if (criteria != '' && criteria != null) {
                            $rows = $rows.filter(criteria);
                        }
                        return $rows;
                    },
            'findCell': function (criteria) {
                        var $table = $(this);
                        var $cells = $table.children('tbody').children('tr').children('td');
                        if (criteria != '' && criteria != null) {
                            $cells = $cells.filter(criteria);
                        }
                        return $cells;
                    },
            'rowFindCell': function ($row, criteria) {
                        //var $table = $(this);
                        var $cells = $row.children('td');
                        if (criteria != '' && criteria != null) {
                            $cells = $cells.filter(criteria);
                        }
                        return $cells;
                    }

        };


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

