// ------------------------------------------------------------------------------------
// Popups and Dialogs
// ------------------------------------------------------------------------------------

function OpenPopup(popupurl) {
    var popup = window.open(popupurl, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
    popup.focus();
    return popup;
}

function OpenDialog(id, url) {
    var $dialogdiv = $('<div id="' + id + '" style="display: none;"></div>');
    $dialogdiv.load(url,
                    {},
                    function (responseText, textStatus, XMLHttpRequest) {
                        $dialogdiv.appendTo('body')
                                  .dialog();
                    });
}

function CloseDialog(id) {
    $('#' + id).remove();
}




// ------------------------------------------------------------------------------------
// for debug
// ------------------------------------------------------------------------------------
function iterate(obj) {
    var str;
    for (var x in obj) {
        str = str + x + "=" + obj[x] + "<br><br>";
    }
    var popup = window.open("", "popup");
    if (popup != null)
        popup.document.write(str);
    else
        console.log("iterate() error: No popup!");
}
