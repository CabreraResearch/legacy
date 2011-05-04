; (function ($) {
    $.fn.CswComboBox = function (method) {

        var methods = {
            init: function(options) {
        
                var o = {
                    ID: '',
                    TopContent: '',
                    SelectContent: 'This ComboBox Is Empty!',
                    Width: '180px'
                };

                if (options) {
                    $.extend(o, options);
                }

                var $Div = $(this);
                $Div.contents().remove();

                var $TopDiv = $('<div id="'+ o.ID +'_top" class="CswComboBox_TopDiv"></div>')
                            .appendTo($Div)
                            .attr('style', 'width: '+ o.Width);

                var $table = $TopDiv.CswTable('init', { ID: o.ID + '_tbl' });
                $table.attr('width', '100%');
        
                $cell1 = $table.CswTable('cell', 1, 1);
                $cell1.attr('width', '100%');
                $cell1.append(o.TopContent);
        
                $cell2 = $table.CswTable('cell', 1, 2);
                $cell2.addClass( "CswComboBox_ImageCell" );

                var $ChildDiv = $('<div id="' + o.ID +'_child" class="CswComboBox_ChildDiv">')
                                  .appendTo($Div)
                                  .attr('style', 'width: '+ o.Width)
                                  .append(o.SelectContent)
								  .hover(function() {}, function() { $ChildDiv.hide(); });

                $cell1.click(function() { Toggle($TopDiv, $ChildDiv) });

                $cell2.CswImageButton({ 'ButtonType': CswImageButton_ButtonType.Select,
                                        'ID': o.ID + '_top_img',
                                        'AlternateText': '',
                                        'onClick': function() { Toggle($TopDiv, $ChildDiv) } });

                },
            TopContent: function(content) {
                    var $Div = $(this);
                    var $TopDiv = $Div.children('.CswComboBox_TopDiv');
                    var $table = $TopDiv.children('table');
                    var $cell1 = $table.CswTable('cell', 1, 1);
                    $cell1.text('');
                    $cell1.contents().remove();
                    $cell1.append(content);
                },
            toggle: function() {
                    var $Div = $(this);
                    var $TopDiv = $Div.children('.CswComboBox_TopDiv');
                    var $ChildDiv = $Div.children('.CswComboBox_ChildDiv');
                    Toggle($TopDiv, $ChildDiv);
                },
            close: function() {
                    var $Div = $(this);
                    var $TopDiv = $Div.children('.CswComboBox_TopDiv');
                    var $ChildDiv = $Div.children('.CswComboBox_ChildDiv');
                    Close($TopDiv, $ChildDiv);
                }
        };
    
        function Toggle($TopDiv, $ChildDiv)
        {
            $TopDiv.toggleClass('CswComboBox_TopDiv_click');
            $ChildDiv.toggle();
        }
        function Close($TopDiv, $ChildDiv)
        {
            $TopDiv.removeClass('CswComboBox_TopDiv_click');
            $ChildDiv.hide();
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