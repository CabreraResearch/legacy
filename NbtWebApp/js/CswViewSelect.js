; (function ($) {
    $.fn.CswViewSelect = function (options) {

        var o = {
            ID: '',
            ViewUrl: '/NbtWebApp/wsNBT.asmx/getViews',
            viewid: '',
            onSelect: function(itemid) { },
            ClickDelay: 300
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
            $selectdiv.CswComboBox('init', { 'ID': o.ID + '_combo', 
                                             'Content': $viewtreediv,
                                             'Width': '266px' });

            $viewtreediv.CswViewTree({ 'onSelect': onTreeSelect });
            
        } // getViewSelect()

        function onTreeSelect(itemid)
        {
            setTimeout(function() { $selectdiv.CswComboBox( 'toggle'); }, o.ClickDelay);
            o.onSelect(itemid);
        }
        
        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

