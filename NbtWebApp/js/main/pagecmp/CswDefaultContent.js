/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../controls/CswDiv.js" />
/// <reference path="../controls/CswLink.js" />

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
        $this.contents().remove();

        CswAjaxJson({
            url: o.Url,
            data: { ViewId: o.viewid },
            success: function (data) {
                
                var $addDiv = $this.CswDiv({ ID: makeId({ id: o.ID, suffix: 'adddiv' }), cssclass: 'adddiv' });
                $addDiv.append('Add New:');

                function _makeAddLinksRecursive(addObj, $parent) {
                    if (contains(addObj, 'entries')) {

                        var $ul = $('<ul></ul>');

                        each(addObj.entries, function (entryObj) {
                            var $li = HandleMenuItem({
                                $ul: $ul,
                                itemKey: entryObj.text,
                                itemJson: entryObj
                            }).appendTo($ul);
                        });
                        
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


