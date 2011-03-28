; (function ($) {
        
    var PluginName = 'CswFieldTypeLink';

    var methods = {
        init: function(o) { 

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
                var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

                var $Link = $('<a href="'+ Href +'" target="_blank">'+ Text +'</a>&nbsp;&nbsp;' )
                                .appendTo($table.CswTable('cell', 1, 1));

                var $EditButton = $('<div/>')
                                .appendTo($table.CswTable('cell', 1, 2))
                                .CswImageButton({
                                                ButtonType: CswImageButton_ButtonType.Edit,
                                                AlternateText: 'Edit',
                                                ID: o.ID + '_edit',
                                                Required: o.Required,
                                                onClick: function ($ImageDiv) 
													{ 
														$edittable.show();
														return CswImageButton_ButtonType.None; 
													}
                                                });

				var $edittable = $Div.CswTable('init', { ID: o.ID + '_edittbl' })
									.hide();

                var $edittext_label = $( '<span>Text</span>' )
                                .appendTo($edittable.CswTable('cell', 1, 1));
                
                var $edittext = $('<input type="text" id="'+ o.ID +'_text" value="'+ Text +'" />' )
                                .appendTo($edittable.CswTable('cell', 1, 2))
								.change(o.onchange);
                
                var $edithref_label = $( '<span>URL</span>' )
                                .appendTo($edittable.CswTable('cell', 2, 1));
                
				var $edithref = $('<input type="text" id="'+ o.ID +'_href" value="'+ Href +'" />' )
                                .appendTo($edittable.CswTable('cell', 2, 2))
								.change(o.onchange);

                if(o.Required && Href == '')
                {
                    $edittable.show();
					$edittext.addClass("required");
					$edithref.addClass("required");
                }
            }
        },
        save: function(o) { 
                var $edittext = o.$propdiv.find('#' + o.ID + '_text');
                var $edithref = o.$propdiv.find('#' + o.ID + '_href');
				o.$propxml.children('text').text($edittext.val());
				o.$propxml.children('href').text($edithref.val());
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
