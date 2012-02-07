/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswTBD() {
    'use strict';


    function jsTreeGetSelected($treediv) {
        var idPrefix = $treediv.CswAttrDom('id');
        var $SelectedItem = $treediv.jstree('get_selected');
        var ret = {
            'iconurl': $SelectedItem.children('a').children('ins').css('background-image'),
            'id': Csw.string($SelectedItem.CswAttrDom('id')).substring(idPrefix.length),
            'text': Csw.string($SelectedItem.children('a').first().text()).trim(),
            '$item': $SelectedItem
        };
        return ret;
    }
    Csw.register('jsTreeGetSelected', jsTreeGetSelected);
    Csw.jsTreeGetSelected = Csw.jsTreeGetSelected || jsTreeGetSelected;
    
    function openPopup(url, height, width) {
        var popup = window.open(url, null, 'height=' + height + ', width=' + width + ', status=no, resizable=yes, scrollbars=yes, toolbar=yes, location=no, menubar=yes');
        popup.focus();
        return popup;
    }
    Csw.register('openPopup', openPopup);
    Csw.openPopup = Csw.openPopup || openPopup;

}());
