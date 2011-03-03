; (function ($) {
        
    var PluginName = 'CswFieldTypeFile';

    var methods = {
        init: function(o) { //o.nodeid, o.$propxml, o.onchange

                var $Div = $(this);
                $Div.contents().remove();

                var Href = o.$propxml.children('href').text().trim();
                var FileName = o.$propxml.children('name').text().trim();

                var $table = $.CswTable({ ID: o.ID + '_tbl' })
                             .appendTo($Div);
                var $cell11 = $table.CswTable('cell', 1, 1);
                var $cell12 = $table.CswTable('cell', 1, 2);
                var $cell13 = $table.CswTable('cell', 1, 3);

                $cell11.append('<a href="'+ Href +'" target="_blank">'+ FileName +'</a>');

                if(!o.ReadOnly)
                {
                    var $editButton = $('<div/>')
                        .appendTo($cell12)
                        .CswImageButton({   
                                            ButtonType: CswImageButton_ButtonType.Edit,
                                            AlternateText: 'Edit',
                                            ID: o.ID + '_edit',
                                            onClick: function (alttext) { alert('this function has not yet been implemented!'); return CswImageButton_ButtonType.None; }
                                        });
                    var $clearButton = $('<div/>')
                        .appendTo($cell13)
                        .CswImageButton({
                                            ButtonType: CswImageButton_ButtonType.Clear,
                                            AlternateText: 'Clear',
                                            ID: o.ID + '_clr',
                                            onClick: function (alttext) { alert('this function has not yet been implemented!'); return CswImageButton_ButtonType.None; }
                                        });
                }

            },
        save: function(o) {
//                var $TextBox = $propdiv.find('input');
//                o.$propxml.children('barcode').text($TextBox.val());
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
