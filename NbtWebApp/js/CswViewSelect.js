; (function ($) {
    $.fn.CswViewSelect = function (options) {

        var o = {
            ID: '',
            ViewUrl: '/NbtWebApp/wsNBT.asmx/getViews',
            viewid: '',
            onSelect: function(viewid) { }
        };

        if (options) {
            $.extend(o, options);
        }

        var $selectdiv = $(this);
        $selectdiv.children().remove();

        getViewSelect(o.viewid);

        function getViewSelect(selectedviewid)
        {
            $viewtreediv = $('<div/>');
            $selectdiv.CswComboBox({ 'ID': o.ID + '_combo', 
                                     'Content': $viewtreediv });

            $viewtreediv.CswViewTree();

        } // getViewSelect()

        
        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

