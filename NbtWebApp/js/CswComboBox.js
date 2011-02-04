; (function ($) {
        
    var methods = {
        init: function(options) {
        
            var o = {
                ID: '',
                Text: '',
                Content: 'This ComboBox Is Empty!',
                Width: '180px'
            };

            if (options) {
                $.extend(o, options);
            }

            var $Div = $(this);
            $Div.children().remove();

            $TopDiv = $('<div id="'+ o.ID +'_top" class="CswComboBox_TopDiv"></div>')
                        .appendTo($Div)
                        .attr('style', 'width: '+ o.Width);

            $table = makeTable(o.ID + '_tbl')
                     .appendTo($TopDiv);
            $table.attr('width', '100%');
        
            $cell1 = getTableCell($table, 1, 1);
            $cell1.click(Toggle);
            $cell1.attr('width', '100%');
            $cell1.append(o.Text);
        
            $cell2 = getTableCell($table, 1, 2);
            $cell2.addClass( "CswComboBox_ImageCell" )
                  .CswImageButton({ 'ButtonType': CswImageButton_ButtonType.Select,
                                    'ID': o.ID + '_top_img',
                                    'AlternateText': '',
                                    'onClick': Toggle });

            $ChildDiv = $('<div id="' + o.ID +'_child" class="CswComboBox_ChildDiv">')
                          .appendTo($Div)
                          .attr('style', 'width: '+ o.Width)
                          .append(o.Content);

            },
        toggle: function() {
                Toggle();
            }
    };
    
    function Toggle()
    {
        $TopDiv.toggleClass('CswComboBox_TopDiv_click');
        $ChildDiv.toggle();
    }

    // Method calling logic
    $.fn.CswComboBox = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);