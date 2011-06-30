/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
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
            var $cell11 = $table.CswTable('cell', 1, 1).CswAttrDom('colspan', '3');
            var $cell21 = $table.CswTable('cell', 2, 1).CswAttrDom('width', Width-36);
            var $cell22 = $table.CswTable('cell', 2, 2).CswAttrDom('align', 'right');
            var $cell23 = $table.CswTable('cell', 2, 3).CswAttrDom('align', 'right');

			if(FileName !== '')
			{
				var $TextBox = $('<a href="'+ Href +'" target="_blank"><img src="' + Href + '" alt="' + FileName + '" width="'+ Width +'" height="'+ Height +'"/></a>')
									.appendTo($cell11);
				$cell21.append('<a href="'+ Href +'" target="_blank">'+ FileName +'</a>');
			}

            if(!o.ReadOnly && o.EditMode != EditMode.AddInPopup.name)
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
												var dataJson = {
                                                    PropId: o.$propxml.CswAttrXml('id'), 
                                                    IncludeBlob: true,
													ViewId: $.CswCookie('get', CswCookieName.CurrentViewId)
                                                };
                                                
                                                CswAjaxJSON({
													'url': '/NbtWebApp/wsNBT.asmx/clearProp',
													'data': dataJson,
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
