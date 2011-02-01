// ------------------------------------------------------------------------------------
// Cookies
// ------------------------------------------------------------------------------------

function SetSessionId(SessionId) {
    $.cookie('CswSessionId', SessionId);
}
function GetSessionId() {
    return $.cookie('CswSessionId');
}
function ClearSessionId() {
    $.cookie('CswSessionId', null);
}

function SetUsername(Username) {
    $.cookie('csw_username', Username);
}
function GetUsername() {
    return $.cookie('csw_username');
}
function ClearUsername() {
    $.cookie('csw_username', null);
}


// ------------------------------------------------------------------------------------
// Ajax
// ------------------------------------------------------------------------------------

function CswAjax(options) {
    var o = {
        url: '',
        data: '',
        success: function ($xml) { }
    };

    if (options) {
        $.extend(o, options);
    }

    var starttime = new Date();
    $.ajax({
        type: 'POST',
        url: o.url,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: o.data,
        success: function (data, textStatus, XMLHttpRequest) {

            var endtime = new Date();
            $('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

            var $xml = $(data.d);
            if ($xml.get(0).nodeName == "ERROR") {
                _handleAjaxError(XMLHttpRequest, $xml.text(), '');
            }
            else {
                o.success($xml);
            }

        }, // success{}
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
        }
    });    // $.ajax({
} // CswAjax()
        
function _handleAjaxError(XMLHttpRequest, textStatus, errorThrown) 
{
    ErrorMessage = "Error: " + textStatus;
    if (null != errorThrown) {
        ErrorMessage += "; Exception: " + errorThrown.toString()
    }
    console.log(ErrorMessage);
} // _handleAjaxError()


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
// Layout mechanics
// ------------------------------------------------------------------------------------

function getTableCell($table, row, col) {
    var $cell = null;
    if ( $table.length > 0 &&
         row != undefined && row != '' &&
         col != undefined && col != '' ) 
    {
        while (row >= $table.find('tr').length) {
            $table.append('<tr></tr>');
        }
        var $row = $($table.find('tr')[row]);
        while (col >= $row.find('td').length) {
            $row.append('<td></td>');
        }
        var $cell = $($row.find('td')[col]);
    }
    return $cell;
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
