/// <reference path="../jquery/jquery-1.5.2-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
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
                             values: [ true, false, true ] }],
                    HeightInRows: 4,
                    //CheckboxesOnLeft: false,
                    UseRadios: false,
                    Required: false,
                    //ReadOnly: false
                    onchange: function() { }
                };

                if (options) {
                    $.extend(o, options);
                }

                var CheckType = "checkbox";
                if(o.UseRadios)
                    CheckType = "radio";

                var $Div = $(this);
                $Div.contents().remove();

                var $OuterDiv = $('<div/>').appendTo($Div);
                var $table = $OuterDiv.CswTable('init', { ID: o.ID + '_tbl' });

                $OuterDiv.css('height', (25 * o.HeightInRows) + 'px');
                $OuterDiv.addClass('cbarraydiv');
                $table.addClass('cbarraytable');

                // Header
                var tablerow = 1;
                for(var c = 0; c < o.cols.length; c++)
                {
                    var $cell = $table.CswTable('cell', tablerow, c+2);
                    $cell.addClass('cbarraycell');
                    $cell.append(o.cols[c]);
                }
                tablerow++;

                //[none] row
                if(o.UseRadios && ! o.Required)
                {
                    // Row label
                    var $labelcell = $table.CswTable('cell', tablerow, 1);
                    $labelcell.addClass('cbarraycell');
                    $labelcell.append('[none]');
                    for(var c = 0; c < o.cols.length; c++)
                    {
                        var $cell = $table.CswTable('cell', tablerow, c+2);
                        $cell.addClass('cbarraycell');
                        var checkid = o.ID + '_none';
                        var $check = $('<input type="'+ CheckType +'" class="CBACheckBox_'+ o.ID +'" id="'+ checkid + '" name="' + o.ID + '" />')
                                       .appendTo($cell)
                                       .click(o.onchange);
                        $check.attr('key', '');
                        $check.attr('rowlabel', '[none]');
                        $check.attr('collabel', o.cols[c]);
                        $check.attr('row', -1);
                        $check.attr('col', c);
                        
                        $check.attr('checked', 'true');   // the browser will override this if another one is checked

                    } // for(var c = 0; c < o.cols.length; c++)
                } // if(o.UseRadios && ! o.Required)
                tablerow++;

                // Data
                for(var r = 0; r < o.data.length; r++)
                {
                    var row = o.data[r];
                    // Row label
                    var $labelcell = $table.CswTable('cell', tablerow + r, 1);
                    $labelcell.addClass('cbarraycell');
                    $labelcell.append(row.label);
                    for(var c = 0; c < o.cols.length; c++)
                    {
                        
                        var $cell = $table.CswTable('cell', tablerow + r, c+2);
                        $cell.addClass('cbarraycell');
                        var checkid = o.ID + '_' + r + '_' + c;
                        var $check = $('<input type="'+ CheckType +'" class="CBACheckBox_'+ o.ID +'" id="'+ checkid + '" name="' + o.ID + '" />')
                                       .appendTo($cell)
                                       .click(o.onchange);
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

				if(!o.UseRadios)
				{
					var CheckAllLinkText = "Check All";
					if($('.CBACheckBox_' + o.ID).not(':checked').length == 0)
						CheckAllLinkText = "Uncheck All";

					var $checkalldiv = $('<div style="text-align: right"><a href="#">'+ CheckAllLinkText +'</a></div>')
										 .appendTo($Div);
					var $checkalllink = $checkalldiv.children('a');
					$checkalllink.click(function() { ToggleCheckAll($checkalllink, o.ID); return false; });
				}
            }, // init

            getdata: function (options) { 
                
                var o = {
                    ID: ''
                };

                if (options) {
                    $.extend(o, options);
                }
                
                var $Div = $(this);
                var data = new Array();
                $Div.find('.CBACheckBox_' + o.ID)
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
    
        function ToggleCheckAll($checkalllink, id)
        {
            // Are there any unchecked checkboxes?
            if($('.CBACheckBox_' + id).not(':checked').length > 0)
            {
                CheckAll($checkalllink, id);
            } else {
                UncheckAll($checkalllink, id);
            }
        } // ToggleCheckAll()

        function CheckAll($checkalllink, id)
        {
            $('.CBACheckBox_' + id).attr('checked', 'checked');
            $checkalllink.text('Uncheck all');
        }
        function UncheckAll($checkalllink, id)
        {
            $('.CBACheckBox_' + id).removeAttr('checked');
            $checkalllink.text('Check all');
        }

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