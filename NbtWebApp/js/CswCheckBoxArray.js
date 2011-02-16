; (function ($) {
    $.fn.CswCheckBoxArray = function (method) {

        var methods = {
            init: function(options) {
        
                var o = {
                    ID: '',
                    cols: ['col1', 'col2', 'col3'],
                    data: [{ label: 'row1', 
                             key: 1,
                             values: [ true, false, true ] },
                           { label: 'row2', 
                             key: 2,
                             values: [ false, true, false ] },
                           { label: 'row3', 
                             key: 3,
                             values: [ true, false, true ] }]
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
                
                // Header
                for(var c = 0; c < o.cols.length; c++)
                {
                    var $cell = getTableCell($table, 1, c+2);
                    $cell.append(o.cols[c]);
                }

                // Data
                for(var r = 0; r < o.data.length; r++)
                {
                    var row = o.data[r];
                    // Row label
                    var $labelcell = getTableCell($table, r+2, 1);
                    $labelcell.append(row.label);
                    for(var c = 0; c < o.cols.length; c++)
                    {
                        
                        var $cell = getTableCell($table, r+2, c+2);
                        var checkid = o.ID + '_' + r + '_' + c;
                        var $check = $('<input type="checkbox" class="CBACheckBox" id="'+ checkid + '" />')
                                       .appendTo($cell);
                        $check.attr('key', row.key);
                        $check.attr('rowlabel', row.label);
                        $check.attr('collabel', o.cols[c]);
                        $check.attr('row', r);
                        $check.attr('col', c);

                        if(row.values[c]) {
                            $check.attr('checked', 'true');
                        }
                    } // for(var c = 0; c < o.cols.length; c++)
                } // for(var r = 0; r < o.data.length; r++)

            }, // init

            getdata: function () {
                var $Div = $(this);
                var data = new Array();
                
                $Div.find('.CBACheckBox')
                    .each(function() {
                            var $check = $(this);
                            var r = parseInt($check.attr('row'));
                            var c = parseInt($check.attr('col'));
                            if(data[r] == undefined) 
                                data[r] = new Array();
                            data[r][c] = { key: $check.attr('key'),
                                           rowlabel: $check.attr('rowlabel'),
                                           collabel: $check.attr('collabel'),
                                           checked: $check.attr('checked') 
                                         };
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