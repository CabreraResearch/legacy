/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    $.fn.CswDefaultContent = function (options) {

        var o = {
            ID: 'defcont',
            Url: '/NbtWebApp/wsNBT.asmx/getDefaultContent',
            viewid: '',
            viewmode: ''
        };
        if (options) $.extend(o, options);

        var $this = $(this);

        Csw.ajax.post({
            url: o.Url,
            data: { ViewId: o.viewid },
            success: function (data) {
                
                var $addDiv = $this.CswDiv({ ID: Csw.makeId({ id: o.ID, suffix: 'adddiv' }), cssclass: 'adddiv' });
                $addDiv.append('Add New:');

                function _makeAddLinksRecursive(addObj, $parent) {
                    var $ul = $('<ul></ul>');
                    function onEach(entryObj) {
                        var $li = Csw.handleMenuItem({
                            $ul: $ul,
                            itemKey: entryObj.text,
                            itemJson: entryObj
                        }).appendTo($ul);
                    }

                    if (Csw.contains(addObj, 'entries')) {
                        Csw.each(addObj.entries, onEach);
                        if ($ul.children().length > 0) {
                            $ul.appendTo($parent);
                            _makeAddLinksRecursive(addObj.children, $ul);
                        } else {
                            _makeAddLinksRecursive(addObj.children, $parent);
                        }
                    } // if(contains(addObj, 'entries'))
                } // _makeAddLinksRecursive()

                _makeAddLinksRecursive(data, $addDiv);
            } // success
        }); // ajax

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


