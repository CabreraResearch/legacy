; (function ($) {
        
    var PluginName = 'CswFieldTypeImage';

    var methods = {
        init: function(nodepk, $xml, onchange) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                var Href = $xml.children('href').text();
                var Width = $xml.children('href').attr('width');
                var Height = $xml.children('href').attr('height');
                var FileName = $xml.children('name').text();

                var $table = makeTable(ID + '_tbl')
                             .appendTo($Div);
                var $cell11 = getTableCell($table, 1, 1).attr('colspan', '3');
                var $cell21 = getTableCell($table, 2, 1).attr('width', Width-36);
                var $cell22 = getTableCell($table, 2, 2).attr('align', 'right');
                var $cell23 = getTableCell($table, 2, 3).attr('align', 'right');

                var $TextBox = $('<a href="'+ Href +'" target="_blank"><img src="' + Href + '" alt="' + FileName + '" width="'+ Width +'" height="'+ Height +'"/></a>')
                                 .appendTo($cell11);
                $cell21.append('<a href="'+ Href +'" target="_blank">'+ FileName +'</a>');

                if(!ReadOnly)
                {
                    var $editButton = $('<div/>')
                        .appendTo($cell22)
                        .CswImageButton({   
                                            ButtonType: CswImageButton_ButtonType.Edit,
                                            AlternateText: 'Edit',
                                            ID: ID + '_edit',
                                            onClick: function (alttext) { alert('this function has not yet been implemented!'); return CswImageButton_ButtonType.None; }
                                        });
                    var $clearButton = $('<div/>')
                        .appendTo($cell23)
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
//                $xml.children('text').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeImage = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
