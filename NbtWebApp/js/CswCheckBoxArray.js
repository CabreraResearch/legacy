; (function ($) {
    $.fn.CswCheckBoxArray = function (method) {

        var methods = {
            init: function(options) {
        
                var o = {
                    ID: '',
                    cols: ['col1', 'col2', 'col3'],
                    data: [[ true, false, true ],
                           [ false, true, false ],
                           [ true, false, true ]]
                    //CheckboxesOnLeft: false,
                    //UseRadios: false,
                    //ReadOnly: false
                };

                if (options) {
                    $.extend(o, options);
                }

                var $Div = $(this);
                $Div.children().remove();

                var $table = makeTable(o.ID + '_tbl')
                               .appendTo($Div);

                var $cell11 = getTableCell($table, 1, 1);
                var $cell12 = getTableCell($table, 1, 2);

                // Header
                for(var c = 0; c < o.cols.length; c++)
                {
                    var $cell = getTableCell($table, 1, c+1);
                    $cell.append(o.cols[c]);
                }

                // Data
                for(var r = 0; r < o.data.length; r++)
                {
                    var row = o.data[r];
                    for(var c = 0; c < o.cols.length; c++)
                    {
                        var $cell = getTableCell($table, r+2, c+1);
                        
                        var $check = $('<input type="checkbox" class="CBACheckBox" row="' + r + '" col="' + c + '" id="'+ o.ID + '_' + r + '_' + c + '" />')
                                       .appendTo($cell);
                        if(row[c]) {
                            $check.attr('checked', 'true');
                        }
                    } // for(var c = 0; c < o.cols.length; c++)
                } // for(var r = 0; r < o.data.length; r++)

            }, // init

            getdata: function(options) {
                var o = {
                    ID: ''
                };

                if (options) {
                    $.extend(o, options);
                }

                var $Div = $(this);
                var data = new Array();
                
                $Div.find('.CBACheckBox')
                    .each(function() {
                        var $check = $(this);
                        var r = parseInt($check.attr('row'));
                        var c = parseInt($check.attr('col'));
                        if(data[r] == undefined) data[r] = new Array();
                        data[r][c] = $check.attr('checked');
                    });
                return data;
            }
        };
    

        // Method calling logic
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);