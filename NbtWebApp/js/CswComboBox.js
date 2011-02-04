; (function ($) {
    $.fn.CswComboBox = function (options) {

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

        $table = makeTable(o.ID + '_tbl').appendTo($TopDiv);
        $table.attr('width', '100%');
        
        $cell1 = getTableCell($table, 1, 1);
        $cell1.click(onClick);
        $cell1.append(o.Text);
        
        $cell2 = getTableCell($table, 1, 2);
        $cell2.addClass( "CswComboBox_ImageCell" )
              .CswImageButton({ 'ButtonType': CswImageButton_ButtonType.Select,
                                'ID': o.ID + '_top_img',
                                'AlternateText': '',
                                'onClick': onClick });

        $ChildDiv = $('<div id="' + o.ID +'_child" class="CswComboBox_ChildDiv">')
                      .appendTo($Div)
                      .attr('style', 'width: '+ o.Width)
                      .append(o.Content);

        function onClick()
        {
            $TopDiv.toggleClass('CswComboBox_TopDiv_click');
            $ChildDiv.toggle();
        }

        // For proper chaining support
        return this;

}; // function(options) {
})(jQuery);
