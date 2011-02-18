; (function ($) {
        
    var PluginName = 'CswFieldTypeFile';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = $xml.attr('required');
                var ReadOnly = $xml.attr('readonly');

                var Href = $xml.children('href').text();
                var FileName = $xml.children('name').text();

                var $table = makeTable(ID + '_tbl')
                             .appendTo($Div);
                var $cell11 = getTableCell($table, 1, 1);
                var $cell12 = getTableCell($table, 1, 2);
                var $cell13 = getTableCell($table, 1, 3);

                $cell11.append('<a href="'+ Href +'" target="_blank">'+ FileName +'</a>');

                if(!ReadOnly)
                {
                    var $editButton = $('<div/>')
                        .appendTo($cell12)
                        .CswImageButton({   
                                            ButtonType: CswImageButton_ButtonType.Edit,
                                            AlternateText: 'Edit',
                                            ID: ID + '_edit',
                                            onClick: function (alttext) { alert('this function has not yet been implemented!'); return CswImageButton_ButtonType.None; }
                                        });
                    var $clearButton = $('<div/>')
                        .appendTo($cell13)
                        .CswImageButton({
                                            ButtonType: CswImageButton_ButtonType.Clear,
                                            AlternateText: 'Clear',
                                            ID: ID + '_clr',
                                            onClick: function (alttext) { alert('this function has not yet been implemented!'); return CswImageButton_ButtonType.None; }
                                        });
                }

            },
        save: function($propdiv, $xml) {
//                var $TextBox = $propdiv.find('input');
//                $xml.children('barcode').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeFile = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
