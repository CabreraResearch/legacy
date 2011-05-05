; (function ($) {
        
    var PluginName = 'CswFieldTypeImage';

    var methods = {
        init: function(o) { //o.nodeid, o.$propxml, o.onchange

            var $Div = $(this);
            $Div.contents().remove();

            var Href = o.$propxml.children('href').text().trim();
            var Width = o.$propxml.children('href').CswAttrXml('width');
            var Height = o.$propxml.children('href').CswAttrXml('height');
            var FileName = o.$propxml.children('name').text().trim();

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
            var $cell11 = $table.CswTable('cell', 1, 1).attr('colspan', '3');
            var $cell21 = $table.CswTable('cell', 2, 1).attr('width', Width-36);
            var $cell22 = $table.CswTable('cell', 2, 2).attr('align', 'right');
            var $cell23 = $table.CswTable('cell', 2, 3).attr('align', 'right');

			if(FileName !== '')
			{
				var $TextBox = $('<a href="'+ Href +'" target="_blank"><img src="' + Href + '" alt="' + FileName + '" width="'+ Width +'" height="'+ Height +'"/></a>')
									.appendTo($cell11);
				$cell21.append('<a href="'+ Href +'" target="_blank">'+ FileName +'</a>');
			}

            if(!o.ReadOnly)
            {
                var $editButton = $('<div/>')
                    .appendTo($cell22)
                    .CswImageButton({   
                                        ButtonType: CswImageButton_ButtonType.Edit,
                                        AlternateText: 'Edit',
                                        ID: o.ID + '_edit',
                                        onClick: function ($ImageDiv) { 
											
											$.CswDialog( 'FileUploadDialog', {
												'url': '/NbtWebApp/wsNBT.asmx/fileForProp',
												'params': { 
															'PropId': o.$propxml.CswAttrXml('id')
														  },
												'onSuccess': function()
													{
														o.onReload();
													}
												});
											return CswImageButton_ButtonType.None; 
										}
                                    });
                var $clearButton = $('<div/>')
                    .appendTo($cell23)
                    .CswImageButton({
                                        ButtonType: CswImageButton_ButtonType.Clear,
                                        AlternateText: 'Clear',
                                        ID: o.ID + '_clr',
                                        onClick: function ($ImageDiv) { 
											
											if(confirm("Are you sure you want to clear this image?"))
											{
												CswAjaxJSON({
													'url': '/NbtWebApp/wsNBT.asmx/clearProp',
													'data': '{ "PropId": "' + o.$propxml.CswAttrXml('id') + '", "IncludeBlob": "true" }',
													'success': function() { o.onReload(); }
												});
											}
											return CswImageButton_ButtonType.None; 
										}
                                    });
            }

        },
        save: function(o) { //$propdiv, o.$propxml
				// nothing to do here
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
