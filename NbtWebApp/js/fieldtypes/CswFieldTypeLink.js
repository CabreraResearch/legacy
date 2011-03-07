; (function ($) {
        
    var PluginName = 'CswFieldTypeLink';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var Text = o.$propxml.children('text').text().trim();
            var Href = o.$propxml.children('href').text().trim();

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $table = $.CswTable({ ID: o.ID + '_tbl' })
                                .appendTo($Div);
                var $cell11 = $table.CswTable('cell', 1, 1);
                var $cell12 = $table.CswTable('cell', 1, 2);

                var $Link = $('<a href="'+ Href +'" target="_blank">'+ Text +'</a>&nbsp;&nbsp;' )
                                .appendTo($cell11);
                    
                var $EditButton = $('<div/>')
                                .appendTo($cell12)
                                .CswImageButton({
                                                ButtonType: CswImageButton_ButtonType.Edit,
                                                AlternateText: 'Edit',
                                                ID: o.ID + '_edit',
                                                Required: o.Required,
                                                onClick: function (alttext) { alert('not implemented yet'); return CswImageButton_ButtonType.None; }
                                                });
//                    if(o.Required)
//                    {
//                        $Link.addClass("required");
//                    }
            }
        },
        save: function(o) { //$propdiv, $xml
//                var $Link = $propdiv.find('input');
//                $xml.children('text').text($Link.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeLink = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
