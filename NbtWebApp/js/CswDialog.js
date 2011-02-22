// ------------------------------------------------------------------------------------
// Popups and Dialogs
// ------------------------------------------------------------------------------------

; (function ($) {
    $.fn.CswDialog = function (options, method, text) {

        var o = {
            disabled: false,
            autoOpen: true,
            buttons: '', //{ "Ok": function() { $(this).dialog("close"); } }
            closeOnEscape: true,
            closeText: 'close',
            dialogClass: 'alert',
            draggable: true,
            height: 'auto',
            hide: null,
            maxHeight: false,
            maxWidth: false,
            minHeight: '150',
            minWidth: '150',
            modal: true,
            position: 'center',
            resizable: true,
            show: '',
            stack: true,
            title: '',
            width: '300',
            zIndex: '1000'
        };

        if (options) {
            $.extend(o, options);
        }
        
        var $Dialog = $(this);

        var $DialogDiv = $('<div id="dialog"><p>' + text + '</p></div>');
                                
        switch(method)
        {
            case 'destroy':
                $DialogDiv.dialog("destroy");
                break;

            case 'disable': 
                $DialogDiv.dialog("disable");
                break;

            case 'enable': 
                $DialogDiv.dialog("enable");
                break;

            case 'option': 
                $DialogDiv.dialog("option", o);
                $DialogDiv.dialog("open");
                break;

            case 'widget': 
                $DialogDiv.dialog("widget");
                break;

            case 'close': 
                $DialogDiv.dialog("close");
                break;

            case 'isOpen': 
                $DialogDiv.dialog("isOpen");
                break;

            case 'moveToTop': 
                $DialogDiv.dialog("moveToTop");
                break;

            case 'open': 
                $DialogDiv.dialog("open");
                break;

            default:
                $DialogDiv.dialog();
                break;
        }

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


